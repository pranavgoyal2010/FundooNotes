namespace RepositoryLayer.CustomException;

public class DeleteFailException : Exception
{
    public DeleteFailException(string message) : base(message) { }
}
