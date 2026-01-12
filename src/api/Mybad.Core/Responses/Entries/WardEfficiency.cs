namespace Mybad.Core.Responses.Entries;

public class WardEfficiency : Ward
{
	public int AverageTimeLived { get; set; }

	public float EfficiencyScore { get; set; }

	public bool IsRadiantSide { get; set; }
}
