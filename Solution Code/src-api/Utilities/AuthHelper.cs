using Contexts;
using DTOs;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Models;
using System.DirectoryServices.Protocols;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace src_api.Utilities
{
    public class AuthHelper : IAuthHelper
    {
        private readonly CentralDBContext _centralContext;
        private readonly SiteDBContext _siteContext;
        private readonly IConfiguration _configuration;
        private ILogger _logger;
        private bool _superadmin;

        public AuthHelper(CentralDBContext centralContext, SiteDBContext siteContext, IConfiguration configuration, ILogger<AuthHelper> logger, bool superadmin)
        {
            _siteContext = siteContext;
            _centralContext = centralContext;
            _configuration = configuration;
            this._logger = logger;
            _superadmin = superadmin;
        }

        public string getTnumberLDAP(string username)
        {
            string TNumber = null;

            string shortName = username.Replace("\\", "");

            string? encodedLdapPass = _configuration["LdapPass"];
            byte[] ldapPassInBytes = System.Convert.FromBase64String(encodedLdapPass);
            string userPassword = Encoding.Unicode.GetString(ProtectedData.Unprotect(ldapPassInBytes, null, DataProtectionScope.LocalMachine));

            using (LdapConnection con = new LdapConnection(new LdapDirectoryIdentifier(_configuration["LdapDomain"], Int32.Parse(_configuration["LdapPort"]))))
            {
                con.SessionOptions.SecureSocketLayer = false;
                con.SessionOptions.ProtocolVersion = 3;
                con.Credential = new NetworkCredential(_configuration["LdapUsername"], userPassword);
                con.AuthType = AuthType.Basic;
                try
                {
                    con.Bind();
                    string filter = "(extshortname={0})";
                    string[] attributes = { "uid" };
                    SearchRequest r = new SearchRequest(
                    "ou = people, ou = pg, o = world",
                    string.Format(filter, shortName),
                    System.DirectoryServices.Protocols.SearchScope.Subtree,
                    attributes);

                    SearchResponse re = (SearchResponse)con.SendRequest(r);

                    if (re.Entries.Count > 0)
                        foreach (SearchResultEntry entry in re.Entries)
                        {
                            TNumber = entry.Attributes["uid"][0].ToString();
                            if (TNumber != null)
                            {
                                _logger.LogInformation("Successfully obtained Tnumber");
                            }
                        }
                }
                catch
                {
                    _logger.LogError("Can not connect to authentication server");
                    throw new Exception("Can not connect to authentication server");
                }
            }
            _logger.LogInformation("TNumber" + TNumber);
            return TNumber;
        }
        public bool AccessValidator(string base64Username, string base64Password)
        {
            string username = Encoding.UTF8.GetString(Convert.FromBase64String(base64Username));
            string password = Encoding.UTF8.GetString(Convert.FromBase64String(base64Password));

            string Tnum = getTnumberLDAP(SanitizeInput(username));

            bool login = false;

            using (LdapConnection connection = new LdapConnection(new LdapDirectoryIdentifier(_configuration["LdapDomain"], Int32.Parse(_configuration["LdapPort"]))))
            {
                connection.SessionOptions.SecureSocketLayer = false;
                string DN = String.Format("uid = {0}, ou = people, ou = pg, o = world", Tnum);
                connection.Credential = new NetworkCredential(DN, password);
                connection.AuthType = AuthType.Basic;
                try
                {
                    connection.Bind();
                    login = true;
                }
                catch (Exception ex)
                {
                    if (ex.HResult == 49)
                    {
                        _logger.LogError(ex.Message);
                        throw new Exception("Invalid Username or Password");
                    }
                    else if (ex.HResult == 81)
                    {
                        _logger.LogError(ex.Message);
                        throw new Exception("LDAP Server is unavailable. Please check the port.");
                    }
                }
            }
            if (login)
            {
                _logger.LogInformation("Success Login & access");
                return true;
            }
            else
            {
                _logger.LogError("Failure Login");
                return false;
            }
        }
        public async Task<List<Response_Auth_Server_Data_DTO>> GetServerDataAsync()
        {
            var servers = new List<Response_Auth_Server_Data_DTO>();
            var connectionString = $"{_configuration.GetConnectionString("CentralDB")}";

            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("SELECT * FROM dbo.Server_Data WITH(NOLOCK)", connection))
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                servers.Add(new Response_Auth_Server_Data_DTO
                                {
                                    serverId = reader.GetInt32(reader.GetOrdinal("ServerId")),
                                    serverName = reader.GetString(reader.GetOrdinal("ServerName")),
                                    server = reader.GetString(reader.GetOrdinal("ServerURL")),
                                    serverDB = reader.GetString(reader.GetOrdinal("ServerDBName")),
                                    groups = reader.GetString(reader.GetOrdinal("ServerSSOGroups"))
                                });
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    _logger.LogError($"Connection error: {ex.Message}");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error: {ex.Message}");
                }
            }

            return servers;
        }
        public async Task<Response_Auth_Server_Data_DTO[]> GetAccessGroups(string base64Username, string base64Password)
        {
            List<string> groups = new List<string>();
            List<string> trimmedGroups = new List<string>();

            string username = Encoding.UTF8.GetString(Convert.FromBase64String(base64Username));
            string password = Encoding.UTF8.GetString(Convert.FromBase64String(base64Password));

            string? encodedLdapPass = _configuration["LdapPass"];
            byte[] ldapPassInBytes = System.Convert.FromBase64String(encodedLdapPass);
            string userPassword = Encoding.Unicode.GetString(ProtectedData.Unprotect(ldapPassInBytes, null, DataProtectionScope.LocalMachine));

            string Tnum = getTnumberLDAP(SanitizeInput(username));
            try
            {
                using (LdapConnection connection = new LdapConnection(new LdapDirectoryIdentifier(_configuration["LdapDomain"], Int32.Parse(_configuration["LdapPort"]))))
                {
                    string DN = String.Format("uid = {0}, ou = people, ou = pg, o = world", Tnum);
                    connection.Credential = new NetworkCredential(DN, password);
                    connection.AuthType = AuthType.Basic;

                    string DName = "ou = people, ou = pg, o = world";
                    string filter = "(extshortname={0})";
                    string[] attributes = { "uid" };
                    SearchRequest searchRequest = new SearchRequest(
                        DName,
                        string.Format(filter, SanitizeInput(username)),
                        System.DirectoryServices.Protocols.SearchScope.Subtree,
                        attributes);

                    searchRequest.Attributes.Add("memberOf");

                    SearchResponse searchResponse = (SearchResponse)connection.SendRequest(searchRequest);

                    SearchResultEntry entry = searchResponse.Entries[0];
                    DirectoryAttribute memberOfAttribute = entry.Attributes["memberOf"];

                    for (int i = 0; i < memberOfAttribute.Count; i++)
                    {
                        groups.Add((string)memberOfAttribute[i]);
                    }

                    trimmedGroups = TrimStringsInList(groups);

                    var admin = _configuration["SuperAdminGroup"];

                    _superadmin = trimmedGroups.Contains(admin);

                    var serverGroups = await GetServerDataAsync();

                    var matchingKeys = serverGroups
                    .Where(sg => trimmedGroups.Any(group => sg.groups != null && sg.groups.Contains(group, StringComparison.OrdinalIgnoreCase)))
                    .Select(sg => new Response_Auth_Server_Data_DTO
                    {
                        serverId = sg.serverId,
                        serverName = sg.serverName,
                        server = sg.server,
                        serverDB = sg.serverDB,
                        groups = sg.groups
                    })
                    .ToArray();

                    _logger.LogInformation("Success Getting Access Groups: " + string.Join(", ", matchingKeys.Select(k => k.serverName)));
                    return matchingKeys;

                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while getting access groups {ex.Message}");
                throw new Exception($"Exception encountered while getting access groups {ex.Message}, {ex.StackTrace}");

            }
        }

        public DTOs.User AuthorizeUser(string base64username, Response_Auth_Server_Data_DTO[] groups, int serverid)
        {
            if (!string.IsNullOrEmpty(base64username) && groups != null)
            {
                string token = null;

                if (serverid != 0)
                {
                    token = GenerateToken(base64username, serverid, groups);
                }

                List<string> siteList = new List<string>();
                foreach (var group in groups)
                {
                    if (!string.IsNullOrEmpty(group.server))
                    {
                        siteList.Add(group.server); 
                    }
                }
                string[] sites = siteList.ToArray();

                DTOs.User user = new DTOs.User
                {
                    sites = sites,
                    token = token ?? "", 
                    message = serverid == 0
                        ? "You have access to more than one server. Please select one to continue."
                        : "Login successful."
                };

                _logger.LogInformation("Authorization successful for user");
                return user;
            }
            else
            {
                _logger.LogError("Username or groups were null");
                throw new Exception("Username or groups were null");
            }
        }
        public async Task<int> GetPPAUser(string username, Response_Auth_Server_Data_DTO matchedServer)
        {
            try
            {
                _siteContext.setOnPremConnection(matchedServer.server);
                _siteContext.Database.SetCommandTimeout(0);
                var result = await _siteContext.Database.SqlQueryRaw<Users_Base>(
                $@"
                SELECT User_Id 
                FROM Users_Base WITH(NOLOCK)
                WHERE WindowsUserInfo LIKE '%\{username}' AND Mixed_Mode_Login = 1;"
                ).ToListAsync();

                _logger.LogInformation("PPAUserId: "+ result.First().User_ID);
                
                if (result != null)
                {
                    return result.First().User_ID;
                }
                else
                {
                    return -1; 
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError($"Connection error: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"You don't have proper access to this server. Please check that your windows user is configured properly on the desired proficy server: {ex.Message}");
                throw new Exception($"You don't have proper access to this server. Please check that your windows user is configured properly on the desired proficy server: {ex.Message}");
            }
            finally
            {
                _siteContext.Dispose();
            }
            return -1;

        }
        public async Task<string> GetAspectingSite(string username, Response_Auth_Server_Data_DTO matchedServer)
        {
            try
            {
                _siteContext.setOnPremConnection(matchedServer.server);
                _siteContext.Database.SetCommandTimeout(0);
                var result = await _siteContext.Database.SqlQueryRaw<Site_Parameters>(
                $@"
                SELECT
                [Aspecting] =
                CASE
                WHEN sp.value = '1' THEN 'ACTIVE'
                ELSE 'NOT ACTIVE'
                END
                FROM dbo.Site_Parameters sp WITH(NOLOCK)
                JOIN dbo.Parameters p WITH(NOLOCK) ON p.parm_id = sp.parm_id
                WHERE p.parm_name LIKE 'UseProficyClient;"
                ).ToListAsync();

                _logger.LogInformation("Aspecting site: " + result);

                return result.First().Aspecting;
            }
            catch (SqlException ex)
            {
                _logger.LogError($"Connection error: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                throw new Exception($"Error: {ex.Message}");
            }
            finally
            {
                _siteContext.Dispose();
            }
            return "NOT ACTIVE";
        }

        private string GenerateToken(string base64username, int serverid, Response_Auth_Server_Data_DTO[] groups)
        {
            int expiration = Int32.Parse(_configuration["TokenExpirationHours"]);
            string? encodedSecret = _configuration["secret"];

            byte[] secretInBytes = System.Convert.FromBase64String(encodedSecret);
            string secret = Encoding.Unicode.GetString(ProtectedData.Unprotect(secretInBytes, null, DataProtectionScope.LocalMachine));

            string key = secret;
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenExpiration = DateTime.UtcNow.AddHours(expiration);

            var matchedServer = groups.FirstOrDefault(g => g.serverId == serverid);
            string username = Encoding.UTF8.GetString(Convert.FromBase64String(base64username));

            var ppauserid = GetPPAUser(username, matchedServer);
            var aspecting = GetAspectingSite(username, matchedServer);

            var tokenDescriptor = new SecurityTokenDescriptor
            {

                Expires = tokenExpiration,
                SigningCredentials = credentials,
                Subject = new ClaimsIdentity(new[]
                {
                new Claim("username", username.ToString()),
                new Claim("ppauserid", ppauserid.Result.ToString()),
                new Claim("serverId", serverid.ToString()),
                new Claim("serverName",matchedServer.serverName),
                new Claim("server",matchedServer.server),
                new Claim("serverDB", matchedServer.serverDB),
                new Claim("superAdmin", _superadmin.ToString()),
                new Claim("aspecting", aspecting.Result.ToString())
            })
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            string jwtToken = tokenHandler.WriteToken(token);
            _logger.LogInformation("Success Token" + jwtToken);

            return jwtToken;

        }
        private string SanitizeInput(string input)
        {
            return Regex.Replace(input, @"[^\w\.]", "");
        }
        public static string TrimString(string input)
        {
            string trimmedStart = input.Substring(3);
            int commaIndex = trimmedStart.IndexOf(',');

            return trimmedStart.Substring(0, commaIndex);
        }

        public List<string> TrimStringsInList(List<string> inputList)
        {
            List<string> trimmedList = new List<string>();

            foreach (string str in inputList)
            {
                trimmedList.Add(TrimString(str));
            }
            _logger.LogInformation("Success StringtoList " + trimmedList.Count);

            return trimmedList;
        }
    }
}
