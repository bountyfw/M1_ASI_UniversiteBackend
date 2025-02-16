using Microsoft.AspNetCore.Identity;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;
using UniversiteEFDataProvider.Entities;
using UniversiteEFDataProvider.Repositories;

namespace UniversiteEFDataProvider.RepositoryFactories;

public class RepositoryFactory (UniversiteDbContext context, 
    UserManager<UniversiteUser> _userManager,
    RoleManager<UniversiteRole> _roleManager): IRepositoryFactory
{
    private IParcoursRepository? _parcours;
    private IEtudiantRepository? _etudiants;
    private IUeRepository? _ues;
    private INoteRepository? _notes;
    private IUniversiteRoleRepository? _universiteRoles;
    private IUniversiteUserRepository? _universiteUsers;
    private ICsvDataAdapterRepository? _csvDataAdapter;
    
    public IParcoursRepository ParcoursRepository()
    {
        if (_parcours == null)
        {
            _parcours = new ParcoursRepository(context ?? throw new InvalidOperationException());
        }
        return _parcours;
    }

    public IEtudiantRepository EtudiantRepository()
    {
        if (_etudiants == null)
        {
            _etudiants = new EtudiantRepository(context ?? throw new InvalidOperationException());
        }
        return _etudiants;
    }

    public IUeRepository UeRepository()
    {
        if (_ues == null)
        {
            _ues = new UeRepository(context ?? throw new InvalidOperationException());
        }
        return _ues;
    }

    public INoteRepository NoteRepository()
    {
        if (_notes == null)
        {
            _notes = new NoteRepository(context ?? throw new InvalidOperationException());
        }
        return _notes;

    }
    
    public IUniversiteRoleRepository UniversiteRoleRepository()
    {
        if (_universiteRoles == null)
        {
            _universiteRoles = new UniversiteRoleRepository(context ?? throw new InvalidOperationException(),
                _roleManager ?? throw new InvalidOperationException());
        }
        return _universiteRoles;
    }
    
    public IUniversiteUserRepository UniversiteUserRepository()
    {
        if (_universiteUsers == null)
        {
            _universiteUsers = new UniversiteUserRepository(context ?? throw new InvalidOperationException(),
                _userManager ?? throw new InvalidOperationException(),
                _roleManager ?? throw new InvalidOperationException());
        }
        return _universiteUsers;
    }
    
    public ICsvDataAdapterRepository CsvDataAdapterRepository()
    {
        if (_csvDataAdapter == null)
        {
            _csvDataAdapter = new CsvDataAdapterRepository(context ?? throw new InvalidOperationException());
        }
        return _csvDataAdapter;
    }
       
    public async Task SaveChangesAsync()
    {
        context.SaveChangesAsync().Wait();
    }
    public async Task EnsureCreatedAsync()
    {
        context.Database.EnsureCreated();
    }
    public async Task EnsureDeletedAsync()
    {
        context.Database.EnsureDeleted();
    }
}
