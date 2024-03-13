﻿using BusinessLayer.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Dto;
using ModelLayer.Response;
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
        public async Task<IActionResult> CreateNote(CreateNoteDto createNoteDto)
        {
            try
            {
                var userIdClaimed = User.FindFirstValue(ClaimTypes.NameIdentifier);

                /*if (userIdClaimed == null)
                {
                    throw new ArgumentNullException("Invalid token");
                }*/

                int userIdClaimedInInt = Convert.ToInt32(userIdClaimed);
                var notes = await _noteBL.CreateNote(createNoteDto, userIdClaimedInInt);

                var response = new FundooResponseModel<IEnumerable<GetNoteDto>>
                {
                    Success = true,
                    Message = "note created successfully",
                    Data = notes
                };
                return Ok(response);
            }
            catch (ArgumentNullException ex)
            {
                var response = new FundooResponseModel<GetNoteDto>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
                return StatusCode(401, response);
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

                var notes = await _noteBL.GetAllNotes(userIdClaimedToInt);

                var response = new FundooResponseModel<IEnumerable<GetNoteDto>>
                {
                    Success = true,
                    Message = "Retrieved all notes successfully",
                    Data = notes
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new FundooResponseModel<GetNoteDto>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
                return StatusCode(500, response); //returning 500 error code as this error
                                                  //can occur only due to server error
            }
        }
    }
}
