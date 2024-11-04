using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Contexts;
using Models;
using DTOs;

namespace src_api.Controllers
{
    [Route("api/v1/servers")]
    [ApiController]
    [Authorize]
    public class ServerDataController : ControllerBase
    {
        private readonly CentralDBContext _centralContext;

        public ServerDataController(CentralDBContext centralContext)
        {
            _centralContext = centralContext;
        }

        [HttpGet()]
        public async Task<ActionResult<List<Response_Server_Data_DTO>>> GetAllServerData()
        {
            try 
            {
                var result = await _centralContext.Database.SqlQueryRaw<Server_Data>(@"
                    SELECT * FROM dbo.Server_Data (NOLOCK) WHERE active>=0
                ").ToListAsync();

                List<Response_Server_Data_DTO> mappedResult = result.Select(r => new Response_Server_Data_DTO
                {
                    serverId = r.ServerId,
                    serverName = r.ServerName,
                    server = r.ServerURL,
                    serverDB = r.ServerDBName,
                    serverSSO = r.ServerSSOGroups,
                    active = r.Active != 1 ? false : true,
                }).ToList();

                return Ok(mappedResult);
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message, statusCode: 500);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Response_Server_Data_DTO>> GetServerDataById(int id)
        {
            try 
            {
                object[] paramItems = new object[]
                {
                    new SqlParameter("@ServerId", id),
                };

                var result = await _centralContext.Database.SqlQueryRaw<Server_Data>(@"
                    SELECT * FROM dbo.Server_Data (NOLOCK) WHERE ServerId = @ServerId
                ", paramItems).ToListAsync();

                List<Response_Server_Data_DTO> mappedResult = result.Select(r => new Response_Server_Data_DTO
                {
                    serverId = r.ServerId,
                    serverName = r.ServerName,
                    server = r.ServerURL,
                    serverDB = r.ServerDBName,
                    serverSSO = r.ServerSSOGroups,
                    active = r.Active != 1 ? false : true,
                }).ToList();

                return Ok(mappedResult[0]);
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message, statusCode: 500);
            }
        }

        [HttpPost("create-server")]
        public async Task<ActionResult> CreateServerData (Create_Server_Data_DTO serverData)
        {
            try 
            {
                object[] paramItems = new object[]
                {
                    new SqlParameter("@ServerName", serverData.serverName),
                    new SqlParameter("@ServerURL", serverData.server),
                    new SqlParameter("@Active", serverData.active != true ? -1 : 1),
                    new SqlParameter("@ServerSSOGroups", serverData.serverSSO),
                };
        
                int rowsAffected = await _centralContext.Database.ExecuteSqlRawAsync(@"
                    INSERT INTO dbo.Server_Data (ServerName, ServerURL, ServerDBName, Active, ServerSSOGroups) 
                    VALUES (@ServerName, @ServerURL, 'GBDB', @Active, @ServerSSOGroups)
                ", paramItems);

                return NoContent();
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message, statusCode: 500);
            }
        }

        [HttpPut("update-server")]
        public async Task<ActionResult> UpdateServerData(Update_Server_Data_DTO serverData)
        {
            try 
            {
                object[] paramItems = new object[]
                {
                    new SqlParameter("@ServerName", serverData.serverName),
                    new SqlParameter("@ServerURL", serverData.server),
                    new SqlParameter("@Active", serverData.active != true ? -1 : 1),
                    new SqlParameter("@ServerSSOGroups", serverData.serverSSO),
                    new SqlParameter("@ServerId", serverData.serverId),
                };
        
                int rowsAffected = await _centralContext.Database.ExecuteSqlRawAsync(@"
                    UPDATE dbo.Server_Data
                    SET ServerName = @ServerName, ServerURL = @ServerURL, Active = @Active, ServerSSOGroups = @ServerSSOGroups
                    WHERE ServerId = @ServerId
                ", paramItems);

                return NoContent();
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message, statusCode: 500);
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> DeleteServerData(int id)
        {
            try 
            {    
                object[] paramItems = new object[]
                {
                    new SqlParameter("@ServerId", id),
                };

                int rowsAffected = await _centralContext.Database.ExecuteSqlRawAsync(@"
                    UPDATE dbo.Server_Data
                        SET active = -1,
                            ServerName = 'zobs_'+ ServerName
                    WHERE ServerId = @ServerId
                ", paramItems);

                return NoContent();
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message, statusCode: 500);
            }
        }
    }
}
