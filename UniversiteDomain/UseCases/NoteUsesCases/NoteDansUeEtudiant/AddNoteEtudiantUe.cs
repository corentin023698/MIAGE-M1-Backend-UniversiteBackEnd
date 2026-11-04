using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.NoteExceptions;

namespace UniversiteDomain.UseCases.NoteUsesCases.NoteDansUeEtudiant;

public class AddNoteEtudiantUe (IRepositoryFactory repositoryFactory)
{
    public async Task<Note> ExecuteAsync(Etudiant etudiant, Ue ue, float valeur)
    {
        ArgumentNullException.ThrowIfNull(etudiant);
        ArgumentNullException.ThrowIfNull(ue);
        await CheckBusinessRules(etudiant.Id, ue.Id, valeur);
        return await ExecuteAsync(etudiant.Id, ue.Id, valeur);
    }  
    
    public async Task<Note> ExecuteAsync(long idEtudiant, long idUe, float valeur)
    {
        await CheckBusinessRules(idEtudiant, idUe, valeur); 
        return await repositoryFactory.NoteRepository().AddNoteAsync(idEtudiant, idUe, valeur);
    }

    private async Task CheckBusinessRules(long idEtudiant, long idUe, float valeur)
    {
        ArgumentNullException.ThrowIfNull(idEtudiant);
        ArgumentNullException.ThrowIfNull(idUe);
        
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(idEtudiant);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(idUe);
        
        ArgumentNullException.ThrowIfNull(repositoryFactory);
        ArgumentNullException.ThrowIfNull(repositoryFactory.NoteRepository());
        
        if (valeur < 0 || valeur > 20)
        {
            throw new InvalidNoteException(valeur+" n'est pas une note valide");
        }
        
        List<Etudiant> etudiant = await repositoryFactory.EtudiantRepository().FindByConditionAsync(e=>e.Id.Equals(idEtudiant));;
        if (etudiant is { Count: 0 }) throw new EtudiantNotFoundException(idEtudiant.ToString());
        
        List<Ue> ue = await repositoryFactory.UeRepository().FindByConditionAsync(u=>u.Id.Equals(idUe));;
        if (ue is { Count: 0 }) throw new UeNotFoundException(idUe.ToString());

        if (etudiant.First().ParcoursSuivi == null)
        {
            throw new EtudiantNotInParcoursException(idEtudiant + " n'est dans aucun parcours");
        }
            
        if(!etudiant.First().ParcoursSuivi.UesEnseignees.Contains(ue.First()))
        {
            throw new UeNotInParcoursException(idUe+" n'est pas dans le parcours de l'etudiant " + idEtudiant);
        }
        
        List<Note> notesEtudiants = await repositoryFactory.NoteRepository().FindByConditionAsync(n => n.Etudiant.Id.Equals(idEtudiant) && n.Ue.Id.Equals(idUe));
        if (notesEtudiants is { Count: > 0 })
            throw new DuplicateNoteUeException("Une note existe déjà pour l'étudiant " + idEtudiant + " dans l'ue " + idUe);
    }

}