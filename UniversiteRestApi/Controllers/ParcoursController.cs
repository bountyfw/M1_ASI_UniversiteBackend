using Microsoft.AspNetCore.Mvc;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.Dtos.Parcours;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.ParcoursUseCases.Get;
using UniversiteDomain.UseCases.ParcoursUseCases.Create;
using UniversiteDomain.UseCases.ParcoursUseCases.Update;
using UniversiteDomain.UseCases.ParcoursUseCases.Delete;

namespace UniversiteRestApi.Controllers
{
    [Route("api/parcours")]
    [ApiController]
    public class ParcoursController(IRepositoryFactory repositoryFactory) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<ParcoursDto>>> GetAllParcours()
        {
            GetAllParcoursUseCase uc = new GetAllParcoursUseCase(repositoryFactory);
            List<Parcours> parcours = await uc.ExecuteAsync();
            return ParcoursDto.ToDtos(parcours);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ParcoursDto>> GetParcoursById(long id)
        {
            GetParcoursByIdUseCase uc = new GetParcoursByIdUseCase(repositoryFactory);
            var parcours = await uc.ExecuteAsync(id);
            return parcours != null ? Ok(ParcoursDto.ToDto(parcours)) : NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<ParcoursDto>> AddParcours([FromBody] ParcoursDto parcoursDto)
        {
            CreateParcoursUseCase uc = new CreateParcoursUseCase(repositoryFactory);
            Parcours parcours = await uc.ExecuteAsync(parcoursDto.ToEntity());
            return CreatedAtAction(nameof(GetParcoursById), new { id = parcours.Id }, ParcoursDto.ToDto(parcours));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateParcours(long id, [FromBody] ParcoursDto parcoursDto)
        {
            if (id != parcoursDto.Id) return BadRequest();
            UpdateParcoursUseCase uc = new UpdateParcoursUseCase(repositoryFactory);
            await uc.ExecuteAsync(parcoursDto.ToEntity());
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteParcours(long id)
        {
            DeleteParcoursUseCase uc = new DeleteParcoursUseCase(repositoryFactory);
            await uc.ExecuteAsync(id);
            return NoContent();
        }

        [HttpPost("{parcoursId}/etudiant/{etudiantId}")]
        public async Task<IActionResult> AddEtudiantToParcours(long parcoursId, long etudiantId)
        {
            AddEtudiantToParcoursUseCase uc = new AddEtudiantToParcoursUseCase(repositoryFactory);
            await uc.ExecuteAsync(parcoursId, etudiantId);
            return NoContent();
        }
    }
}
