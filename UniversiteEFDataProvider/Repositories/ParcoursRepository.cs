using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;

namespace UniversiteEFDataProvider.Repositories;

public class ParcoursRepository (UniversiteDbContext context) : Repository<Parcours>(context), IParcoursRepository
{
    public Task<Parcours> AddEtudiantAsync(Parcours parcours, Etudiant etudiant)
    {
        return AddEtudiantAsync(parcours.Id, etudiant.Id);
    }

    public async Task<Parcours> AddEtudiantAsync(long idParcours, long idEtudiant)
    {
        ArgumentNullException.ThrowIfNull(Context.Etudiants);
        ArgumentNullException.ThrowIfNull(Context.Parcours);
        Etudiant etudiant = (await Context.Etudiants.FindAsync(idEtudiant))!;
        Parcours parcours = (await Context.Parcours.FindAsync(idParcours))!;
        if (parcours.Inscrits == null)
        {
            parcours.Inscrits = new List<Etudiant>();
            parcours.Inscrits.Add(etudiant);
        }
        else
        {
            parcours.Inscrits.Add(etudiant);
        }
        parcours.Inscrits.Add(etudiant);
        etudiant.ParcoursSuivi = parcours;
        await Context.SaveChangesAsync();
        return parcours;
    }

    public Task<Parcours> AddEtudiantAsync(Parcours parcours, List<Etudiant> etudiants)
    {
        return AddEtudiantAsync(parcours.Id, etudiants.Select(e => e.Id).ToArray());
    }

    public async Task<Parcours> AddEtudiantAsync(long idParcours, long[] idEtudiants)
    {
        Parcours parcours = new Parcours();
        foreach (long id in idEtudiants)
        {
            parcours = await AddEtudiantAsync(idParcours, id);
        }
        return parcours;
    }

    public async Task<Parcours> AddUeAsync(long idParcours, long idUe)
    {
        ArgumentNullException.ThrowIfNull(Context.Ues);
        ArgumentNullException.ThrowIfNull(Context.Parcours);
        Ue ue = (await Context.Ues.FindAsync(idUe))!;
        Parcours parcours = (await Context.Parcours.FindAsync(idParcours))!;
        if (parcours.UesEnseignees == null)
        {
            parcours.UesEnseignees = new List<Ue>();
            parcours.UesEnseignees.Add(ue);
        }
        else
        {
            parcours.UesEnseignees.Add(ue);
        }
        
        if (ue.EnseigneeDans == null)
        {
            ue.EnseigneeDans = new List<Parcours>();
            ue.EnseigneeDans.Add(parcours);
        }
        else
        {
            ue.EnseigneeDans.Add(parcours);
        }
        
        await Context.SaveChangesAsync();
        return parcours;
    }

    public async Task<Parcours> AddUeAsync(long idParcours, long[] idUes)
    {
        Parcours parcours = new Parcours();
        foreach (var idUe in idUes)
        {
            parcours = await AddUeAsync(idParcours, idUe);
        }
        return parcours;
    }
}