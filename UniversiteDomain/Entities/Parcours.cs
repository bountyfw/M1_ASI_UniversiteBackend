﻿namespace UniversiteDomain.Entities;

public class Parcours
{
    public long Id { get; set; }
    public string NomParcours { get; set; }
    public int AnneeFormation { get; set; }
    
    // OneToMany : un parcours contient plusieurs étudiants
    // Remarque : pour éviter quelques NullPointerException disgracieux, j'ai choisi de créer une liste d'incrits vide quand aucun étudiant n'est inscrit dans un parcours plutôt que de l'initialiser à null
    public virtual List<Etudiant> Inscrits { get; set; } = new();
    
    // ManyToMany : un parcours contient plusieurs Ues 
    public virtual List<Ue> UesEnseignees { get; set; } = new();

    public override string ToString()
    {
        return "ID "+Id +" : "+NomParcours+" - Master "+AnneeFormation;
    }
    
}