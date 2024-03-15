using ModelLayer.Dto;

namespace BusinessLayer.Interface;

public interface INoteBL
{
    public Task<GetNoteDto> CreateNote(CreateNoteDto createNoteDto, int userId);
    public Task<IEnumerable<GetNoteDto>> GetAllNotes(int userId);
    public Task<GetNoteDto> UpdateNote(UpdateNoteDto updateNoteDto, int userId, int noteId);
    public Task<GetNoteDto> TrashNote(int userId, int noteId);
    public Task<GetNoteDto> ArchiveNote(int userId, int noteId);
}
