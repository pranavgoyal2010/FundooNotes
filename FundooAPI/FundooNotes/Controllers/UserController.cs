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

    }
}
