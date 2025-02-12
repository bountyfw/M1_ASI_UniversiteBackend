using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.EtudiantExceptions;

namespace UniversiteDomain.UseCases.NoteUseCases.Get;

public class GetNotesByEtudiantUseCase(IRepositoryFactory factory)
{
    public async Task<List<Note>> ExecuteAsync(long etudiantId)
    {
        await CheckBusinessRules();
        IUniversiteUser? user = await factory.UniversiteUserRepository().FindAsync(etudiantId);
        if (user==null) throw new EtudiantNotFoundException("L'utilisateur n'existe pas");
        
        List<Note> notes = await factory.NoteRepository().FindByConditionAsync(n => n.Etudiant.Id.Equals(etudiantId));
        return notes;
    }
    
    private async Task CheckBusinessRules()
    {
        ArgumentNullException.ThrowIfNull(factory);
        INoteRepository noteRepository=factory.NoteRepository();
        ArgumentNullException.ThrowIfNull(noteRepository);
    }
    
    public bool IsAuthorized(string role, long idEtudiant, IUniversiteUser user)
    {
        if (role.Equals(Roles.Scolarite) || role.Equals(Roles.Responsable)) return true;
        return user.Etudiant!=null && role.Equals(Roles.Etudiant) && user.Etudiant.Id==idEtudiant;
    }
}