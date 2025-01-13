namespace UniversiteDomain.Exceptions.EtudiantExceptions;

[Serializable]
public class DuplicateNumEtudException : Exception
{
    public DuplicateNumEtudException() : base() { }
    public DuplicateNumEtudException(string message) : base(message) { }
    public DuplicateNumEtudException(string message, Exception innerException) : base(message, innerException) { }
}