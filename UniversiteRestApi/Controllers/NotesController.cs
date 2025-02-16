using System.Text;
using Microsoft.AspNetCore.Mvc;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.CsvUseCases.Export;
using UniversiteDomain.UseCases.CsvUseCases.Import;
using UniversiteDomain.UseCases.NoteUseCases.Create;
using UniversiteDomain.UseCases.NoteUseCases.Delete;
using UniversiteDomain.UseCases.NoteUseCases.Get;
using UniversiteDomain.UseCases.NoteUseCases.Update;

namespace UniversiteRestApi.Controllers
{
    [Route("api/notes")]
    [ApiController]
    public class NotesController(IRepositoryFactory repositoryFactory) : ControllerBase
    {
        //TODO: Check ça crée une erreur 
        [HttpGet("etudiant/{etudiantId}")]
        public async Task<ActionResult<List<NoteAvecUeDto>>> GetNotesByEtudiant(long etudiantId)
        {
            GetNotesByEtudiantUseCase uc = new GetNotesByEtudiantUseCase(repositoryFactory);
            List<Note> notes = await uc.ExecuteAsync(etudiantId);
            return NoteAvecUeDto.ToDtos(notes);
        }
        
        [HttpPost]
        public async Task<ActionResult<NoteAvecUeDto>> AddNoteAsync([FromBody] NoteAvecUeDto noteDto)
        {
            CreateNoteUseCase uc = new CreateNoteUseCase(repositoryFactory);
            Note note = noteDto.ToEntity();
            try
            {   
                note = await uc.ExecuteAsync(note);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }
            return CreatedAtAction(nameof(GetNotesByEtudiant), new { etudiantId = note.EtudiantId }, new NoteAvecUeDto().ToDto(note));
        }

        [HttpPut]
        public async Task<ActionResult<NoteAvecUeDto>> UpdateNoteAsync([FromBody] NoteAvecUeDto noteDto)
        {
            UpdateNoteUseCase uc = new UpdateNoteUseCase(repositoryFactory);
            Note note = noteDto.ToEntity();
            try
            {
                note = await uc.ExecuteAsync(note);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }
            return new NoteAvecUeDto().ToDto(note);
        }

        [HttpDelete("{idNote}")]
        public async Task<ActionResult<NoteAvecUeDto>> DeleteNoteAsync(long idNote)
        {
            DeleteNoteUseCase uc = new DeleteNoteUseCase(repositoryFactory);
            try
            {
                Note note = await uc.ExecuteAsync(idNote);
                return new NoteAvecUeDto().ToDto(note);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }
        }
        
        [HttpGet("{idUe}/export")]
        public async Task<IActionResult> ExportNotes(long idUe)
        {
            // Instancier le Use Case pour gérer l'export
            var useCase = new ExporterNotesUseCase(repositoryFactory);

            try
            {
                // Exécuter le Use Case pour récupérer le contenu CSV
                var csvContent = await useCase.ExecuteAsync(idUe);
            
                // Retourner une réponse HTTP avec le fichier CSV
                return File(
                    Encoding.UTF8.GetBytes(csvContent), // Convertir le contenu en bytes UTF-8
                    "text/csv", // Type MIME du fichier
                    $"Notes_UE_{idUe}.csv" // Nom du fichier
                );
            }
            catch (Exception ex)
            {
                // En cas d'erreurs, retourner une réponse 400 avec le message d'erreur
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{idUe}/import")]
        public async Task<IActionResult> ImportNotes(long idUe, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Un fichier CSV valide est requis.");
            }

            string csvContent;
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                csvContent = await reader.ReadToEndAsync();
            }

            var useCase = new ImporterNotesDepuisCsvUseCase(repositoryFactory);

            try
            {
                await useCase.ExecuteAsync(idUe, csvContent);
                return Ok("Les notes ont été importées avec succès.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}