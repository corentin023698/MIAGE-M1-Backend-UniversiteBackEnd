namespace UniversiteDomain.Entities;

public class Note
{
    public long Id { get; set; }
    
    public Etudiant Etudiant { get; set; }
    
    public Ue Ue { get; set; }
    
    public float Valeur { get; set; }

    //TODO : rajouter pour éviter un bug dans UniversiteDbContext
    public long EtudiantId { get; set; }
    
    public long UeId { get; set; } 

    public override string ToString()
    {
        return "ID "+Id +" : Etudiant "+Etudiant+" - Ue "+Ue+" - Valeur "+Valeur;
    }
}