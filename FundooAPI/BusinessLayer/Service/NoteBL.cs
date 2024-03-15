using BusinessLayer.Interface;
using ModelLayer.Dto;
using RepositoryLayer.Interface;

namespace BusinessLayer.Service;

public class NoteBL : INoteBL
{
    private readonly INoteRL _noteRL;

    public NoteBL(INoteRL noteRL)
    {
        _noteRL = noteRL;
    }

    public Task<GetNoteDto> CreateNote(CreateNoteDto createNoteDto, int userId)
    {
        return _noteRL.CreateNote(createNoteDto, userId);
    }

    public Task<IEnumerable<GetNoteDto>> GetAllNotes(int userId)
    {
        return _noteRL.GetAllNotes(userId);
    }

    public Task<GetNoteDto> UpdateNote(UpdateNoteDto updateNoteDto, int userId, int noteId)
    {
        return _noteRL.UpdateNote(updateNoteDto, userId, noteId);
    }
}
