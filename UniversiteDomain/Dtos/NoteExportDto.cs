namespace UniversiteDomain.Dtos;

public class NoteExportDto
{
    public string NumEtud { get; set; }
    public string Nom { get; set; }
    public string Prenom { get; set; }
    public string NumeroUe { get; set; }
    public string Intitule { get; set; }
    public float? Note { get; set; } // Null lorsqu'aucune note n'existe pour cet étudiant
}