using ModelLayer.Dto;

namespace BusinessLayer.Interface;

public interface INoteBL
{
    public Task<GetNoteDto> CreateNote(CreateNoteDto createNoteDto, int userId);
    public Task<IEnumerable<GetNoteDto>> GetAllNotes(int userId);
    public Task<GetNoteDto> GetNoteById(int userId, int noteId);
    public Task<GetNoteDto> UpdateNote(UpdateNoteDto updateNoteDto, int userId, int noteId);
    public Task<bool> TrashNote(int userId, int noteId);
    public Task<bool> ArchiveNote(int userId, int noteId);
    public Task<bool> DeleteNote(int userId, int noteId);
}
