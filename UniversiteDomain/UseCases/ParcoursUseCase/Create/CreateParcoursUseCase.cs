using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.ParcoursExceptions;

namespace UniversiteDomain.UseCases.ParcoursUseCase.Create;

public class CreateParcoursUseCase(IParcoursRepository parcoursRepository)
{
    public async Task<Parcours> ExecuteAsync(string nomParcours, int anneeParcours)
    {
        var parcours = new Parcours{NomParcours = nomParcours, AnneeFormation =  anneeParcours};
        return await ExecuteAsync(parcours);
    }
    public async Task<Parcours> ExecuteAsync(Parcours parcours)
    {
        await CheckBusinessRules(parcours);
        Parcours p = await parcoursRepository.CreateAsync(parcours);
        parcoursRepository.SaveChangesAsync().Wait();
        return p;
    }
    
    private async Task CheckBusinessRules(Parcours parcours)
    {
        ArgumentNullException.ThrowIfNull(parcours);
        ArgumentNullException.ThrowIfNull(parcours.NomParcours);
        ArgumentNullException.ThrowIfNull(parcoursRepository);
        
        List<Parcours> existe = await parcoursRepository.FindByConditionAsync(p=>p.NomParcours.Equals(parcours.NomParcours));

        if (existe is {Count:>0}) throw new DuplicateNomParcoursException(parcours.NomParcours+ " - ce nom de parcours existe déjà");
        
        // Le métier définit que les nom doite contenir plus de 3 lettres
        if (parcours.NomParcours.Length < 3) throw new InvalidNomParcoursException(parcours.NomParcours +" incorrect - Le nom d'un parcours doit contenir plus de 3 caractères");
    }
}