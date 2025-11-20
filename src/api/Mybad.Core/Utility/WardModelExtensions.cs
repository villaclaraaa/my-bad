using Mybad.Core.DomainModels;

namespace Mybad.Core.Utility;

public static class WardModelExtensions
{
	public static List<WardModel> GetApproximatedList(this List<WardModel> wards)
	{
		wards.Sort();
		var merged = new List<WardModel>(wards.Count);

		int i = 0, deviation1 = 2;
		while (i < wards.Count)
		{
			// Start with current element
			var current = wards[i];

			int j = i + 1;

			// Merge all consecutive items within deviation
			while (j < wards.Count &&
				   Math.Abs(current.PosX - wards[j].PosX) <= deviation1 &&
				   Math.Abs(current.PosY - wards[j].PosY) <= deviation1)
			{
				current.PosX = (current.PosX + wards[j].PosX) / 2;
				current.PosY = (current.PosY + wards[j].PosY) / 2;
				current.TimeLivedSeconds += wards[j].TimeLivedSeconds;
				current.Amount++;
				current.WasDestroyed = current.WasDestroyed || wards[j].WasDestroyed;
				j++;
			}

			merged.Add(current);

			// Skip over the merged items
			i = j;
		}

		wards = merged;
		return wards;
	}
}
