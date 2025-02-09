using Microsoft.AspNetCore.Mvc;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.Dtos.Parcours;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.ParcoursUseCases.Get;
using UniversiteDomain.UseCases.ParcoursUseCases.Create;
using UniversiteDomain.UseCases.ParcoursUseCases.Update;
using UniversiteDomain.UseCases.ParcoursUseCases.Delete;
using UniversiteDomain.UseCases.ParcoursUseCases.EtudiantDansParcours;

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
            return parcours != null ? Ok(ParcoursDto.ToDto(parcours)) : NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<ParcoursDto>> PostAsync([FromBody] ParcoursDto parcoursDto)
        {
            CreateParcoursUseCase uc = new CreateParcoursUseCase(repositoryFactory);
            Parcours parcours = await uc.ExecuteAsync(parcoursDto.ToEntity());
            return CreatedAtAction(nameof(GetParcoursById), new { id = parcours.Id }, ParcoursDto.ToDto(parcours));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(long id, [FromBody] ParcoursDto parcoursDto)
        {
            if (id != parcoursDto.Id) return BadRequest();
            UpdateParcoursUseCase uc = new UpdateParcoursUseCase(repositoryFactory);
            await uc.ExecuteAsync(parcoursDto.ToEntity());
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(long id)
        {
            DeleteParcoursUseCase uc = new DeleteParcoursUseCase(repositoryFactory);
            await uc.ExecuteAsync(id);
            return NoContent();
        }

        [HttpPost("{parcoursId}/etudiant/{etudiantId}")]
        public async Task<IActionResult> AddEtudiantToParcours(long parcoursId, long etudiantId)
        {
            AddEtudiantDansParcoursUseCase uc = new AddEtudiantDansParcoursUseCase(repositoryFactory);
            await uc.ExecuteAsync(parcoursId, etudiantId);
            return NoContent();
        }
    }
}
