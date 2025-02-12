using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.EtudiantUseCases.Delete;

public class DeleteEtudiantUseCase(IRepositoryFactory factory)
{
    //Il y a un problème qui s'est produit en suivant le TD, je n'ai pas eu le temps de le résoudre
    public async Task<bool> ExecuteAsync(long idEtudiant)
    {
        await CheckBusinessRules();
        bool success = factory.EtudiantRepository().DeleteAsync(idEtudiant).IsCompletedSuccessfully;
        await factory.SaveChangesAsync();
        return success;
    }
    private async Task CheckBusinessRules()
    {
        ArgumentNullException.ThrowIfNull(factory);
        IEtudiantRepository etudiantRepository=factory.EtudiantRepository();
        ArgumentNullException.ThrowIfNull(etudiantRepository);
    }
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Scolarite) || role.Equals(Roles.Responsable);
    }
    
    
}