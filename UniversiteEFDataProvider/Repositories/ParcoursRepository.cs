using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.EtudiantExceptions;
using UniversiteDomain.Exceptions.ParcoursExceptions;
using UniversiteEFDataProvider.Data;

namespace UniversiteEFDataProvider.Repositories;

public class ParcoursRepository(UniversiteDbContext context) : Repository<Parcours>(context), IParcoursRepository
{
    public async Task<Parcours> AddEtudiantAsync(Parcours parcours, Etudiant etudiant)
    {
        parcours.Inscrits.Add(etudiant);
        await Context.SaveChangesAsync();
        return parcours;
    }

    public async Task<Parcours> AddEtudiantAsync(long idParcours, long idEtudiant)
    {
        ArgumentNullException.ThrowIfNull(Context.Etudiants);
        ArgumentNullException.ThrowIfNull(Context.Parcours);
        Parcours parcours = (await Context.Parcours.FindAsync(idParcours))!;
        Etudiant etudiant = (await Context.Etudiants.FindAsync(idEtudiant))!;
        return await AddEtudiantAsync(parcours, etudiant);
    }

    public async Task<Parcours> AddEtudiantAsync(Parcours parcours, List<Etudiant> etudiants)
    {
        parcours.Inscrits.AddRange(etudiants);
        await Context.SaveChangesAsync();
        return parcours;
    }

    public async Task<Parcours> AddEtudiantAsync(long idParcours, long[] idEtudiants)
    {
        ArgumentNullException.ThrowIfNull(Context.Etudiants);
        ArgumentNullException.ThrowIfNull(Context.Parcours);
        Parcours parcours = (await Context.Parcours.FindAsync(idParcours))!;
        List<Etudiant> etudiants = new();
        foreach (var idEtudiant in idEtudiants)
        {
            etudiants.Add((await Context.Etudiants.FindAsync(idEtudiant))!);
        }
        return await AddEtudiantAsync(parcours, etudiants);
    }

    public async Task<Parcours> AddUeAsync(Parcours parcours, Ue ue)
    {
        parcours.UesEnseignees.Add(ue);
        await Context.SaveChangesAsync();
        return parcours;
    }

    public async Task<Parcours> AddUeAsync(long idParcours, long idUe)
    {
        ArgumentNullException.ThrowIfNull(Context.Ues);
        ArgumentNullException.ThrowIfNull(Context.Parcours);
        
        Parcours parcours = (await Context.Parcours.FindAsync(idParcours))!;
        Ue ue = (await Context.Ues.FindAsync(idUe))!;
        return await AddUeAsync(parcours, ue);
    }

    public async Task<Parcours> AddUeAsync(Parcours parcours, List<Ue> ues)
    {
        parcours.UesEnseignees.AddRange(ues);
        await Context.SaveChangesAsync();
        return parcours;
    }

    public async Task<Parcours> AddUeAsync(long idParcours, long[] idUes)
    {
        ArgumentNullException.ThrowIfNull(Context.Ues);
        ArgumentNullException.ThrowIfNull(Context.Parcours);
        
        Parcours parcours = (await Context.Parcours.FindAsync(idParcours))!;
        List<Ue> ues = new();
        foreach (var idUe in idUes)
        {
            ues.Add((await Context.Ues.FindAsync(idUe))!);
        }
        return await AddUeAsync(parcours, ues);
    }
}