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

    public Task<IEnumerable<GetNoteDto>> CreateNote(CreateNoteDto createNoteDto, int userId)
    {
        return _noteRL.CreateNote(createNoteDto, userId);
    }

    public Task<IEnumerable<GetNoteDto>> GetAllNotes(int userId)
    {
        return _noteRL.GetAllNotes(userId);
    }
}
