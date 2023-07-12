using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceApp.Abstractions;

// TODO - Can't we remove the generics and just use Eloquent instead?
public abstract class Eloquent {
    public bool ExistsOnDb;
    private long id;
    
    [Column("ID")]
    public long ID {
        get => id;
        set
        {
            if (!ExistsOnDb) {
                id = value;
            } else {
                throw new Exception("Cannot assign ID to existing DB entry");
            }
        }
    }

    public Eloquent Save()
    {
        return ExistsOnDb ? Update() : Insert();
    }

	protected abstract Eloquent Update();

	protected abstract Eloquent Insert();
}
