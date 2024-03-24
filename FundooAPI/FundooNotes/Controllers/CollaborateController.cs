using BusinessLayer.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Dto;
using ModelLayer.Response;
using RepositoryLayer.CustomException;
using System.Security.Claims;

namespace FundooNotes.Controllers
{
    [Route("api/collaborate")]
    [ApiController]
    public class CollaborateController : ControllerBase
    {
        private readonly ICollaborateBL _collaborateBL;

        public CollaborateController(ICollaborateBL collaborateBL)
        {
            _collaborateBL = collaborateBL;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllCollaborators()
        {
            try
            {
                var userIdClaimed = User.FindFirstValue(ClaimTypes.NameIdentifier);
                int userIdClaimedToInt = Convert.ToInt32(userIdClaimed);

                var allCollaborators = await _collaborateBL.GetAllCollaborators(userIdClaimedToInt);

                var response = new FundooResponseModel<IEnumerable<GetCollaboratorDto>>
                {
                    Message = "Retrieved all collaborators successfully",
                    Data = allCollaborators
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new FundooResponseModel<string>
                {
                    Success = false,
                    Message = ex.Message,
                };
                return StatusCode(500, response); //returning 500 error code as this error
                                                  //can occur only due to server error
            }
        }

        [Authorize]
        [HttpGet("getallcollabsbyid")]
        public async Task<IActionResult> GetAllCollaboratorsById(int noteId)
        {
            try
            {
                var userIdClaimed = User.FindFirstValue(ClaimTypes.NameIdentifier);
                int userIdClaimedToInt = Convert.ToInt32(userIdClaimed);

                var allCollaborators = await _collaborateBL.GetAllCollaboratorsById(userIdClaimedToInt, noteId);

                var response = new FundooResponseModel<IEnumerable<GetCollaboratorDto>>
                {
                    Message = "Retrieved all collaborators successfully",
                    Data = allCollaborators
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new FundooResponseModel<string>
                {
                    Success = false,
                    Message = ex.Message,
                };
                return StatusCode(500, response); //returning 500 error code as this error
                                                  //can occur only due to server error
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddCollaborate(int noteId, AddCollaboratorDto addCollaboratorDto)
        {
            try
            {
                var userIdClaimed = User.FindFirstValue(ClaimTypes.NameIdentifier);

                int userIdClaimedInInt = Convert.ToInt32(userIdClaimed);

                var result = await _collaborateBL.AddCollaborator(userIdClaimedInInt, noteId, addCollaboratorDto);

                var response = new FundooResponseModel<string>
                {
                    Message = "Added Collaborator successfully"
                };

                return Ok(response);
            }
            catch (InvalidCredentialsException ex)
            {
                var response = new FundooResponseModel<string>
                {
                    Success = false,
                    Message = ex.Message,
                };
                return BadRequest(response); //status code of 400 is returned as there is client error
            }
            catch (NoteDoesNotExistException ex)
            {
                var response = new FundooResponseModel<string>
                {
                    Success = false,
                    Message = ex.Message,
                };
                return BadRequest(response); //status code of 400 is returned as there is client error
            }
            catch (Exception ex)
            {
                var response = new FundooResponseModel<string>
                {
                    Success = false,
                    Message = ex.Message,
                };
                return StatusCode(500, response); //returning 500 error code as this error
                                                  //can occur only due to server error
            }

        }


        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> RemoveCollaborate(int noteId, RemoveCollaboratorDto removeCollaboratorDto)
        {
            try
            {
                var userIdClaimed = User.FindFirstValue(ClaimTypes.NameIdentifier);

                int userIdClaimedInInt = Convert.ToInt32(userIdClaimed);

                var result = await _collaborateBL.RemoveCollaborator(userIdClaimedInInt, noteId, removeCollaboratorDto);

                var response = new FundooResponseModel<string>
                {
                    Message = "Removed Collaborator successfully"
                };

                return Ok(response);
            }
            catch (InvalidCredentialsException ex)
            {
                var response = new FundooResponseModel<string>
                {
                    Success = false,
                    Message = ex.Message,
                };
                return BadRequest(response); //status code of 400 is returned as there is client error
            }
            catch (NoteDoesNotExistException ex)
            {
                var response = new FundooResponseModel<string>
                {
                    Success = false,
                    Message = ex.Message,
                };
                return BadRequest(response); //status code of 400 is returned as there is client error
            }
            catch (DeleteFailException ex)
            {
                var response = new FundooResponseModel<string>
                {
                    Success = false,
                    Message = ex.Message,
                };
                return BadRequest(response); //status code of 400 is returned as there is client error
            }
            catch (Exception ex)
            {
                var response = new FundooResponseModel<string>
                {
                    Success = false,
                    Message = ex.Message,
                };
                return StatusCode(500, response); //returning 500 error code as this error
                                                  //can occur only due to server error
            }

        }
    }
}
