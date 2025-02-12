using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.UeUseCases.Get;

public class GetUeByIdUseCase(IRepositoryFactory repositoryFactory)
{
    
    public async Task<Ue?> ExecuteAsync(long idUe)
    {
        await CheckBusinessRules();
        Ue? ue = await repositoryFactory.UeRepository().FindAsync(idUe);
        return ue;
    }
    
    private async Task CheckBusinessRules()
    {
        ArgumentNullException.ThrowIfNull(repositoryFactory);
        IUeRepository ueRepository=repositoryFactory.UeRepository();
        ArgumentNullException.ThrowIfNull(ueRepository);
    }
    
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Scolarite) || role.Equals(Roles.Responsable);
    }
        
}