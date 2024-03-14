using ModelLayer.Dto;

namespace BusinessLayer.Interface;

public interface INoteBL
{
    public Task<IEnumerable<GetNoteDto>> CreateNote(CreateNoteDto createNoteDto, int userId);
    public Task<IEnumerable<GetNoteDto>> GetAllNotes(int userId);
    public Task<IEnumerable<GetNoteDto>> UpdateNote(UpdateNoteDto updateNoteDto, int userId, int noteId);
}
