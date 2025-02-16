using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.CsvUseCases.Import;

public class ImporterNotesDepuisCsvUseCase(IRepositoryFactory repositoryFactory)
{

    public async Task ExecuteAsync(long idUe, string csvContent)
    {
        await CheckBusinessRules();
        Ue? ue = await repositoryFactory.UeRepository().FindAsync(idUe);
        if (ue == null)
        {
            throw new ArgumentException($"L'UE avec l'ID {idUe} est introuvable.");
        }

        List<Note> notesFromCsv = repositoryFactory.CsvDataAdapterRepository().ImportNotesFromCsv(csvContent);

        if (notesFromCsv == null || !notesFromCsv.Any())
        {
            throw new ArgumentException("Le fichier CSV est vide ou mal formaté.");
        }

        foreach (Note csvNote in notesFromCsv)
        {
            if (csvNote.Etudiant == null)
            {
                throw new ArgumentException("Le fichier CSV contient une note sans étudiant associé.");
            }

            string numEtud = csvNote.Etudiant.NumEtud;
            Etudiant? etudiant = (await repositoryFactory.EtudiantRepository()
                .FindByConditionAsync(e => e.NumEtud == numEtud)).FirstOrDefault();

            if (etudiant == null)
            {
                throw new ArgumentException($"L'étudiant avec le numéro {numEtud} n'existe pas dans le système.");
            }

            if (csvNote.Ue == null)
            {
                throw new ArgumentException("Le fichier CSV mentionne une note sans UE associée.");
            }

            if (csvNote.Ue.NumeroUe != ue.NumeroUe)
            {
                throw new ArgumentException(
                    $"Un enregistrement dans le fichier CSV mentionne une UE ({csvNote.Ue.NumeroUe}) qui ne correspond pas à l'UE spécifiée ({ue.NumeroUe})."
                );
            }

            Note? existingNote = (await repositoryFactory.NoteRepository()
                .FindByConditionAsync(n => n.EtudiantId == etudiant.Id && n.UeId == idUe))
                .FirstOrDefault();

            if (existingNote != null)
            {
                existingNote.Valeur = csvNote.Valeur; // Met à jour la valeur
                await repositoryFactory.NoteRepository().UpdateAsync(existingNote);
            }
            else
            {
                Note newNote = new Note
                {
                    Valeur = csvNote.Valeur,
                    EtudiantId = etudiant.Id,
                    UeId = ue.Id,
                    Etudiant = etudiant,
                    Ue = ue
                };
                await repositoryFactory.NoteRepository().CreateAsync(newNote);
            }
        }

        await repositoryFactory.NoteRepository().SaveChangesAsync();
    }
    
    private async Task CheckBusinessRules()
    {
        ArgumentNullException.ThrowIfNull(repositoryFactory);
        INoteRepository noteRepository = repositoryFactory.NoteRepository();
        ArgumentNullException.ThrowIfNull(noteRepository);
        IEtudiantRepository etudiantRepository = repositoryFactory.EtudiantRepository();
        ArgumentNullException.ThrowIfNull(etudiantRepository);
        IUeRepository ueRepository = repositoryFactory.UeRepository();
        ArgumentNullException.ThrowIfNull(ueRepository);
    }
    
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Scolarite);
    }
}