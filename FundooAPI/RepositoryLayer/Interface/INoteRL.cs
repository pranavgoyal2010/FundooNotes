using ModelLayer.Dto;

namespace RepositoryLayer.Interface;

public interface INoteRL
{
    public Task<IEnumerable<GetNoteDto>> CreateNote(CreateNoteDto createNoteDto, int userId);
    public Task<IEnumerable<GetNoteDto>> GetAllNotes(int userId);
    public Task<IEnumerable<GetNoteDto>> UpdateNote(UpdateNoteDto updateNoteDto, int userId);

}
