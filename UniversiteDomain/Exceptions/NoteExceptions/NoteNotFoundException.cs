﻿namespace UniversiteDomain.Exceptions.NoteExceptions;

public class NoteNotFoundException : Exception
{
    public NoteNotFoundException() : base() { }
    public NoteNotFoundException(string message) : base(message) { }
    public NoteNotFoundException(string message, Exception innerException) : base(message, innerException) { }
}