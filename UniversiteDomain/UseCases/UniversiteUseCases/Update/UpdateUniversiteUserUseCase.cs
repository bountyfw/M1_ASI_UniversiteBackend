using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.UniversiteUseCases.Update;

public class UpdateUniversiteUserUseCase(IRepositoryFactory repositoryFactory)
{
    
    public async Task ExecuteAsync(Etudiant etudiant)
    {
        await CheckBusinessRules();
        await repositoryFactory.UniversiteUserRepository().UpdateAsync(etudiant);
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