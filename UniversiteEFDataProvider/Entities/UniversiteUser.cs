using Microsoft.AspNetCore.Identity;
using UniversiteDomain.Entities;

namespace UniversiteEFDataProvider.Entities;

public class UniversiteUser:IdentityUser, IUniversiteUser {
    [PersonalData]
    public virtual Etudiant? Etudiant { get; set; }
    [PersonalData]
    public long? EtudiantId { get; set; }
}