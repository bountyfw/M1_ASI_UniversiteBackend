using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.Dtos.Notes;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.NotesUseCases.Create;
using UniversiteDomain.UseCases.NotesUseCases.Delete;
using UniversiteDomain.UseCases.NotesUseCases.Get;
using UniversiteDomain.UseCases.NotesUseCases.Update;
using UniversiteDomain.UseCases.NoteUseCases.Create;

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
            List<Notes> notes = await uc.ExecuteAsync(etudiantId);
            return NoteAvecUeDto.ToDtos(notes);
        }
        
        [HttpPost]
        public async Task<ActionResult<NoteAvecUeDto>> AddNoteAsync([FromBody] NoteAvecUeDto noteDto)
        {
            CreateNoteUseCase uc = new CreateNoteUseCase(repositoryFactory);
            Notes note = await uc.ExecuteAsync(noteDto.ToEntity());
            return CreatedAtAction(nameof(GetNotesByEtudiant), new { etudiantId = note.EtudiantId }, NoteAvecUeDto.ToDto(note));
        }
    }
}