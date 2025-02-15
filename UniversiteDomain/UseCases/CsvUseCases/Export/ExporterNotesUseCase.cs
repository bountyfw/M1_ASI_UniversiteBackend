using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.ParcoursExceptions;

namespace UniversiteDomain.UseCases.CsvUseCases.Export;

public class ExporterNotesUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<string> ExecuteAsync(long idUe)
    {
        // Charger l'UE par son ID.
        var ue = await repositoryFactory.UeRepository().FindAsync(idUe);

        // Vérification : l'UE doit exister.
        if (ue == null)
            throw new UeNotFoundException($"L'UE avec l'ID {idUe} n'existe pas.");

        // Générer le CSV correspondant.
        return repositoryFactory.CsvDataAdapterRepository().ExportUeWithNotesToCsv(ue);
    }
    
    //TODO: Ajouter les règles métier
    
    public bool IsAuthorized(string role)
    {
        // La scolarité est la seule habilitée à réaliser cette fonctionnalité.
        return role.Equals(Roles.Scolarite);
    }
}