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
            ExporterNotesUseCase useCase = new(repositoryFactory);
            try
            {
                var csvContent = await useCase.ExecuteAsync(idUe);
                return File(Encoding.UTF8.GetBytes(csvContent), "text/csv", $"Notes_UE_{idUe}.csv");
            }
            catch (Exception e)
            {
                // Gérer les erreurs métier ou techniques
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }
        }

        [HttpPost("{idUe}/import")]
        public async Task<IActionResult> ImportNotes(long idUe, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Aucun fichier fourni.");

            var filePath = Path.GetTempFileName();
            try
            {
                // Sauvegarde temporaire du fichier uploadé
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Use Case pour importer les notes.
                ImporterNotesDepuisCsvUseCase.ImporterNotesUseCase useCase = new(repositoryFactory);
                var notes = await useCase.ExecuteAsync(idUe, filePath);

                // Optionnel : Retourner les notes importées
                return Ok(new { message = "Import réussi !", notes });
            }
            catch (Exception e)
            {
                // Gérer les erreurs métier ou techniques
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }
            finally
            {
                // Nettoyage du fichier temporaire
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }
        }
    }
}