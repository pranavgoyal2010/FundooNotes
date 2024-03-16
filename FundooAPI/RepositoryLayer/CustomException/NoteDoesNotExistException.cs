namespace RepositoryLayer.CustomException;

public class NoteDoesNotExistException : Exception
{
    public NoteDoesNotExistException(string message) : base(message) { }
}
