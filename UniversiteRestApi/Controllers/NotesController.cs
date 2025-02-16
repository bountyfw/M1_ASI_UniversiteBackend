using System.Security.Claims;
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
using UniversiteDomain.UseCases.SecurityUseCases.Get;

namespace UniversiteRestApi.Controllers
{
    [Route("api/notes")]
    [ApiController]
    public class NotesController(IRepositoryFactory repositoryFactory) : ControllerBase
    {
        [HttpGet("etudiant/{etudiantId}")]
        public async Task<ActionResult<List<NoteAvecUeDto>>> GetNotesByEtudiant(long etudiantId)
        {
            string role;
            string email;
            IUniversiteUser? user;
            try
            {
                CheckSecu(out role, out email, out user);
            }
            catch (Exception e)
            {
                return Unauthorized();
            }
            GetNotesByEtudiantUseCase uc = new GetNotesByEtudiantUseCase(repositoryFactory);
            if (!uc.IsAuthorized(role, etudiantId, user)) return Unauthorized();
            List<Note> notes = await uc.ExecuteAsync(etudiantId);
            return NoteAvecUeDto.ToDtos(notes);
        }
        
        [HttpPost]
        public async Task<ActionResult<NoteAvecUeDto>> AddNoteAsync([FromBody] NoteAvecUeDto noteDto)
        {
            string role;
            string email;
            IUniversiteUser? user;
            try
            {
                CheckSecu(out role, out email, out user);
            }
            catch (Exception e)
            {
                return Unauthorized();
            }
            CreateNoteUseCase uc = new CreateNoteUseCase(repositoryFactory);
            if (!uc.IsAuthorized(role)) return Unauthorized();
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
            string role;
            string email;
            IUniversiteUser? user;
            try
            {
                CheckSecu(out role, out email, out user);
            }
            catch (Exception e)
            {
                return Unauthorized();
            }
            UpdateNoteUseCase uc = new UpdateNoteUseCase(repositoryFactory);
            if (!uc.IsAuthorized(role)) return Unauthorized();
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
            string role;
            string email;
            IUniversiteUser? user;
            try
            {
                CheckSecu(out role, out email, out user);
            }
            catch (Exception e)
            {
                return Unauthorized();
            }
            DeleteNoteUseCase uc = new DeleteNoteUseCase(repositoryFactory);
            if (!uc.IsAuthorized(role)) return Unauthorized();
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
            string role;
            string email;
            IUniversiteUser? user;
            try
            {
                CheckSecu(out role, out email, out user);
            }
            catch (Exception e)
            {
                return Unauthorized();
            }
            ExporterNotesUseCase uc = new ExporterNotesUseCase(repositoryFactory);
            if (!uc.IsAuthorized(role)) return Unauthorized();
            try
            {
                var csvContent = await uc.ExecuteAsync(idUe);
            
                return File(
                    Encoding.UTF8.GetBytes(csvContent), // Convertir le contenu en bytes UTF-8
                    "text/csv", // Type MIME du fichier
                    $"Notes_UE_{idUe}.csv" // Nom du fichier
                );
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{idUe}/import")]
        public async Task<IActionResult> ImportNotes(long idUe, IFormFile file)
        {
            string role;
            string email;
            IUniversiteUser? user;
            try
            {
                CheckSecu(out role, out email, out user);
            }
            catch (Exception e)
            {
                return Unauthorized();
            }
            if (file == null || file.Length == 0)
            {
                return BadRequest("Un fichier CSV valide est requis.");
            }

            string csvContent;
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                csvContent = await reader.ReadToEndAsync();
            }

            ImporterNotesDepuisCsvUseCase uc = new ImporterNotesDepuisCsvUseCase(repositoryFactory);
            if (!uc.IsAuthorized(role)) return Unauthorized();
            try
            {
                await uc.ExecuteAsync(idUe, csvContent);
                return Ok("Les notes ont été importées avec succès.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private void CheckSecu(out string role, out string email, out IUniversiteUser? user)
        {
            role = "";
            ClaimsPrincipal claims = HttpContext.User;
            if (claims.FindFirst(ClaimTypes.Email)==null) throw new UnauthorizedAccessException();
            email = claims.FindFirst(ClaimTypes.Email).Value;
            if (email==null) throw new UnauthorizedAccessException();
            user = new FindUniversiteUserByEmailUseCase(repositoryFactory).ExecuteAsync(email).Result;
            if(user==null) throw new UnauthorizedAccessException();
            if (claims.Identity?.IsAuthenticated != true) throw new UnauthorizedAccessException();
            var ident = claims.Identities.FirstOrDefault();
            if (ident == null)throw new UnauthorizedAccessException();
            if (claims.FindFirst(ClaimTypes.Role)==null) throw new UnauthorizedAccessException();
            role = ident.FindFirst(ClaimTypes.Role).Value;
            if (role == null) throw new UnauthorizedAccessException();
        }
    }
}