using Microsoft.Data.Sqlite;
using System.Collections;
using System.Data;

namespace Server.Database;

public class Parameter {
	private SqliteParameter param;

	internal SqliteParameter SqliteParameter => param;

	public Parameter(SqlDbType type, string key, object value) {
		/*// I am lagggggggggggggggggggggggging
		// Wow, I'm amazed text editing could be so bandwidth-intensive.
		// Haha yeah I can't say I have a greate internet connection where I live. It's not terrible but it's not great eeither.
		// Which is weird conidering that I can watch Youtbue on it...
		// That is strange - I for one am using Parsec to remote to my home computer and am currently on a wireless connection in the outback, so. Not ideal in some ways either.

		// Yeah if anything you should probably have it worse than me, and look at where we are lol
		// Wow, you are lagging, I see
		// Me: Why is Cory typing out the same initialization code that I am?
		// Also Me: Ah. Lag.
		// Me: Ah.*/

		param = new(key, type) { Value = value };
		param.Value = value;
	}

	public T? GetValue<T>() {
		/*// Now what step am I missing here?
		// Now that I am no longer on my phone wifi hotspot, my connection is lagging 1% less.
		// Also at this point since we are casting manually and forcefully (like you have done below),
		// An exception will be thrown if T does not match param.DbType. We can surround this in a try catch
		// then throw a custom exception if you want, or we can explore other options. Just please, for the love of
		// god: do not involve null into this. Nice.
		// Hm, I do wonder if null would be a parameter that would come up - maybe. That said, doing it this way doesn't seem super practical? I guess, actually, we should look at its' usage and work backwards?
		// This is how they like it, and this is us doing it ourselves... which reminds me that null has to be a part of it, since it can definitely be returned from
		// a query / stored proc. Dammit. The good thing about this is that we don't need to switch on the DbType actually. The SQLiteParameter class guarantees that the
		// enum type matches the object; it's just a matter of making sure that the object type matches T.
		// Okay, so how are we gonna do that?*/

		if (param.Value?.GetType() != typeof(T))
        {
			throw new Exception("Type is wrong");
			// TODO: Throw
        }

		return (T?)param.Value;
		/* TODO: Parsing here
		// 
		// Considering Parallel algorithms since parsing has to be specialised for each type supported
		// Oh, I see, because the value won't necessarily be a string, it could be an int, right... would that not mean we need the type in the class definition?
		// Side note: I love how we have just made this our chat This was 100% the intended use case of comments. HAHAHAHHAHAHAHAHAH
		// So how parse?
		// The problem is knowing when to parse. We could parse beforehand, actually. That does delegate all of the parsing to a different place, and ties in
		// easily when adding parameters to the List<Parameter> in Parameters.
		// That seems fair enough? So, we accept 2 strings and expect to get out strings, and perform parsing when we iterate through the List? I guess, so, 
		// I do feel like a lot of this work has probably already been done for us using some libraries lol
		// Aha, I suppose probably... I'll take a 
		// In most cases I've seen, they do deserialization and return as System.Object, which needs to be casted manually. they quick look and see if anything comes up
		// That really doesn't sound particularly elegant.
		// Yeah lol I'm sure newer libraries would have a better solution
		// There is this? https://learn.microsoft.com/en-us/dotnet/api/system.data.sqlclient.sqlparameter?view=dotnet-plat-ext-7.0
		// Ah, right. It uses System.Object, but it has an enum property that indicates the type of the value. So I guess this is up to you on when you want to
		// throw an error in the case that there is an error with the type.
		// I suppose throwing an error would fit best when the parameter is created? If possible?
		// So, proactively throwing an error rather than waiting for the user to obtain the value? Sounds good
		// Ya, indeed. So question is: how do? Can we build on that built-in class, or just use that to the same effect?*/
	}
}
