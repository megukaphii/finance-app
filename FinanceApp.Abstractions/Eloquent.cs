using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceApp.Abstractions;

public abstract class Eloquent {
    // TODO - Would like this to be private and not nullable? But the parser is the issue with this.
	public IDatabase? Database;
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
