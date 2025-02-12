using Microsoft.AspNetCore.Mvc;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;
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
    }
}