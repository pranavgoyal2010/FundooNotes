﻿using BusinessLayer.Interface;
using ModelLayer.Dto;
using RepositoryLayer.Interface;

namespace BusinessLayer.Service;

public class NoteBL : INoteBL
{
    private readonly INoteRL _noteRL;

    public NoteBL(INoteRL noteRL)
    {
        _noteRL = noteRL;
    }

    public Task<GetNoteDto> CreateNote(CreateNoteDto createNoteDto, int userId)
    {
        return _noteRL.CreateNote(createNoteDto, userId);
    }

    public Task<IEnumerable<GetNoteDto>> GetAllNotes(int userId)
    {
        return _noteRL.GetAllNotes(userId);
    }

    public Task<GetNoteDto> GetNoteById(int userId, int noteId)
    {
        return _noteRL.GetNoteById(userId, noteId);
    }

    public Task<GetNoteDto> UpdateNote(UpdateNoteDto updateNoteDto, int userId, int noteId)
    {
        return _noteRL.UpdateNote(updateNoteDto, userId, noteId);
    }

    public Task<bool> TrashNote(int userId, int noteId)
    {
        return _noteRL.TrashNote(userId, noteId);
    }

    public Task<bool> ArchiveNote(int userId, int noteId)
    {
        return _noteRL.ArchiveNote(userId, noteId);
    }

    public Task<bool> DeleteNote(int userId, int noteId)
    {
        return _noteRL.DeleteNote(userId, noteId);
    }
}
