using BusinessLayer.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Dto;
using ModelLayer.Response;
using RepositoryLayer.CustomException;
using System.Security.Claims;

namespace FundooNotes.Controllers
{
    [Route("api/note")]
    [ApiController]
    public class NoteController : ControllerBase
    {
        private readonly INoteBL _noteBL;

        public NoteController(INoteBL noteBL)
        {
            _noteBL = noteBL;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateNote([FromBody] CreateNoteDto createNoteDto)
        {
            try
            {
                var userIdClaimed = User.FindFirstValue(ClaimTypes.NameIdentifier);

                int userIdClaimedInInt = Convert.ToInt32(userIdClaimed);
                var newNote = await _noteBL.CreateNote(createNoteDto, userIdClaimedInInt);

                var response = new FundooResponseModel<GetNoteDto>
                {
                    //Success = true,
                    Message = "note created successfully",
                    Data = newNote
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new FundooResponseModel<string>
                {
                    Success = false,
                    Message = ex.Message,
                    //Data = null
                };
                return StatusCode(500, response); //returning 500 error code as this error
                                                  //can occur only due to server error
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllNotes()
        {
            try
            {
                var userIdCliamed = User.FindFirstValue(ClaimTypes.NameIdentifier);
                int userIdClaimedToInt = Convert.ToInt32(userIdCliamed);

                var allNotes = await _noteBL.GetAllNotes(userIdClaimedToInt);

                var response = new FundooResponseModel<IEnumerable<GetNoteDto>>
                {
                    //Success = true,
                    Message = "Retrieved all notes successfully",
                    Data = allNotes
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new FundooResponseModel<string>
                {
                    Success = false,
                    Message = ex.Message,
                    //Data = null
                };
                return StatusCode(500, response); //returning 500 error code as this error
                                                  //can occur only due to server error
            }
        }

        [Authorize]
        [HttpGet("{noteId}")]
        public async Task<IActionResult> GetNoteById(int noteId)
        {
            try
            {
                var userIdCliamed = User.FindFirstValue(ClaimTypes.NameIdentifier);
                int userIdClaimedToInt = Convert.ToInt32(userIdCliamed);

                var note = await _noteBL.GetNoteById(userIdClaimedToInt, noteId);

                var response = new FundooResponseModel<GetNoteDto>
                {
                    //Success = true,
                    Message = "Retrieved note successfully",
                    Data = note
                };
                return Ok(response);
            }
            catch (NullReferenceException ex)
            {
                var response = new FundooResponseModel<string>
                {
                    Success = false,
                    Message = ex.Message,
                    //Data = null
                };
                return BadRequest(response); //returning 500 error code as this error
                                             //can occur only due to server error
            }
            catch (Exception ex)
            {
                var response = new FundooResponseModel<string>
                {
                    Success = false,
                    Message = ex.Message,
                    //Data = null
                };
                return StatusCode(500, response); //returning 500 error code as this error
                                                  //can occur only due to server error
            }
        }

        [Authorize]
        [HttpPut("{noteId}")]
        public async Task<IActionResult> UpdateNote(int noteId, [FromBody] UpdateNoteDto updateNoteDto)
        {
            try
            {
                var userIdCliamed = User.FindFirstValue(ClaimTypes.NameIdentifier);
                int userIdClaimedToInt = Convert.ToInt32(userIdCliamed);

                var result = await _noteBL.UpdateNote(updateNoteDto, userIdClaimedToInt, noteId);

                var response = new FundooResponseModel<GetNoteDto>
                {
                    Message = "Updated note successfully",
                    Data = result
                };
                return Ok(response);
            }
            catch (UpdateFailException ex)
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
                    //Data = null
                };
                return StatusCode(500, response); //returning 500 error code as this error
                                                  //can occur only due to server error
            }
        }

        [Authorize]
        [HttpPatch("trash/{noteId}")]
        public async Task<IActionResult> TrashNote(int noteId)
        {
            try
            {
                var userIdCliamed = User.FindFirstValue(ClaimTypes.NameIdentifier);
                int userIdClaimedToInt = Convert.ToInt32(userIdCliamed);

                var result = await _noteBL.TrashNote(userIdClaimedToInt, noteId);

                var response = new FundooResponseModel<GetNoteDto>
                {
                    Message = "Operation performed successfully",
                    Data = result
                };
                return Ok(response);
            }
            catch (UpdateFailException ex)
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
        [HttpPatch("archive/{noteId}")]
        public async Task<IActionResult> ArchiveNote(int noteId)
        {
            try
            {
                var userIdCliamed = User.FindFirstValue(ClaimTypes.NameIdentifier);
                int userIdClaimedToInt = Convert.ToInt32(userIdCliamed);

                var result = await _noteBL.ArchiveNote(userIdClaimedToInt, noteId);

                var response = new FundooResponseModel<GetNoteDto>
                {
                    Message = "Operation performed successfully",
                    Data = result
                };
                return Ok(response);
            }
            catch (UpdateFailException ex)
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
        [HttpDelete("{noteId}")]
        public async Task<IActionResult> DeleteNote(int noteId)
        {
            try
            {
                var userIdCliamed = User.FindFirstValue(ClaimTypes.NameIdentifier);
                int userIdClaimedToInt = Convert.ToInt32(userIdCliamed);

                var result = await _noteBL.DeleteNote(userIdClaimedToInt, noteId);

                var response = new FundooResponseModel<bool>
                {
                    Message = "Operation performed successfully",
                    Data = result
                };
                return Ok(response);
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
