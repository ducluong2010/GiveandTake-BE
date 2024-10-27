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

        [HttpPost(ApiEndPointConstant.Report.ReportsEndPoint)] 
        [SwaggerOperation(Summary = "Create a new Report")]
        public async Task<IActionResult> CreateReport([FromBody] ReportCreateDTO reportCreateDto)
        {
            var response = await _reportService.CreateReport(reportCreateDto);
            if (response.Status >= 0)
                return Ok(response.Message);
            else
                return BadRequest(response.Message);
        }
    }
}
