using Microsoft.AspNetCore.Mvc;
using GiveandTake_Repo.Models;
using Giveandtake_Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Giveandtake_Business;
using GiveandTake_API.Constants;
using GiveandTake_Repo.DTOs.Member;
using Swashbuckle.AspNetCore.Annotations;

namespace GiveandTake_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembershipController : ControllerBase
    {
        private readonly IMembershipService _membershipService;
        private readonly MemberShipBusiness _memberShipBusiness;

        public MembershipController(IMembershipService membershipService)
        {
            _membershipService = membershipService;
            _memberShipBusiness = new MemberShipBusiness();
        }

        [HttpPost("create-payment")]
        public IActionResult CreatePayment([FromBody] PaymentInformationModel model)
        {
            try
            {
                // Kiểm tra ModelState
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { Message = "Thông tin thanh toán không hợp lệ", Errors = ModelState });
                }

                // Kiểm tra null và amount
                if (model == null || model.Amount <= 0)
                {
                    return BadRequest(new { Message = "Thông tin thanh toán không hợp lệ" });
                }

                var paymentUrl = _membershipService.CreatePaymentUrlAsync(model, HttpContext);

                // Kiểm tra paymentUrl không null
                if (string.IsNullOrEmpty(paymentUrl))
                {
                    return BadRequest(new { Message = "Không thể tạo URL thanh toán" });
                }

                return Ok(new
                {
                    PaymentUrl = paymentUrl,
                    Message = "Tạo URL thanh toán thành công"
                });
            }
            catch (Exception ex)
            {

                var errorDetails = new List<string>
        {
            $"Message: {ex.Message}",
            $"StackTrace: {ex.StackTrace}"
        };


                if (ex.InnerException != null)
                {
                    errorDetails.Add($"InnerException Message: {ex.InnerException.Message}");
                    errorDetails.Add($"InnerException StackTrace: {ex.InnerException.StackTrace}");
                }

                Console.WriteLine(string.Join(Environment.NewLine, errorDetails));

                return StatusCode(500, new
                {
                    Message = "Có lỗi xảy ra khi tạo URL thanh toán",
                    Details = errorDetails 
                });
            }
        }

        [HttpGet("payment-callback")]
        public async Task<IActionResult> PaymentCallback()
        {
            try
            {
                var response = _membershipService.PaymentExecuteAsync(Request.Query);

                if (response.Success)
                {
                    //var paymentResponse = await _membershipService.HandlePaymentCallbackAsync(Request.Query);
                    await _membershipService.UpdateAccountIsPremiumAsync(response.AccountId);

                    return Ok(new
                    {
                        Success = true,
                        Message = "Thanh toán thành công",
                        Data = new
                        {
                            OrderId = response.OrderId,
                            PaymentMethod = response.PaymentMethod,
                            PaymentId = response.PaymentId,
                            TransactionId = response.TransactionId,
                            OrderDescription = response.OrderDescription,
                            VnPayResponseCode = response.VnPayResponseCode
                        }
                    });
                }

                return BadRequest(new
                {
                    Success = false,
                    Message = "Thanh toán thất bại",
                    VnPayResponseCode = response.VnPayResponseCode
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi xử lý callback thanh toán",
                    Error = ex.Message
                });
            }
        }

        [HttpGet(ApiEndPointConstant.Membership.MembershipsEndPoint)]
        [SwaggerOperation(Summary = "Get all Memberships with pagination")]
        public async Task<IActionResult> GetAllMemberships([FromQuery] int page = 1, [FromQuery] int pageSize = 8)
        {
            var response = await _memberShipBusiness.GetAllMemberships(page, pageSize);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response.Message);
        }

        [HttpGet(ApiEndPointConstant.Membership.MembershipEndPoint)]
        [SwaggerOperation(Summary = "Get Membership by Account Id")]
        public async Task<IActionResult> GetMembershipById(int id)
        {
            var response = await _memberShipBusiness.GetMembershipByAccountId(id);
            if (response.Status >= 0)
                return Ok(response.Data);
            else
                return BadRequest(response.Message);
        }

        [HttpPost(ApiEndPointConstant.Membership.MembershipsEndPoint)]
        [SwaggerOperation(Summary = "Create new Membership")]
        public async Task<IActionResult> CreateMembership([FromBody] CreateMembershipDTO createMembershipDto)
        {
            var response = await _memberShipBusiness.CreateMembership(createMembershipDto);

            if (response.Status >= 0)
            {
                return Ok(response.Message);
            }
            else
            {
                return BadRequest(response.Message);
            }
        }

        [HttpPost("3-months")]
        [SwaggerOperation(Summary = "Create 3 Months Membership")]
        public async Task<IActionResult> CreateMembership3Months([FromBody] CreateMembershipDTO createMembershipDto)
        {
            var response = await _memberShipBusiness.CreateMembership3Months(createMembershipDto);

            if (response.Status >= 0)
            {
                return Ok(response.Message);
            }
            else
            {
                return BadRequest(response.Message);
            }
        }

        [HttpPost("6-months")]
        [SwaggerOperation(Summary = "Create 6 Months Membership")]
        public async Task<IActionResult> CreateMembership6Months([FromBody] CreateMembershipDTO createMembershipDto)
        {
            var response = await _memberShipBusiness.CreateMembership6Months(createMembershipDto);

            if (response.Status >= 0)
            {
                return Ok(response.Message);
            }
            else
            {
                return BadRequest(response.Message);
            }
        }

        [HttpPut(ApiEndPointConstant.Membership.MembershipEndPoint)]
        [SwaggerOperation(Summary = "Update Membership")]
        public async Task<IActionResult> UpdateMembership(int id, [FromBody] UpdateMembershipDTO membershipInfo)
        {
            if (membershipInfo == null)
            {
                return BadRequest("Membership data is required");
            }

            var response = await _memberShipBusiness.UpdateMembership(id, membershipInfo);
            if (response.Status >= 0)
            {
                return Ok(response.Message);
            }
            else
            {
                return BadRequest(response.Message);
            }
        }

        [HttpDelete(ApiEndPointConstant.Membership.MembershipEndPoint)]
        [SwaggerOperation(Summary = "Delete Membership by its id")]
        public async Task<IActionResult> DeleteMembership(int id)
        {
            var response = await _memberShipBusiness.DeleteMembership(id);

            if (response.Status >= 0)
            {
                return Ok(response.Message);
            }
            else
            {
                return BadRequest(response.Message);
            }
        }

        [HttpGet("check-expiry")]
        [SwaggerOperation(Summary = "Check membership expiration date")]
        public async Task<IActionResult> CheckMembershipExpiry()
        {
            var accountIdClaim = HttpContext.User.FindFirst("accountId");

            if (accountIdClaim == null)
            {
                return BadRequest(new { message = "Không tìm thấy thông tin Account ID trong token." });
            }

            if (!int.TryParse(accountIdClaim.Value, out var accountId))
            {
                return BadRequest(new { message = "Account ID không hợp lệ." });
            }

            var result = await _membershipService.CheckMembershipExpiry(accountId);

            if (result.Message == "Success")
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}