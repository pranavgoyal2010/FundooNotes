using BusinessLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Dto;

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
                if (result)
                    return Ok("Registration successful");
                else
                    return BadRequest("Invalid email or password");
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser(UserLoginDto userLoginDto)
        {
            try
            {
                var result = await _userBL.LoginUser(userLoginDto);
                if (result)
                {
                    return Ok("Login successful");
                }
                else
                {
                    return Unauthorized("Invalid email or password");
                }
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }
    }
}
