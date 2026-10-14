using Moq;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.ParcoursUseCase.Create;
using UniversiteDomain.UseCases.ParcoursUseCases.UeDansParcours;
using UniversiteDomain.UseCases.UeUsesCases.Create;

namespace UniversiteDomainUnitTests;

public class UeUnitTests
{
    [Test]
    public async Task CreateParcoursUseCase()
    {
        long idUe = 1;
        String numeroUe = "Ue 1";
        String intitule = "test";
        
        // On crée le parcours qui doit être ajouté en base
        Ue ueAvant = new Ue{NumeroUe = numeroUe, Intitule = intitule};
        
        var mockParcours = new Mock<IUeRepository>();
        
        // Il faut ensuite aller dans le use case pour simuler les appels des fonctions vers la datasource
        // Nous devons simuler FindByCondition et Create
        // On dit à ce mock que le parcours n'existe pas déjà
        mockParcours
            .Setup(repo=>repo.FindByConditionAsync(u=>u.Id.Equals(idUe)))
            .ReturnsAsync((List<Ue>)null);
        // On lui dit que l'ajout d'un étudiant renvoie un étudiant avec l'Id 1
        Ue ueFinal =new Ue{Id=idUe,NumeroUe = numeroUe, Intitule = intitule};
        mockParcours.Setup(repo=>repo.CreateAsync(ueAvant)).ReturnsAsync(ueFinal);
        
        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(facto=>facto.UeRepository()).Returns(mockParcours.Object);
        
        // Création du use case en utilisant le mock comme datasource
        CreateUeUseCase useCase=new CreateUeUseCase(mockFactory.Object.UeRepository());
        
        // Appel du use case
        var ueTest=await useCase.ExecuteAsync(ueAvant);
        
        // Vérification du résultat
        Assert.That(ueTest.Id, Is.EqualTo(ueFinal.Id));
        Assert.That(ueTest.NumeroUe, Is.EqualTo(ueFinal.NumeroUe));
        Assert.That(ueTest.Intitule, Is.EqualTo(ueFinal.Intitule));
    }
    
    [Test]
    public async Task AddUeDansParcoursUseCase()
    {
        long idUe = 1;
        long idParcours = 3;
        Ue ue= new Ue { Id = 1, NumeroUe = "1", Intitule =  "test" };
        Parcours parcours = new Parcours{Id=3, NomParcours = "Ue 3", AnneeFormation = 1};
        
        // On initialise des faux repositories
        var mockUe = new Mock<IUeRepository>();
        var mockParcours = new Mock<IParcoursRepository>();
        List<Ue> ues = new List<Ue>();
        ues.Add(new Ue{Id=1});
        mockUe
            .Setup(repo=>repo.FindByConditionAsync(u=>u.Id.Equals(idUe)))
            .ReturnsAsync(ues);

        List<Parcours> parcourses = new List<Parcours>();
        parcourses.Add(parcours);
        
        List<Parcours> parcoursFinaux = new List<Parcours>();
        Parcours parcoursFinal = new Parcours{Id=3, NomParcours = "Ue 3", AnneeFormation = 1};
        parcoursFinal.UesEnseignees.Add(ue);
        parcoursFinaux.Add(parcoursFinal);
        
        mockParcours
            .Setup(repo=>repo.FindByConditionAsync(u=>u.Id.Equals(idParcours)))
            .ReturnsAsync(parcourses);
        mockParcours
            .Setup(repo => repo.AddUeAsync(idParcours, idUe))
            .ReturnsAsync(parcoursFinal);
        
        // Création d'une fausse factory qui contient les faux repositories
        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(facto=>facto.UeRepository()).Returns(mockUe.Object);
        mockFactory.Setup(facto=>facto.ParcoursRepository()).Returns(mockParcours.Object);
        
        // Création du use case en utilisant le mock comme datasource
        AddUeDansParcoursUseCase useCase=new AddUeDansParcoursUseCase(mockFactory.Object);
        
        // Appel du use case
        var parcoursTest=await useCase.ExecuteAsync(idParcours, idUe);
        // Vérification du résultat
        Assert.That(parcoursTest.Id, Is.EqualTo(parcoursFinal.Id));
        Assert.That(parcoursTest.UesEnseignees, Is.Not.Null);
        Assert.That(parcoursTest.UesEnseignees.Count, Is.EqualTo(1));
        Assert.That(parcoursTest.UesEnseignees[0].Id, Is.EqualTo(idUe));
    }
}