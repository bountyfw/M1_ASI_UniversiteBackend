using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;
using UniversiteEFDataProvider.Entities;

namespace UniversiteEFDataProvider.Repositories;

public class UniversiteUserRepository(UniversiteDbContext context, UserManager<UniversiteUser> userManager, RoleManager<UniversiteRole> roleManager) : Repository<IUniversiteUser>(context), IUniversiteUserRepository
{
    public async Task<IUniversiteUser?> AddUserAsync(string login, string email, string password, string role,  Etudiant? etudiant)
    {
        UniversiteUser user = new UniversiteUser { UserName = login, Email = email, Etudiant = etudiant };
        IdentityResult result = await userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, role);
        }
        await context.SaveChangesAsync();
        return result.Succeeded ? user : null;
        return user;
    }

    public async Task<IUniversiteUser> FindByEmailAsync(string email)
    {
        return await userManager.FindByEmailAsync(email);
    }
    
    public async Task UpdateAsync(Etudiant etudiant)
    {
        // Peut ne pas fonctionner car problèmes en suivant le TD.
        UniversiteUser? user = await context.UniversiteUsers.Include(u => u.Etudiant)
            .FirstOrDefaultAsync(u => u.EtudiantId == etudiant.Id);
        if (user == null) return;

        // Mise à jour des valeurs sans attacher une nouvelle instance
        user.Etudiant.Nom = etudiant.Nom;
        user.Etudiant.Prenom = etudiant.Prenom;
        user.Etudiant.Email = etudiant.Email;
        user.Etudiant.NumEtud = etudiant.NumEtud;

        await context.SaveChangesAsync();
    }

    public async Task<int> DeleteAsync(long id)
    {
        Etudiant etud = context.Etudiants.Find(id);
        UniversiteUser user= await userManager.FindByEmailAsync(etud.Email);
        if (user!=null)
        {
            await userManager.DeleteAsync(user);
            int res=await  context.SaveChangesAsync();
            return 1;
        }
        return 0;
    }

    public async Task<bool> IsInRoleAsync(string email, string role)
    {
        bool res = false;
        var user =await userManager.FindByEmailAsync(email);
        return await userManager.IsInRoleAsync(user, role);
    }
}