using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.EtudiantUseCases.Delete;

public class DeleteEtudiantUseCase(IRepositoryFactory factory)
{
    //TODO: Vérifier car je suis crevé mdr
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
    public bool IsAuthorized(string role, IUniversiteUser user, long idEtudiant)
    {
        if (role.Equals(Roles.Scolarite) || role.Equals(Roles.Responsable)) return true;
        return user.Etudiant!=null && role.Equals(Roles.Etudiant) && user.Etudiant.Id==idEtudiant;
    }
    
    
}