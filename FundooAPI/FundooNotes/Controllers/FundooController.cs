using BusinessLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Dto;
using ModelLayer.Response;
using RepositoryLayer.CustomException;

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
                //if (!isValidEmail(userRegistrationDto.Email))
                //return BadRequest("Invalid email");
                //if (!Regex.IsMatch(@"^[a-zA-Z]([\w]*|\.[\w]+)*\@[a-zA-Z0-9]+\.[a-z]{2,3}$", userRegistrationDto.Email))
                //if (!Regex.IsMatch(@"^[\w-]+(?:\.[\w-]+)*@(?:[\w-]+\.)+[a-zA-Z]{2,7}$", userRegistrationDto.Email))
                /*if (!Regex.IsMatch(userRegistrationDto.Email, @"^[\w-]+(?:\.[\w-]+)*@(?:[\w-]+\.)+[a-zA-Z]{2,7}$"))
                {
                    /*var response = new FundooResponseModel<UserRegistrationDto>
                    {
                        Message = "Invalid email"
                    };
                    return BadRequest(response);
                    throw new InvalidEmailFormatException("Invalid email format");
                }*/

                /*if (!ModelState.IsValid)
                {
                    throw new Exception("Cannot be empty.");
                }*/

                var result = await _userBL.RegisterUser(userRegistrationDto);
                //if (result)
                //{
                var response = new FundooResponseModel<UserRegistrationDto>
                {
                    Message = "Registration successful"
                };
                return Ok(response);
                //}

                /*else
                {
                    /*var response = new FundooResponseModel<UserRegistrationDto>
                    {
                        Message = "User already exists"
                    };
                    return BadRequest(response);
                    throw new UserExistsException("User already exists");
                }*/
            }
            catch (InvalidEmailFormatException ex)
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
                //if (result)
                //{
                var response = new FundooResponseModel<UserLoginDto>
                {
                    Message = "Login successful"
                };
                return Ok(response);
                //}
                /*else
                {
                    throw new InvalidCredentialsException("Invalid email or password");
                }*/
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

    }
}
