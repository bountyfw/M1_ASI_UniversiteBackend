using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.UniversiteUseCases.Delete;

public class DeleteUniversiteUserUseCase(IRepositoryFactory repositoryFactory)
{
    
    public async Task<bool> ExecuteAsync(Etudiant etudiant)
    {
        await CheckBusinessRules();
        await repositoryFactory.UniversiteUserRepository().DeleteAsync(etudiant.Id);
        return true;
    }
    
    public async Task<bool> ExecuteAsync(long id)
    {
        await CheckBusinessRules();
        await repositoryFactory.UniversiteUserRepository().DeleteAsync(id);
        return true;
    }
    
    private async Task CheckBusinessRules()
    {
        ArgumentNullException.ThrowIfNull(repositoryFactory);
        IUniversiteUserRepository universiteUserRepository=repositoryFactory.UniversiteUserRepository();
        ArgumentNullException.ThrowIfNull(universiteUserRepository);
    }
    
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Scolarite) || role.Equals(Roles.Responsable);
    }
    
}