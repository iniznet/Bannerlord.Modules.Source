using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromBattleServerManager.ToBattleServer
{
	[MessageDescription("BattleServerManager", "BattleServer")]
	[Serializable]
	public class StartBattleMessage : Message
	{
		public string SceneName { get; private set; }

		public string GameType { get; private set; }

		public Guid BattleId { get; private set; }

		public string Faction1 { get; private set; }

		public string Faction2 { get; private set; }

		public int MinRequiredPlayerCountToStartBattle { get; private set; }

		public int BattleSize { get; private set; }

		public int RoundThreshold { get; private set; }

		public float MoraleThreshold { get; private set; }

		public bool UseAnalytics { get; private set; }

		public bool CaptureMovementData { get; private set; }

		public string AnalyticsServiceAddress { get; private set; }

		public int MaxFriendlyKillCount { get; private set; }

		public float MaxFriendlyDamage { get; private set; }

		public float MaxFriendlyDamagePerSingleRound { get; private set; }

		public float RoundFriendlyDamageLimit { get; private set; }

		public int MaxRoundsOverLimitCount { get; private set; }

		public bool IsPremadeGame { get; private set; }

		public string[] ProfanityList { get; private set; }

		public PremadeGameType PremadeGameType { get; private set; }

		public string[] AllowList { get; private set; }

		public PlayerId[] AssignedPlayers { get; private set; }

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
