using BusinessLayer.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Dto;
using ModelLayer.Response;
using RepositoryLayer.CustomException;
using System.Security.Claims;

namespace FundooNotes.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserBL _userBL;
        private readonly IMailServiceBL _mailServiceBL;

        public UserController(IUserBL userBL, IMailServiceBL mailServiceBL)
        {
            _userBL = userBL;
            _mailServiceBL = mailServiceBL;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser(UserRegistrationDto userRegistrationDto)
        {
            try
            {
                var result = await _userBL.RegisterUser(userRegistrationDto);

                var response = new FundooResponseModel<UserRegistrationDto>
                {
                    Message = "Registration successful"
                };
                return Ok(response);

            }
            catch (InvalidCredentialsException ex)
            {
                var response = new FundooResponseModel<UserRegistrationDto>
                {
                    Success = false,
                    Message = ex.Message
                };
                return BadRequest(response);
                //return BadRequest(ex.Message);
            }
            catch (UserExistsException ex)
            {
                var response = new FundooResponseModel<UserRegistrationDto>
                {
                    Success = false,
                    Message = ex.Message
                };
                return BadRequest(response);
                //return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                var response = new FundooResponseModel<UserRegistrationDto>
                {
                    Success = false,
                    Message = ex.Message
                };
                return StatusCode(500, response);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser(UserLoginDto userLoginDto)
        {
            try
            {
                var result = await _userBL.LoginUser(userLoginDto);

                var response = new FundooResponseModel<string>
                {
                    Message = "Login successful",
                    Data = result
                };
                return Ok(response);

            }
            catch (InvalidCredentialsException ex)
            {
                var response = new FundooResponseModel<UserLoginDto>
                {
                    Success = false,
                    Message = ex.Message
                };
                return NotFound(response);
                //return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                var response = new FundooResponseModel<UserLoginDto>
                {
                    Success = false,
                    Message = ex.Message
                };
                return StatusCode(500, response);
            }
        }

        /*[Authorize]
        [HttpGet("protected")]
        public IActionResult ProtectedEndpoint(string expectedUserEmail)
        {
            //extracting userId from token's payload
            var userIdClaimed = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEmailClaimed = User.FindFirstValue(ClaimTypes.Email);

            //if userId does not exist return the following
            if (userIdClaimed == null || userEmailClaimed == null)
                return Unauthorized("401 : user does not exist");

            //if userId does not match then return the following
            if (!userEmailClaimed.Equals(expectedUserEmail))
                return Unauthorized("401 : you are not unauthorized to access this resource");

            // This endpoint can only be accessed with a valid JWT token.
            else
                return Ok("Welcome to Fundoo notes");
        }

        [HttpGet]
        public async Task<IActionResult> SendEmail(string to)
        {
            string message = "Hello";
            string mailSubject = "This is a Subject";

            try
            {
                bool isEMailSent = await _mailServiceBL.SendEmail(to, message, mailSubject);
                if (isEMailSent)
                {
                    var response = new FundooResponseModel<string>
                    {
                        Message = "Email sent successfully",
                    };
                    return Ok(response);
                }
                else
                {
                    var response = new FundooResponseModel<string>
                    {
                        Message = "Failed to send email",
                    };
                    return BadRequest(response);
                }
            }
            catch (EmailSendingException ex)
            {
                var response = new FundooResponseModel<string>
                {
                    Success = false,
                    Message = ex.Message
                };
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                var response = new FundooResponseModel<string>
                {
                    Success = false,
                    Message = ex.Message
                };
                return StatusCode(500, response);
            }

        }*/

        [HttpPatch]
        public async Task<IActionResult> ForgotPassword(UserEmailDto userEmailDto)
        {
            try
            {
                string token = await _userBL.ForgotPassword(userEmailDto.Email);

                //HttpContext.Response.Headers.Add("Authorization", $"Bearer {token}");

                var response = new FundooResponseModel<string>
                {
                    Message = "Email sent successfully",
                    //Data = result
                };
                return Ok(response);
            }
            catch (InvalidCredentialsException ex)
            {
                var response = new FundooResponseModel<string>
                {
                    Success = false,
                    Message = ex.Message
                };
                return BadRequest(response);
            }
            catch (EmailSendingException ex)
            {
                var response = new FundooResponseModel<string>
                {
                    Success = false,
                    Message = ex.Message
                };
                return StatusCode(500, response);
            }
            catch (Exception ex)
            {
                var response = new FundooResponseModel<string>
                {
                    Success = false,
                    Message = ex.Message
                };
                return StatusCode(500, response);
            }

        }

        [Authorize]
        [HttpPatch("resetpassword")]
        public async Task<IActionResult> ResetPassword(UserPasswordDto userPasswordDto)
        {
            try
            {
                //var handler = new JwtSecurityTokenHandler();

                // Convert the token string into a JwtSecurityToken object to access its properties.
                //var jwtToken = handler.ReadJwtToken(userPasswordDto.Token);

                // Attempt to extract the email claim value.
                //var userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "nameid")?.Value;

                var userIdCliamed = User.FindFirstValue(ClaimTypes.NameIdentifier);
                int userIdClaimedToInt = Convert.ToInt32(userIdCliamed);
                //Console.WriteLine(userIdClaimedToInt);
                //var userEmail = User.FindFirstValue(ClaimTypes.Email);

                await _userBL.ResetPassword(userPasswordDto.Password, userIdClaimedToInt);

                var response = new FundooResponseModel<string>
                {
                    Message = "Password reset successfully",
                };
                return Ok(response);
            }
            catch (InvalidCredentialsException ex)
            {
                var response = new FundooResponseModel<string>
                {
                    Success = false,
                    Message = ex.Message
                };
                return BadRequest(response);
            }
            catch (UpdateFailException ex)
            {
                var response = new FundooResponseModel<string>
                {
                    Success = false,
                    Message = ex.Message
                };
                return StatusCode(500, response);
            }
            catch (Exception ex)
            {
                var response = new FundooResponseModel<string>
                {
                    Success = false,
                    Message = ex.Message
                };
                return StatusCode(500, response);
            }



        }
    }
}
