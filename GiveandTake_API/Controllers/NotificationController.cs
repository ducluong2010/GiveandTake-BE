using GiveandTake_API.Constants;
using GiveandTake_Repo.DTOs.Notification;
using GiveandTake_Repo.Models;
using Giveandtake_Services.Implements;
using Giveandtake_Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GiveandTake_API.Controllers
{
    [ApiController]
    public class NotificationController : Controller
    {
        private readonly NotificationService _notificationService;

        public NotificationController()
        {
            _notificationService = new NotificationService();
        }

        [HttpGet(ApiEndPointConstant.Notification.NotisEndPoint)]
        [SwaggerOperation(Summary = "Get all Notifications with pagination")]
        public async Task<IActionResult> GetAllNotifications([FromQuery] int page = 1, [FromQuery] int pageSize = 8)
        {
            var response = await _notificationService.GetAllNotifications(page, pageSize);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response.Message);
        }

        [HttpGet(ApiEndPointConstant.Notification.NotiEndPoint)]
        [SwaggerOperation(Summary = "Get Notification by its id")]
        public async Task<IActionResult> GetNotificationById([FromRoute] int id)
        {
            var response = await _notificationService.GetNotificationById(id);
            if (response.Status >= 0)
            {
                return Ok(response.Data);
            }
            else
            {
                return BadRequest(response); 
            }
        }
        [HttpGet(ApiEndPointConstant.Notification.NotiAccEndPoint)]
        [SwaggerOperation(Summary = "Get all Notifications by Account Id")]
        public async Task<IActionResult> GetAllNotificationsByAccountId([FromRoute] int id, [FromQuery] int page = 1, [FromQuery] int pageSize = 8)
        {
            var response = await _notificationService.GetAllNotificationsByAccountId(id, page, pageSize);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response.Message);
        }
        [HttpGet(ApiEndPointConstant.Notification.NotiAppEndPoint)]
        [SwaggerOperation(Summary = "Get all Notifications approved")]
        public async Task<IActionResult> GetNotiApprovedAccount([FromRoute] int id, [FromQuery] int page = 1, [FromQuery] int pageSize = 8)
        {
            var response = await _notificationService.GetNotiApprovedAccount(id, page, pageSize);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response.Message);
        }

        [HttpGet(ApiEndPointConstant.Notification.NotiBonusEndPoint)]
        [SwaggerOperation(Summary = "Get all Notifications Bonus")]
        public async Task<IActionResult> GetNotiBonusAccount([FromRoute] int id, [FromQuery] int page = 1, [FromQuery] int pageSize = 8)
        {
            var response = await _notificationService.GetNotiBonusAccount(id, page, pageSize);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response.Message);
        }

        [HttpGet(ApiEndPointConstant.Notification.NotiPointEndPoint)]
        [SwaggerOperation(Summary = "Get all Notifications Point")]
        public async Task<IActionResult> GetNotiPointAccount([FromRoute] int id, [FromQuery] int page = 1, [FromQuery] int pageSize = 8)
        {
            var response = await _notificationService.GetNotiPointAccount(id, page, pageSize);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response.Message);
        }

        [HttpGet(ApiEndPointConstant.Notification.NotiRejectEndPoint)]
        [SwaggerOperation(Summary = "Get all Notifications Reject")]
        public async Task<IActionResult> GetNotiRejectAccount([FromRoute] int id, [FromQuery] int page = 1, [FromQuery] int pageSize = 8)
        {
            var response = await _notificationService.GetNotiRejectAccount(id, page, pageSize);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response.Message);
        }

        [HttpGet(ApiEndPointConstant.Notification.NotiAcceptEndPoint)]
        [SwaggerOperation(Summary = "Get all Notifications Accept")]
        public async Task<IActionResult> GetNotiAcceptAccount([FromRoute] int id, [FromQuery] int page = 1, [FromQuery] int pageSize = 8)
        {
            var response = await _notificationService.GetNotiAcceptAccount(id, page, pageSize);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response.Message);
        }

        [HttpGet(ApiEndPointConstant.Notification.NotiCancelEndPoint)]
        [SwaggerOperation(Summary = "Get all Notifications Cancelled")]
        public async Task<IActionResult> GetNotiCancelAccount([FromRoute] int id, [FromQuery] int page = 1, [FromQuery] int pageSize = 8)
        {
            var response = await _notificationService.GetNotiCancelAccount(id, page, pageSize);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response.Message);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "2")]
        [HttpGet(ApiEndPointConstant.Notification.NotiStaffEndPoint)]
        [SwaggerOperation(Summary = "Get all Notifications by Staff Id")]
        public async Task<IActionResult> GetAllNotificationsByStaffId([FromRoute] int id, [FromQuery] int page = 1, [FromQuery] int pageSize = 8)
        {
            var response = await _notificationService.GetAllNotificationsByStaffId(id, page, pageSize);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response.Message);
        }
        [HttpPost(ApiEndPointConstant.Notification.NotiCreateEndPoint)]
        [SwaggerOperation(Summary = "Create a new Notification")]
        public async Task<IActionResult> CreateNotification([FromBody] NotificationCreateDTO notificationInfo)
        {
            var response = await _notificationService.CreateNotification(notificationInfo);
            if (response.Status >= 0)
                return Ok(response.Message);
            else
                return BadRequest(response);
        }
        [HttpPut(ApiEndPointConstant.Notification.NotiUpdateEndPoint)]
        [SwaggerOperation(Summary = "Update an existing Notification")]
        public async Task<IActionResult> UpdateNotification([FromRoute] int id, [FromBody] NotificationUpdateDTO notificationInfo)
        {
            var response = await _notificationService.UpdateNotification(id, notificationInfo);
            if (response.Status >= 0)
                return Ok(response.Message);
            else
                return BadRequest(response);
        }
        [HttpDelete(ApiEndPointConstant.Notification.NotiDeleteEndPoint)]
        [SwaggerOperation(Summary = "Delete a Notification")]
        public async Task<IActionResult> DeleteNotification([FromRoute] int id)
        {
            var response = await _notificationService.DeleteNotification(id);
            if (response.Status >= 0)
                return Ok(response.Message);
            else
                return BadRequest(response);
        }
        [HttpPut(ApiEndPointConstant.Notification.NotiChangeEndPoint)]
        [SwaggerOperation(Summary = "Toggle status of a Notification")]
        public async Task<IActionResult> ToggleIsReadStatus([FromRoute] int id)
        {
            var response = await _notificationService.ToggleIsReadStatus(id);
            if (response.Status >= 0)
                return Ok(response.Message);
            else
                return BadRequest(response);
        }
    }
}
