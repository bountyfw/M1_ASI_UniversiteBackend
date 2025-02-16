namespace UniversiteDomain.Entities;

public class Ue
{
    public long Id { get; set; }
    public string NumeroUe { get; set; } = String.Empty;
    public string Intitule { get; set; } = String.Empty;
    // ManyToMany : une Ue est enseignée dnas plusieurs parcours
    public virtual ICollection<Parcours>? EnseigneeDans { get; set; }
    // OneToMany : une Ue est composée de plusieurs notes
    public virtual ICollection<Note>? Notes { get; set; }

    public override string ToString()
    {
        return "ID "+Id +" : "+NumeroUe+" - "+Intitule;
    }
}