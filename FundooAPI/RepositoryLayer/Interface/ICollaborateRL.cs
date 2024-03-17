using ModelLayer.Dto;

namespace RepositoryLayer.Interface;

public interface ICollaborateRL
{
    public Task<IEnumerable<GetCollaboratorDto>> GetAllCollaborators(int userId);
    public Task<bool> AddCollaborator(int userId, int noteId, AddCollaboratorDto addCollaboratorDto);
    public Task<bool> RemoveCollaborator(int userId, int noteId, RemoveCollaboratorDto removeCollaboratorDto);
}
