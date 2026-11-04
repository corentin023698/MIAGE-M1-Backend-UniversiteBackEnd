namespace UniversiteDomain.Exceptions.NoteExceptions;

public class EtudiantNotInParcoursException : Exception
{
    public EtudiantNotInParcoursException() : base() { }
    public EtudiantNotInParcoursException(string message) : base(message) { }
    public EtudiantNotInParcoursException(string message, Exception inner) : base(message, inner) { }
}