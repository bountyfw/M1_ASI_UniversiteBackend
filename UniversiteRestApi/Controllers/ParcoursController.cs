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

namespace UniversiteRestApi.Controllers
{
    [Route("api/parcours")]
    [ApiController]
    public class ParcoursController(IRepositoryFactory repositoryFactory) : ControllerBase
    {
        [HttpGet] 
        public async Task<ActionResult<List<ParcoursDto>>> GetAsync()
        {
            GetTousLesParcoursUseCase uc = new GetTousLesParcoursUseCase(repositoryFactory);
            List<Parcours> parcours = await uc.ExecuteAsync();
            return new ParcoursDto().ToDtos(parcours);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ParcoursDto>> GetUnParcours(long id)
        {
            GetParcoursByIdUseCase uc = new GetParcoursByIdUseCase(repositoryFactory);
            var parcours = await uc.ExecuteAsync(id);
            return parcours != null ? Ok(new ParcoursDto().ToDto(parcours)) : NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<ParcoursDto>> PostAsync([FromBody] ParcoursDto parcoursDto)
        {
            CreateParcoursUseCase uc = new CreateParcoursUseCase(repositoryFactory);
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
            if (id != parcoursDto.Id) return BadRequest();
            UpdateParcoursUseCase uc = new UpdateParcoursUseCase(repositoryFactory);
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
            DeleteParcoursUseCase uc = new DeleteParcoursUseCase(repositoryFactory);
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
            AddEtudiantDansParcoursUseCase uc = new AddEtudiantDansParcoursUseCase(repositoryFactory);
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
            AddUeDansParcoursUseCase uc = new AddUeDansParcoursUseCase(repositoryFactory);
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
    }
    
}
