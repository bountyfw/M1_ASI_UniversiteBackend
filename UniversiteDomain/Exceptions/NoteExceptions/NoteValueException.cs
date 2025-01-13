namespace UniversiteDomain.Exceptions.NoteExceptions;

public class NoteValueException : Exception
{
    public NoteValueException() : base() { }
    public NoteValueException(string message) : base(message) { }
    public NoteValueException(string message, Exception innerException) : base(message, innerException) { }
}