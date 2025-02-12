using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.UeUseCases.Delete;

public class DeleteUeUseCase(IRepositoryFactory repositoryFactory)
{
    
    public async Task<bool> ExecuteAsync(long idUe)
    {
        await CheckBusinessRules();
        await repositoryFactory.UeRepository().DeleteAsync(idUe);
        return true;
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