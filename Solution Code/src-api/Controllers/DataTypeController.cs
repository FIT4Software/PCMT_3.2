using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
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
    [Route("api/v1/data-types")]
    [ApiController]
    [Authorize]
    public class DataTypeController : ControllerBase
    {
        private readonly CentralDBContext _centralContext;
        private readonly SiteDBContext _siteContext;
    

        public DataTypeController(CentralDBContext centralContext, SiteDBContext siteContext)
        {
            _centralContext = centralContext;
            _siteContext = siteContext;
        }

        private async Task<Response_Data_Type_DTO> findDataType(int id)
        {
            _siteContext.setOnPremConnection(HttpContext.Items["Server"].ToString());

            object[] paramItems = new object[]
            {
                new SqlParameter("@DataTypeId", id),
            };

            var result = await _siteContext.Database.SqlQueryRaw<Data_Type>(@"
                SELECT * FROM dbo.data_type (NOLOCK) where data_type_id = @DataTypeId AND User_Defined = 1
            ", paramItems).ToListAsync();

            List<Response_Data_Type_DTO> mappedResult = result.Select(r => new Response_Data_Type_DTO
            {
                dataTypeId = r.Data_Type_Id,
                dataTypeDesc = r.Data_Type_Desc,
                usePrecision = r.Use_Precision,
                userDefined = r.User_Defined
            }).ToList();

            return mappedResult[0];
        }

        private async Task<List<Response_Phrase_DTO>> findPhrases(int id)
        {
            _siteContext.setOnPremConnection(HttpContext.Items["Server"].ToString());
                
            object[] paramItems = new object[]
            {
                new SqlParameter("@DataTypeId", id),
            };

            var result = await _siteContext.Database.SqlQueryRaw<Phrase>(@"
                SELECT * FROM dbo.Phrase (NOLOCK) WHERE Data_Type_Id = @DataTypeId ORDER BY Phrase_Order
            ", paramItems).ToListAsync();

            List<Response_Phrase_DTO> mappedResult = result.Select(r => new Response_Phrase_DTO
            {
                phraseId = r.Phrase_Id,
                dataTypeId = r.Data_Type_Id,
                oldPhrase = r.Old_Phrase,
                phraseOrder = r.Phrase_Order,
                phraseValue = r.Phrase_Value
            }).ToList();

            return mappedResult;
        }

        [HttpGet()]
        public async Task<ActionResult<List<Response_Data_Type_DTO>>> GetDataTypes([FromQuery(Name = "user-defined")] int userDefined)
        {
            try 
            {
                _siteContext.setOnPremConnection(HttpContext.Items["Server"].ToString());

                bool isUserDefined = userDefined == 0 ? false : true;

                var result = await _siteContext.Database.SqlQueryRaw<Data_Type>(isUserDefined ? @"
                    SELECT * FROM dbo.data_type (NOLOCK) WHERE User_Defined = 1 ORDER BY Data_Type_Desc
                " : @"
                    SELECT * FROM dbo.data_type (NOLOCK) ORDER BY Data_Type_Desc
                ").ToListAsync();

                List<Response_Data_Type_DTO> mappedResult = result.Select(r => new Response_Data_Type_DTO
                {
                    dataTypeId = r.Data_Type_Id,
                    dataTypeDesc = r.Data_Type_Desc,
                    usePrecision = r.Use_Precision,
                    userDefined = r.User_Defined
                }).ToList();

                return Ok(mappedResult);
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message, statusCode: 500);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Response_Data_Type_DTO>> GetDataTypeById(int id)
        {
            try 
            {
                return Ok(await findDataType(id));
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message, statusCode: 500);
            }
        }

        [HttpGet("{id}/phrases")]
        public async Task<ActionResult<List<Response_Phrase_DTO>>> GetPhrasesById(int id)
        {
            try 
            {
                return Ok(await findPhrases(id));
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message, statusCode: 500);
            }
        }

        // [HttpPost()]
        // public async Task<ActionResult<Response_Data_Type_DTO>> CreateUpdateDataType (Create_Update_Data_Type_DTO dataType)
        // {
        //     try 
        //     {
        //         _siteContext.setOnPremConnection(HttpContext.Items["Server"].ToString());

        //         var phrases = dataType.phrases;

        //         var phrasesToRemove = dataType.trash.phrases;

        //         int retrievedDataTypeId = 0;

        //         if(dataType.dataTypeId > 0) {
        //             retrievedDataTypeId = dataType.dataTypeId;

        //             if (string.IsNullOrEmpty(dataType.dataTypeDesc)) {
        //                 return Problem(@"
        //                     Data Type Name could not be empty
        //                 ");
        //             }

        //             if (dataType.dataTypeId == null) {
        //                 return Problem(@"
        //                     Data Type Id is not valid
        //                 ");
        //             }

        //             var dataTypeToUpdate = await findDataType(retrievedDataTypeId);
                    
        //             if(dataTypeToUpdate.dataTypeDesc != dataType.dataTypeDesc) {
        //                 object[] paramItems = new object[]
        //                 {
        //                     new SqlParameter("@Data_Type_Id", dataType.dataTypeId),
        //                     new SqlParameter("@Description", dataType.dataTypeDesc),
        //                     new SqlParameter("@User_Id", HttpContext.Items["PPAUserId"].ToString()),
        //                 };

        //                 int rowsAffected = await _siteContext.Database.ExecuteSqlRawAsync(@"
        //                     EXECUTE spEM_RenameDataType @Data_Type_Id, @Description, @User_Id
        //                 ", paramItems);
        //             }

        //             List<Response_Phrase_DTO> currentPhrases = await findPhrases(retrievedDataTypeId);
        //             Response_Phrase_DTO currentLastElement = currentPhrases.ElementAt(currentPhrases.Count() - 1);
        //             List<Response_Phrase_DTO> phrasesToUpdate = phrases.Where(p => p.phraseId > 0).ToList();
        //             List<Response_Phrase_DTO> phrasesToAdd = phrases.Where(p => p.phraseId <= 0).ToList();

        //             List<Response_Phrase_DTO> phrasesToUpdateTargetted = phrasesToUpdate.Where(p => currentPhrases.Find(cp => cp.phraseId == p.phraseId).phraseValue != p.phraseValue).ToList();

        //             foreach(Response_Phrase_DTO ptut in phrasesToUpdateTargetted) {
        //                 if (string.IsNullOrEmpty(ptut.phraseValue)) {
        //                     return Problem(@"
        //                         Phrase value could not be empty
        //                     ");
        //                 }

        //                 if (ptut.phraseId == null) {
        //                     return Problem(@"
        //                         Phrase id is not valid
        //                     ");
        //                 }

        //                 object[] paramItems = new object[]
        //                 {
        //                     new SqlParameter("@Phrase_Id", ptut.phraseId),
        //                     new SqlParameter("@New_Phrase_Value", ptut.phraseValue),
        //                     new SqlParameter("@HistoryRename", 1),
        //                     new SqlParameter("@User_Id", HttpContext.Items["PPAUserId"].ToString()),
        //                 };

        //                 int rowsAffected = await _siteContext.Database.ExecuteSqlRawAsync(@"
        //                     EXECUTE spEM_RenamePhrase @Phrase_Id, @New_Phrase_Value, @HistoryRename, @User_Id
        //                 ", paramItems);
        //             }

        //             List<Response_Phrase_DTO> phrasesToCreateTargetted = currentLastElement.Where(p => currentPhrases.Find(cp => cp.phraseId == p.phraseId).phraseValue != p.phraseValue).ToList();

        //         } else {
        //             var outputParameter = new SqlParameter();
        //             outputParameter.ParameterName = "@Data_Type_Id";
        //             outputParameter.SqlDbType = SqlDbType.Int;
        //             outputParameter.Direction = ParameterDirection.Output;

        //             object[] createParamItems = new object[]
        //             {
        //                 new SqlParameter("@Description", dataType.dataTypeDesc),
        //                 new SqlParameter("@User_Id", HttpContext.Items["PPAUserId"].ToString()),
        //                 outputParameter
        //             };                   

        //             await _siteContext.Database.ExecuteSqlRawAsync(@"
        //                 EXECUTE spEM_CreateDataType @Description, @User_Id, @Data_Type_Id OUT
        //             ", createParamItems);

        //             retrievedDataTypeId = Convert.ToInt32(outputParameter.Value);
        //         }

        //         return Ok(await findDataType(retrievedDataTypeId));
        //     }
        //     catch (Exception e)
        //     {
        //         return Problem(detail: e.Message, statusCode: 500);
        //     }
        // }
    }
}
