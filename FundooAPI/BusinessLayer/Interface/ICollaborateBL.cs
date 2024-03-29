﻿using ModelLayer.Dto;

namespace BusinessLayer.Interface;

public interface ICollaborateBL
{
    public Task<IEnumerable<GetCollaboratorDto>> GetAllCollaborators(int userId);
    public Task<IEnumerable<GetCollaboratorDto>> GetAllCollaboratorsById(int userId, int noteId);
    public Task<bool> AddCollaborator(int userId, int noteId, AddCollaboratorDto addCollaboratorDto);
    public Task<bool> RemoveCollaborator(int userId, int noteId, RemoveCollaboratorDto removeCollaboratorDto);
}
