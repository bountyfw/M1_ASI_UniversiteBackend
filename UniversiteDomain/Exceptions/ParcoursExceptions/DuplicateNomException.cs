namespace UniversiteDomain.Exceptions.ParcoursExceptions;

public class DuplicateNomException : Exception
{
    public DuplicateNomException() : base() { }
    public DuplicateNomException(string message) : base(message) { }
    public DuplicateNomException(string message, Exception innerException) : base(message, innerException) { }
}