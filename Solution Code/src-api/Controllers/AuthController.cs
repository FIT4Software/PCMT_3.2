using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Contexts;
using System.ComponentModel.DataAnnotations;
using src_api.Utilities;
using DTOs;

namespace src_api.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly CentralDBContext _centralContext;
    private readonly SiteDBContext _siteContext;
    private readonly IAuthHelper _authHelper;
    private ILogger _logger;

    public AuthController(CentralDBContext centralContext, SiteDBContext siteContext, IAuthHelper authHelper, ILogger<AuthController> logger)
    {
        _centralContext = centralContext;
        _siteContext = siteContext;
        _authHelper = authHelper;
        this._logger = logger;

    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Login([Required] string base64Username, [Required] string base64Password, string? serverid)
    {
        try
        {
            if (string.IsNullOrEmpty(serverid))
            {
                if (!string.IsNullOrEmpty(base64Username) && !string.IsNullOrEmpty(base64Password))
                {
                    bool access = _authHelper.AccessValidator(base64Username, base64Password);
                    Response_Auth_Server_Data_DTO[] groupArray;

                    if (access)
                    {
                        groupArray = await _authHelper.GetAccessGroups(base64Username, base64Password);
                    }
                    else
                    {
                        _logger.LogError("Unauthorized: " + access);
                        return Unauthorized();
                    }

                    if (groupArray.Length != 0)
                    {
                        DTOs.User result = _authHelper.AuthorizeUser(base64Username, groupArray, serverid);
                        _logger.LogInformation("Authorization success: " + result);

                        HttpContext.Session.SetString("Base64Username", base64Username);
                        HttpContext.Session.SetString("Base64Password", base64Password);
                        HttpContext.Session.SetString("GroupArray", JsonSerializer.Serialize(groupArray));

                        return Ok(result);
                    }
                    else
                    {
                        _logger.LogError("Failed login attempt - user does not have access to PCMT");
                        return Unauthorized("User does not have access to PCMT");
                    }
                }
                else
                {
                    _logger.LogError("Username or password missing: " + base64Username + base64Password);
                    return BadRequest("Username or password missing");
                }
            }
            else
            {
                string savedUsername = HttpContext.Session.GetString("Base64Username");
                string savedPassword = HttpContext.Session.GetString("Base64Password");
                string groupArrayJson = HttpContext.Session.GetString("GroupArray");

                if (string.IsNullOrEmpty(savedUsername) || string.IsNullOrEmpty(savedPassword) || string.IsNullOrEmpty(groupArrayJson))
                {
                    _logger.LogError("Session expired or credentials/groups missing");
                    return Unauthorized("Session expired, please login again");
                }

                Response_Auth_Server_Data_DTO[] groupArray = JsonSerializer.Deserialize<Response_Auth_Server_Data_DTO[]>(groupArrayJson);

                DTOs.User result = _authHelper.AuthorizeUser(savedUsername, groupArray, serverid);
                string token = result.token;
                _logger.LogInformation("Token generated for server ID: " + serverid);

                return Ok(token);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Error during login: " + ex.Message);
            return StatusCode(500, "Internal server error");
        }
    }



}
