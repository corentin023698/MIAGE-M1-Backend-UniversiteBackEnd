using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;

namespace UniversiteEFDataProvider.Repositories;

public class NoteRepository(UniversiteDbContext context) : Repository<Note>(context), INoteRepository
{
    public async Task<Note> AddNoteAsync(Etudiant etudiant, Ue ue, float valeur)
    {
        return await AddNoteAsync(etudiant.Id, ue.Id, valeur);
    }

    public async Task<Note> AddNoteAsync(long idEtudiant, long idUe, float valeur)
    {
        ArgumentNullException.ThrowIfNull(Context.Etudiants);
        ArgumentNullException.ThrowIfNull(Context.Ues);
        Etudiant e = (await Context.Etudiants.FindAsync(idEtudiant))!;
        Ue u = (await Context.Ues.FindAsync(idUe))!;
        Note note = new Note();
        note.Etudiant = e; note.Ue = u; note.Valeur = valeur;
        e.NotesObtenues.Add(note);
        u.Notes.Add(note);
        await Context.SaveChangesAsync();
        return note;
    }
}