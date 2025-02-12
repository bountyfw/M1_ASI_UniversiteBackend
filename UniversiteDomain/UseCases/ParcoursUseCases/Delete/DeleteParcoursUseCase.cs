using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.ParcoursUseCases.Delete;

public class DeleteParcoursUseCase(IRepositoryFactory factory)
{
    public async Task<bool> ExecuteAsync(long idParcours)
    {
        await CheckBusinessRules();
        await factory.ParcoursRepository().DeleteAsync(idParcours);
        return true;
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