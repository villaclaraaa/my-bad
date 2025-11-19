namespace Mybad.Core.Responses.Entries;

public class WardLog : Ward, IComparable<WardLog>
{
	public int TimeLived { get; set; }
	public bool WasDestroyed { get; set; }

	public int CompareTo(WardLog? other)
	{
		int r = X.CompareTo(other!.X);
		if (r != 0)
		{
			return r;
		}

		return Y.CompareTo(other!.Y);
	}
}
