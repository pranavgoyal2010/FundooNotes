namespace RepositoryLayer.CustomException;

public class ArchiveFailException : Exception
{
    public ArchiveFailException(string message) : base(message) { }
}
