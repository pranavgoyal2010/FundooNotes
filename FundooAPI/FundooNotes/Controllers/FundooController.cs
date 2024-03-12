using BusinessLayer.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Dto;
using ModelLayer.Response;
using RepositoryLayer.CustomException;
using System.Security.Claims;

namespace FundooNotes.Controllers
{
    [Route("api/fundoo")]
    [ApiController]
    public class FundooController : ControllerBase
    {
        private readonly IUserBL _userBL;

        public FundooController(IUserBL userBL)
        {
            _userBL = userBL;
        }

        [HttpPost("register")]
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

                var response = new FundooResponseModel<UserLoginDto>
                {
                    Message = "Login successful",
                    Token = result
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

        [Authorize]
        [HttpGet("protected")]
        public IActionResult ProtectedEndpoint(string email)
        {
            //extracting email from token's payload
            var emailClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //if email does not exist return the following
            if (emailClaim == null)
                return Unauthorized("401 : user does not exist");

            //if email does not match then return the following
            if (!email.Equals(emailClaim))
                return Unauthorized("403 : unauthorized user");
            else
                // This endpoint can only be accessed with a valid JWT token.
                return Ok("Welcome to Fundoo notes");
        }

    }
}
