using CustomerOnboarding.Api.DTOs;
using CustomerOnboarding.Api.Models;
using CustomerOnboarding.Api.Repositories;
using CustomerOnboarding.Api.Utilities;
using System.Security.Cryptography;

namespace CustomerOnboarding.Api.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly IUserSecurityRepository _securityRepo;
        private readonly IOTPRepository _otpRepo;

        public UserService(
            IUserRepository userRepo,
            IUserSecurityRepository securityRepo,
            IOTPRepository otpRepo)
        {
            _userRepo = userRepo;
            _securityRepo = securityRepo;
            _otpRepo = otpRepo;
        }

        // PATH A & B: Check IC Number Status
        public async Task<ICCheckResponse> CheckICNumberAsync(string icNumber)
        {
            var user = await _userRepo.GetByICNumberAsync(icNumber);
            
            if (user != null && user.VerifiedEmail && user.VerifiedMobile)
            {
                // Fully registered user
                return new ICCheckResponse
                {
                    Status = "EXISTS",
                    Message = "Account already exists. User is fully registered.",
                    Action = "CONTINUE",
                    UserId = user.UserId
                };
            }
            else if (user != null)
            {
                // User exists but not fully verified - can continue registration
                return new ICCheckResponse
                {
                    Status = "IN_PROGRESS",
                    Message = "Registration incomplete. Continue verification?",
                    Action = "CONTINUE",
                    UserId = user.UserId
                };
            }

            // New IC - can start registration
            return new ICCheckResponse
            {
                Status = "NEW",
                Message = "IC not found. Start new registration.",
                Action = "START",
                UserId = null
            };
        }

        public async Task<User?> GetUserByICAsync(string icNumber)
        {
            return await _userRepo.GetByICNumberAsync(icNumber);
        }

        public async Task<UserResponse> StartOrContinueRegistrationAsync(RegisterUserRequest request)
        {
            var user = await _userRepo.GetByICNumberAsync(request.ICNumber);
            
            // Check if user already exists with both email and mobile verified
            if (user != null && user.VerifiedEmail && user.VerifiedMobile)
            {
                throw new InvalidOperationException("User already exists with this IC number. Both email and mobile are verified. Please login instead.");
            }
            
            if (user == null)
            {
                // Create new user (not verified yet)
                user = new User
                {
                    ICNumber = request.ICNumber,
                    CustomerName = request.CustomerName,
                    PhoneCode = request.PhoneCode,
                    PhoneNumber = request.PhoneNumber,
                    EmailAddress = request.EmailAddress,
                    VerifiedEmail = false,
                    VerifiedMobile = false,
                    TermsAgreed = false,
                    BiometricEnabled = false,
                    RegistrationDate = DateTime.UtcNow,
                    LastUpdated = DateTime.UtcNow
                };
                await _userRepo.AddAsync(user);
            }
            else
            {
                // User exists but not fully verified - allow update
                user.CustomerName = request.CustomerName;
                user.PhoneCode = request.PhoneCode;
                user.PhoneNumber = request.PhoneNumber;
                user.EmailAddress = request.EmailAddress;
                user.LastUpdated = DateTime.UtcNow;
            }

            await _userRepo.SaveChangesAsync();

            return new UserResponse
            {
                UserId = user.UserId,
                ICNumber = user.ICNumber,
                CustomerName = user.CustomerName,
                PhoneCode = user.PhoneCode,
                PhoneNumber = user.PhoneNumber,
                EmailAddress = user.EmailAddress,
                BiometricEnabled = user.BiometricEnabled,
                RegistrationDate = user.RegistrationDate
            };
        }

        // Send OTP for registration, migration, or change email/mobile
