using Microsoft.AspNetCore.Mvc;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.UeUseCases.Create;
using UniversiteDomain.UseCases.UeUseCases.Delete;
using UniversiteDomain.UseCases.UeUseCases.Get;
using UniversiteDomain.UseCases.UeUseCases.Update;

namespace UniversiteRestApi.Controllers;

[Route("api/ue")]
[ApiController]
public class UeController(IRepositoryFactory repositoryFactory) : ControllerBase
{
    
    [HttpGet("{id}")]
    public async Task<ActionResult<UeDto>> GetUe(long id)
    {
        GetUeByIdUseCase uc = new GetUeByIdUseCase(repositoryFactory);
        var ue = await uc.ExecuteAsync(id);
        return ue != null ? Ok(new UeDto().ToDto(ue)) : NotFound();
    }
    
    [HttpGet]
    public async Task<ActionResult<List<UeDto>>> GetUes()
    {
        GetToutesLesUesUseCase uc = new GetToutesLesUesUseCase(repositoryFactory);
        List<Ue> ues = await uc.ExecuteAsync();
        return new UeDto().ToDtos(ues);
    }
    
    [HttpPost]
    public async Task<ActionResult<UeDto>> PostUe([FromBody] UeDto ueDto)
    {
        CreateUeUseCase uc = new CreateUeUseCase(repositoryFactory);
        Ue ue = ueDto.ToEntity();
        try
        {
            await uc.ExecuteAsync(ue);
        }
        catch (Exception e)
        {
            ModelState.AddModelError(nameof(e), e.Message);
            return ValidationProblem();
        }
        return CreatedAtAction(nameof(GetUe), new { id = ue.Id }, new UeDto().ToDto(ue));
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> PutUe(long id, [FromBody] UeDto ueDto)
    {
        if (id != ueDto.Id) return BadRequest();
        UpdateUeUseCase uc = new UpdateUeUseCase(repositoryFactory);
        Ue ue = ueDto.ToEntity();
        try
        {
            await uc.ExecuteAsync(ue);
        }
        catch (Exception e)
        {
            ModelState.AddModelError(nameof(e), e.Message);
            return ValidationProblem();
        }
        return NoContent();
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUe(long id)
    {
        DeleteUeUseCase uc = new DeleteUeUseCase(repositoryFactory);
        try
        {
            await uc.ExecuteAsync(id);
        } catch (Exception e)
        {
            ModelState.AddModelError(nameof(e), e.Message);
            return ValidationProblem();
        }
        return NoContent();
    }
    
}