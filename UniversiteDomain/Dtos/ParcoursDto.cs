using UniversiteDomain.Entities;

namespace UniversiteDomain.Dtos;

public class ParcoursDto
{
    public long Id { get; set; }
    public string NomParcours { get; set; }
    public int AnneeFormation { get; set; }

    public ParcoursDto ToDto(Parcours parcours)
    {
        Id = parcours.Id;
        NomParcours = parcours.NomParcours;
        AnneeFormation = parcours.AnneeFormation;
        return this;
    }

    public List<ParcoursDto> ToDtos(List<Parcours> parcours)
    {
        return parcours.Select(p => new ParcoursDto().ToDto(p)).ToList();
    }
    
    public Parcours ToEntity()
    {
        return new Parcours {Id = this.Id, NomParcours = this.NomParcours, AnneeFormation = this.AnneeFormation};
    }
}