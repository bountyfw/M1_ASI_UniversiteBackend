namespace UniversiteDomain.Exceptions.EtudiantExceptions;

public class InvalidNomEtudiantException : Exception
{
    public InvalidNomEtudiantException() : base() { }
    public InvalidNomEtudiantException(string message) : base(message) { }
    public InvalidNomEtudiantException(string message, Exception innerException) : base(message, innerException) { }
}