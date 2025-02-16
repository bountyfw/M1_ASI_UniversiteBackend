using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.ParcoursUseCases.Get;
using UniversiteDomain.UseCases.ParcoursUseCases.Create;
using UniversiteDomain.UseCases.ParcoursUseCases.Update;
using UniversiteDomain.UseCases.ParcoursUseCases.Delete;
using UniversiteDomain.UseCases.ParcoursUseCases.EtudiantDansParcours;
using UniversiteDomain.UseCases.ParcoursUseCases.UeDansParcours;
using UniversiteDomain.UseCases.SecurityUseCases.Get;

namespace UniversiteRestApi.Controllers
{
    [Route("api/parcours")]
    [ApiController]
    public class ParcoursController(IRepositoryFactory repositoryFactory) : ControllerBase
    {
        [HttpGet] 
        public async Task<ActionResult<List<ParcoursDto>>> GetAsync()
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
            GetTousLesParcoursUseCase uc = new GetTousLesParcoursUseCase(repositoryFactory);
            if (!uc.IsAuthorized(role)) return Unauthorized();
            List<Parcours> parcours = await uc.ExecuteAsync();
            return new ParcoursDto().ToDtos(parcours);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ParcoursDto>> GetUnParcours(long id)
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
            GetParcoursByIdUseCase uc = new GetParcoursByIdUseCase(repositoryFactory);
            if (!uc.IsAuthorized(role)) return Unauthorized();
            var parcours = await uc.ExecuteAsync(id);
            return parcours != null ? Ok(new ParcoursDto().ToDto(parcours)) : NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<ParcoursDto>> PostAsync([FromBody] ParcoursDto parcoursDto)
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
            CreateParcoursUseCase uc = new CreateParcoursUseCase(repositoryFactory);
            if (!uc.IsAuthorized(role)) return Unauthorized();
            Parcours parcours = parcoursDto.ToEntity();
            try
            {
                await uc.ExecuteAsync(parcours);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }
            return CreatedAtAction(nameof(GetUnParcours), new { id = parcours.Id }, new ParcoursDto().ToDto(parcours));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(long id, [FromBody] ParcoursDto parcoursDto)
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
            if (id != parcoursDto.Id) return BadRequest();
            UpdateParcoursUseCase uc = new UpdateParcoursUseCase(repositoryFactory);
            if (!uc.IsAuthorized(role)) return Unauthorized();
            Parcours parcours = parcoursDto.ToEntity();
            try
            {
                await uc.ExecuteAsync(parcours);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(long id)
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
            DeleteParcoursUseCase uc = new DeleteParcoursUseCase(repositoryFactory);
            if (!uc.IsAuthorized(role)) return Unauthorized();
            try
            {
                await uc.ExecuteAsync(id);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }
            return NoContent();
        }

        [HttpPost("{parcoursId}/etudiant/{etudiantId}")]
        public async Task<IActionResult> AddEtudiantToParcours(long parcoursId, long etudiantId)
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
            AddEtudiantDansParcoursUseCase uc = new AddEtudiantDansParcoursUseCase(repositoryFactory);
            if (!uc.IsAuthorized(role)) return Unauthorized();
            try
            {
                await uc.ExecuteAsync(parcoursId, etudiantId);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }
            return NoContent();
        }
        
        [HttpPost("{parcoursId}/ue/{ueId}")]
        public async Task<IActionResult> AddUeToParcours(long parcoursId, long ueId)
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
            AddUeDansParcoursUseCase uc = new AddUeDansParcoursUseCase(repositoryFactory);
            if (!uc.IsAuthorized(role)) return Unauthorized();
            try
            {
                await uc.ExecuteAsync(parcoursId, ueId);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }
            return NoContent();
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
