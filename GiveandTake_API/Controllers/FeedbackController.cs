using GiveandTake_API.Constants;
using Giveandtake_Services.Implements;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GiveandTake_API.Controllers
{
    [ApiController]
    public class FeedbackController : Controller
    {
        private readonly FeedbackService _feedbackService;

        public FeedbackController()
        {
            _feedbackService = new FeedbackService();
        }

        [HttpGet(ApiEndPointConstant.Feedback.FeedbacksEndPoint)]
        [SwaggerOperation(Summary = "Get all Feedbacks with pagination")]
        public async Task<IActionResult> GetAllFeedbacks([FromQuery] int page = 1, [FromQuery] int pageSize = 8)
        {
            var response = await _feedbackService.GetAllFeedbacks(page, pageSize);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response.Message);
        }

        [HttpGet(ApiEndPointConstant.Feedback.FeedbackEndPoint)]
        [SwaggerOperation(Summary = "Get Feedback by its id")]
        public async Task<IActionResult> GetFeedbackById(int id)
        {
            var response = await _feedbackService.GetFeedbackById(id);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response.Message);
        }
    }
}
