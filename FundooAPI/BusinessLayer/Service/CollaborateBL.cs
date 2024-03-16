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
    public Task<bool> AddCollaborator(int userId, int noteId, AddCollaboratorDto addCollaboratorDto)
    {
        return _collaborateRL.AddCollaborator(userId, noteId, addCollaboratorDto);
    }
}
