using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.NoteExceptions;

namespace UniversiteDomain.UseCases.NoteUseCases.Update;

public class UpdateNoteUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<Note> ExecuteAsync(long idEtudiant, long idUe, float valeur)
    {
        Note? note = await repositoryFactory.NoteRepository().FindAsync(idEtudiant, idUe);
        if (note == null) throw new NoteNotFoundException("La note n'existe pas");
        note.Valeur = valeur;
        return await ExecuteAsync(note);
    }

    public async Task<Note> ExecuteAsync(Note note) 
    { 
        await CheckBusinessRules(note);
        await repositoryFactory.NoteRepository().UpdateAsync(note); 
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