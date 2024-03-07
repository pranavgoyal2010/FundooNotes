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

        [HttpPost]
        public async Task<IActionResult> RegisterUser(UserRegistrationDto userRegistrationDto)
        {
            try
            {
                var result = await _userBL.RegisterUser(userRegistrationDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }
    }
}
