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
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NuGet.Protocol.Core.Types;
using query;
using System.Reflection.Metadata.Ecma335;
using Utilities;
using Azure;

namespace src_api.Controllers
{
    [Route("api/v1/variables")]
    [ApiController]
    [Authorize]
     public class VariableController : ControllerBase
    {
        private readonly CentralDBContext _centralContext;
        private readonly SiteDBContext _siteContext;
        private readonly AuditLogger auditLog;
    

        public VariableController (CentralDBContext centralContext, SiteDBContext siteContext)
        {
            _centralContext = centralContext;
            _siteContext = siteContext;
            auditLog = new AuditLogger(_centralContext);
        }
    

        [HttpGet("data-sources")]
        public async Task<ActionResult<List<Response_Data_Source_DTO>>> GetDataSources()
        {
            try
            {
                _siteContext.setOnPremConnection(HttpContext.Items["Server"].ToString());
                var result = await _siteContext.Database.SqlQueryRaw<Data_Source>(
                    @"SELECT DS_Id, DS_Desc
                    FROM dbo.Data_Source (NOLOCK)
                    WHERE DS_Desc IN ('Autolog', 'Base Unit', 'Base Variable', 'Historian', 'Undefined')
                    OR DS_Id > 50000"
                ).ToListAsync();

                List <Response_Data_Source_DTO> mappedResult = result.Select(r => new Response_Data_Source_DTO{
                    dsId = r.DS_Id,
                    dsDesc = r.DS_Desc
                }).ToList();

                return Ok(mappedResult);
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message, statusCode: 500);
            }
      }

