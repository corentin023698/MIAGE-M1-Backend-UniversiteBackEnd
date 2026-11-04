using System.Linq.Expressions;
using Moq;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.NoteUsesCases.NoteDansUeEtudiant;

namespace UniversiteDomainUnitTests;

public class NoteUnitTest
{
    [Test]
    public async Task AddNoteEtudiantUeUseCase()
    {
        long idEtudiant = 1;
        long idUe = 2;
        float valeur = 15;
        Ue ue = new Ue { Id = idUe, NumeroUe = "UE1", Intitule = "Test" };
        Parcours parcours = new Parcours { Id = 3, NomParcours = "Parcours test", AnneeFormation = 1 };
        parcours.UesEnseignees.Add(ue);
        Etudiant etudiant = new Etudiant
        {
            Id = idEtudiant,
            NumEtud = "1",
            Nom = "Nom",
            Prenom = "Prenom",
            Email = "test@etud.u-picardie.fr",
            ParcoursSuivi = parcours
        };
        Note noteFinale = new Note { Id = 4, Etudiant = etudiant, Ue = ue, Valeur = valeur };

        // On initialise des faux repositories
        var mockEtudiant = new Mock<IEtudiantRepository>();
        var mockUe = new Mock<IUeRepository>();
        var mockNote = new Mock<INoteRepository>();
        mockEtudiant
            .Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Etudiant, bool>>>()))
            .ReturnsAsync(new List<Etudiant> { etudiant });
        mockUe
            .Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Ue, bool>>>()))
            .ReturnsAsync(new List<Ue> { ue });
        mockNote
            .Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Note, bool>>>()))
            .ReturnsAsync(new List<Note>());
        mockNote
            .Setup(repo => repo.AddNoteAsync(idEtudiant, idUe, valeur))
            .ReturnsAsync(noteFinale);

        // Création d'une fausse factory qui contient les faux repositories
        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(facto => facto.EtudiantRepository()).Returns(mockEtudiant.Object);
        mockFactory.Setup(facto => facto.UeRepository()).Returns(mockUe.Object);
        mockFactory.Setup(facto => facto.NoteRepository()).Returns(mockNote.Object);

        // Création du use case en utilisant le mock comme datasource
        AddNoteEtudiantUe useCase = new AddNoteEtudiantUe(mockFactory.Object);

        // Appel du use case
        var noteTestee = await useCase.ExecuteAsync(idEtudiant, idUe, valeur);

        // Vérification du résultat
        Assert.That(noteTestee.Id, Is.EqualTo(noteFinale.Id));
        Assert.That(noteTestee.Etudiant.Id, Is.EqualTo(idEtudiant));
        Assert.That(noteTestee.Ue.Id, Is.EqualTo(idUe));
        Assert.That(noteTestee.Valeur, Is.EqualTo(valeur));
    }
}