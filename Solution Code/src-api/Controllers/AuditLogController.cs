using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    [Route("api/v1/audit-log")]
    [ApiController]
    [Authorize]
    public class AuditLogController : ControllerBase
    {
        private readonly CentralDBContext _centralContext;
    
        public AuditLogController(CentralDBContext centralContext)
        {
            _centralContext = centralContext;
        }

        [HttpGet("types")]
        public async Task<ActionResult<Response_Transaction_Types_DTO[]>> GetTransactionTypes()
        {
            try 
            {
                string[] defaultColumns = ["username", "serverId", "serverName", "serverURL", "transactionDate"];

                Response_Transaction_Types_DTO[] types = [
                    new Response_Transaction_Types_DTO() {
                        name = "LOGIN",
                        columns = defaultColumns.Concat(["hasErrors", "message"]).ToArray() 
                    },
                    new Response_Transaction_Types_DTO() {
                        name = "ERROR",
                        columns = defaultColumns.Concat(["url", "error.message", "error.status", "error.name"]).ToArray()
                    }
                ];
                
                return Ok(types);
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message, statusCode: 500);
            }
        }

        [HttpGet("{type}")]
        public async Task<ActionResult<Response_Transaction_Log_DTO[]>> GetTransactionLogByType(string type, [FromQuery(Name = "startDate")] string startDate, [FromQuery(Name = "endDate")] string endDate)
        {
            try 
            {
                object[] paramItems = new object[]
                {
                    new SqlParameter("@TransactionType", type),
                    new SqlParameter("@StartDate", startDate),
                    new SqlParameter("@EndDate", endDate),
                };

                var result = await _centralContext.Database.SqlQueryRaw<Response_Transaction_Log_DTO>(@"
                    EXECUTE dbo.spLocal_GetTransactionLog @TransactionType, @StartDate, @EndDate 
                ", paramItems).ToListAsync();


                return Ok(result);
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message, statusCode: 500);
            }
        }
    }
}
