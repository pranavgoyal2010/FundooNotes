using ModelLayer.Dto;

namespace RepositoryLayer.Interface;

public interface ICollaborateRL
{
    public Task<bool> AddCollaborator(int userId, int noteId, AddCollaboratorDto addCollaboratorDto);
}
