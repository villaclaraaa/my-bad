using System.Text.Json.Serialization;

namespace Mybad.Services.OpenDota.ApiResponseModels;

internal class MatchWardLogInfo
{
	/// <summary>
	/// Gets or sets the unique Id of the match.
	/// </summary>
	[JsonPropertyName("match_id")]
	public long MatchId { get; set; }

	/// <summary>
	/// Gets or sets if the radiant won. If false then Dire won.
	/// </summary>
	[JsonPropertyName("radiant_win")]
	public bool IsRadiantWin { get; set; }

	/// <summary>
	/// Gets or sets duration of the match in seconds.
	/// </summary>
	[JsonPropertyName("duration")]
	public int Duration { get; set; }

	/// <summary>
	/// Gets or sets list of <see cref="Player"/> objects which is players in the match.
	/// </summary>
	[JsonPropertyName("players")]
	public List<MatchPlayer> Players { get; set; } = [];
}

internal class MatchPlayer
{
	/// <summary>
	/// Gets or sets account id of player.
	/// </summary>
	[JsonPropertyName("account_id")]
	public long AccountId { get; set; }

	/// <summary>
	/// Gets or sets which slot in the match the player is in.
	/// </summary>
	/// <remarks>
	/// 0-127 are Radiant, 128-255 are Dire
	/// </remarks>
	[JsonPropertyName("player_slot")]
	public int Slot { get; set; }

	/// <summary>
	/// Gets or sets id of hero played.
	/// </summary>
	/// <remarks>Id is taken from <see href="https://https://github.com/odota/dotaconstants/blob/master/build/heroes.json"/>.
	/// Also it can be converted to Hero name with <see cref="HeroesEnum"/> enum.
	/// </remarks>
	[JsonPropertyName("hero_id")]
	public int HeroId { get; set; }

	/// <summary>
	/// Gets or sets line the player plays. 
	/// <para>1 - bot, 2 - mid, 3 - top.</para>
	/// </summary>
	[JsonPropertyName("lane")]
	public int Lane { get; set; }

	/// <summary>
	/// Gets or sets pos of the player hero.
	/// <para>
	/// The actual position of the player should be calculated with <see cref="Lane"/> and <see cref="Slot"/> objects.
	/// <br/>  
	/// <list type="bullet">
	/// <item>Plays as Radiant:
	/// <description>
	/// <br/>Pos1 - Lane = 1, LanePos = 1.
	/// <br/>Pos2 - Lane = 2, LanePos = 2.
	/// <br/>Pos3 - Lane = 3, LanePos = 3.
	/// <br/>Pos4 - Lane = 3, LanePos = 3.
	/// <br/>Pos5 - Lane = 1, LanePos = 1.
	/// </description>
	/// </item>
	/// <item>Plays as Dire:
	/// <description>
	/// <br/>Pos1 - Lane = 3, LanePos = 1.
	/// <br/>Pos2 - Lane = 2, LanePos = 2.
	/// <br/>Pos3 - Lane = 1, LanePos = 3.
	/// <br/>Pos4 - Lane = 1, LanePos = 3.
	/// <br/>Pos5 - Lane = 3, LanePos = 1.
	/// </description>
	/// </item>
	/// </list>
	/// </para>
	/// </summary>
	/// <remarks>
	/// Two players will have 'lane_role' the same as we dont know for sure the actual position players plays.
	/// We can only assume based on hero_id and heroname.
	/// </remarks>
	[JsonPropertyName("lane_role")]
	public int LanePos { get; set; }

	/// <summary>
	/// Gets or sets name of player.
	/// </summary>
	[JsonPropertyName("personaname")]
	public string Name { get; set; } = null!;

	/// <summary>
	/// Gets or sets count of observers placed.
	/// </summary>
	[JsonPropertyName("obs_placed")]
	public int ObserversPlacedCount { get; set; }

	/// <summary>
	/// Gets or sets count of sentries placed.
	/// </summary>
	[JsonPropertyName("sen_placed")]
	public int SentriesPlacedCount { get; set; }

	/// <summary>
	/// Gets or sets list of observers placed.
	/// </summary>
	[JsonPropertyName("obs_log")]
	public List<WardLogEntry> ObsLog { get; set; } = [];

	/// <summary>
	/// Gets or sets list of sentries placed.
	/// </summary>
	[JsonPropertyName("sen_log")]
	public List<WardLogEntry> SenLog { get; set; } = [];

	/// <summary>
	///  Gets or sets list of when observers placed were destroyed.
	/// </summary>
	[JsonPropertyName("obs_left_log")]
	public List<WardLeftLogEntry> ObsLeftLog { get; set; } = [];

	/// <summary>
	/// Gets or sets list of when sentries placed were destroyed.
	/// </summary>
	[JsonPropertyName("sen_left_log")]
	public List<WardLeftLogEntry> SenLeftLog { get; set; } = [];

}

