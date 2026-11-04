using UniversiteDomain.Entities;

namespace UniversiteDomain.DataAdapters;

public interface INoteRepository : IRepository<Note>   
{
    Task<Note> AddNoteAsync(Etudiant etudiant, Ue ue, float valeur);
    
    Task<Note> AddNoteAsync(long idEtudiant, long idUe, float valeur);
    
}