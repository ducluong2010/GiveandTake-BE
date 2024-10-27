using GiveandTake_API.Constants;
using GiveandTake_Repo.DTOs.ReportType;
using Giveandtake_Services.Implements;
using Giveandtake_Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GiveandTake_API.Controllers
{
    [ApiController]

    public class ReportTypeController : Controller
    {
        private readonly IReportTypeService _reportTypeService;

        public ReportTypeController()
        {
            _reportTypeService = new ReportTypeService(); 
        }

        [HttpGet(ApiEndPointConstant.ReportType.ReportTypesEndPoint)] 
        [SwaggerOperation(Summary = "Get all Report Types")]
        public async Task<IActionResult> GetAllReportTypes()
        {
            var response = await _reportTypeService.GetAllReportTypes();
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response);
        }

        [HttpGet(ApiEndPointConstant.ReportType.ReportTypeEndPoint)] 
        [SwaggerOperation(Summary = "Get Report Type by id")]
        public async Task<IActionResult> GetReportTypeById(int id)
        {
            var response = await _reportTypeService.GetReportTypeById(id);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "1")] 
        [HttpPost(ApiEndPointConstant.ReportType.ReportTypesEndPoint)] 
        [SwaggerOperation(Summary = "Create a new Report Type")]
        public async Task<IActionResult> CreateReportType(ReportCreateTypeDTO reportType)
        {
            var response = await _reportTypeService.CreateReportType(reportType);
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response);
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "1")] 
        [HttpPut(ApiEndPointConstant.ReportType.ReportTypeUpdateEndPoint)] 
        [SwaggerOperation(Summary = "Update an existing Report Type")]
        public async Task<IActionResult> UpdateReportType(int id, [FromBody] ReportUpdateTypeDTO reportTypeInfo)
        {
            if (reportTypeInfo == null)
            {
                return BadRequest("Report type information is null");
            }

            var response = await _reportTypeService.UpdateReportType(id, reportTypeInfo);

            if (response.Status == -1)
            {
                return NotFound(response.Message); 
            }

            return Ok(response); 
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "1")] 
        [HttpDelete(ApiEndPointConstant.ReportType.ReportTypeDeleEndPoint)]
        [SwaggerOperation(Summary = "Delete Report Type by id")]
        public async Task<IActionResult> DeleteReportType(int id)
        {
            var response = await _reportTypeService.DeleteReportType(id);
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response);
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "1")] 
        [HttpPut(ApiEndPointConstant.ReportType.ChangeStatusEndPoint)]
        [SwaggerOperation(Summary = "Change status of Report Type")]
        public async Task<IActionResult> ChangeStatusReportType(int id)
        {
            var response = await _reportTypeService.ChangeStatusReportType(id);
            if (response.Status >= 0)
                return Ok(response);
            else
                return BadRequest(response);
        }
    }
}