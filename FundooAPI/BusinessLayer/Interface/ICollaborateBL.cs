using ModelLayer.Dto;

namespace BusinessLayer.Interface;

public interface ICollaborateBL
{
    public Task<bool> AddCollaborator(int userId, int noteId, AddCollaboratorDto addCollaboratorDto);
}
