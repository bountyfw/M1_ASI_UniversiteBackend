using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.UeException;

namespace UniversiteDomain.UseCases.UeUseCases.Create;

public class CreateUeUseCase(IRepositoryFactory repositoryFactory)
{
    
    public async Task<Ue> ExecuteAsync(Ue ue)
    {
        await CheckBusinessRules(ue);
        await repositoryFactory.UeRepository().CreateAsync(ue);
        await repositoryFactory.SaveChangesAsync();
        return ue;
    }

    public async Task<Ue> ExecuteAsync(string numero, string intitule)
    {
        Ue ue = new Ue {Intitule = intitule, NumeroUe = numero};
        return await ExecuteAsync(ue);
    }
    
    private async Task CheckBusinessRules(Ue ue)
    {
        ArgumentNullException.ThrowIfNull(ue);
        ArgumentNullException.ThrowIfNull(ue.Intitule);
        ArgumentNullException.ThrowIfNull(ue.NumeroUe);
        ArgumentNullException.ThrowIfNull(repositoryFactory);
        
        // L'UE doit être unique
        List<Ue> ues = await repositoryFactory.UeRepository().FindByConditionAsync(u => u.NumeroUe.Equals(ue.NumeroUe));
        if (ues is { Count: > 0 }) throw new DuplicateUeException(ue.NumeroUe+" - existe déjà");
        
    }
    
}