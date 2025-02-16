﻿namespace UniversiteDomain.Entities;

public class Note
{
    public long Id { get; set; }
    public float Valeur { get; set; }
    public virtual Etudiant Etudiant { get; set; } = new();
    public virtual Ue Ue { get; set; } = new();
    public long EtudiantId { get; set; }
    public long UeId { get; set; }
    
    public override string ToString()
    {
        return $"ID {Id} : {Valeur} pour l'étudiant {Etudiant} en {Ue}";
    }
    
}