using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;

namespace UniversiteDomain.DataAdapters;

public interface ICsvDataAdapterRepository
{
    string ExportUeWithNotesToCsv(Ue ue); // Génération de CSV à partir d'une UE et ses données associées
    string GenerateCsvFromExport(IEnumerable<NoteExportDto> noteExports); // Génération de CSV à partir de NoteExportDto
    List<Note> ImportNotesFromCsv(string filePath); // Import de notes depuis un fichier CSV
}