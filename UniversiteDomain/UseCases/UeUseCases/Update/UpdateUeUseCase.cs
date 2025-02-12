using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.ParcoursExceptions;

namespace UniversiteDomain.UseCases.UeUseCases.Update;

public class UpdateUeUseCase(IRepositoryFactory repositoryFactory)
{

    public async Task<Ue> ExecuteAsync(long idUe, string numeroUe, string intitule)
    {
        Ue? ue = await repositoryFactory.UeRepository().FindAsync(idUe);
        if (ue == null) throw new UeNotFoundException("L'ue n'existe pas");
        ue.NumeroUe = numeroUe;
        ue.Intitule = intitule;
        return await ExecuteAsync(ue);
    }

    public async Task<Ue> ExecuteAsync(Ue ue)
    {
        await CheckBusinessRules(ue);
        await repositoryFactory.UeRepository().UpdateAsync(ue);
        await repositoryFactory.SaveChangesAsync();
        return ue;
    }
    
    private async Task CheckBusinessRules(Ue ue)
    {
        
        ArgumentNullException.ThrowIfNull(ue);
        ArgumentNullException.ThrowIfNull(repositoryFactory.UeRepository());
        
    }
    
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Scolarite) || role.Equals(Roles.Responsable);
    }
    
    
}