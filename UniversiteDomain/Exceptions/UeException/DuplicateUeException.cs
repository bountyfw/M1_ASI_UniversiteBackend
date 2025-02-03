﻿namespace UniversiteDomain.Exceptions.UeException;

public class DuplicateUeException : Exception
{
    public DuplicateUeException() : base() { }
    public DuplicateUeException(string message) : base(message) { }
    public DuplicateUeException(string message, Exception innerException) : base(message, innerException) { }
}