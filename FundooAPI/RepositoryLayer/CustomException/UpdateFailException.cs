namespace RepositoryLayer.CustomException;

public class UpdateFailException : Exception
{
    public UpdateFailException(string message) : base(message) { }
}