      [HttpGet("sampling-types")]
        public async Task<ActionResult<List<Response_Sampling_Type_DTO>>> GetSamplingTypes()
        {
            try
            {
                _siteContext.setOnPremConnection(HttpContext.Items["Server"].ToString());
                var result = await _siteContext.Database.SqlQueryRaw<SamplingType>(
                    @" SELECT ST_Id, ST_Desc
                    FROM dbo.Sampling_Type (NOLOCK)"
                ).ToListAsync();

                List <Response_Sampling_Type_DTO> mappedResult = result.Select(r => new Response_Sampling_Type_DTO{
                    stId =  r.ST_Id,
                    stDesc = r.ST_Desc
                }).ToList();

                return Ok(mappedResult);
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message, statusCode: 500);
            }
      }

    [HttpGet("eng-units")]
        public async Task<ActionResult<List<Response_Eng_Units_DTO>>> GetEngUnits()
        {
            try
            {
                _siteContext.setOnPremConnection(HttpContext.Items["Server"].ToString());
                var result = await _siteContext.Database.SqlQueryRaw<EngUnits>(
                    @" SELECT DISTINCT v.Eng_Units
                    FROM dbo.Variables_Base v (NOLOCK)
                        JOIN dbo.event_subtypes es ON v.event_subtype_id = es.event_subtype_id
                    WHERE v.eng_units IS NOT NULL AND es.event_subtype_Desc LIKE '%%'
                    ORDER BY v.eng_units"
                ).ToListAsync();

                List <Response_Eng_Units_DTO> mappedResult = result.Select(r => new Response_Eng_Units_DTO{
                    engUnits = r.Eng_Units
                }).ToList();

                return Ok(mappedResult);
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message, statusCode: 500);
            }
      }

      [HttpGet("spc-types")]
        public async Task<ActionResult<List<Response_SPC_Type_DTO>>> GetSPCTypes()
        {
            try
            {
                _siteContext.setOnPremConnection(HttpContext.Items["Server"].ToString());
                var result = await _siteContext.Database.SqlQueryRaw<SPC_Type>(
                    @" SELECT SPC_Calculation_Type_Id, SPC_Calculation_Type_Desc 
                        FROM dbo.SPC_Calculation_Types (NOLOCK)
                        ORDER BY SPC_Calculation_Type_Desc ASC"
                ).ToListAsync();

                List <Response_SPC_Type_DTO> mappedResult = result.Select(r => new Response_SPC_Type_DTO{
                    spcCalculationTypeId = r.SPC_Calculation_Type_Id,
                    spcCalculationTypeDesc = r.SPC_Calculation_Type_Desc
                }).ToList();

                return Ok(mappedResult);
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message, statusCode: 500);
            }
      }

      [HttpGet("extended-infos")]
        public async Task<ActionResult<List<Response_Extended_Info_DTO>>> GetExtendedInfo()
        {
            try
            {
                _siteContext.setOnPremConnection(HttpContext.Items["Server"].ToString());
                var result = await _siteContext.Database.SqlQueryRaw<Variable>(
                    @" SELECT DISTINCT v.Extended_Info 
                        FROM dbo.variables_base v (NOLOCK)
                        WHERE v.extended_info IS NOT NULL 
                        AND v.extended_info <> ''
                        AND LTRIM(RTRIM(v.extended_info)) <> ''"
                ).ToListAsync();

                List <Response_Extended_Info_DTO> mappedResult = result.Select(r => new Response_Extended_Info_DTO{
                    extendedInfo = r.Extended_Info
                }).ToList();

                return Ok(mappedResult);
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message, statusCode: 500);
            }
      }


      [HttpGet("event-types")]
        public async Task<ActionResult<List<Response_Event_Types_DTO>>> GetEventTypes()
        {
            try
            {
                _siteContext.setOnPremConnection(HttpContext.Items["Server"].ToString());
                var result = await _siteContext.Database.SqlQueryRaw<EventType>(
                    @" SELECT ET_Id, ET_Desc
                    FROM dbo.Event_Types (NOLOCK)"
                ).ToListAsync();

                List <Response_Event_Types_DTO> mappedResult = result.Select(r => new Response_Event_Types_DTO{
                    etId = r.ET_Id,
                    etDesc = r.ET_Desc
                }).ToList();

                return Ok(mappedResult);
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message, statusCode: 500);
            }
      }


      [HttpGet("event-sub-types")]
        public async Task<ActionResult<List<Response_Event_Types_DTO>>> GetEventSubTypes([FromQuery(Name = "event-type-id")] int eventTypeId)
        {
            try
            {
                _siteContext.setOnPremConnection(HttpContext.Items["Server"].ToString());
                bool isEventType = (eventTypeId != 1 && eventTypeId != 14) ? true : false;
                if (isEventType){
                    object[] paramItems = new object[]
                    {
                        new SqlParameter("@etId", eventTypeId),
                    };

                    var result = await _siteContext.Database.SqlQueryRaw<EventSubtype>(
                                @" SELECT Event_Subtype_Id, Event_Subtype_Desc
                                FROM dbo.Event_Subtypes (NOLOCK)
                                WHERE ET_Id = @etId", paramItems
                                ).ToListAsync();

                    List <Response_Event_SubTypes_DTO> mappedResult = result.Select(r => new Response_Event_SubTypes_DTO{
                        eventSubType = r.Event_Subtype_Id,
                        eventSubTypeDesc = r.Event_Subtype_Desc
                    }).ToList();

                    return Ok(mappedResult);

                } else {
                    return Ok(new List<Response_Event_SubTypes_DTO>());
                }
                
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message, statusCode: 500);
            }
      }

    [HttpGet("sampling-interval")]
        public async Task<ActionResult<List<Response_Sampling_Interval_DTO>>> GetSamplingInterval()
        {
            try
            {
                _siteContext.setOnPremConnection(HttpContext.Items["Server"].ToString());
                var result = await _siteContext.Database.SqlQueryRaw<Sampling_Intervals>(
                    @" SELECT DISTINCT v.Sampling_Interval
                    FROM dbo.Variables_Base v (NOLOCK)
                        JOIN dbo.Prod_Units_Base pu (NOLOCK) ON (v.pu_id = pu.pu_id)
                        JOIN dbo.Event_Types et (NOLOCK) ON (et.et_id = v.event_type)
                    WHERE 
                            et.et_desc = 'Time' AND
                            v.sampling_interval IS NOT NULL
                    ORDER BY v.sampling_interval"
                ).ToListAsync();

                List <Response_Sampling_Interval_DTO> mappedResult = result.Select(r => new Response_Sampling_Interval_DTO{
                    samplingInterval = r.Sampling_Interval
                }).ToList();

                return Ok(mappedResult);
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message, statusCode: 500);
            }
 
      }
      [HttpGet("sampling-offset")]
        public async Task<ActionResult<List<Response_Sampling_Offset_DTO>>> GetSampleOffset()
        {
            try
            {
                _siteContext.setOnPremConnection(HttpContext.Items["Server"].ToString());
                var result = await _siteContext.Database.SqlQueryRaw<Sampling_Offsets>(
                    @" SELECT DISTINCT v.Sampling_Offset
                    FROM dbo.Variables_Base v (NOLOCK)
                        JOIN dbo.Prod_Units_Base pu (NOLOCK) ON (v.pu_id = pu.pu_id)
                        JOIN dbo.Event_Types et (NOLOCK) ON (et.et_id = v.event_type)
                    WHERE 
                            et.et_desc = 'Time' AND
                            v.sampling_offset IS NOT NULL
                    ORDER BY v.sampling_offset"
                ).ToListAsync();

                List <Response_Sampling_Offset_DTO> mappedResult = result.Select(r => new Response_Sampling_Offset_DTO{
                    samplingOffset = r.Sampling_Offset
                }).ToList();

                return Ok(mappedResult);
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message, statusCode: 500);
            }
 
      }
    
      [HttpGet("sub-group-sizes")]
        public async Task<ActionResult<List<Response_Sub_Group_Size_DTO>>> GetSubGroupSize()
        {
            try
            {
                var list = Enumerable.Range(0, 26).Select(r => new Response_Sub_Group_Size_DTO{
                    sizeId = r
                }).ToList();
                list.RemoveRange(1,1);
                return list;
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message, statusCode: 500);
            }
 
      }

    [HttpGet("comparison-operators")] 
        public async Task<ActionResult<List<Response_Comparison_Operator_DTO>>> GetComparisonOperators()
        {
            try
            {
                _siteContext.setOnPremConnection(HttpContext.Items["Server"].ToString());
                var result = await _siteContext.Database.SqlQueryRaw<Comparison_Operators>(
                    @" SELECT c.Comparison_Operator_Id, c.Comparison_Operator_Value
                    FROM dbo.Comparison_Operators c (NOLOCK)"
                ).ToListAsync();

                List <Response_Comparison_Operator_DTO> mappedResult = result.Select(r => new Response_Comparison_Operator_DTO{
                    comparisonOperatorId = r.Comparison_Operator_Id,
                    comparisonOperatorValue = r.Comparison_Operator_Value
                }).ToList();

                if (result == null || !result.Any())
                {
                    return NotFound("No comparison operators found.");
                }

                return Ok(mappedResult);
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message, statusCode: 500);
            }
 
      }
    
    [HttpGet("calculations")]
        public async Task<ActionResult<List<Response_Calculation_DTO>>> GetCalculations()
        {
            try
            {
                _siteContext.setOnPremConnection(HttpContext.Items["Server"].ToString());
                var result = await _siteContext.Database.SqlQueryRaw<Calculation>(
                    @" SELECT Calculation_Id, Calculation_Name 
                    FROM dbo.calculations (NOLOCK) 
                    ORDER BY 
                    Calculation_Name ASC"
                ).ToListAsync();

                List <Response_Calculation_DTO> mappedResult = result.Select(r => new Response_Calculation_DTO{
                    calculationId = r.Calculation_Id,
                    calculationName = r.Calculation_Name
                }).ToList();

                return Ok(mappedResult);
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message, statusCode: 500);
            }
      }

      [HttpGet("calculations/Data")] 
        public async Task<ActionResult<List<Response_Calculation_Input_Data_DTO>>> GetCalculationInputData([FromQuery(Name = "calculationid")] int calculationid, [FromQuery(Name = "variableId")] int variableId)
        {
            try
            {
                object[] paramItems = new object[]
                {
                    new SqlParameter("@calculationId", calculationid),
                    new SqlParameter("@varId", variableId)
                };
                _siteContext.setOnPremConnection(HttpContext.Items["Server"].ToString());
                var result = await _siteContext.Database.SqlQueryRaw<CalculationInputData>(
                    @" SELECT ci.Calc_Input_Id, cid.Member_Var_Id, 
                        ISNULL(cid.input_name,ci.input_name) AS Input_Name, 
                        cie.Entity_Name, 
                        v.Var_Desc,
                        ISNULL(cid.default_value, ci.default_value) AS Default_Value
                    FROM dbo.calculation_inputs ci (NOLOCK)
                        JOIN dbo.calculation_input_entities cie (NOLOCK)
                            ON (ci.calc_input_entity_id = cie.calc_input_entity_id)
                        LEFT JOIN dbo.calculation_input_data cid (NOLOCK)  
                            ON (ci.calc_input_id = cid.calc_input_id AND cid.result_var_id =@varId)
                        LEFT JOIN dbo.variables_Base v (NOLOCK) 
                            ON (v.var_id = cid.member_var_id) 
                    WHERE ci.calculation_id = @calculationId AND
                        ci.calc_input_entity_id IN (1, 3)      
                    ORDER BY  ci.calc_input_order ASC", paramItems
                ).ToListAsync();

                List <Response_Calculation_Input_Data_DTO> mappedResult = result.Select(r => new Response_Calculation_Input_Data_DTO{
                    calculationInputId = r.Calc_Input_Id,
                    calculationInputName = r.Input_Name,
                    entityName = r.Entity_Name,
                    memberVarId = r.Member_Var_Id,
                    defaultValue = r.Default_Value,
                    varDesc = r.Var_Desc
                }).ToList();

                return Ok(mappedResult);
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message, statusCode: 500);
            }
 
      }

      [HttpGet("calculations/{id}")]
      public async Task<ActionResult<List<Response_Calculation_Input_DTO>>> GetCalculationInputs(int id)
        {
            object[] paramItems = new object[]
            {
                new SqlParameter("@calculationid", id),
            };

            try{
                _siteContext.setOnPremConnection(HttpContext.Items["Server"].ToString());
                var result = await _siteContext.Database.SqlQueryRaw<CalculationInputData>(
                    @"SELECT ci.Calc_Input_Id, cid.Member_Var_Id, 
                        ISNULL(cid.input_name,ci.input_name) AS Input_Name, 
                        cie.Entity_Name, 
                        pl.pl_desc + '\' + pu.pu_desc + '\' + pug.pug_desc + '\' + v.var_desc AS var_desc,
                        ISNULL(cid.default_value, ci.default_value) AS default_value
                    FROM dbo.calculation_inputs ci (NOLOCK)
                        JOIN dbo.calculation_input_entities cie (NOLOCK) 
                                ON (ci.calc_input_entity_id = cie.calc_input_entity_id)
                        LEFT JOIN dbo.calculation_input_data cid (NOLOCK)  
                                ON (ci.calc_input_id = cid.calc_input_id AND cid.result_var_id = null)
                        LEFT JOIN dbo.variables_base v (NOLOCK)
                                ON (v.var_id = cid.member_var_id) 
                        LEFT JOIN dbo.Prod_Units_Base pu (NOLOCK) ON (pu.pu_id = v.pu_id)
                        LEFT JOIN dbo.Pu_Groups pug (NOLOCK) ON (pug.pug_id = v.pug_id)
                        LEFT JOIN dbo.Prod_Lines_Base pl (NOLOCK) ON (pl.pl_id = pu.pl_id)  
                    WHERE ci.calculation_id = @calculationId AND
                            ci.calc_input_entity_id IN (1, 3)      
                    ORDER BY  ci.calc_input_order ASC", paramItems
                            ).ToListAsync();

                List <Response_Calculation_Input_DTO> mappedResult = result.Select(r => new Response_Calculation_Input_DTO{
                    calculationInputId = r.Calc_Input_Id,
                    calculationInputName = r.Input_Name,
                    entityName = r.Entity_Name,
                }).ToList();

                return Ok(mappedResult);
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message, statusCode: 500);
            }
 
      }

      [HttpGet()]
      public async Task<ActionResult<List<Response_Variable_All_DTO>>> GetAll([FromQuery(Name = "pug-ids")] int [] pugs, 
                                                    [FromQuery(Name = "master-unit")] int masterUnitId = 0,
                                                    [FromQuery(Name = "event-subtype-id")] int? eventSubTypeId =0,
                                                    [FromQuery(Name = "event-type-id")] int? eventTypeId = -1,
                                                    [FromQuery(Name = "var-desc")] string? varDesc = "",
                                                    [FromQuery(Name = "pl-id")] int plId = 0,
                                                    [FromQuery(Name = "for-alarm-display")] bool forAlarmDisplay = false){
            try{
                _siteContext.setOnPremConnection(HttpContext.Items["Server"].ToString());
                bool varDescNotEmpty = !string.IsNullOrEmpty(varDesc);
                
                var data = masterUnitId > 0 || varDescNotEmpty;
                string filterByAlarmTemplate = forAlarmDisplay
                                                ? "JOIN dbo.Alarm_Template_Var_Data atvd (NOLOCK) ON atvd.Var_Id = v.Var_Id"
                                                : "";
                // var eventTypeIdNotNull = eventTypeId >= 0 && eventSubTypeId <= 0 ? eventTypeId : null;
                object[] paramItems;
                string query = "";
                
                if (data) {
                    paramItems = new object[] {
                        new SqlParameter("@masterUnit", masterUnitId),
                        new SqlParameter("@eventSubTypeId", eventSubTypeId),
                        new SqlParameter("@eventType", eventTypeId),
                        new SqlParameter("@plId", plId),
                        new SqlParameter("@varDesc", varDesc)
                    };

                    
                    
                    query = @"SELECT 
                        Var_Id, 
                        Var_Desc, 
                        Var_Desc_Global, 
                        v.PU_Id, 
                        v.PUG_Id, 
                        v.PUG_Order,
                        v.DS_Id,
                        Data_Type_Id,
                        Var_Precision,
                        v.Eng_Units,
                        v.Group_Id,
                        v.Event_SubType_Id,
                        evst.Event_SubType_Desc,
                        pug.PUG_Desc,
                        pu.PU_Desc,
                        pl.PL_Desc,
                        pl.PL_Id,
                        evt.ET_Id,
                        evt.ET_Desc,
                        ds.DS_Desc
                    FROM dbo.Variables_Base v (NOLOCK)
                    JOIN dbo.PU_Groups pug (NOLOCK) ON v.PUG_Id = pug.PUG_Id
                    JOIN dbo.Prod_Units_Base pu (NOLOCK) ON pug.PU_Id = pu.PU_Id
                    JOIN dbo.Prod_lines_Base pl (NOLOCK) ON pu.PL_Id = pl.PL_Id
                    LEFT JOIN Event_Types evt (NOLOCK) ON v.Event_Type = evt.ET_Id
                    LEFT JOIN Event_SubTypes evst (NOLOCK) ON v.Event_SubType_Id = evst.Event_SubType_Id
                    LEFT JOIN dbo.Data_Source ds (NOLOCK) ON v.DS_Id = ds.DS_Id
                    WHERE (v.PU_Id IN (SELECT PU_Id FROM Prod_Units_Base (NOLOCK) WHERE Master_Unit = @masterUnit) OR v.PU_Id = @masterUnit OR  @masterUnit = 0)
                    AND (v.Event_Subtype_Id = @eventSubtypeId OR @eventSubtypeId = 0) 
                    AND (v.Event_Type = @eventType OR @eventType IS NULL)
                    AND (Var_Desc = @varDesc OR @varDesc = '')
                    AND (pl.PL_Id = @plId OR @plId = 0)";
                } else{
                    paramItems = new object[] {
                        new SqlParameter("@pugs", string.Join(",", pugs)),
                        new SqlParameter("@eventTypeId", eventTypeId)
                    };

                    query = @"SELECT DISTINCT
                        v.Var_Id, 
                        Var_Desc, 
                        ISNULL(Var_Desc_Global, Var_Desc) Var_Desc_Global, 
                        v.PU_Id, 
                        v.PUG_Id, 
                        v.PUG_Order,
                        v.DS_Id,
                        Data_Type_Id,
                        Var_Precision,
                        v.Eng_Units,
                        v.Group_Id,
                        v.Event_SubType_Id,
                        evst.Event_SubType_Desc,
                        pug.PUG_Desc,
                        pu.PU_Desc,
                        pl.PL_Desc,
                        pl.PL_Id,
                        evt.ET_Id,
                        evt.ET_Desc,
                        ds.DS_Desc
                    FROM dbo.Variables_Base v (NOLOCK)
                    JOIN dbo.PU_Groups pug (NOLOCK) ON v.PUG_Id = pug.PUG_Id
                    JOIN dbo.Prod_Units_Base pu (NOLOCK) ON pug.PU_Id = pu.PU_Id
                    JOIN dbo.Prod_lines_Base pl (NOLOCK) ON pu.PL_Id = pl.PL_Id
                    ${filterByAlarmTemplate}
                    LEFT JOIN Event_Types evt (NOLOCK) ON v.Event_Type = evt.ET_Id
                    LEFT JOIN Event_Subtypes evst (NOLOCK) ON v.Event_Subtype_Id = evst.Event_Subtype_Id
                    LEFT JOIN dbo.Data_Source ds (NOLOCK) ON v.DS_Id = ds.DS_Id
                    WHERE v.PUG_Id IN (SELECT value
                        FROM STRING_SPLIT(@pugs, ',')) 
                        AND (v.Event_Type = @eventTypeId OR @eventTypeId IS NULL)";
                }
                var result = await _siteContext.Database.SqlQueryRaw<Variable_All>(query, paramItems).ToListAsync();
                
                List <Response_Variable_All_DTO> mappedResult = result.Select(r => new Response_Variable_All_DTO{
                    varId = r.Var_Id,
                    varDesc = r.Var_Desc,
                    varDescGlobal = r.Var_Desc_Global,
                    pugId = r.PUG_Id,
                    pugDesc = r.PUG_Desc,
                    puId = r.PU_Id,
                    puDesc = r.PU_Desc,
                    pugOrder = r.PUG_Order,
                    dsId = r.DS_Id,
                    dataTypeId = r.Data_Type_Id,
                    varPrecision = r.Var_Precision,
                    engUnits = r.Eng_Units,
                    groupId = r.Group_Id,
                    plId = r.PL_Id,
                    plDesc = r.PL_Desc,
                    eventSubTypeId = r.Event_Subtype_Id,
                    eventSubTypeDesc = r.Event_Subtype_Desc,
                    etId = r.ET_Id,
                    etDesc = r.ET_Desc,
                    dsDesc = r.DS_Desc
                }).ToList();

                return Ok(mappedResult);
            } catch(Exception e)
            {
                return Problem(detail: e.Message, statusCode: 500);
            }

      }
      [HttpGet("search")]
      public async Task<ActionResult<List<Response_Search_DTO>>> Search([FromQuery(Name = "pl-id")] int plId,
                                                                            [FromQuery(Name = "pug-desc")] string pugDesc,
                                                                            [FromQuery (Name = "var-desc")]string varDesc,
                                                                            [FromQuery(Name = "active")] bool isActive)
        {
            object[] paramItems = new object[]
            {
                new SqlParameter("@line", plId),
                new SqlParameter("@pugDesc", pugDesc),
                new SqlParameter ("@varDesc", varDesc),
                new SqlParameter("@active", isActive ? 1 : 0)
            };

            try
            {
                _siteContext.setOnPremConnection(HttpContext.Items["Server"].ToString());
                var result = await _siteContext.Database.SqlQueryRaw<Variable_Lookup>(
                    @" SELECT 
                            Var_Id, 
                            Var_Desc,
                            Var_Desc_Global, 
                            v.PU_Id, 
                            v.PUG_Id, 
                            v.PUG_Order,
                            DS_Id,
                            Data_Type_Id,
                            Var_Precision,
                            Eng_Units,
                            v.Group_Id,
                            pug.PUG_Desc,
                            pu.PU_Desc,
                            pl.PL_Desc,
                            pl.PL_Id
                    FROM dbo.Variables_Base v (NOLOCK)
                    JOIN dbo.PU_Groups pug (NOLOCK) ON v.PUG_Id = pug.PUG_Id
                    JOIN dbo.Prod_Units_Base pu (NOLOCK) ON pug.PU_Id = pu.PU_Id
                    JOIN dbo.Prod_lines_Base pl (NOLOCK) ON pu.PL_Id = pl.PL_Id
                    WHERE Is_Active = @active AND (pl.PL_Id = @line OR @line IS NULL) AND PUG_Desc LIKE '%' + @pugDesc + '%' AND (Var_Desc LIKE '%'+ @varDesc + '%' OR Var_Desc_Global LIKE '%' + @varDesc + '%')", paramItems
                ).ToListAsync();
                Console.WriteLine(result);

                List <Response_Search_DTO> mappedResult = result.Select(r => new Response_Search_DTO{
                    varId = r.Var_Id,
                    varDesc = r.Var_Desc,
                    varDescGlobal = r.Var_Desc_Global,
                    puId = r.PU_Id,
                    pugId = r.PUG_Id,
                    pugOrder = r.PUG_Order,
                    dsId = r.DS_Id,
                    dataTypeId = r.Data_Type_Id,
                    varPrecision = r.Var_Precision,
                    engUnits = r.Eng_Units,
                    groupId = r.Group_Id,
                    plId = r.PL_Id,
                    plDesc = r.PL_Desc,
                    puDesc = r.PU_Desc,
                    pugDesc = r.PUG_Desc
                }).ToList();

                return Ok(mappedResult);
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message, statusCode: 500);
            }
 
      }

      [HttpGet("lookup")]
      public async Task<ActionResult<List<Response_Lookup_DTO>>> LookUp([FromQuery (Name = "var-desc")]string varDesc)
        {
            object[] paramItems = new object[]
            {
                new SqlParameter ("@varDesc", varDesc),
            };

            try
            {
                _siteContext.setOnPremConnection(HttpContext.Items["Server"].ToString());
                var result = await _siteContext.Database.SqlQueryRaw<Variable_Lookup>(
                    @" SELECT 
                        Var_Id, 
                        Var_Desc,
                        Var_Desc_Global, 
                        v.PU_Id, 
                        v.PUG_Id, 
                        v.PUG_Order,
                        DS_Id,
                        Data_Type_Id,
                        Var_Precision,
                        Eng_Units,
                        v.Group_Id,
                        pug.PUG_Desc,
                        pu.PU_Desc,
                        pl.PL_Desc,
                        pl.PL_Id
                FROM dbo.Variables_Base v (NOLOCK)
                JOIN dbo.PU_Groups pug (NOLOCK) ON v.PUG_Id = pug.PUG_Id
                JOIN dbo.Prod_Units_Base pu (NOLOCK) ON pug.PU_Id = pu.PU_Id
                JOIN dbo.Prod_lines_Base pl (NOLOCK) ON pu.PL_Id = pl.PL_Id
                WHERE (Var_Desc LIKE '%'+ @varDesc + '%' OR Var_Desc_Global LIKE '%' + @varDesc + '%')", paramItems
                ).ToListAsync();

                List <Response_Lookup_DTO> mappedResult = result.Select(r => new Response_Lookup_DTO{
                    varId = r.Var_Id,
                    varDesc = r.Var_Desc,
                    varDescGlobal = r.Var_Desc_Global,
                    puId = r.PU_Id,
                    pugId = r.PUG_Id,
                    pugOrder = r.PUG_Order,
                    dsId = r.DS_Id,
                    dataTypeId = r.Data_Type_Id,
                    varPrecision = r.Var_Precision,
                    engUnits = r.Eng_Units,
                    groupId = r.Group_Id,
                    plId = r.PL_Id,
                    plDesc = r.PL_Desc,
                    puDesc = r.PU_Desc,
                    pugDesc = r.PUG_Desc
                }).ToList();

                return Ok(mappedResult);
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message, statusCode: 500);
            }
      }


      private async Task<Response_Find_DTO> FindSheetData(Variable_Search Variable){
        object[] paramItems = new object[]
            {
                new SqlParameter ("@Var_Id", Variable.Var_Id),
            };
        _siteContext.setOnPremConnection(HttpContext.Items["Server"].ToString());
        var result = await _siteContext.Database.SqlQueryRaw<VariableSheet>(@"
                             EXECUTE spEM_GetVarSheetData @Var_Id
                         ", paramItems).ToListAsync();
        
         List <Response_Find_DTO> variablemapped = result.Select(r => new Response_Find_DTO{
                specId = r.Spec_Id,
                specDescs = r.Spec_Desc,
                propDesc = r.Prop_Desc,
                samplingInterval = r.Sampling_Interval,
                samplingOffset = r.Sampling_Offset,
                samplingType = r.Sampling_Type,
                samplingWindow = r.Sampling_Window,
                samplingWindowType = r.Sampling_Window_Type,
                eventType = r.Event_Type,
                varReject = r.Var_Reject,
                unitReject = r.Unit_Reject,
                rank = r.Rank,
                inputTag = r.Input_Tag,
                inputTag2 = r.Input_Tag2,
                outputTag = r.Output_Tag,
                dqTag = r.DQ_Tag,
                uelTag = r.UEL_Tag,
                urlTag = r.URL_Tag,
                uwlTag = r.UWL_Tag,
                uulTag = r.UUL_Tag,
                targetTag = r.Target_Tag,
                lulTag = r.LUL_Tag,
                lwlTag = r.LWL_Tag,
                lrlTag = r.LRL_Tag,
                lelTag = r.LEL_Tag,
                totFactor = r.Tot_Factor,
                tfReset = r.TF_Reset,
                saId = r.SA_Id,
                comparisonOperatorId = r.Comparison_Operator_Id,
                comparisonValue = r.Comparison_Value,
                repeating = r.Repeating,
                repeatBackTime = r.Repeat_Backtime,
                extendedInfo = r.Extended_Info,
                userDefined1 = r.User_Defined1,
                userDefined2 = r.User_Defined2,
                userDefined3 = r.User_Defined3,
                unitSummarize = r.Unit_Summarize,
                forceSignEntry = r.Force_Sign_Entry,
                extendedTestFreq = r.Extended_Test_Freq,
                maxRPM = r.Max_RPM,
                resetValue = r.Reset_Value,
                isConformanceVariable = r.Is_Conformance_Variable,
                eSignatureLevel = r.Esignature_Level,
                eventSubTypeId = r.Event_Subtype_Id,
                eventDimension = r.Event_Dimension,
                peiId = r.PEI_Id,
                spcCalculationTypeId = r.SPC_Calculation_Type_Id,
                spcGroupVariableTypeId = r.SPC_Group_Variable_Type_Id,
                samplingReferenceVarId = r.Sampling_Reference_Var_Id,
                stringSpecificationSetting = r.String_Specification_Setting,
                writeGroupDSId = r.Write_Group_DS_Id,
                reloadFlag = r.Reload_Flag,
                readLagTime = r.ReadLagTime,
                shouldArchive = r.Should_Archive,
                eventLookup = r.EventLookup,
                ignoreEventStatus = r.Ignore_Event_Status
                }).ToList();
        Console.WriteLine("Variable Mapped");
        Console.WriteLine(variablemapped[0]);
        return variablemapped[0];
      }

      

      [HttpGet("{id}")]
      public async Task<ActionResult<Response_Find_DTO>> FindVariable (string id)
        {
            object[] paramItems = new object[]
            {
                new SqlParameter ("@varId", id),
            };

            _siteContext.setOnPremConnection(HttpContext.Items["Server"].ToString());
            var result = await _siteContext.Database.SqlQueryRaw<Variable_Search>(
                @" SELECT 
                    Var_Id, 
                    Var_Desc, 
                    Var_Desc_Global, 
                    pu.PU_Id,
                    pu.PU_Desc, 
                    pug.PUG_Id, 
                    pug.PUG_Order,
                    pug.PUG_Desc,
                    DS_Id,
                    Data_Type_Id,
                    Var_Precision,
                    Eng_Units,
                    v.Group_Id,
                    v.External_Link,
                    v.Calculation_Id,
                    pl.PL_Id,
                    pl.PL_Desc
                FROM dbo.Variables_Base v (NOLOCK)
                JOIN dbo.PU_Groups pug (NOLOCK) ON v.PUG_Id = pug.PUG_Id 
                JOIN dbo.Prod_Units_Base pu (NOLOCK) ON v.PU_Id = pu.PU_Id
                JOIN dbo.Prod_Lines_Base pl (NOLOCK) ON pu.PL_Id = pl.PL_Id 
                WHERE v.Var_Id = @varId", paramItems
            ).ToListAsync();
            
            List <Response_Find_DTO> mappedResult = result.Select(r => new Response_Find_DTO{
                varId = r.Var_Id,
                varDesc = r.Var_Desc,
                varDescGlobal = r.Var_Desc_Global,
                puId = r.PU_Id,
                pugId = r.PUG_Id,
                pugOrder = r.PUG_Order,
                dsId = r.DS_Id,
                dataTypeId = r.Data_Type_Id,
                varPrecision = r.Var_Precision,
                engUnits = r.Eng_Units,
                groupId = r.Group_Id,
                plId = r.PL_Id,
                plDesc = r.PL_Desc,
                puDesc = r.PU_Desc,
                pugDesc = r.PUG_Desc,
            }).ToList();

            var variable = (mappedResult.Count > 0) ? mappedResult[0] : null;

            return Ok((variable != null) ? await FindSheetData(result[0]) : null);
      }

      [HttpGet("{id}/udp")]
        public async Task<ActionResult<List<Response_UDP_DTO>>> GetUDP(int id, string tableDesc = "Variables")
            {
                object[] paramItems = new object[]
                {
                    new SqlParameter ("@keyId", id),
                    new SqlParameter ("@tableDesc", tableDesc)
                };
    
                try
                {
                    _siteContext.setOnPremConnection(HttpContext.Items["Server"].ToString());
                    var result = await _siteContext.Database.SqlQueryRaw<UDP>(
                        @" SELECT DISTINCT tfv.Table_Field_Id, tfv.TableId, Value, KeyId, tf.Table_Field_Desc, eft.Field_Type_Desc, t.TableName
                            FROM dbo.Table_Fields_Values tfv (NOLOCK)
                            JOIN dbo.Table_Fields tf (NOLOCK) ON tfv.Table_Field_Id = tf.Table_Field_Id
                            JOIN dbo.ed_fieldtypes eft	(NOLOCK) ON tf.ED_Field_Type_Id = eft.ED_Field_Type_Id
                            JOIN dbo.Tables t	(NOLOCK) ON tfv.TableId = t.TableId
                            WHERE KeyId = @keyId AND t.TableName = @tableDesc", paramItems
                    ).ToListAsync();
    
                    List <Response_UDP_DTO> mappedResult = result.Select(r => new Response_UDP_DTO{
                        tableFieldId = r.Table_Field_Id,
                        tableId = r.TableId,
                        value = r.Value,
                        keyId = r.KeyId,
                        tableFieldDesc = r.Table_Field_Desc,
                        fieldTypeDesc = r.Field_Type_Desc,
                    }).ToList();
    
                    return Ok(mappedResult);
                }
                catch (Exception e)
                {
                    return Problem(detail: e.Message, statusCode: 500);
                }
     
        }

        private async Task<Response_Find_DTO> FindSheetData(Child_Variable Variable){
        object[] paramItems = new object[]
            {
                new SqlParameter ("@Var_Id", Variable.Var_Id),
            };
        _siteContext.setOnPremConnection(HttpContext.Items["Server"].ToString());
        var result = await _siteContext.Database.SqlQueryRaw<VariableSheet>(@"
                             EXECUTE spEM_GetVarSheetData @Var_Id
                         ", paramItems).ToListAsync();
    
         List <Response_Find_DTO> variablemapped = result.Select(r => new Response_Find_DTO{
                specId = r.Spec_Id,
                specDescs = r.Spec_Desc,
                propDesc = r.Prop_Desc,
                samplingInterval = r.Sampling_Interval,
                samplingOffset = r.Sampling_Offset,
                samplingType = r.Sampling_Type,
                samplingWindow = r.Sampling_Window,
                samplingWindowType = r.Sampling_Window_Type,
                eventType = r.Event_Type,
                varReject = r.Var_Reject,
                unitReject = r.Unit_Reject,
                rank = r.Rank,
                inputTag = r.Input_Tag,
                inputTag2 = r.Input_Tag2,
                outputTag = r.Output_Tag,
                dqTag = r.DQ_Tag,
                uelTag = r.UEL_Tag,
                urlTag = r.URL_Tag,
                uwlTag = r.UWL_Tag,
                uulTag = r.UUL_Tag,
                targetTag = r.Target_Tag,
                lulTag = r.LUL_Tag,
                lwlTag = r.LWL_Tag,
                lrlTag = r.LRL_Tag,
                lelTag = r.LEL_Tag,
                totFactor = r.Tot_Factor,
                tfReset = r.TF_Reset,
                saId = r.SA_Id,
                comparisonOperatorId = r.Comparison_Operator_Id,
                comparisonValue = r.Comparison_Value,
                repeating = r.Repeating,
                repeatBackTime = r.Repeat_Backtime,
                extendedInfo = r.Extended_Info,
                userDefined1 = r.User_Defined1,
                userDefined2 = r.User_Defined2,
                userDefined3 = r.User_Defined3,
                unitSummarize = r.Unit_Summarize,
                forceSignEntry = r.Force_Sign_Entry,
                extendedTestFreq = r.Extended_Test_Freq,
                maxRPM = r.Max_RPM,
                resetValue = r.Reset_Value,
                isConformanceVariable = r.Is_Conformance_Variable,
                eSignatureLevel = r.Esignature_Level,
                eventSubTypeId = r.Event_Subtype_Id,
                eventDimension = r.Event_Dimension,
                peiId = r.PEI_Id,
                spcCalculationTypeId = r.SPC_Calculation_Type_Id,
                spcGroupVariableTypeId = r.SPC_Group_Variable_Type_Id,
                samplingReferenceVarId = r.Sampling_Reference_Var_Id,
                stringSpecificationSetting = r.String_Specification_Setting,
                writeGroupDSId = r.Write_Group_DS_Id,
                reloadFlag = r.Reload_Flag,
                readLagTime = r.ReadLagTime,
                shouldArchive = r.Should_Archive,
                eventLookup = r.EventLookup,
                ignoreEventStatus = r.Ignore_Event_Status
                }).ToList();
        return variablemapped[0];
      }

        [HttpGet("{id}/childs")]
        public async Task<ActionResult<List<Response_Find_Child_DTO>>> GetChildVariables(int id){
            try{
                object[] paramItems = new object[]
                {
                    new SqlParameter ("@varId", id),
                };
                _siteContext.setOnPremConnection(HttpContext.Items["Server"].ToString());
                var result = await _siteContext.Database.SqlQueryRaw<Child_Variable>(
                    @" SELECT 
                        Var_Id, 
                        Var_Desc, 
                        Var_Desc_Global, 
                        pu.PU_Id,
                        pu.PU_Desc, 
                        PUG_Id, 
                        PUG_Order,
                        DS_Id,
                        Data_Type_Id,
                        Var_Precision,
                        Eng_Units,
                        v.Group_Id,
                        v.External_Link
                    FROM dbo.Variables_Base v (NOLOCK)
                    JOIN dbo.Prod_Units_Base pu (NOLOCK) ON v.PU_Id = pu.PU_Id WHERE v.PVar_Id = @varId", paramItems
                ).ToListAsync();
        
                List <Response_Find_Child_DTO> mappedResult = result.Select(r => new Response_Find_Child_DTO{
                    varId = r.Var_Id,
                    varDesc = r.Var_Desc,
                    varDescGlobal = r.Var_Desc_Global,
                    puId = r.PU_Id,
                    pugId = r.PUG_Id,
                    pugOrder = r.PUG_Order,
                    dsId = r.DS_Id,
                    dataTypeId = r.Data_Type_Id,
                    varPrecision = r.Var_Precision,
                    engUnits = r.Eng_Units,
                    groupId = r.Group_Id,
                    externalLink = r.External_Link,
                    puDesc = r.PU_Desc
                    
                }).ToList();

                var variable = (mappedResult.Count > 0) ? mappedResult[0] : null;
                
                return Ok((variable != null) ? await FindSheetData(result[0]) : null);

            }catch(Exception e){
                return Problem(detail: e.Message, statusCode: 500);
            }
        }

        [HttpGet("{id}/display-templates")]
        public async Task<ActionResult<List<Response_Display_Template_DTO>>> GetDisplayTemplates(int id){
            try{
                object[] paramItems = new object[]
                {
                    new SqlParameter ("@varId", id),
                };
                _siteContext.setOnPremConnection(HttpContext.Items["Server"].ToString());
                var displays = await _siteContext.Database.SqlQueryRaw<Sheet>(
                    @" SELECT s.Sheet_Id, Sheet_Desc, Var_Order, Sheet_Type, st.Sheet_Type_Desc FROM Sheet_Variables sv (NOLOCK)
                        JOIN Sheets s (NOLOCK) ON sv.Sheet_Id = s.Sheet_Id
                        JOIN Sheet_Type st (NOLOCK) on s.Sheet_Type = st.Sheet_Type_Id
                        WHERE Var_Id = @varId AND Sheet_Type IN (1, 2, 25,11)", paramItems
                ).ToListAsync();

                Console.WriteLine("1");

                List <Response_Variable_Sheet_DTO> mappedResultDisplays = displays.Select(r => new Response_Variable_Sheet_DTO{
                    sheetId = r.Sheet_Id,
                    sheetDesc = r.Sheet_Desc,
                    varOrder = r.Var_Order,
                    sheetTypeId = r.Sheet_Type,
                    sheetTypeDesc = r.Sheet_Type_Desc
                }).ToList();
                Console.WriteLine("2");
                var templates = await _siteContext.Database.SqlQueryRaw<Template>(
                    @" SELECT DISTINCT at.AT_Id, AT_Desc
                        FROM dbo.Alarm_Templates at (NOLOCK) 
                        JOIN [dbo].[Alarm_Template_Var_Data] atvd (NOLOCK) ON (atvd.at_id = at.at_id)
                        WHERE atvd.Var_Id = @varId", paramItems
                ).ToListAsync();
                Console.WriteLine("3");
                List <Response_Variable_Template_DTO> mappedResultTemplates = templates.Select(r => new Response_Variable_Template_DTO{
                    atId = r.AT_Id,
                    atDesc = r.AT_Desc
                }).ToList();

               List<Response_Display_Template_DTO> displayTemplateData = new List<Response_Display_Template_DTO>();
                displayTemplateData.AddRange(displays.Select(r => new Response_Display_Template_DTO
                {
                    sheetId = r.Sheet_Id,
                    sheetDesc = r.Sheet_Desc,
                    varOrder = r.Var_Order,
                    sheetTypeId = r.Sheet_Type,
                    sheetTypeDesc = r.Sheet_Type_Desc
                }));

                displayTemplateData.AddRange(templates.Select(r => new Response_Display_Template_DTO
                {
                    atId = r.AT_Id,
                    atDesc = r.AT_Desc
                }));

                return Ok(displayTemplateData);
            }catch(Exception e){
                        return Problem(detail: e.Message, statusCode: 500);
                    }
                }
                        
        private async Task<Response_Find_DTO> find(int id) {
            _siteContext.setOnPremConnection(HttpContext.Items["Server"].ToString());
            object[] paramItems = new object[]
            {
                new SqlParameter ("@varId", id),
            };

            var data = await _siteContext.Database.SqlQueryRaw<Variable_Search>(@"SELECT 
                    Var_Id, 
                    Var_Desc, 
                    Var_Desc_Global, 
                    pu.PU_Id,
                    pu.PU_Desc, 
                    pug.PUG_Id, 
                    pug.PUG_Order,
                    pug.PUG_Desc,
                    DS_Id,
                    Data_Type_Id,
                    Var_Precision,
                    Eng_Units,
                    v.Group_Id,
                    v.External_Link,
                    v.Calculation_Id,
                    pl.PL_Id,
                    pl.PL_Desc
                FROM dbo.Variables_Base v (NOLOCK)
                JOIN dbo.PU_Groups pug (NOLOCK) ON v.PUG_Id = pug.PUG_Id 
               JOIN dbo.Prod_Units_Base pu (NOLOCK) ON v.PU_Id = pu.PU_Id
                JOIN dbo.Prod_Lines_Base pl (NOLOCK) ON pu.PL_Id = pl.PL_Id 
                WHERE v.Var_Id = @varId", paramItems).ToListAsync();
            List <Response_Find_DTO> mappedResult = data.Select(r => new Response_Find_DTO{
                varId = r.Var_Id,
                varDesc = r.Var_Desc,
                varDescGlobal = r.Var_Desc_Global,
                puId = r.PU_Id,
                pugId = r.PUG_Id,
                pugOrder = r.PUG_Order,
                dsId = r.DS_Id,
                dataTypeId = r.Data_Type_Id,
                varPrecision = r.Var_Precision,
                engUnits = r.Eng_Units,
                groupId = r.Group_Id,
                plId = r.PL_Id,
                plDesc = r.PL_Desc,
                puDesc = r.PU_Desc,
                pugDesc = r.PUG_Desc,
                calculationId = r.Calculation_Id
            }).ToList();           
            var variable = (mappedResult.Count > 0) ? mappedResult[0] : null;
            
            return (variable != null) ? await FindSheetData(data[0]) : null;
        }

   private async Task<Response_Find_DTO> update(Create_Variable_DTO data) {
            List<string> udpValues = data.udpList.Select(r => r.Value).ToList();
            if(udpValues == null){
                udpValues = new List<string>(); 
            }
            
            List<int> udpIds = data.udpList.Select(r => r.Table_Field_Id).ToList();

            if(udpIds == null) {
               udpIds = new List<int>();
            }

            

            string subGroupUDPs = (data.subGroupSizeVariable.Count() > 0) ? 
                string.Join("[REC1]", data.subGroupSizeVariable.Concat([data.variable])
                    .Select(r => $"{r.Var_Desc}[FLD1]{string.Join("[REC]", udpIds)}@${string.Join("[REC]", udpValues)}").ToList())
                    : "" ;
      
                
            string subGroupExtendedInfo = (data.subGroupSizeVariable.Count() > 0) ? 
            string.Join("[REC]", data.subGroupSizeVariable.Concat(new[] { data.variable })
                .Select((r, i) => (i > 0) ? 
                    $"{r.Var_Desc}[FLD]{r.Extended_Info}[FLD]{r.User_Defined1}[FLD]{r.User_Defined2}[FLD]{r.User_Defined3}" : 
                    $"{data.variable.Var_Desc}[FLD]{data.variable.Extended_Info}[FLD]{data.variable.User_Defined1}[FLD]{data.variable.User_Defined2}[FLD]{data.variable.User_Defined3}"))
            : "";


            object[] paramItems = new object[]
            {
                new SqlParameter("@intPLId", data.variable.PL_Id),
                new SqlParameter("@vcrPUDesc", data.variable.PU_Desc),
                new SqlParameter("@intPUG_Id", data.variable.PUG_Id),
                new SqlParameter("@vcrPUG_desc", data.variable.PUG_Desc),
                new SqlParameter("@intVar_id", data.variable.Var_Id),
                new SqlParameter("@vcrGlobalDesc", data.variable.Var_Desc_Global),
                new SqlParameter("@tintEvent_Type", data.variable.Event_Type),
                new SqlParameter("@intDS_Id", data.variable.DS_Id),
                new SqlParameter("@vcrEng_Units", data.variable.Eng_Units ?? null),
                new SqlParameter("@intST_Id", data.variable.Sampling_Type ?? null),
                // new SqlParameter("@intBase_Var_Id", null),
                new SqlParameter("@intData_Type_Id", data.variable.Data_Type_Id),
                new SqlParameter("@tintVar_Precision", data.variable.Var_Precision),
                new SqlParameter("@sintSampling_Interval", data.variable.Sampling_Interval),
                new SqlParameter("@sintSampling_Offset", data.variable.Sampling_Offset),
                new SqlParameter("@tintRepeating", data.variable.Repeating),
                new SqlParameter("@intRepeat_Backtime", data.variable.Repeat_Back_Time),
                new SqlParameter("@intSpec_Id", data.variable.Spec_Id ?? null),
                new SqlParameter("@tintSA_Id", data.variable.SA_Id),
                new SqlParameter("@vcrUserDefined1", data.variable.User_Defined1),
                new SqlParameter("@intCalculation_Id", data.variable.Calculation_Id ?? null),
                new SqlParameter("@vcrInput_Tag", data.variable.Input_Tag ?? null),
                new SqlParameter("@vcrOutput_Tag", data.variable.Output_Tag ?? null),
                new SqlParameter("@vcrDQ_Tag", data.variable.DQ_Tag ?? null),
                new SqlParameter("@vcrComparison_Operator_Id", data.variable.Comparison_Operator_Id ?? null),
                new SqlParameter("@vcrComparison_Value", data.variable.Comparison_Value),
                new SqlParameter("@vcrExternal_Link", data.variable.External_Link),
                new SqlParameter("@intAlarm_Template_Id", data.alarmTemplate.AT_Id),
                new SqlParameter("@intAlarm_Display_Id", data.alarmDisplay.Sheet_Id),
                new SqlParameter("@intAutolog_Display_Id", data.autologDisplay.Sheet_Id ?? null),
                new SqlParameter("@intAutolog_Display_Order", data.autologDisplay.Order ?? null),
                new SqlParameter("@vcrUserDefined2", data.variable.User_Defined2),
                new SqlParameter("@vcrUserDefined3", data.variable.User_Defined3),
                new SqlParameter("@vcrExtendedInfo", data.variable.Extended_Info),
                new SqlParameter("@tintEventSubtype", data.variable.Event_Subtype_Id ?? null),
                new SqlParameter("@vcrVar_Desc", data.variable.Var_Desc),
                new SqlParameter("@vcrTestName", data.variable.Test_Name),
                new SqlParameter("@vcrTableFieldValues", string.Join("[REC]", data.udpList.Select(r => r.Value).ToList())),
                new SqlParameter("@vcrTableFieldIDs", string.Join("[REC]", data.udpList.Select(r => r.Table_Field_Id).ToList())),
                new SqlParameter("@bitForceSignEntry", data.variable.Force_Sign_Entry != null ? 1 : 0),
                new SqlParameter("@intUser_Id", HttpContext.Items["PPAUserId"]),
                new SqlParameter("@intPVar_Id", data.variable.PVar_Id),
                new SqlParameter("@intSPCTypeId", data.variable.SPC_Variable_Type_Id),
                new SqlParameter("@vcrSPCFailure", ""),
                new SqlParameter("@intSGSizeOld", 1),
                new SqlParameter("@intSGSize", data.variable.CPK_SubGroup_Size ?? null),
                new SqlParameter("@vcrEIs", subGroupExtendedInfo),
                new SqlParameter("@vcrUDPs", subGroupUDPs)
            };


            _siteContext.setOnPremConnection(HttpContext.Items["Server"].ToString());
            var result = await _siteContext.Database.SqlQueryRaw<VariableAdd>(
                Queries.UpdateVariableQuery, paramItems).ToListAsync();
                
            return await this.find(data.variable.Var_Id);
     }

    private async Task<Response_Find_DTO> add (Create_Variable_DTO data) {
            List<string> udpValues = data.udpList.Select(r => r.Value).ToList();
            if(udpValues == null){
                udpValues = new List<string>(); 
            }
            
            List<int> udpIds = data.udpList.Select(r => r.Table_Field_Id).ToList();

            if(udpIds == null) {
               udpIds = new List<int>();
            } 

            string subGroupUDPs = (data.subGroupSizeVariable.Count() > 0) ? 
            string.Join("[REC1]", data.subGroupSizeVariable.Concat(new[] { data.variable })
                .Select(r => $"{r.Var_Desc}[FLD1]{string.Join("[REC]", udpIds)}@{string.Join("[REC]", udpValues)}"))
            : "";

            // string subGroupUDPs = "";
            // string subGroupExtendedInfo = "";

            string subGroupExtendedInfo = (data.subGroupSizeVariable.Count() > 0) ? 
                string.Join("[REC]", data.subGroupSizeVariable.Concat(new[] { data.variable })
                    .Select((r, i) => $"{r.Var_Desc}[FLD]{r.Extended_Info}[FLD]{r.User_Defined1}[FLD]{r.User_Defined2}[FLD]{r.User_Defined3}"))
                : "";


            object[] paramItems = new object[]{
                new SqlParameter("@intPLId", data.variable.PL_Id),
                new SqlParameter("@vcrPUDesc", data.variable.PU_Desc),
                new SqlParameter("@intPUG_Id", data.variable.PUG_Id),
                new SqlParameter("@vcrPUG_desc", data.variable.PUG_Desc),
                new SqlParameter("@intVar_id", data.variable.Var_Id),
                new SqlParameter("@vcrGlobalDesc", data.variable.Var_Desc_Global),
                new SqlParameter("@tintEvent_Type", data.variable.Event_Type),
                new SqlParameter("@intDS_Id", data.variable.DS_Id),
                new SqlParameter("@vcrEng_Units", data.variable.Eng_Units ?? null),
                new SqlParameter("@intST_Id", data.variable.Sampling_Type ?? null),
                //new SqlParameter("@intBase_Var_Id", null),
                // new SqlParameter("@intQAlarm_Template_Id", null),
                new SqlParameter("@intQAlarm_Display_Id", data.qAlarmDisplay.Sheet_Id),
                new SqlParameter("@intData_Type_Id", data.variable.Data_Type_Id),
                new SqlParameter("@tintVar_Precision", data.variable.Var_Precision),
                new SqlParameter("@sintSampling_Interval", data.variable.Sampling_Interval),
                new SqlParameter("@sintSampling_Offset", data.variable.Sampling_Offset),
                new SqlParameter("@tintRepeating", data.variable.Repeating),
                new SqlParameter("@intRepeat_Backtime", data.variable.Repeat_Back_Time),
                new SqlParameter("@intSpec_Id", data.variable.Spec_Id ?? null),
                new SqlParameter("@tintSA_Id", data.variable.SA_Id),
                new SqlParameter("@vcrUserDefined1", data.variable.User_Defined1),
                new SqlParameter("@intCalculation_Id", data.variable.Calculation_Id ?? null),
                new SqlParameter("@vcrInput_Tag", data.variable.Input_Tag ?? null),
                new SqlParameter("@vcrOutput_Tag", data.variable.Output_Tag ?? null),
                new SqlParameter("@vcrDQ_Tag", data.variable.DQ_Tag ?? null),
                new SqlParameter("@vcrComparison_Operator_Id", data.variable.Comparison_Operator_Id ?? null),
                new SqlParameter("@vcrComparison_Value", data.variable.Comparison_Value),
                new SqlParameter("@vcrExternal_Link", data.variable.External_Link),
                new SqlParameter("@intAlarm_Template_Id", data.alarmTemplate.AT_Id),
                new SqlParameter("@intAlarm_Display_Id", data.alarmDisplay.Sheet_Id),
                new SqlParameter("@intAutolog_Display_Id", data.autologDisplay.Sheet_Id ?? null),
                new SqlParameter("@intAutolog_Display_Order", data.autologDisplay.Order ?? null),
                new SqlParameter("@vcrUserDefined2", data.variable.User_Defined2),
                new SqlParameter("@vcrUserDefined3", data.variable.User_Defined3),
                new SqlParameter("@vcrExtendedInfo", data.variable.Extended_Info),
                new SqlParameter("@tintEventSubtype", data.variable.Event_Subtype_Id ?? null),
                new SqlParameter("@vcrVar_Desc", data.variable.Var_Desc),
                new SqlParameter("@vcrTestName", data.variable.Test_Name),
                new SqlParameter("@vcrTableFieldValues", string.Join("[REC]", data.udpList.Select(r => r.Value).ToList())),
                new SqlParameter("@vcrTableFieldIDs", string.Join("[REC]", data.udpList.Select(r => r.Table_Field_Id).ToList())),
                new SqlParameter("@bitForceSignEntry", data.variable.Force_Sign_Entry != null ? 1 : 0),
                new SqlParameter("@intUser_Id", HttpContext.Items["PPAUserId"]),
                new SqlParameter("@intPVar_Id", data.variable.PVar_Id),
                new SqlParameter("@intSPCTypeId", data.variable.SPC_Variable_Type_Id),
                new SqlParameter("@vcrSPCFailure", ""),
                // new SqlParameter("@intSGSizeOld", 0),
                new SqlParameter("@intSGSize", data.variable.CPK_SubGroup_Size ?? null),
                new SqlParameter("@vcrEIs", subGroupExtendedInfo),
                new SqlParameter("@vcrUDPs", subGroupUDPs)
            };

            _siteContext.setOnPremConnection(HttpContext.Items["Server"].ToString());
            var result = await _siteContext.Database.SqlQueryRaw<VariableAdd>(
                Queries.AddVariableQuery, paramItems).ToListAsync();

            
                
            return await this.find(result[0].Var_Id);  
     }   

    private async Task dropAndReasignCalculation(int varId, int? calcId) {
        object[] paramItems = new object[] {
            new SqlParameter("@ListType", 94),
            new SqlParameter("@CalcId", calcId),
            new SqlParameter("@id1", varId),
            new SqlParameter("@id2", null),
            new SqlParameter("@id3", null),
            new SqlParameter("@str1", null),
            new SqlParameter("@str2", null),
            new SqlParameter("@User_Id", HttpContext.Items["PPAUserId"])
        };

       _siteContext.setOnPremConnection(HttpContext.Items["Server"].ToString());
       var result = await _siteContext.Database.ExecuteSqlRawAsync(@"
                    EXECUTE spEMCC_BuildDataSetUpdate @ListType, @CalcId, @id1, @id2, @id3, @str1, @str2, @User_Id
                    ", paramItems);

    }

    private async Task addCalculationInput(int varId, int? calcId, CalculationInputData input) {
        object[] paramItems = new object[]{
            new SqlParameter("@ListType", 35),
            new SqlParameter("@CalcId", calcId),
            new SqlParameter("@id1", input.Calc_Input_Id),
            new SqlParameter("@id2", varId),
            new SqlParameter("@id3", input.Member_Var_Id),
            new SqlParameter("@str1", input.Default_Value),
            new SqlParameter("@str2", null),
            new SqlParameter("@User_Id", HttpContext.Items["PPAUserId"])
        };

        _siteContext.setOnPremConnection(HttpContext.Items["Server"].ToString());
       var result = await _siteContext.Database.ExecuteSqlRawAsync(@"
                    EXECUTE spEMCC_BuildDataSetUpdate @ListType, @CalcId, @id1, @id2, @id3, @str1, @str2, @User_Id
                    ", paramItems);
    }

    [HttpPost]
    public async Task<ActionResult<Response_Find_DTO>> AddVariable(Create_Variable_DTO data) {
        try {

            if (string.IsNullOrEmpty(data.variable.Var_Desc))
                {
                     return BadRequest("Variable Description could not be empty");
                }
            Response_Find_DTO updatedVariable = null;
            Response_Find_DTO  currentVariable = null;

        

            if (data.variable.Var_Id != 0)
            {
                currentVariable = await this.find(data.variable.Var_Id);
                updatedVariable = await this.update(data);
            }

            if (updatedVariable == null) {
                updatedVariable = await this.add(data);
                if (updatedVariable == null) {
                    return StatusCode(404, "Error Creating Variable");
                }
            }

            if (updatedVariable?.varId != null && data?.variable.Calculation_Id > 0 && data.calculationInputs?.Length > 0) {
                if (currentVariable != null && currentVariable.calculationId != null) {
                    await this.dropAndReasignCalculation(updatedVariable.varId, currentVariable.calculationId);
                } else{ 
                    await this.dropAndReasignCalculation(updatedVariable.varId, data.variable.Calculation_Id);}

                foreach (var input in data.calculationInputs) {
                    await this.addCalculationInput(updatedVariable.varId, data.variable.Calculation_Id, input);
                }
            }
            var logItems = new object[]
                {
                    new SqlParameter("@ServerId", Int32.Parse(HttpContext.Items["ServerId"].ToString())),
                    new SqlParameter("@TransactionType", "ADD VARIABLE"),
                    new SqlParameter("@UserName", HttpContext.User.Identity.Name.Split('\\')[1]),
                    new SqlParameter("@HasErrors", false),
                    new SqlParameter("@TransactionDetails", updatedVariable.varId.ToString())
                };
                
                var command = await _centralContext.Database.ExecuteSqlRawAsync(@"
                    EXECUTE spLocal_AddTransactionLog @ServerId, @TransactionType, @UserName, @HasErrors, @TransactionDetails", logItems);
          
            return Ok(updatedVariable);
        } catch (Exception e)
            {
                var logItems = new object[]
                {
                    new SqlParameter("@ServerId", Int32.Parse(HttpContext.Items["ServerId"].ToString())),
                    new SqlParameter("@TransactionType", "ADD VARIABLE"),
                    new SqlParameter("@UserName", HttpContext.User.Identity.Name.Split('\\')[1]),
                    new SqlParameter("@HasErrors", true),
                    new SqlParameter("@TransactionDetails", e.Message)
                };
                
                var command = await _centralContext.Database.ExecuteSqlRawAsync(@"
                    EXECUTE spLocal_AddTransactionLog @ServerId, @TransactionType, @UserName, @HasErrors, @TransactionDetails", logItems);
                return Problem(detail: e.Message, statusCode: 500);
            }
      }

      private async Task<Response_Find_DTO> activeUnactiveVariable(int varId, bool isActive) {
            object[] paramItems = new object[]{
                new SqlParameter("@VarId", varId),
                new SqlParameter("@NewState", isActive ? 1 : 0),
                new SqlParameter("@User_Id",HttpContext.Items["PPAUserId"])
            };
            Console.WriteLine("active:2");
            _siteContext.setOnPremConnection(HttpContext.Items["Server"].ToString());
            var result = await _siteContext.Database.ExecuteSqlRawAsync(@"
                            EXECUTE spEM_ActivateVariable @VarId, @NewState, @User_Id
                            ", paramItems);
            
           return await this.find(varId);
            
        }

        private async Task addVariabletoDisplay (VarSheetData sheetVariable, bool isLast, bool isFirst) {
            object[] paramItems = new object[]{
                new SqlParameter("@SheetId", sheetVariable.Sheet_Id),
                new SqlParameter("@Id", sheetVariable.Var_Id),
                new SqlParameter("@Order", sheetVariable.Var_Order),
                new SqlParameter("@Title", sheetVariable.Title),
                new SqlParameter("@IsLast", isLast ? 1 : 0),
                new SqlParameter("@IsFirst", isFirst ? 1 : 0),
                new SqlParameter("@User_Id", HttpContext.Items["PPAUserId"])
            };

            _siteContext.setOnPremConnection(HttpContext.Items["Server"].ToString());
            var result = await _siteContext.Database.ExecuteSqlRawAsync(@"
                            EXECUTE spEM_PutSheetVariables @SheetId, @Order, @Id, , @Title, @IsLast, @IsFirst, @User_Id
                            ", paramItems);
        }

        private async Task<List<Response_Sheet_Variable_DTO>> addVariablesSheet(int sheetId, AddVariablesDisplay data){
            try {
                if (data.variables.Count() > 0) {
                    data.variables.Select(async (r, i) => await addVariabletoDisplay(r, i == data.variables.Count() - 1, i == 0));
                }
                var logItems = new object[]
                {
                    new SqlParameter("@ServerId", Int32.Parse(HttpContext.Items["ServerId"].ToString())),
                    new SqlParameter("@TransactionType", "ADD VARIABLES SHEET"),
                    new SqlParameter("@UserName", HttpContext.User.Identity.Name.Split('\\')[1]),
                    new SqlParameter("@HasErrors", false),
                    new SqlParameter("@TransactionDetails",data.variables.Select(r => r.Var_Id).ToList().ToString())
                };
                
                var command = await _centralContext.Database.ExecuteSqlRawAsync(@"
                    EXECUTE spLocal_AddTransactionLog @ServerId, @TransactionType, @UserName, @HasErrors, @TransactionDetails", logItems);

                return await getSheetsbyVariable(sheetId);
            } catch(Exception e) {
                var logItems = new object[]
                {
                    new SqlParameter("@ServerId", Int32.Parse(HttpContext.Items["ServerId"].ToString())),
                    new SqlParameter("@TransactionType", "ADD VARIABLES SHEET"),
                    new SqlParameter("@UserName", HttpContext.User.Identity.Name.Split('\\')[1]),
                    new SqlParameter("@HasErrors", true),
                    new SqlParameter("@TransactionDetails", e.Message)
                };
                
                var command = await _centralContext.Database.ExecuteSqlRawAsync(@"
                    EXECUTE spLocal_AddTransactionLog @ServerId, @TransactionType, @UserName, @HasErrors, @TransactionDetails", logItems);
                throw new HttpRequestException(e.Message, e,  System.Net.HttpStatusCode.InternalServerError);
            }
        }

        private async Task<Response_Sheet_Variable_DTO> removeVariableFromSheet(int varId) {
            try {
                List<Response_Sheet_Variable_DTO> sheets = await this.getSheetsbyVariable(varId); 

                foreach(Response_Sheet_Variable_DTO sheet in sheets) {
                    List<Response_Sheet_Variable_DTO> variables = await this.getSheetVariables(sheet.sheetId);
                    await this.addVariablesSheet(sheet.sheetId, new AddVariablesDisplay(sheets));
                }
                var logItems = new object[]
                {
                    new SqlParameter("@ServerId", Int32.Parse(HttpContext.Items["ServerId"].ToString())),
                    new SqlParameter("@TransactionType", "REMOVE VARIABLES SHEET"),
                    new SqlParameter("@UserName", HttpContext.User.Identity.Name.Split('\\')[1]),
                    new SqlParameter("@HasErrors", false),
                    new SqlParameter("@TransactionDetails", sheets.Select(r => r.sheetId).ToList().ToString())
                };
                
                var command = await _centralContext.Database.ExecuteSqlRawAsync(@"
                    EXECUTE spLocal_AddTransactionLog @ServerId, @TransactionType, @UserName, @HasErrors, @TransactionDetails", logItems);
                
              

                return null;
            } catch(Exception e) {
                var logItems = new object[]
                {
                    new SqlParameter("@ServerId", Int32.Parse(HttpContext.Items["ServerId"].ToString())),
                    new SqlParameter("@TransactionType", "REMOVE VARIABLES SHEET"),
                    new SqlParameter("@UserName", HttpContext.User.Identity.Name.Split('\\')[1]),
                    new SqlParameter("@HasErrors", true),
                    new SqlParameter("@TransactionDetails", e.Message)
                };
                
                var command = await _centralContext.Database.ExecuteSqlRawAsync(@"
                    EXECUTE spLocal_AddTransactionLog @ServerId, @TransactionType, @UserName, @HasErrors, @TransactionDetails", logItems);
                throw new HttpRequestException(e.Message, e,  System.Net.HttpStatusCode.InternalServerError);
            }


        }

        private async Task<List<Response_Sheet_Variable_DTO>> getSheetsbyVariable(int varId) {
            object[] paramItems = new object[]{
                new SqlParameter("@VarId", varId)
            };

            _siteContext.setOnPremConnection(HttpContext.Items["Server"].ToString());
            var result = await _siteContext.Database.SqlQueryRaw<Sheet>(@"
                                SELECT DISTINCT Sheet_Id  FROM Sheet_Variables sv (NOLOCK) WHERE Sheet_Id IN (
                                    SELECT Sheet_Id FROM Sheet_Variables (NOLOCK)  WHERE Var_Id = @varId
                                ) AND ISNULL(Var_Id,0) <> @varId
                                Order By Sheet_Id", paramItems).ToListAsync();
            
            
            List <Response_Sheet_Variable_DTO> mappedResult = result.Select(r => new Response_Sheet_Variable_DTO{
                    sheetId = r.Sheet_Id
                }).ToList();
                
            return mappedResult;
        
        }

        private async Task<List<Response_Sheet_Variable_DTO>> getSheetVariables(int sheetId) {
            object[] paramItems = new object[]{
                new SqlParameter("@SheetId", sheetId)
            };

            _siteContext.setOnPremConnection(HttpContext.Items["Server"].ToString());
            var result = await _siteContext.Database.SqlQueryRaw<VarSheetData>(@"
                               SELECT sv.Var_Id, 
                                    Var_Order, 
                                    ISNULL(v.Var_Desc, Title) Var_Desc, 
                                    pl.PL_Desc, 
                                    pl.PL_Id, 
                                    pu.PU_Desc,
                                    pu.PU_Id,
                                    pg.PUG_Desc,
                                    pg.PUG_Id,
                                    sv.Sheet_Id,
                                    Title,
                                    Sheet_Desc
                            FROM dbo.Sheet_Variables sv (NOLOCK)
                            JOIN dbo.Sheets s (NOLOCK) ON sv.Sheet_Id = s.Sheet_Id
                            LEFT JOIN dbo.Variables_Base v (NOLOCK) ON sv.Var_Id = v.Var_Id
                            LEFT JOIN dbo.PU_Groups pg (NOLOCK) ON v.PUG_Id = pg.PUG_Id
                            LEFT JOIN dbo.Prod_Units_Base pu (NOLOCK) ON v.PU_Id = pu.PU_Id
                            LEFT JOIN dbo.Prod_Lines_Base pl (NOLOCK) ON pu.PL_Id = pl.PL_Id
                            WHERE sv.Sheet_Id = @sheetId
                            ORDER BY Var_Order", paramItems).ToListAsync();
            
            
            List <Response_Sheet_Variable_DTO> mappedResult = result.Select(r => new Response_Sheet_Variable_DTO{
                    varId = r.Var_Id,
                    varDesc = r.Var_Desc,
                    plId = r.PL_Id,
                    plDesc = r.PL_Desc,
                    puId = r.PU_Id,
                    puDesc = r.PU_Desc,
                    pugId = r.PUG_Id,
                    pugDesc = r.PUG_Desc,
                    sheetId = r.Sheet_Id,
                    title = r.Title,
                    sheetDesc = r.Sheet_Desc
                    
                }).ToList();
                
            return mappedResult;
        }


        [HttpPut("activate")]
        public async Task<ActionResult<Response_Find_DTO>> ActivateVariable(AlterVariableDto[] variables) {
            List<Response_Find_DTO> changedVariables = new List<Response_Find_DTO>();
            
            try {
                Console.WriteLine("1");
                Console.WriteLine($"Variables length: {variables.Length}");

                

                changedVariables = (await Task.WhenAll(variables.Select(async r => await this.activeUnactiveVariable(r.varId, true)).ToList())).ToList();
                Console.WriteLine("2");

                // Log the changedVariables count
                Console.WriteLine($"Changed variables count: {changedVariables.Count}");
                var logItems = new object[]
                {
                    new SqlParameter("@ServerId", Int32.Parse(HttpContext.Items["ServerId"].ToString())),
                    new SqlParameter("@TransactionType", "ACTIVATE VARIABLES"),
                    new SqlParameter("@UserName", HttpContext.User.Identity.Name.Split('\\')[1]),
                    new SqlParameter("@HasErrors", false),
                    new SqlParameter("@TransactionDetails", changedVariables.Select(r => r.varId).ToList().ToString())
                };
                
                var command = await _centralContext.Database.ExecuteSqlRawAsync(@"
                    EXECUTE spLocal_AddTransactionLog @ServerId, @TransactionType, @UserName, @HasErrors, @TransactionDetails", logItems);
               

                return Ok(changedVariables);
            } catch (Exception e) {
                var logItems = new object[]
                {
                    new SqlParameter("@ServerId", Int32.Parse(HttpContext.Items["ServerId"].ToString())),
                    new SqlParameter("@TransactionType", "ACTIVATE VARIABLES"),
                    new SqlParameter("@UserName", HttpContext.User.Identity.Name.Split('\\')[1]),
                    new SqlParameter("@HasErrors", true),
                    new SqlParameter("@TransactionDetails", e.Message)
                };
                
                var command = await _centralContext.Database.ExecuteSqlRawAsync(@"
                    EXECUTE spLocal_AddTransactionLog @ServerId, @TransactionType, @UserName, @HasErrors, @TransactionDetails", logItems);
                return Problem(detail: e.Message, statusCode: 500);
            }
        }

        [HttpPut("deactivate")]
        public async Task<ActionResult<Response_Find_DTO>> DeactivateVariable(AlterVariableDto[] variables) {
            List<Response_Find_DTO> changedVariables;
            
            try {
                changedVariables = (await Task.WhenAll(variables.Select(async r => await this.activeUnactiveVariable(r.varId, false)).ToList())).ToList();

                changedVariables.Select(r => removeVariableFromSheet(r.varId));

                var logItems = new object[]
                {
                    new SqlParameter("@ServerId", Int32.Parse(HttpContext.Items["ServerId"].ToString())),
                    new SqlParameter("@TransactionType", "DEACTIVATE VARIABLES"),
                    new SqlParameter("@UserName", HttpContext.User.Identity.Name.Split('\\')[1]),
                    new SqlParameter("@HasErrors", false),
                    new SqlParameter("@TransactionDetails", changedVariables.Select(r => r.varId).ToList().ToString())
                };
                
                var command = await _centralContext.Database.ExecuteSqlRawAsync(@"
                    EXECUTE spLocal_AddTransactionLog @ServerId, @TransactionType, @UserName, @HasErrors, @TransactionDetails", logItems);

                return Ok(changedVariables);
            } catch (Exception e) {
                 var logItems = new object[]
                {
                    new SqlParameter("@ServerId", Int32.Parse(HttpContext.Items["ServerId"].ToString())),
                    new SqlParameter("@TransactionType", "DEACTIVATE VARIABLES"),
                    new SqlParameter("@UserName", HttpContext.User.Identity.Name.Split('\\')[1]),
                    new SqlParameter("@HasErrors", true),
                    new SqlParameter("@TransactionDetails", e.Message)
                };
                
                var command = await _centralContext.Database.ExecuteSqlRawAsync(@"
                    EXECUTE spLocal_AddTransactionLog @ServerId, @TransactionType, @UserName, @HasErrors, @TransactionDetails", logItems);
                return Problem(detail: e.Message, statusCode: 500);
            }
        }

        private async Task<Response_Find_DTO> renameVariable (int varId, string varDesc, string varDescGlobal){
            if (varDescGlobal != null){

                object[] paramItems = new object[]{
                    new SqlParameter("@Var_Id", varId),
                    new SqlParameter("@Description", varDescGlobal),
                    new SqlParameter("User_Id", HttpContext.Items["PPAUserId"])
                };

                _siteContext.setOnPremConnection(HttpContext.Items["Server"].ToString());
                var result = await _siteContext.Database.ExecuteSqlRawAsync(@"
                                EXECUTE spEM_RenameVar @Var_Id, @Description, @User_Id
                         ", paramItems);
            }
            if (varDesc != null){
                object[] paramItems = new object[]{
                    new SqlParameter("@Var_Id", varId),
                    new SqlParameter("@Description", varDesc),
                    new SqlParameter("User_Id", HttpContext.Items["PPAUserId"])
                };

                _siteContext.setOnPremConnection(HttpContext.Items["Server"].ToString());
                var result = await _siteContext.Database.ExecuteSqlRawAsync(@"
                                EXECUTE spEM_RenameVar @Var_Id, @Description, @User_Id
                         ", paramItems);
            }

            return await this.find(varId);
        }
        
        [HttpPut("rename")]
        public async Task<ActionResult<Response_Find_DTO>> RenameVariable (AlterVariableDto[] variables) {
            List<Response_Find_DTO> variablesRenamed;
            try{
                variablesRenamed = (await Task.WhenAll(variables.Select(async r => await this.renameVariable(r.varId, r.varDesc, r.varDescGlobal)).ToList())).ToList();
                 var logItems = new object[]
                {
                    new SqlParameter("@ServerId", Int32.Parse(HttpContext.Items["ServerId"].ToString())),
                    new SqlParameter("@TransactionType", "RENAME VARIABLES"),
                    new SqlParameter("@UserName", HttpContext.User.Identity.Name.Split('\\')[1]),
                    new SqlParameter("@HasErrors", false),
                    new SqlParameter("@TransactionDetails", variablesRenamed.Select(r => r.varId).ToList().ToString())
                };
                
                var command = await _centralContext.Database.ExecuteSqlRawAsync(@"
                    EXECUTE spLocal_AddTransactionLog @ServerId, @TransactionType, @UserName, @HasErrors, @TransactionDetails", logItems);
               

                return Ok(variablesRenamed);
            } catch (Exception e) {
                 var logItems = new object[]
                {
                    new SqlParameter("@ServerId", Int32.Parse(HttpContext.Items["ServerId"].ToString())),
                    new SqlParameter("@TransactionType", "RENAME VARIABLES"),
                    new SqlParameter("@UserName", HttpContext.User.Identity.Name.Split('\\')[1]),
                    new SqlParameter("@HasErrors", true),
                    new SqlParameter("@TransactionDetails", e.Message)
                };
                
                var command = await _centralContext.Database.ExecuteSqlRawAsync(@"
                    EXECUTE spLocal_AddTransactionLog @ServerId, @TransactionType, @UserName, @HasErrors, @TransactionDetails", logItems);;
                return Problem(detail: e.Message, statusCode: 500);
            }
        }

    }
}