public async Task<OTPResponse> SendOTPAsync(SendOTPRequest request, string? newContact = null)
{
    if (request.VerificationType != "EMAIL" && request.VerificationType != "MOBILE")
        return new OTPResponse { IsSuccess = false, Message = "Invalid verification type" };

    var user = await _userRepo.GetByIdAsync(request.UserId);
    if (user == null)
        return new OTPResponse { IsSuccess = false, Message = "User not found. Please start registration first." };

    // Determine flow and target based on context
    OTPFlow flow;
    string target;
    
    if (!string.IsNullOrEmpty(newContact))
    {
        // Change email/mobile flow - send OTP to NEW contact
        flow = OTPFlow.ChangeEmail;
        target = newContact;
    }
    else if (user.VerifiedEmail && user.VerifiedMobile)
    {
        // User is fully verified, this is migration flow
        flow = OTPFlow.Migration;
        target = request.VerificationType == "EMAIL" ? user.EmailAddress : $"{user.PhoneCode}{user.PhoneNumber}";
    }
    else
    {
        // User is not fully verified, this is registration flow
        flow = OTPFlow.Registration;
        target = request.VerificationType == "EMAIL" ? user.EmailAddress : $"{user.PhoneCode}{user.PhoneNumber}";
    }

    if (string.IsNullOrEmpty(target))
        return new OTPResponse { IsSuccess = false, Message = "Contact information not available" };

    string obfuscated = request.VerificationType == "EMAIL"
        ? ObfuscationHelper.ObfuscateEmail(target)
        : ObfuscationHelper.ObfuscateMobile(target);

    var otpCode = RandomNumberGenerator.GetInt32(1000, 9999).ToString("D4");

    var otpAttempt = new OTPAttempt
    {
        UserId = user.UserId,
        Flow = flow,
        TargetType = request.VerificationType,
        TargetValue = target,
        OTPCode = otpCode,
        AttemptCount = 0,
        IsVerified = false
    };

    await _otpRepo.AddAsync(otpAttempt);
    await _otpRepo.SaveChangesAsync();

    Console.WriteLine($"[OTP] Flow={flow}, Sent to {obfuscated}: {otpCode}");

    return new OTPResponse
    {
        IsSuccess = true,
        Message = "OTP sent successfully",
        ObfuscatedTarget = obfuscated,
        AttemptId = otpAttempt.AttemptId,
        OTPCode = otpCode + " NOTE: Only for testing - it will be Removed in production from the response "
    };
}

        // Verify OTP for registration, migration, or change email
