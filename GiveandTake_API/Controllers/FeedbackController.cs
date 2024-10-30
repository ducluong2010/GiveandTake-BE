using GiveandTake_API.Constants;
using GiveandTake_Repo.DTOs.Feedback;
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
        [HttpGet(ApiEndPointConstant.Feedback.FeedbackSenEndPoint)]
        [SwaggerOperation(Summary = "Get all Feedbacks by SenderId")]
        public async Task<IActionResult> GetFeedbacksBySenderId(int id, [FromQuery] int page = 1, [FromQuery] int pageSize = 8)
        {
            var response = await _feedbackService.GetFeedbacksBySenderId(id, page, pageSize);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response.Message);
        }
        [HttpGet(ApiEndPointConstant.Feedback.FeedbackAccEndPoint)]
        [SwaggerOperation(Summary = "Get all Feedbacks by AccountId")]
        public async Task<IActionResult> GetFeedbacksByAccountId(int id, [FromQuery] int page = 1, [FromQuery] int pageSize = 8)
        {
            var response = await _feedbackService.GetFeedbacksByAccountId(id, page, pageSize);
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

        [HttpPost(ApiEndPointConstant.Feedback.FeedbacksEndPoint)]
        [SwaggerOperation(Summary = "Create new Feedback")]
        public async Task<IActionResult> CreateFeedback([FromBody] CreateFeedbackDTO createFeedbackDto)
        {
            var response = await _feedbackService.CreateFeedback(createFeedbackDto);

            if (response.Status >= 0)
            {
                return Ok(response.Message);
            }
            else
            {
                return BadRequest(response.Message);
            }
        }

        [HttpPut(ApiEndPointConstant.Feedback.FeedbackEndPoint)]
        [SwaggerOperation(Summary = "Update Feedback")]
        public async Task<IActionResult> UpdateFeedback(int id, [FromBody] UpdateFeedbackDTO feedbackInfo)
        {
            if (feedbackInfo == null)
            {
                return BadRequest("Feedback data is required");
            }

            var response = await _feedbackService.UpdateFeedback(id, feedbackInfo);
            if (response.Status >= 0)
            {
                return Ok(response.Message);
            }
            else
            {
                return BadRequest(response.Message);
            }
        }

        [HttpDelete(ApiEndPointConstant.Feedback.FeedbackEndPoint)]
        [SwaggerOperation(Summary = "Delete Feedback by its id")]
        public async Task<IActionResult> DeleteFeedback(int id)
        {
            var response = await _feedbackService.DeleteFeedback(id);

            if (response.Status >= 0)
            {
                return Ok(response.Message);
            }
            else
            {
                return BadRequest(response.Message);
            }
        }
    }
}
