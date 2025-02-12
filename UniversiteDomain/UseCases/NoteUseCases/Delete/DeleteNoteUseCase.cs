using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.NoteExceptions;

namespace UniversiteDomain.UseCases.NoteUseCases.Delete;

public class DeleteNoteUseCase(IRepositoryFactory repositoryFactory)
{
    
    public async Task<Note> ExecuteAsync(long idNote)
    {
        Note? note = await repositoryFactory.NoteRepository().FindAsync(idNote);
        if(note==null) throw new NoteNotFoundException("La note n'existe pas");
        await CheckBusinessRules(note);
        return await ExecuteAsync(note);
    }

    public async Task<Note> ExecuteAsync(Note note)
    {
        await CheckBusinessRules(note);
        await repositoryFactory.NoteRepository().DeleteAsync(note);
        await repositoryFactory.SaveChangesAsync();
        return note;
    }
    
    private async Task CheckBusinessRules(Note note)
    {
        ArgumentNullException.ThrowIfNull(note);
        ArgumentNullException.ThrowIfNull(repositoryFactory.NoteRepository());
    }
    
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Scolarite) || role.Equals(Roles.Responsable);
    }
    
}