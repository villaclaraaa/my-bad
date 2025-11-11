namespace Test.ConsoleUI;

using System.Collections.Generic;
using System.Text.Json.Serialization;

public class MatchData
{
	[JsonPropertyName("match_id")]
	public long MatchId { get; set; }

	[JsonPropertyName("barracks_status_dire")]
	public int BarracksStatusDire { get; set; }

	[JsonPropertyName("barracks_status_radiant")]
	public int BarracksStatusRadiant { get; set; }

	[JsonPropertyName("chat")]
	public List<ChatMessage> Chat { get; set; }

	[JsonPropertyName("cluster")]
	public int Cluster { get; set; }

	[JsonPropertyName("cosmetics")]
	public Dictionary<string, int> Cosmetics { get; set; }

	[JsonPropertyName("dire_score")]
	public int DireScore { get; set; }

	[JsonPropertyName("draft_timings")]
	public List<DraftTiming> DraftTimings { get; set; }

	[JsonPropertyName("duration")]
	public int Duration { get; set; }

	[JsonPropertyName("engine")]
	public int Engine { get; set; }

	[JsonPropertyName("first_blood_time")]
	public int FirstBloodTime { get; set; }

	[JsonPropertyName("game_mode")]
	public int GameMode { get; set; }

	[JsonPropertyName("human_players")]
	public int HumanPlayers { get; set; }

	[JsonPropertyName("leagueid")]
	public int LeagueId { get; set; }

	[JsonPropertyName("lobby_type")]
	public int LobbyType { get; set; }

	[JsonPropertyName("players")]
	public List<Player> Players { get; set; }

	[JsonPropertyName("radiant_win")]
	public bool RadiantWin { get; set; }

	[JsonPropertyName("radiant_score")]
	public int RadiantScore { get; set; }

	[JsonPropertyName("dire_team")]
	public Team DireTeam { get; set; }

	[JsonPropertyName("radiant_team")]
	public Team RadiantTeam { get; set; }

	[JsonPropertyName("league")]
	public League League { get; set; }

	[JsonPropertyName("replay_url")]
	public string ReplayUrl { get; set; }

	[JsonPropertyName("pauses")]
	public List<PauseInfo> Pauses { get; set; }
}

public class ChatMessage
{
	[JsonPropertyName("time")]
	public int Time { get; set; }

	[JsonPropertyName("unit")]
	public string Unit { get; set; }

	[JsonPropertyName("key")]
	public string Key { get; set; }

	[JsonPropertyName("slot")]
	public int Slot { get; set; }

	[JsonPropertyName("player_slot")]
	public int PlayerSlot { get; set; }
}

public class DraftTiming
{
	[JsonPropertyName("order")]
	public int Order { get; set; }

	[JsonPropertyName("pick")]
	public bool Pick { get; set; }

	[JsonPropertyName("active_team")]
	public int ActiveTeam { get; set; }

	[JsonPropertyName("hero_id")]
	public int HeroId { get; set; }

	[JsonPropertyName("player_slot")]
	public int PlayerSlot { get; set; }

	[JsonPropertyName("extra_time")]
	public int ExtraTime { get; set; }

	[JsonPropertyName("total_time_taken")]
	public int TotalTimeTaken { get; set; }
}

public class PauseInfo
{
	[JsonPropertyName("time")]
	public int Time { get; set; }

	[JsonPropertyName("duration")]
	public int Duration { get; set; }
}

public class Team
{
	// placeholder; JSON shows empty object {}
}

public class League
{
	// placeholder; JSON shows empty object {}
}

public class Player
{
	[JsonPropertyName("match_id")]
	public long MatchId { get; set; }

	[JsonPropertyName("player_slot")]
	public int PlayerSlot { get; set; }

	[JsonPropertyName("personaname")]
	public string Personaname { get; set; }

	[JsonPropertyName("hero_id")]
	public int HeroId { get; set; }

	[JsonPropertyName("kills")]
	public int Kills { get; set; }

	[JsonPropertyName("deaths")]
	public int Deaths { get; set; }

	[JsonPropertyName("assists")]
	public int Assists { get; set; }

	[JsonPropertyName("isRadiant")]
	public bool IsRadiant { get; set; }

	[JsonPropertyName("win")]
	public int Win { get; set; }

	[JsonPropertyName("lose")]
	public int Lose { get; set; }

	[JsonPropertyName("xp_per_min")]
	public int XpPerMin { get; set; }

	[JsonPropertyName("gold_per_min")]
	public int GoldPerMin { get; set; }

	[JsonPropertyName("hero_damage")]
	public int HeroDamage { get; set; }

	[JsonPropertyName("hero_healing")]
	public int HeroHealing { get; set; }

	[JsonPropertyName("tower_damage")]
	public int TowerDamage { get; set; }

	[JsonPropertyName("benchmarks")]
	public Dictionary<string, object> Benchmarks { get; set; }

	[JsonPropertyName("cosmetics")]
	public List<CosmeticItem> Cosmetics { get; set; }

	[JsonPropertyName("kills_log")]
	public List<KillLog> KillsLog { get; set; }

	[JsonPropertyName("buyback_log")]
	public List<BuybackLog> BuybackLog { get; set; }
}

public class CosmeticItem
{
	[JsonPropertyName("item_id")]
	public int ItemId { get; set; }

	[JsonPropertyName("name")]
	public string Name { get; set; }

	[JsonPropertyName("item_description")]
	public string ItemDescription { get; set; }

	[JsonPropertyName("item_name")]
	public string ItemName { get; set; }

	[JsonPropertyName("item_rarity")]
	public string ItemRarity { get; set; }

	[JsonPropertyName("item_type_name")]
	public string ItemTypeName { get; set; }

	[JsonPropertyName("used_by_heroes")]
	public string UsedByHeroes { get; set; }
}

public class KillLog
{
	[JsonPropertyName("time")]
	public int Time { get; set; }

	[JsonPropertyName("key")]
	public string Key { get; set; }
}

public class BuybackLog
{
	[JsonPropertyName("time")]
	public int Time { get; set; }

	[JsonPropertyName("slot")]
	public int Slot { get; set; }

	[JsonPropertyName("player_slot")]
	public int PlayerSlot { get; set; }
}


public class ObsLeftLogEntry
{
	[JsonPropertyName("time")]
	public int Time { get; set; }

	[JsonPropertyName("type")]
	public string Type { get; set; }

	[JsonPropertyName("slot")]
	public int Slot { get; set; }

	[JsonPropertyName("attackername")]
	public string AttackerName { get; set; }

	[JsonPropertyName("x")]
	public double X { get; set; }

	[JsonPropertyName("y")]
	public double Y { get; set; }

	[JsonPropertyName("z")]
	public double Z { get; set; }

	[JsonPropertyName("entityleft")]
	public bool EntityLeft { get; set; }

	[JsonPropertyName("ehandle")]
	public long EHandle { get; set; }

	[JsonPropertyName("key")]
	public string Key { get; set; }

	[JsonPropertyName("player_slot")]
	public int PlayerSlot { get; set; }
}

public class MatchInfo
{
	[JsonPropertyName("players")]
	public List<Player1> Players { get; set; }
}

public class Player1
{
	[JsonPropertyName("player_slot")]
	public int Slot { get; set; }

	[JsonPropertyName("obs_left_log")]
	public List<ObsLeftLogEntry> ObsLeftLogs { get; set; } = new List<ObsLeftLogEntry>();
}

