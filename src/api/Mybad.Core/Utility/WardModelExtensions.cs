using Mybad.Core.DomainModels;

namespace Mybad.Core.Utility;

public static class WardModelExtensions
{
	public static List<WardModel> GetApproximatedList(this List<WardModel> wards)
	{
		int deviation = 4;
		wards.Sort();
		var cellSize = deviation;
		var grid = new Dictionary<(int, int), List<WardModel>>();
		var result = new List<WardModel>();

		foreach (var item in wards)
		{
			int cellX = item.PosX / cellSize;
			int cellY = item.PosY / cellSize;

			WardModel? mergedInto = null;

			// check this cell and neighbors
			for (int dx = -1; dx <= 1 && mergedInto == null; dx++)
			{
				for (int dy = -1; dy <= 1 && mergedInto == null; dy++)
				{
					var key = (cellX + dx, cellY + dy);

					if (!grid.TryGetValue(key, out var bucket))
					{
						continue;
					}

					foreach (var existing in bucket)
					{
						if (Math.Abs(existing.PosX - item.PosX) <= deviation &&
							Math.Abs(existing.PosY - item.PosY) <= deviation)
						{
							int oldAmount = existing.Amount;
							int newAmount = oldAmount + item.Amount;

							// weighted average
							existing.PosX = (existing.PosX * oldAmount + item.PosX * item.Amount) / newAmount;
							existing.PosY = (existing.PosY * oldAmount + item.PosY * item.Amount) / newAmount;

							existing.Amount = newAmount;
							existing.TimeLivedSeconds += item.TimeLivedSeconds;
							existing.WasDestroyed |= item.WasDestroyed;

							mergedInto = existing;

							break;
						}
					}
				}
			}

			if (mergedInto == null)
			{
				// new cluster
				result.Add(item);

				var key = (cellX, cellY);
				if (!grid.TryGetValue(key, out var bucket))
				{
					bucket = [];
					grid[key] = bucket;
				}

				bucket.Add(item);
			}
		}

		return result;
	}
}
