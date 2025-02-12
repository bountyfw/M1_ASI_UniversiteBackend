using Microsoft.AspNetCore.Mvc;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.NoteUseCases.Create;
using UniversiteDomain.UseCases.NoteUseCases.Get;

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
            Note note = await uc.ExecuteAsync(noteDto.ToEntity());
            return CreatedAtAction(nameof(GetNotesByEtudiant), new { etudiantId = note.EtudiantId }, new NoteAvecUeDto().ToDto(note));
        }
    }
}