namespace UniversiteDomain.Exceptions.ParcoursExceptions;

public class UeNotFoundException : Exception
{
    public UeNotFoundException() : base() { }
    public UeNotFoundException(string message) : base(message) { }
    public UeNotFoundException(string message, Exception innerException) : base(message, innerException) { }
}