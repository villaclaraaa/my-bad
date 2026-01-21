namespace Mybad.API.Services;

/// <summary>
/// Represents the configuration status for the hero matchup caching feature.
/// </summary>
public class HeroMatchupCacherStatus
{
	/// <summary>
	/// Gets or sets a value indicating whether the feature is enabled.
	/// </summary>
	/// <remarks>If the app environment is set to Development then changing this value will have no impact,
	/// as the Hosted Service <see cref="HeroMatchupCacherHostedService"/> is registered only in Prod. </remarks>
	public bool IsEnabled { get; set; } = true;
}
