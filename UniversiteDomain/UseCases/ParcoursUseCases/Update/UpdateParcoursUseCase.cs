using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.ParcoursUseCases.Update;

public class UpdateParcoursUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<Parcours?> ExecuteAsync(Parcours parcours)
    {
        await CheckBusinessRules(parcours);
        await repositoryFactory.ParcoursRepository().UpdateAsync(parcours);
        await repositoryFactory.SaveChangesAsync();
        return parcours;
    }
    
    private async Task CheckBusinessRules(Parcours parcours)
    {
        ArgumentNullException.ThrowIfNull(parcours);
        ArgumentNullException.ThrowIfNull(parcours.NomParcours);
        ArgumentNullException.ThrowIfNull(parcours.AnneeFormation);
        ArgumentNullException.ThrowIfNull(repositoryFactory.ParcoursRepository());
    }
    
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Scolarite) || role.Equals(Roles.Responsable);
    }
}