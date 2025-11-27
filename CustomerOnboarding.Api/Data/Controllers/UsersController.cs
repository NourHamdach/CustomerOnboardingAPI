using CustomerOnboarding.Api.DTOs;
using CustomerOnboarding.Api.Models;
using CustomerOnboarding.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CustomerOnboarding.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // -------------------------------
        // STEP 1: Check IC & Start Registration
        // -------------------------------
        [HttpGet("check-ic/{icNumber}")]
        public async Task<IActionResult> CheckIC(string icNumber)
        {
            var icStatus = await _userService.CheckICNumberAsync(icNumber);
            return Ok(icStatus);
        }

        // -------------------------------
        // STEP 1/CONTINUE REGISTRATION
        // -------------------------------
        [HttpPost("registration")]
        public async Task<IActionResult> StartRegistration([FromBody] RegisterUserRequest request)
        {
            try
            {
                var registration = await _userService.StartOrContinueRegistrationAsync(request);
                return Ok(registration);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        // -------------------------------
        // STEP 1/2: Send OTP
        // -------------------------------
        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOTP([FromBody] SendOTPRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Service handles both new registration & existing user
            var result = await _userService.SendOTPAsync(request, null);

            return Ok(result);
        }

        // -------------------------------
        // STEP 2/3: Verify OTP
        // -------------------------------
[HttpPost("verify-otp")]
public async Task<IActionResult> VerifyOTP([FromBody] VerifyOTPRequest request)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);

    var result = await _userService.VerifyOTPAsync(request);

    if (!result.IsSuccess)
        return BadRequest(new { message = result.Message });

    return Ok(result);
}

        // -------------------------------
        // STEP 3: Privacy Policy Agreement
        // -------------------------------
        [HttpPost("agree-terms/{userId}")]
        public async Task<IActionResult> AgreeTerms(int userId)
        {
            var result = await _userService.AgreeTermsAsync(userId);
            if (!result)
                return BadRequest(new { message = "User not found. Please complete registration first." });
            
            return Ok(new { success = result, message = "Terms accepted successfully." });
        }

        // -------------------------------
        // STEP 4: Create PIN
        // -------------------------------
        [HttpPost("pin/{userId}")]
        public async Task<IActionResult> CreatePIN(int userId, [FromBody] CreatePinRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(new { message = "Validation failed", errors });
            }

            if (request.Pin != request.ConfirmPin)
                return BadRequest(new { message = "PINs do not match. Please enter your PIN again." });

            var result = await _userService.SetPINAsync(userId, request.Pin, request.ConfirmPin);
            if (!result)
                return BadRequest(new { message = "User not found. Please complete registration first." });
            
            return Ok(new { success = result, message = "PIN set successfully." });
        }
        


        // -------------------------------
        // STEP 5: Enable Biometric
        // -------------------------------
        [HttpPost("biometric/{userId}")]
        public async Task<IActionResult> EnableBiometric(int userId, [FromBody] EnableBiometricRequest request)
        {
            var result = await _userService.SetBiometricAsync(userId, request.Enable);
            if (!result)
                return BadRequest(new { message = "User not found. Please complete registration first." });
            
            return Ok(new { success = result, message = request.Enable ? "Biometric enabled successfully." : "Biometric disabled successfully." });
        }

        // -------------------------------
        // PATH B: Initiate Migration
        // -------------------------------
        [HttpPost("migration/initiate")]
        public async Task<IActionResult> InitiateMigration([FromBody] MigrationRequest request)
        {
            var result = await _userService.InitiateMigrationAsync(request.ICNumber);
            return Ok(result);
        }

        // -------------------------------
        // PATH B: Change Email During Migration
        // -------------------------------
        [HttpPost("migration/change-email")]
        public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailRequest request)
        {
            var result = await _userService.ChangeEmailAsync(request.UserId, request.NewEmailAddress);
            return Ok(result);
        }

        // -------------------------------
        // GET All Registered Users (for testing/admin)
        // -------------------------------
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        // -------------------------------
        // GET User by IC Number
        // -------------------------------
        [HttpGet("ic/{icNumber}")]
        public async Task<IActionResult> GetUserByIC(string icNumber)
        {
            var user = await _userService.GetUserByICAsync(icNumber);
            if (user == null)
                return NotFound(new { message = "User not found" });
            
            return Ok(user);
        }
    }
}
