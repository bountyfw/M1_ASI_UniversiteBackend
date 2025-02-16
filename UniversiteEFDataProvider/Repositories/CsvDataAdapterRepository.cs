using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;

namespace UniversiteEFDataProvider.Repositories;

public class CsvDataAdapterRepository(UniversiteDbContext context) : ICsvDataAdapterRepository
{
    public string ExportUeWithNotesToCsv(Ue ue)
    {
        using var writer = new StringWriter();
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

        // Ajout des en-têtes
        csv.WriteField("NumEtud");
        csv.WriteField("Nom");
        csv.WriteField("Prenom");
        csv.WriteField("NumeroUe");
        csv.WriteField("Intitule");
        csv.WriteField("Valeur");
        csv.NextRecord();

        // Récupérer les étudiants associés aux parcours
        var parcoursAssocies = ue.EnseigneeDans ?? new List<Parcours>();
        var etudiantsAssocies = parcoursAssocies
            .SelectMany(p => p.Inscrits)
            .Distinct()
            .ToList();

        foreach (var etudiant in etudiantsAssocies)
        {
            // Trouver la note associée à cet étudiant
            var note = ue.Notes?.FirstOrDefault(n => n.EtudiantId == etudiant.Id);

            // Ajouter une ligne de contenu au CSV
            csv.WriteField(etudiant.NumEtud);
            csv.WriteField(etudiant.Nom);
            csv.WriteField(etudiant.Prenom);
            csv.WriteField(ue.NumeroUe);
            csv.WriteField(ue.Intitule);
            csv.WriteField(note?.Valeur.ToString(CultureInfo.InvariantCulture) ?? ""); // Valeur ou champ vide
            csv.NextRecord();
        }

        return writer.ToString(); // Retourne le contenu du fichier CSV
    }

    public string GenerateCsvFromExport(IEnumerable<NoteExportDto> noteExports)
    {
        using var writer = new StringWriter();
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

        // Écriture des en-têtes
        csv.WriteField("NumEtud");
        csv.WriteField("Nom");
        csv.WriteField("Prenom");
        csv.WriteField("NumeroUe");
        csv.WriteField("Intitule");
        csv.WriteField("Note");
        csv.NextRecord();

        // Écriture des enregistrements
        foreach (var noteExport in noteExports)
        {
            csv.WriteField(noteExport.NumEtud);
            csv.WriteField(noteExport.Nom);
            csv.WriteField(noteExport.Prenom);
            csv.WriteField(noteExport.NumeroUe);
            csv.WriteField(noteExport.Intitule);
            csv.WriteField(noteExport.Note?.ToString(CultureInfo.InvariantCulture) ?? ""); // Note ou champ vide
            csv.NextRecord();
        }

        return writer.ToString(); // Retourne le CSV en tant que chaîne
    }

    public List<Note> ImportNotesFromCsv(string csvContent)
    {
        var notes = new List<Note>();

        using (var reader = new StringReader(csvContent))
        using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
        {
            // Lire l'entête en premier lieu avant d'accéder aux colonnes par nom
            csv.Read();
            csv.ReadHeader();

            while (csv.Read())
            {
                // Lire les champs du CSV en utilisant leurs noms
                string numEtud = csv.GetField<string>("NumEtud");
                string nom = csv.GetField<string>("Nom");
                string prenom = csv.GetField<string>("Prenom");
                string numeroUe = csv.GetField<string>("NumeroUe");
                string intitule = csv.GetField<string>("Intitule");
                float? noteValeur = csv.TryGetField<float>("Valeur", out float parsedNote) ? (float?)parsedNote : null;

                // Créer une instance de Note avec les relations nécessaires
                notes.Add(new Note
                {
                    Valeur = noteValeur ?? 0, // Si aucune valeur n'est spécifiée, on met à 0.
                    Etudiant = new Etudiant
                    {
                        NumEtud = numEtud,
                        Nom = nom,
                        Prenom = prenom
                    },
                    Ue = new Ue
                    {
                        NumeroUe = numeroUe,
                        Intitule = intitule
                    }
                });
            }
        }

        return notes;
    }
}