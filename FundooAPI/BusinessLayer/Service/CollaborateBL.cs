using BusinessLayer.Interface;
using ModelLayer.Dto;
using RepositoryLayer.Interface;

namespace BusinessLayer.Service;

public class CollaborateBL : ICollaborateBL
{
    private readonly ICollaborateRL _collaborateRL;

    public CollaborateBL(ICollaborateRL collaborateRL)
    {
        _collaborateRL = collaborateRL;
    }
    public Task<IEnumerable<GetCollaboratorDto>> GetAllCollaborators(int userId)
    {
        return _collaborateRL.GetAllCollaborators(userId);
    }
    public Task<IEnumerable<GetCollaboratorDto>> GetAllCollaboratorsById(int userId, int noteId)
    {
        return _collaborateRL.GetAllCollaboratorsById(userId, noteId);
    }
    public Task<bool> AddCollaborator(int userId, int noteId, AddCollaboratorDto addCollaboratorDto)
    {
        return _collaborateRL.AddCollaborator(userId, noteId, addCollaboratorDto);
    }
    public Task<bool> RemoveCollaborator(int userId, int noteId, RemoveCollaboratorDto removeCollaboratorDto)
    {
        return _collaborateRL.RemoveCollaborator(userId, noteId, removeCollaboratorDto);
    }
}
