using System.Linq.Expressions;
using Moq;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.ParcoursExceptions;
using UniversiteDomain.UseCases.ParcoursUseCases.Create;
using UniversiteDomain.UseCases.ParcoursUseCases.EtudiantDansParcours;
using UniversiteDomain.UseCases.ParcoursUseCases.UeDansParcours;

namespace UniversiteDomainUnitTests;

public class ParcoursUnitTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task CreateParcoursUseCase()
    {
        long idParcours = 1;
        String nomParcours = "Ue 1";
        int anneFormation = 2;
        
        // On crée le parcours qui doit être ajouté en base
        Parcours parcoursAvant = new Parcours{NomParcours = nomParcours, AnneeFormation = anneFormation};
        
        // On initialise une fausse datasource qui va simuler un EtudiantRepository
        var mockParcours = new Mock<IParcoursRepository>();
        
        // Il faut ensuite aller dans le use case pour simuler les appels des fonctions vers la datasource
        // Nous devons simuler FindByCondition et Create
        // On dit à ce mock que le parcours n'existe pas déjà
        mockParcours
            .Setup(repo=>repo.FindByConditionAsync(p=>p.Id.Equals(idParcours)))
            .ReturnsAsync((List<Parcours>)null);
        // On lui dit que l'ajout d'un étudiant renvoie un étudiant avec l'Id 1
        Parcours parcoursFinal =new Parcours{Id=idParcours,NomParcours= nomParcours, AnneeFormation = anneFormation};
        mockParcours.Setup(repo=>repo.CreateAsync(parcoursAvant)).ReturnsAsync(parcoursFinal);
        
        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(facto=>facto.ParcoursRepository()).Returns(mockParcours.Object);
        
        // Création du use case en utilisant le mock comme datasource
        CreateParcoursUseCase useCase=new CreateParcoursUseCase(mockFactory.Object);
        
        // Appel du use case
        var parcoursTeste=await useCase.ExecuteAsync(parcoursAvant);
        
        // Vérification du résultat
        Assert.That(parcoursTeste.Id, Is.EqualTo(parcoursFinal.Id));
        Assert.That(parcoursTeste.NomParcours, Is.EqualTo(parcoursFinal.NomParcours));
        Assert.That(parcoursTeste.AnneeFormation, Is.EqualTo(parcoursFinal.AnneeFormation));
    }
    
    [Test]
    public async Task AddUeDansParcoursUseCaseTest()
    {
        long idUe = 1;
        long idParcours = 3;

        // Création d'une UE et d'un parcours
        Ue ue = new Ue { Id = idUe, NumeroUe = "UE1", Intitule = "Intitulé UE 1" };
        Parcours parcoursSansUe = new Parcours
        {
            Id = idParcours,
            NomParcours = "Parcours Test",
            AnneeFormation = 2,
            UesEnseignees = new List<Ue>() // Liste vide au départ
        };

        Parcours parcoursAvecUe = new Parcours
        {
            Id = idParcours,
            NomParcours = "Parcours Test",
            AnneeFormation = 2,
            UesEnseignees = new List<Ue> { ue } // UE ajoutée
        };

        // Mock des repositories
        var mockUeRepository = new Mock<IUeRepository>();
        var mockParcoursRepository = new Mock<IParcoursRepository>();

        // Simuler la recherche de l'UE
        mockUeRepository
            .Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Ue, bool>>>()))
            .ReturnsAsync(new List<Ue> { ue });

        // Simuler la recherche du parcours
        mockParcoursRepository
            .SetupSequence(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Parcours, bool>>>()))
            // 1ère recherche : Parcours sans UE
            .ReturnsAsync(new List<Parcours> { parcoursSansUe })
            // 2ème recherche : Parcours avec UE ajoutée
            .ReturnsAsync(new List<Parcours> { parcoursAvecUe });

        // Simuler l'ajout de l'UE au parcours
        mockParcoursRepository
            .Setup(repo => repo.AddUeAsync(idParcours, idUe))
            .ReturnsAsync(parcoursAvecUe);

        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(facto => facto.UeRepository()).Returns(mockUeRepository.Object);
        mockFactory.Setup(facto => facto.ParcoursRepository()).Returns(mockParcoursRepository.Object);

        // Création du use case
        AddUeDansParcoursUseCase useCase = new AddUeDansParcoursUseCase(mockFactory.Object);

        // Étape 1 : Ajout réussi
        var result = await useCase.ExecuteAsync(idParcours, idUe);
        Assert.That(result.Id, Is.EqualTo(parcoursAvecUe.Id));
        Assert.That(result.UesEnseignees, Is.Not.Null);
        Assert.That(result.UesEnseignees.Count, Is.EqualTo(1));
        Assert.That(result.UesEnseignees[0].Id, Is.EqualTo(ue.Id));

        // Étape 2 : Ajout en doublon (doit lever une exception)
        var ex = Assert.ThrowsAsync<DuplicateUeDansParcoursException>(async () =>
            await useCase.ExecuteAsync(idParcours, idUe));
        Assert.That(ex.Message, Is.EqualTo($"{idUe} est déjà présente dans le parcours : {idParcours}"));

        // Vérification des appels des mocks
        mockUeRepository.Verify(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Ue, bool>>>()), Times.Exactly(2));
        mockParcoursRepository.Verify(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Parcours, bool>>>()), Times.Exactly(2));
        mockParcoursRepository.Verify(repo => repo.AddUeAsync(idParcours, idUe), Times.Once);
    }


}
