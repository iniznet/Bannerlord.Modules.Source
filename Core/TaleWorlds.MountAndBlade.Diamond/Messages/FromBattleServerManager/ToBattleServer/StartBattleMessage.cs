using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromBattleServerManager.ToBattleServer
{
	[MessageDescription("BattleServerManager", "BattleServer")]
	[Serializable]
	public class StartBattleMessage : Message
	{
		[JsonProperty]
		public string SceneName { get; private set; }

		[JsonProperty]
		public string GameType { get; private set; }

		[JsonProperty]
		public Guid BattleId { get; private set; }

		[JsonProperty]
		public string Faction1 { get; private set; }

		[JsonProperty]
		public string Faction2 { get; private set; }

		[JsonProperty]
		public int MinRequiredPlayerCountToStartBattle { get; private set; }

		[JsonProperty]
		public int BattleSize { get; private set; }

		[JsonProperty]
		public int RoundThreshold { get; private set; }

		[JsonProperty]
		public float MoraleThreshold { get; private set; }

		[JsonProperty]
		public bool UseAnalytics { get; private set; }

		[JsonProperty]
		public bool CaptureMovementData { get; private set; }

		[JsonProperty]
		public string AnalyticsServiceAddress { get; private set; }

		[JsonProperty]
		public int MaxFriendlyKillCount { get; private set; }

		[JsonProperty]
		public float MaxFriendlyDamage { get; private set; }

		[JsonProperty]
		public float MaxFriendlyDamagePerSingleRound { get; private set; }

		[JsonProperty]
		public float RoundFriendlyDamageLimit { get; private set; }

		[JsonProperty]
		public int MaxRoundsOverLimitCount { get; private set; }

		[JsonProperty]
		public bool IsPremadeGame { get; private set; }

		[JsonProperty]
		public string[] ProfanityList { get; private set; }

		[JsonProperty]
		public PremadeGameType PremadeGameType { get; private set; }

		[JsonProperty]
		public string[] AllowList { get; private set; }

		[JsonProperty]
		public PlayerId[] AssignedPlayers { get; private set; }

		public StartBattleMessage()
		{
		}

		public StartBattleMessage(Guid battleId, string sceneName, string gameType, string faction1, string faction2, int minRequiredPlayerCountToStartBattle, int battleSize, int roundThreshold, float moraleThreshold, bool useAnalytics, bool captureMovementData, string analyticsServiceAddress, int maxFriendlyKillCount, float maxFriendlyDamage, float maxFriendlyDamagePerSingleRound, float roundFriendlyDamageLimit, int maxRoundsOverLimitCount, bool isPremadeGame, PremadeGameType premadeGameType, string[] profanityList, string[] allowList, PlayerId[] assignedPlayers)
		{
			this.SceneName = sceneName;
			this.GameType = gameType;
			this.BattleId = battleId;
			this.Faction1 = faction1;
			this.Faction2 = faction2;
			this.MinRequiredPlayerCountToStartBattle = minRequiredPlayerCountToStartBattle;
			this.BattleSize = battleSize;
			this.UseAnalytics = useAnalytics;
			this.CaptureMovementData = captureMovementData;
			this.AnalyticsServiceAddress = analyticsServiceAddress;
			this.RoundThreshold = roundThreshold;
			this.MoraleThreshold = moraleThreshold;
			this.MaxFriendlyKillCount = maxFriendlyKillCount;
			this.MaxFriendlyDamage = maxFriendlyDamage;
			this.MaxFriendlyDamagePerSingleRound = maxFriendlyDamagePerSingleRound;
			this.RoundFriendlyDamageLimit = roundFriendlyDamageLimit;
			this.MaxRoundsOverLimitCount = maxRoundsOverLimitCount;
			this.IsPremadeGame = isPremadeGame;
			this.PremadeGameType = premadeGameType;
			this.ProfanityList = profanityList;
			this.AllowList = allowList;
			this.AssignedPlayers = assignedPlayers;
		}
	}
}
