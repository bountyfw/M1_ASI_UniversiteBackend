using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.EtudiantExceptions;
using UniversiteDomain.Exceptions.NoteExceptions;
using UniversiteDomain.Exceptions.ParcoursExceptions;

namespace UniversiteDomain.UseCases.NoteUseCases.Create;

public class CreateNoteUseCase(IRepositoryFactory repositoryFactory)
{
    
    public async Task<Note> ExecuteAsync(float valeurNote, long idEtudiant, long idUe)
    {
        Etudiant etudiant = await repositoryFactory.EtudiantRepository().FindAsync(idEtudiant);
        Ue ue = await repositoryFactory.UeRepository().FindAsync(idUe);
        var note = new Note{ Valeur = valeurNote, Etudiant = etudiant, Ue = ue};
        return await ExecuteAsync(note);
    }
    
    public async Task<Note> ExecuteAsync(Note note)
    {
        await CheckBusinessRules(note);
        Note n = await repositoryFactory.NoteRepository().CreateAsync(note);
        repositoryFactory.NoteRepository().SaveChangesAsync().Wait();
        return n;
    }

    private async Task CheckBusinessRules(Note note)
    {
        ArgumentNullException.ThrowIfNull(note);
        ArgumentNullException.ThrowIfNull(note.Valeur);
        ArgumentNullException.ThrowIfNull(note.Etudiant);
        ArgumentNullException.ThrowIfNull(note.Ue);
        ArgumentNullException.ThrowIfNull(repositoryFactory.NoteRepository());

        // L'étudiant ne doit pas aoir déjà une note pour cette Ue
        List<Note> notes = await repositoryFactory.NoteRepository()
            .FindByConditionAsync(n => n.Etudiant.Id.Equals(note.Etudiant.Id) && n.Ue.Id.Equals(note.Ue.Id));
        if (notes is { Count: > 0 }) throw new DuplicateNoteException("L'étudiant a déjà une note pour cette Ue");

        // La note doit être comprise entre 0 et 20
        if (note.Valeur < 0 || note.Valeur > 20)
            throw new NoteValueException("La note doit être comprise entre 0 et 20");
        
        // L'Ue doit être dans le parcours de l'étudiant
        List<Ue> ues = await repositoryFactory.UeRepository().FindByConditionAsync(u => u.Id.Equals(note.Ue.Id));
        if (ues is { Count: 0 }) throw new UeNotFoundException("L'Ue n'existe pas");

    }

}