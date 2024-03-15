namespace RepositoryLayer.CustomException;

public class NoteNotCreatedException : Exception
{
    public NoteNotCreatedException(string message) : base(message) { }
}
