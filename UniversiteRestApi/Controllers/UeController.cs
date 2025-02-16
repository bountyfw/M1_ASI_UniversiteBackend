using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.SecurityUseCases.Get;
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
        GetUeByIdUseCase uc = new GetUeByIdUseCase(repositoryFactory);
        if (!uc.IsAuthorized(role)) return Unauthorized();
        var ue = await uc.ExecuteAsync(id);
        return ue != null ? Ok(new UeDto().ToDto(ue)) : NotFound();
    }
    
    [HttpGet]
    public async Task<ActionResult<List<UeDto>>> GetUes()
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
        GetToutesLesUesUseCase uc = new GetToutesLesUesUseCase(repositoryFactory);
        if (!uc.IsAuthorized(role)) return Unauthorized();
        List<Ue> ues = await uc.ExecuteAsync();
        return new UeDto().ToDtos(ues);
    }
    
    [HttpPost]
    public async Task<ActionResult<UeDto>> PostUe([FromBody] UeDto ueDto)
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
        CreateUeUseCase uc = new CreateUeUseCase(repositoryFactory);
        if (!uc.IsAuthorized(role)) return Unauthorized();
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
        if (id != ueDto.Id) return BadRequest();
        UpdateUeUseCase uc = new UpdateUeUseCase(repositoryFactory);
        if (!uc.IsAuthorized(role)) return Unauthorized();
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
        DeleteUeUseCase uc = new DeleteUeUseCase(repositoryFactory);
        if (!uc.IsAuthorized(role)) return Unauthorized();
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