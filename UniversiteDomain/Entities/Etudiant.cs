﻿namespace UniversiteDomain.Entities;

public class Etudiant
{
    public long Id { get; set; }
    public string NumEtud { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // ManyToOne : l'étudiant est inscrit dans un parcours
    public virtual Parcours? ParcoursSuivi { get; set; } = null;
    // OneToMany : l'étudiant a plusieurs notes
    public virtual List<Note> NotesObtenues { get; set; } = new();

    public override string ToString()
    {
        return $"ID {Id} : {NumEtud} - {Nom} {Prenom} inscrit en "+ParcoursSuivi;
    }
}