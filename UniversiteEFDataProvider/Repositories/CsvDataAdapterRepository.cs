using System.Globalization;
using CsvHelper;
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

        // Les en-têtes du fichier CSV.
        csv.WriteField("NumEtud");
        csv.WriteField("Nom");
        csv.WriteField("Prenom");
        csv.WriteField("Valeur");
        csv.NextRecord();

        // Écriture des notes des étudiants pour cette UE.
        foreach (var note in ue.Notes ?? new List<Note>())
        {
            var etudiant = note.Etudiant;
            csv.WriteField(etudiant.NumEtud);
            csv.WriteField(etudiant.Nom);
            csv.WriteField(etudiant.Prenom);
            csv.WriteField(note.Valeur);
            csv.NextRecord();
        }

        return writer.ToString();
    }

    public List<NoteAvecUeDto> ImportNotesFromCsv(string filePath)
    {
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        // Utilisation de CsvHelper pour lire et mapper le contenu en NoteAvecUeDto.
        return csv.GetRecords<NoteAvecUeDto>().ToList();
    }
}