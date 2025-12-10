namespace Mybad.API.Services;

/// <summary>
/// Represents the configuration status for the hero matchup caching feature.
/// </summary>
public class HeroMatchupCacherStatus
{
	/// <summary>
	/// Gets or sets a value indicating whether the feature is enabled.
	/// </summary>
	public bool IsEnabled { get; set; } = true;
}