public async Task<OTPResponse> VerifyOTPAsync(VerifyOTPRequest request)
{
    var otpAttempt = await _otpRepo.GetByIdAsync(request.AttemptId);
    if (otpAttempt == null)
        return new OTPResponse { IsSuccess = false, Message = "OTP attempt not found." };

    if (otpAttempt.IsVerified)
        return new OTPResponse { IsSuccess = false, Message = "This OTP has already been verified." };

    if (otpAttempt.OTPCode != request.OTPCode)
    {
        otpAttempt.AttemptCount++;
        await _otpRepo.SaveChangesAsync();
        return new OTPResponse { IsSuccess = false, Message = "Incorrect OTP. Please try again." };
    }

    if ((DateTime.UtcNow - otpAttempt.CreationTime).TotalMinutes > 5)
        return new OTPResponse { IsSuccess = false, Message = "OTP expired. Please request a new one." };

    otpAttempt.IsVerified = true;
    await _otpRepo.SaveChangesAsync();

    if (!otpAttempt.UserId.HasValue)
        return new OTPResponse { IsSuccess = false, Message = "Invalid OTP record." };

    var user = await _userRepo.GetByIdAsync(otpAttempt.UserId.Value);
    if (user == null)
        return new OTPResponse { IsSuccess = false, Message = "User not found." };

    // Handle different flows
    switch (otpAttempt.Flow)
    {
        case OTPFlow.Registration:
            // Mark email or mobile as verified
            if (otpAttempt.TargetType == "EMAIL")
                user.VerifiedEmail = true;
            else
                user.VerifiedMobile = true;

            user.LastUpdated = DateTime.UtcNow;
            await _userRepo.SaveChangesAsync();

            // Check if both are verified
            if (user.VerifiedEmail && user.VerifiedMobile)
            {
                return new OTPResponse 
                { 
                    IsSuccess = true, 
                    Message = "Registration completed! Please set your PIN and accept terms." 
                };
            }
            return new OTPResponse { IsSuccess = true, Message = "OTP verified. Please verify your other contact." };

        case OTPFlow.ChangeEmail:
            // Update user's email or mobile based on target type
            if (otpAttempt.TargetType == "EMAIL")
            {
                user.EmailAddress = otpAttempt.TargetValue;
                user.VerifiedEmail = true;
            }
            else
            {
                // For mobile, split the target value into phone code and number
                // Assuming format: +60123456789
                var fullNumber = otpAttempt.TargetValue;
                var phoneCode = fullNumber.Substring(0, fullNumber.Length - 10); // Get phone code part
                var phoneNumber = fullNumber.Substring(fullNumber.Length - 10); // Get last 10 digits
                user.PhoneCode = phoneCode;
                user.PhoneNumber = phoneNumber;
                user.VerifiedMobile = true;
            }
            user.LastUpdated = DateTime.UtcNow;
            await _userRepo.SaveChangesAsync();
            return new OTPResponse { IsSuccess = true, Message = otpAttempt.TargetType == "EMAIL" ? "Email changed successfully." : "Mobile number changed successfully." };

        case OTPFlow.Migration:
            // Just verify OTP for migration flow
            user.LastUpdated = DateTime.UtcNow;
            await _userRepo.SaveChangesAsync();
            return new OTPResponse { IsSuccess = true, Message = "OTP verified successfully." };

        default:
            return new OTPResponse { IsSuccess = false, Message = "Unknown flow type." };
    }
}

        // Agree to Terms during user registration (after verification)
        public async Task<bool> AgreeTermsAsync(int userId)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null) return false;

            // Update terms agreement on user
            user.TermsAgreed = true;
            await _userRepo.SaveChangesAsync();
            
            return true;
        }

        // Create and Confirm PIN (user must exist after OTP verification)
        public async Task<bool> SetPINAsync(int userId, string pin, string confirmPin)
        {
            // Validate PIN match
            if (pin != confirmPin)
            {
                return false;
            }

            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null) return false;

            // Hash the PIN before storing
            string hashedPIN = HashingHelper.HashPIN(pin);

            var security = await _securityRepo.GetByUserIdAsync(user.UserId);
            if (security == null)
            {
                security = new UserSecurity
                {
                    UserId = user.UserId,
                    HashedPIN = hashedPIN,
                    PINLastUpdated = DateTime.UtcNow
                };
                await _securityRepo.AddAsync(security);
            }
            else
            {
                security.HashedPIN = hashedPIN;
                security.PINLastUpdated = DateTime.UtcNow;
            }
            await _securityRepo.SaveChangesAsync();

            return true;
        }

        // Step 4: Enable Biometric Login
        public async Task<bool> SetBiometricAsync(int userId, bool enable)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null) return false;

            user.BiometricEnabled = enable;
            await _userRepo.SaveChangesAsync();

            return true;
        }

        // PATH B: Migration - Initiate migration for existing user
        public async Task<MigrationResponse> InitiateMigrationAsync(string icNumber)
        {
            var user = await _userRepo.GetByICNumberAsync(icNumber);
            if (user == null)
            {
                return new MigrationResponse
                {
                    IsSuccess = false,
                    Message = "User not found. Please register as a new user."
                };
            }

            // Check if user has verified both email and mobile
            if (!user.VerifiedEmail || !user.VerifiedMobile)
            {
                return new MigrationResponse
                {
                    IsSuccess = false,
                    Message = "Migration not allowed. Please verify both your email and mobile number first."
                };
            }

            // Automatically send OTP to mobile number using existing SendOTP logic
            var otpResponse = await SendOTPAsync(new SendOTPRequest
            {
                UserId = user.UserId,
                VerificationType = "MOBILE"
            });

            if (!otpResponse.IsSuccess)
            {
                return new MigrationResponse
                {
                    IsSuccess = false,
                    Message = otpResponse.Message
                };
            }

            // Return obfuscated contact info with OTP details
            return new MigrationResponse
            {
                IsSuccess = true,
                Message = $"Migration initiated. OTP sent to {otpResponse.ObfuscatedTarget}. Please verify to continue.",
                UserId = user.UserId,
                ObfuscatedMobile = otpResponse.ObfuscatedTarget,
                ObfuscatedEmail = ObfuscationHelper.ObfuscateEmail(user.EmailAddress),
                AttemptId = otpResponse.AttemptId,
                OTPCode = otpResponse.OTPCode // NOTE: Only for testing - Remove in production
            };
        }

        // PATH B: Initiate change email (sends OTP to new email)
        public async Task<ChangeEmailResponse> ChangeEmailAsync(int userId, string newEmail)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null)
            {
                return new ChangeEmailResponse
                {
                    IsSuccess = false,
                    Message = "User not found."
                };
            }

            // Check if user is fully verified
            if (!user.VerifiedEmail || !user.VerifiedMobile)
            {
                return new ChangeEmailResponse
                {
                    IsSuccess = false,
                    Message = "You must complete registration before changing contact information."
                };
            }

            // Send OTP to new email
            var otpResponse = await SendOTPAsync(new SendOTPRequest 
            { 
                UserId = userId, 
                VerificationType = "EMAIL" 
            }, newEmail);

            return new ChangeEmailResponse
            {
                IsSuccess = otpResponse.IsSuccess,
                Message = otpResponse.IsSuccess 
                    ? $"OTP sent to new email {otpResponse.ObfuscatedTarget}. Please verify to complete the change." 
                    : otpResponse.Message,
                AttemptId = otpResponse.AttemptId
            };
        }

        public async Task<IEnumerable<UserResponse>> GetAllUsersAsync()
        {
            var users = await _userRepo.GetAllAsync();
            return users.Select(u => new UserResponse
            {
                UserId = u.UserId,
                ICNumber = u.ICNumber,
                CustomerName = u.CustomerName,
                PhoneCode = u.PhoneCode,
                PhoneNumber = u.PhoneNumber,
                EmailAddress = u.EmailAddress,
                BiometricEnabled = u.BiometricEnabled,
                RegistrationDate = u.RegistrationDate
            });
        }
    }
}