/// <summary>
/// Represents the obs_log/sen_log entry in the api response which is when the ward was PUT.
/// Are used for both Observer and Sentry wards.
/// </summary>
/// <remarks>
/// The objects looks like this in the json response:
/// <code>
/// {
///       "time": 1095, - time in seconds from horn (- if before) when the ward was put.
///       "type": "obs_log",
///       "slot": 1,
///       "x": 162.5,
///       "y": 156.1,
///       "z": 130,
///       "entityleft": false,
///       "ehandle": 2951573,
///       "key": "[163,156]",
///       "player_slot": 1
///     }
/// </code>
/// </remarks>
internal record class WardLogEntry
{
	/// <summary>
	/// Gets or sets time in seconds when the ward was put.
	/// <br/> Time is negative before horn, starting from 0 after the horn.
	/// </summary>
	/// <remarks>To find ward duration time this <see cref="WardLeftLogEntry.Time"/> should be subtracted by <see cref="WardLogEntry.Time"/>.</remarks>
	[JsonPropertyName("time")]
	public virtual int Time { get; set; }

	/// <summary>
	/// Gets or sets type of entry. 
	/// </summary>
	/// <remarks>It will be obs_log or obs_left_log. Same for sentry.</remarks>
	[JsonPropertyName("type")]
	public string Type { get; set; } = null!;

	/// <summary>
	/// Gets or sets player slot in the game. (0-127 for Radiant, 128-255 for Dire). 
	/// </summary>
	[JsonPropertyName("slot")]
	public int Slot { get; set; }

	/// <summary>
	/// Gets or sets X point on map where ward was placed.
	/// </summary>
	[JsonPropertyName("x")]
	public virtual double X { get; set; }

	/// <summary>
	/// Gets or sets Y point on map where ward was placed.
	/// </summary>
	[JsonPropertyName("y")]
	public virtual double Y { get; set; }

	/// <summary>
	/// Gets or sets Z point on map where ward was placed.
	/// </summary>
	/// <remarks>Is used probably because wards can be stacked on top of each other.</remarks>
	[JsonPropertyName("z")]
	public double Z { get; set; }

	/// <summary>
	/// Unknow behavior.
	/// </summary>
	[JsonPropertyName("entityleft")]
	public bool EntityLeft { get; set; }

	/// <summary>
	/// Gets or sets ehandle. It seems that it is identifier for ward entry. The same value is used in <see cref="WardLeftLogEntry"/>.
	/// </summary>
	[JsonPropertyName("ehandle")]
	public virtual long EHandle { get; set; }

	/// <summary>
	/// Gets or sets 2d point were ward was placed as '<c>[x,y]</c>' coordinates object. 
	/// Is Integers, rounded with default math rules.
	/// </summary>
	[JsonPropertyName("key")]
	public string Key { get; set; } = null!;

	/// <summary>
	/// Gets or sets player_slot. 
	/// </summary>
	/// <remarks>Probably the same as <see cref="Slot"/>.</remarks>
	[JsonPropertyName("player_slot")]
	public int PlayerSlot { get; set; }
}


/// <summary>
/// Represents the obs_left_log/sen_left_log entry in the api response which is when the ward was DESTROYED.
/// Are used for both Observer and Sentry wards.
/// </summary>
/// <remarks>
/// The objects looks like this in the json response:
/// <code>
/// {
///		"time": 674, - time when the ward was destroyed. It also includes the ward duration ends.
///		"type": "obs_left_log",
///		"slot": 1,
///		"attackername": "npc_dota_hero_oracle", - hero who destroyed ward. If the same then or denied or duration ended.
///		"x": 173.5, - the X point should match the X point from ward_log entry.
///		"y": 101.5, - the Y point should match the Y point from ward_log entry.
///		"z": 129,
///		"entityleft": true,
///		"ehandle": 10225969,
///		"key": "[174,102]",
///		"player_slot": 1 - player who put the ward.
///	},
/// </code>
/// </remarks>
internal record class WardLeftLogEntry : WardLogEntry
{
	/// <summary>
	/// Gets or sets hero who destroyed ward with string like '<c>npc_dota_hero_[heroname]</c>.
	/// </summary>
	/// <remarks>
	/// Ward putter hero will be put here if the ward was destroyed after duration ends.
	/// <br/> Should also be checked if the hero who destroyed the ward is ally.
	/// </remarks>
	[JsonPropertyName("attackername")]
	public string AttackerName { get; set; } = null!;

	/// <summary>
	/// Gets or sets time when the ward was destroyed.
	/// </summary>
	/// <remarks>To find ward duration time this <see cref="WardLeftLogEntry.Time"/> should be subtracted by <see cref="WardLogEntry.Time"/>.
	/// <br/> Also this will be empty if the ward did not expire when the match has ended. 
	/// <br/> For observers default duration - 6min, for sentries - 7min.
	/// </remarks>
	[JsonPropertyName("time")]
	public override int Time { get; set; }

	/// <summary>
	/// Gets or sets X point on map where ward was destroyed.
	/// </summary>
	/// <remarks>
	/// This or <see cref="WardLogEntry.Key"/> should be used to find the matches between wards to check if the ward was destroyed.
	/// </remarks>
	[JsonPropertyName("x")]
	public override double X { get; set; }

	/// <summary>
	/// Gets or sets Y point on map where ward was destroyed.
	/// </summary>
	/// <remarks>
	/// This or <see cref="WardLogEntry.EHandle"/> should be used to find the matches between wards to check if the ward was destroyed.
	/// </remarks>
	[JsonPropertyName("y")]
	public override double Y { get; set; }

	/// <summary>
	/// Gets or sets ehandle. It seems that it is identifier for ward entry. The same value is used in <see cref="WardLogEntry"/>.
	/// </summary>
	[JsonPropertyName("ehandle")]
	public override long EHandle { get; set; }
}
