using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;

namespace UniversiteDomain.DataAdapters;

public interface ICsvDataAdapterRepository
{
    string ExportUeWithNotesToCsv(Ue ue);
    List<NoteAvecUeDto> ImportNotesFromCsv(string filePath);
}