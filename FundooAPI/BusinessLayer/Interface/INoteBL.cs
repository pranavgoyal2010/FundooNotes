﻿using ModelLayer.Dto;

namespace BusinessLayer.Interface;

public interface INoteBL
{
    public Task<IEnumerable<GetNoteDto>> CreateNote(CreateNoteDto createNoteDto, int userId);
}