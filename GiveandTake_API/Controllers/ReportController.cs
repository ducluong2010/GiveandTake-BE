using GiveandTake_API.Constants;
using GiveandTake_Repo.DTOs.Report;
using Giveandtake_Services.Implements;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GiveandTake_API.Controllers
{
    [ApiController]
    public class ReportController : Controller
    {
        private readonly ReportService _reportService;

        public ReportController()
        {
            _reportService = new ReportService();
        }

        [HttpGet(ApiEndPointConstant.Report.ReportsEndPoint)]
        [SwaggerOperation(Summary = "Get all Reports with pagination")]
        public async Task<IActionResult> GetAllReports([FromQuery] int page = 1, [FromQuery] int pageSize = 8)
        {
            var response = await _reportService.GetAllReports(page, pageSize);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response.Message);
        }
        [HttpGet(ApiEndPointConstant.Report.ReportsUserPoint)]
        [SwaggerOperation(Summary = "Get all Reports User")]
        public async Task<IActionResult> GetReportUser()
        {
            var response = await _reportService.GetReportUser();
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response.Message);
        }
        [HttpGet(ApiEndPointConstant.Report.ReportsDonaPoint)]
        [SwaggerOperation(Summary = "Get all Reports Donation")]
        public async Task<IActionResult> GetReportDonation()
        {
            var response = await _reportService.GetReportDonation();
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response.Message);
        }
        [HttpGet(ApiEndPointConstant.Report.ReportsTechPoint)]
        [SwaggerOperation(Summary = "Get all Reports Tech")]
        public async Task<IActionResult> GetReportTech()
        {
            var response = await _reportService.GetReportTech();
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response.Message);
        }
        [HttpGet(ApiEndPointConstant.Report.ReportStaffEndPoint)]
        [SwaggerOperation(Summary = "Get all Reports by Staff")]
        public async Task<IActionResult> GetAllReportsByStaff([FromQuery] int id)
        {
            var response = await _reportService.GetAllReportsByStaff(id);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response.Message);
        }

        [HttpGet(ApiEndPointConstant.Report.ReportAByEndPoint)]
        [SwaggerOperation(Summary = "Get all Reports by Approved")]
        public async Task<IActionResult> GetReportsByApprovedBy([FromQuery] int id)
        {
            var response = await _reportService.GetReportsByApprovedBy(id);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response.Message);
        }

        [HttpGet(ApiEndPointConstant.Report.ReportAccEndPoint)]
        [SwaggerOperation(Summary = "Get all Reports by Account")]
        public async Task<IActionResult> GetReportsByAccountId([FromQuery] int id)
        {
            var response = await _reportService.GetReportsByAccountId(id);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response.Message);
        }

        [HttpGet(ApiEndPointConstant.Report.ReportEndPoint)]
        [SwaggerOperation(Summary = "Get Report by its id")]
        public async Task<IActionResult> GetReportById(int id)
        {
            var response = await _reportService.GetReportById(id);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response.Message);
        }

        [HttpPost(ApiEndPointConstant.Report.ReportCreateEndPoint)] 
        [SwaggerOperation(Summary = "Create a new Report")]
        public async Task<IActionResult> CreateReport([FromBody] ReportCreateDTO reportCreateDto)
        {
            var response = await _reportService.CreateReport(reportCreateDto);
            if (response.Status >= 0)
                return Ok(response.Message);
            else
                return BadRequest(response.Message);
        }
        [HttpPut(ApiEndPointConstant.Report.ReportUpdateEndPoint)] 
        [SwaggerOperation(Summary = "Update an existing Report")]
        public async Task<IActionResult> UpdateReport(int id, [FromBody] ReportUpdateDTO reportUpdateDto) 
        {
            var response = await _reportService.UpdateReport(id, reportUpdateDto);
            if (response.Status >= 0)
                return Ok(response.Message);
            else
                return BadRequest(response.Message);
        }
        [HttpDelete(ApiEndPointConstant.Report.ReportDeleteEndPoint)] 
        [SwaggerOperation(Summary = "Delete a Report by id")]
        public async Task<IActionResult> DeleteReport(int id)
        {
            var response = await _reportService.DeleteReport(id);
            if (response.Status >= 0)
                return Ok(response.Message);
            else
                return BadRequest(response.Message);
        }

        [HttpPut(ApiEndPointConstant.Report.ReportCompleteEndPoint)]
        [SwaggerOperation(Summary = "Toggle report status between Processing and Processed")]
        public async Task<IActionResult> ToggleProcessingStatus(int id)
        {
            var response = await _reportService.ToggleProcessingStatus(id);
            if (response.Status >= 0)
                return Ok(response.Message);
            else
                return BadRequest(response.Message);
        }
    }
}
