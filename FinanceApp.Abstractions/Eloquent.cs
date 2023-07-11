using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceApp.Abstractions;

// TODO - Can't we remove the generics and just use Eloquent instead?
public abstract class Eloquent<T> where T : Eloquent<T>, new() {
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

    public T Save()
    {
        return ExistsOnDb ? Update() : Insert();
    }

	protected abstract T Update();

	protected abstract T Insert();
}
