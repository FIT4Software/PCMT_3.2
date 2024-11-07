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
using NuGet.Protocol;

namespace src_api.Controllers
{
    [Route("api/v1/services")]
    [ApiController]
    [Authorize]
   public class ServiceController : ControllerBase
    {
        private readonly CentralDBContext _centralContext;
        private readonly SiteDBContext _siteContext;
    

        public ServiceController (CentralDBContext centralContext, SiteDBContext siteContext)
        {
            _centralContext = centralContext;
            _siteContext = siteContext;
        }

        
        [HttpGet]
        public async Task<ActionResult<Response_Service_DTO>> GetTransactionTypes(){
            try{
                var services = new List<Service>()
                {
                    new Service(19, "Calculation Manager", "CalculationMgr"),
                    new Service(17, "Alarm Manager", "AlarmMgr"),
                    new Service(8, "Stubber", "Stubber"),
                    new Service(7, "Summary Mgr", "SummaryMgr"),
                    new Service(5, "Reader", "Reader"),
                    new Service(6, "Writer", "Writer")
                };

                List<Response_Service_DTO> mappedResult = services.Select(r => new Response_Service_DTO
                {
                    serviceId = r.Service_Id,
                    serviceDesc = r.Service_Desc,
                    serviceDisplay = r.Service_Desc
                }).ToList();

                return Ok(mappedResult);

            } catch (Exception e)
            {
                return Problem(detail: e.Message, statusCode: 500);
            }
        
            
        }
        

        [HttpPut]
        public async Task<ActionResult<Request_Reload_Service_Ids_DTO>> AddEdit([FromBody] Request_Reload_Service_Ids_DTO serviceId)
        {
            try
            {
                _siteContext.setOnPremConnection(HttpContext.Items["Server"].ToString());
                List<int>? responses = new List<int>();
                int reloadFlag = 2;

                foreach (int id in serviceId.serviceIds)
                {
                    Console.WriteLine(id);
                    object[] paramItems = new object[]
                    {
                        new SqlParameter("@Service_Id", id),
                        new SqlParameter("@Time_Stamp", DateTime.UtcNow),
                        new SqlParameter("@ReloadFlag", reloadFlag),
                        new SqlParameter("@User_Id", HttpContext.Items["PPAUserId"])
                    };
                   
                    var result = await _siteContext.Database.ExecuteSqlRawAsync(@"
                        EXECUTE spEM_ReloadService @Service_Id, @Time_Stamp, @ReloadFlag, @User_Id
                        ", paramItems);
                    responses.Add(result);
                }

                string serviceIdString = string.Join(",", serviceId.serviceIds.ToString());
                
                var logItems = new object[]
                {
                    new SqlParameter("@ServerId", Int32.Parse(HttpContext.Items["ServerId"].ToString())),
                    new SqlParameter("@TransactionType", "RELOAD SERVICES"),
                    new SqlParameter("@UserName", HttpContext.User.Identity.Name.Split('\\')[1]),
                    new SqlParameter("@HasErrors", false),
                    new SqlParameter("@TransactionDetails", "Services: " + serviceIdString)
                };
                
                var command = await _centralContext.Database.ExecuteSqlRawAsync(@"
                    EXECUTE spLocal_AddTransactionLog @ServerId, @TransactionType, @UserName, @HasErrors, @TransactionDetails", logItems);

                return Ok(serviceId);
            }
            catch (Exception e)
            {
                var logItems = new object[]
                {
                    new SqlParameter("@ServerId", Int32.Parse(HttpContext.Items["ServerId"].ToString())),
                    new SqlParameter("@TransactionType", "RELOAD SERVICES"),
                    new SqlParameter("@UserName", HttpContext.User.Identity.Name.Split('\\')[1]),
                    new SqlParameter("@HasErrors", true),
                    new SqlParameter("@TransactionDetails", e.Message)
                };
                
                var command = await _centralContext.Database.ExecuteSqlRawAsync(@"
                    EXECUTE spLocal_AddTransactionLog @ServerId, @TransactionType, @UserName, @HasErrors, @TransactionDetails", logItems);
                
                return Problem(detail: e.Message, statusCode: 500);
            }
        }
   }
}