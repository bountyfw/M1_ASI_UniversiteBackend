using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.CsvUseCases.Import;

public class ImporterNotesDepuisCsvUseCase(IRepositoryFactory repositoryFactory)
{

    public class ImporterNotesUseCase(IRepositoryFactory repositoryFactory)
    {
        public async Task<List<Note>> ExecuteAsync(long idUe, string filePath)
        {
            ArgumentNullException.ThrowIfNull(filePath);

            // Charger l'UE concernée.
            var ue = await repositoryFactory.UeRepository().FindAsync(idUe);
            if (ue == null)
                throw new ArgumentException("L'UE spécifiée est introuvable.");

            // Importer les notes depuis le CSV.
            var noteDtos = repositoryFactory.CsvDataAdapterRepository().ImportNotesFromCsv(filePath);

            // Validation métier et conversion en entités.
            List<Note> notes = new();
            foreach (var dto in noteDtos)
            {
                if (dto.UeId != idUe)
                    throw new ArgumentException($"Une note appartient à une autre UE : {dto.UeId}.");

                if (dto.Valeur is < 0 or > 20)
                    throw new ArgumentException($"Valeur de note invalide : {dto.Valeur}.");

                var etudiant = await repositoryFactory.EtudiantRepository().FindAsync(dto.EtudiantId);
                if (etudiant == null)
                    throw new ArgumentException($"L'étudiant avec l'ID {dto.EtudiantId} est introuvable.");

                // Créer la note validée et prête à sauvegarder.
                notes.Add(new Note
                {
                    Valeur = dto.Valeur,
                    Etudiant = etudiant,
                    EtudiantId = etudiant.Id,
                    Ue = ue,
                    UeId = ue.Id
                });
            }

            // Sauvegarder toutes les notes dans le repository.
            foreach (var note in notes)
            {
                await repositoryFactory.NoteRepository().CreateAsync(note);
            }

            await repositoryFactory.NoteRepository().SaveChangesAsync();

            return notes;
        }

        public bool IsAuthorized(string role)
        {
            // Ex : seuls les responsables peuvent effectuer cette action.
            return role.Equals(Roles.Responsable);
        }
    }
}