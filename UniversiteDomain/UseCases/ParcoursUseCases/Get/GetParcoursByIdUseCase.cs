using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.ParcoursUseCases.Get;

public class GetParcoursByIdUseCase(IRepositoryFactory factory)
{
    
    public async Task<Parcours?> ExecuteAsync(long idParcours)
    {
        await CheckBusinessRules();
        return await factory.ParcoursRepository().FindAsync(idParcours);
    }
    
    private async Task CheckBusinessRules()
    {
        ArgumentNullException.ThrowIfNull(factory);
        IParcoursRepository parcoursRepository=factory.ParcoursRepository();
        ArgumentNullException.ThrowIfNull(parcoursRepository);
    }
    
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Scolarite) || role.Equals(Roles.Responsable);
    }
}