﻿using BusinessLayer.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Dto;
using ModelLayer.Response;
using Newtonsoft.Json;
using RepositoryLayer.CustomException;
using StackExchange.Redis;
using System.Security.Claims;


namespace FundooNotes.Controllers;

[Route("api/note")]
[ApiController]
public class NoteController : ControllerBase
{

    private readonly INoteBL _noteBL;
    private readonly IDatabase _cache;

    public NoteController(INoteBL noteBL, IConnectionMultiplexer redis)
    {
        _noteBL = noteBL;
        _cache = redis.GetDatabase();
    }


    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateNote([FromBody] CreateNoteDto createNoteDto)
    {
        try
        {
            var userIdClaimed = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int userId = Convert.ToInt32(userIdClaimed);

            var cacheKey = $"UserNotes:{userId}";

            var newNote = await _noteBL.CreateNote(createNoteDto, userId);

            await _cache.HashSetAsync(cacheKey, $"Note:{newNote.NoteId}", Serialize(newNote));
            await _cache.KeyExpireAsync(cacheKey, TimeSpan.FromMinutes(10)); // Cache for 10 minutes

            var response = new FundooResponseModel<GetNoteDto>
            {
                Message = "Note created successfully in db",
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
            };
            return StatusCode(500, response);
        }
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAllNotes()
    {
        try
        {
            var userIdClaimed = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int userId = Convert.ToInt32(userIdClaimed);

            var cacheKey = $"UserNotes:{userId}";


            // Attempt to retrieve the notes from the cache
            var cachedNotesHash = await _cache.HashGetAllAsync(cacheKey);
            if (cachedNotesHash.Length > 0)
            {
                var cachedNotes = cachedNotesHash.Select(entry => Deserialize<GetNoteDto>(entry.Value)).ToList();

                var cacheResponse = new FundooResponseModel<IEnumerable<GetNoteDto>>
                {
                    Message = "Retrieved all notes successfully from cache",
                    Data = cachedNotes
                };
                return Ok(cacheResponse);
            }

            // Notes not found in cache, retrieve them from the database
            var allNotes = await _noteBL.GetAllNotes(userId);

            // Cache the retrieved notes
            foreach (var note in allNotes)
            {
                await _cache.HashSetAsync(cacheKey, $"Note:{note.NoteId}", Serialize(note));
            }
            await _cache.KeyExpireAsync(cacheKey, TimeSpan.FromMinutes(10)); // Cache for 10 minutes


            var response = new FundooResponseModel<IEnumerable<GetNoteDto>>
            {
                Message = "Retrieved all notes successfully from db",
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
            };
            return StatusCode(500, response);
        }
    }

    [Authorize]
    [HttpGet("getnotebyid")]
    public async Task<IActionResult> GetNoteById(int noteId)
    {
        try
        {
            var userIdClaimed = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int userId = Convert.ToInt32(userIdClaimed);

            var cacheKey = $"UserNotes:{userId}";
            var noteField = $"Note:{noteId}";

            // Attempt to retrieve the note from the cache
            var cachedNoteJson = await _cache.HashGetAsync(cacheKey, noteField);
            if (cachedNoteJson.HasValue)
            {
                var cachedNote = Deserialize<GetNoteDto>(cachedNoteJson);
                var cacheResponse = new FundooResponseModel<GetNoteDto>
                {
                    Message = "Retrieved note successfully from cache",
                    Data = cachedNote
                };
                return Ok(cacheResponse);
            }


            // Note not found in cache, retrieve it from the database
            var note = await _noteBL.GetNoteById(userId, noteId);


            // Cache the note
            await _cache.HashSetAsync(cacheKey, noteField, Serialize(note));
            await _cache.KeyExpireAsync(cacheKey, TimeSpan.FromMinutes(10)); // Cache for 10 minutes

            var response = new FundooResponseModel<GetNoteDto>
            {
                Message = "Retrieved note successfully from db",
                Data = note
            };
            return Ok(response);
        }
        catch (NoteDoesNotExistException ex)
        {
            var response = new FundooResponseModel<string>
            {
                Success = false,
                Message = ex.Message,
            };
            return BadRequest(response);
        }
        catch (Exception ex)
        {
            var response = new FundooResponseModel<string>
            {
                Success = false,
                Message = ex.Message,
            };
            return StatusCode(500, response);
        }
    }

    [Authorize]
    [HttpPut]
    public async Task<IActionResult> UpdateNote(int noteId, [FromBody] UpdateNoteDto updateNoteDto)
    {
        try
        {
            var userIdClaimed = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int userId = Convert.ToInt32(userIdClaimed);

            // Perform the update operation in the database
            var updatedNote = await _noteBL.UpdateNote(updateNoteDto, userId, noteId);


            var cacheKeyPrefix = $"UserNotes:"; // Key for user's notes            
            var noteField = $"Note:{noteId}"; // Field for the specific note

            // Get all user cache keys containing the note
            var cacheKeys = (await GetAllCacheKeysAsync()).Where(k => k.StartsWith(cacheKeyPrefix));

            // Update the note in each user's cache
            foreach (var cacheKey in cacheKeys)
            {
                await _cache.HashSetAsync(cacheKey, noteField, Serialize(updatedNote));
                await _cache.KeyExpireAsync(cacheKey, TimeSpan.FromMinutes(10)); // Cache for 10 minutes
            }


            // Cache the updated note                        
            //await _cache.HashSetAsync(cacheKey, noteField, Serialize(updatedNote));
            //await _cache.KeyExpireAsync(cacheKey, TimeSpan.FromMinutes(10)); // Cache for 10 minutes

            var response = new FundooResponseModel<GetNoteDto>
            {
                Message = "Updated note successfully",
                Data = updatedNote
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
            return BadRequest(response); // Status code of 400 is returned as there is a client error
        }
        catch (Exception ex)
        {
            var response = new FundooResponseModel<string>
            {
                Success = false,
                Message = ex.Message,
            };
            return StatusCode(500, response); // Returning 500 error code as this error can occur only due to a server error
        }
    }

    [Authorize]
    [HttpPatch("trash")]
    public async Task<IActionResult> TrashNote(int noteId)
    {
        try
        {
            var userIdClaimed = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int userId = Convert.ToInt32(userIdClaimed);

            // Perform the update operation in the database
            var updatedNote = await _noteBL.TrashNote(userId, noteId);

            // Update the cached notes
            //var cacheKey = $"UserNotes:{userId}"; // Key for user's notes            
            //var noteField = $"Note:{noteId}"; // Field for the specific note


            // Cache the updated note 
            //await _cache.HashSetAsync(cacheKey, noteField, Serialize(updatedNote));
            //await _cache.KeyExpireAsync(cacheKey, TimeSpan.FromMinutes(10)); // Cache for 10 minutes


            var cacheKeyPrefix = $"UserNotes:"; // Key for user's notes            
            var noteField = $"Note:{noteId}"; // Field for the specific note

            // Get all user cache keys containing the note
            var cacheKeys = (await GetAllCacheKeysAsync()).Where(k => k.StartsWith(cacheKeyPrefix));

            // Update the note in each user's cache
            foreach (var cacheKey in cacheKeys)
            {
                await _cache.HashSetAsync(cacheKey, noteField, Serialize(updatedNote));
                await _cache.KeyExpireAsync(cacheKey, TimeSpan.FromMinutes(10)); // Cache for 10 minutes
            }

            var response = new FundooResponseModel<string>
            {
                Message = updatedNote.IsDeleted ? "Note trashed successfully" : "Note untrashed successfully"
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
            return BadRequest(response);
        }
        catch (Exception ex)
        {
            var response = new FundooResponseModel<string>
            {
                Success = false,
                Message = ex.Message,
            };
            return StatusCode(500, response);
        }
    }

    [Authorize]
    [HttpPatch("archive")]
    public async Task<IActionResult> ArchiveNote(int noteId)
    {
        try
        {
            var userIdClaimed = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int userId = Convert.ToInt32(userIdClaimed);

            // Perform the update operation in the database
            var updatedNote = await _noteBL.ArchiveNote(userId, noteId);


            //var cacheKey = $"UserNotes:{userId}"; // Key for user's notes            
            //var noteField = $"Note:{noteId}"; // Field for the specific note


            // Cache the updated note
            //await _cache.HashSetAsync(cacheKey, noteField, Serialize(updatedNote));
            //await _cache.KeyExpireAsync(cacheKey, TimeSpan.FromMinutes(10)); // Cache for 10 minutes

            var cacheKeyPrefix = $"UserNotes:"; // Key for user's notes            
            var noteField = $"Note:{noteId}"; // Field for the specific note

            // Get all user cache keys containing the note
            var cacheKeys = (await GetAllCacheKeysAsync()).Where(k => k.StartsWith(cacheKeyPrefix));

            // Update the note in each user's cache
            foreach (var cacheKey in cacheKeys)
            {
                await _cache.HashSetAsync(cacheKey, noteField, Serialize(updatedNote));
                await _cache.KeyExpireAsync(cacheKey, TimeSpan.FromMinutes(10)); // Cache for 10 minutes
            }

            var response = new FundooResponseModel<string>
            {
                Message = updatedNote.IsArchived ? "Note archived successfully" : "Note unarchived successfully"
            };
            return Ok(response);
        }
        catch (ArchiveFailException ex)
        {
            var response = new FundooResponseModel<string>
            {
                Success = false,
                Message = ex.Message,
            };
            return BadRequest(response);
        }
        catch (UpdateFailException ex)
        {
            var response = new FundooResponseModel<string>
            {
                Success = false,
                Message = ex.Message,
            };
            return BadRequest(response);
        }
        catch (Exception ex)
        {
            var response = new FundooResponseModel<string>
            {
                Success = false,
                Message = ex.Message,
            };
            return StatusCode(500, response);
        }
    }

    [Authorize]
    [HttpDelete]
    public async Task<IActionResult> DeleteNote(int noteId)
    {
        try
        {
            var userIdClaimed = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int userId = Convert.ToInt32(userIdClaimed);

            // Perform the delete operation in the database
            await _noteBL.DeleteNote(userId, noteId);

            //var cacheKey = $"UserNotes:{userId}"; // Key for user's notes            
            //var noteField = $"Note:{noteId}"; // Field for the specific note

            // Remove the note from the cache
            //await _cache.HashDeleteAsync(cacheKey, noteField);

            var cacheKeyPrefix = $"UserNotes:"; // Key for user's notes            
            var noteField = $"Note:{noteId}"; // Field for the specific note

            // Get all user cache keys containing the note
            var cacheKeys = (await GetAllCacheKeysAsync()).Where(k => k.StartsWith(cacheKeyPrefix));

            // Update the note in each user's cache
            foreach (var cacheKey in cacheKeys)
            {
                await _cache.HashDeleteAsync(cacheKey, noteField);
            }

            var response = new FundooResponseModel<string>
            {
                Message = "Note deleted permanently"
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
            return BadRequest(response);
        }
        catch (Exception ex)
        {
            var response = new FundooResponseModel<string>
            {
                Success = false,
                Message = ex.Message,
            };
            return StatusCode(500, response);
        }
    }

    //methods to serialize and deserialize objects to/from JSON
    private T Deserialize<T>(string value)
    {
        return JsonConvert.DeserializeObject<T>(value);
    }

    private string Serialize(object value)
    {
        return JsonConvert.SerializeObject(value);
    }

    // Helper method to get all cache keys
    private async Task<IEnumerable<string>> GetAllCacheKeysAsync()
    {
        var endpoints = _cache.Multiplexer.GetEndPoints();
        var server = _cache.Multiplexer.GetServer(endpoints.First());
        var keys = server.Keys();

        return keys.Select(key => (string)key);
    }
}
