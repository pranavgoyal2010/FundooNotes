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

        public UserController(IUserBL userBL)
        {
            _userBL = userBL;
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
            }
            catch (UserExistsException ex)
            {
                var response = new FundooResponseModel<UserRegistrationDto>
                {
                    Success = false,
                    Message = ex.Message
                };
                return BadRequest(response);
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

        [HttpPost("forgotpassword")]
        public async Task<IActionResult> ForgotPassword(UserEmailDto userEmailDto)
        {
            try
            {
                string token = await _userBL.ForgotPassword(userEmailDto.Email);

                //HttpContext.Response.Headers.Add("Authorization", $"Bearer {token}");

                var response = new FundooResponseModel<string>
                {
                    Message = "Email sent successfully",
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
        [HttpPost("resetpassword")]
        public async Task<IActionResult> ResetPassword(UserPasswordDto userPasswordDto)
        {
            try
            {
                var userIdClaimed = User.FindFirstValue(ClaimTypes.NameIdentifier);
                int userIdClaimedToInt = Convert.ToInt32(userIdClaimed);

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
