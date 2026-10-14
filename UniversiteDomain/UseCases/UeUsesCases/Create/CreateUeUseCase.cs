using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.ParcoursExceptions;
using UniversiteDomain.Exceptions.UeExceptions;

namespace UniversiteDomain.UseCases.UeUsesCases.Create;

public class CreateUeUseCase(IUeRepository ueRepository)
{
    public async Task<Ue> ExecuteAsync(string intitule, String numeroUe)
    {
        var ue = new Ue{Intitule = intitule, NumeroUe =  numeroUe};
        return await ExecuteAsync(ue);
    }
    public async Task<Ue> ExecuteAsync(Ue ue)
    {
        await CheckBusinessRules(ue);
        Ue u = await ueRepository.CreateAsync(ue);
        ueRepository.SaveChangesAsync().Wait();
        return u;
    }
    
    private async Task CheckBusinessRules(Ue ue)
    {
        ArgumentNullException.ThrowIfNull(ue);
        ArgumentNullException.ThrowIfNull(ue.NumeroUe);
        ArgumentNullException.ThrowIfNull(ue.Intitule);
        ArgumentNullException.ThrowIfNull(ueRepository);
        
        List<Ue> existe = await ueRepository.FindByConditionAsync(u=>u.NumeroUe.Equals(ue.NumeroUe));

        if (existe is {Count:>0}) throw new DuplicateUeException(ue.NumeroUe+ " - ce numéro d'UE existe déjà");
        
        // Le métier définit que les nom doite contenir plus de 3 lettres
        if (ue.Intitule.Length < 3) throw new InvalidIntituleUeException(ue.Intitule +" incorrect - L'intitule d'une ue doit contenir plus de 3 caractères");
    }
}