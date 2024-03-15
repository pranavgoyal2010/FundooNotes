using ModelLayer.Dto;

namespace RepositoryLayer.Interface;

public interface INoteRL
{
    public Task<GetNoteDto> CreateNote(CreateNoteDto createNoteDto, int userId);
    public Task<IEnumerable<GetNoteDto>> GetAllNotes(int userId);
    public Task<GetNoteDto> UpdateNote(UpdateNoteDto updateNoteDto, int userId, int noteId);

}
