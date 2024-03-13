using ModelLayer.Dto;

namespace RepositoryLayer.Interface;

public interface INoteRL
{
    //public Task<IEnumerable<NoteEntity>> GetStudents();
    //public Task<StudentEntity> GetStudentById(int id);
    public Task<IEnumerable<GetNoteDto>> CreateNote(CreateNoteDto createNoteDto, int userId);
    //public Task UpdateStudent(int id, StudentUpdateDto studentDto);
    //public Task DeleteStudent(int id);
}
