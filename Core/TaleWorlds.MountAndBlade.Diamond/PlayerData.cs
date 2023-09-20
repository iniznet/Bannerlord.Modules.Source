using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.Diamond.MultiplayerBadges;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class PlayerData
	{
		public PlayerId PlayerId { get; set; }

		public PlayerId OwnerPlayerId { get; set; }

		public string Sigil { get; set; }

		public BodyProperties BodyProperties
		{
			get
			{
				return this._bodyProperties;
			}
			set
			{
				this.SetBodyProperties(value);
			}
		}

		private void SetBodyProperties(BodyProperties bodyProperties)
		{
			this._bodyProperties = bodyProperties.ClampForMultiplayer();
		}

		[JsonIgnore]
		public int ShownBadgeIndex
		{
			get
			{
				Badge byId = BadgeManager.GetById(this.ShownBadgeId);
				if (byId == null)
				{
					return -1;
				}
				return byId.Index;
			}
		}

		public PlayerStatsBase[] Stats { get; set; }

		public int Race { get; set; }

		public bool IsFemale { get; set; }

		[JsonIgnore]
		public int KillCount
		{
			get
			{
				int num = 0;
				if (this.Stats != null)
				{
					foreach (PlayerStatsBase playerStatsBase in this.Stats)
					{
						num += playerStatsBase.KillCount;
					}
				}
				return num;
			}
		}

		[JsonIgnore]
		public int DeathCount
		{
			get
			{
				int num = 0;
				if (this.Stats != null)
				{
					foreach (PlayerStatsBase playerStatsBase in this.Stats)
					{
						num += playerStatsBase.DeathCount;
					}
				}
				return num;
			}
		}

		[JsonIgnore]
		public int AssistCount
		{
			get
			{
				int num = 0;
				if (this.Stats != null)
				{
					foreach (PlayerStatsBase playerStatsBase in this.Stats)
					{
						num += playerStatsBase.AssistCount;
					}
				}
				return num;
			}
		}

		[JsonIgnore]
		public int WinCount
		{
			get
			{
				int num = 0;
				if (this.Stats != null)
				{
					foreach (PlayerStatsBase playerStatsBase in this.Stats)
					{
						num += playerStatsBase.WinCount;
					}
				}
				return num;
			}
		}

		[JsonIgnore]
		public int LoseCount
		{
			get
			{
				int num = 0;
				if (this.Stats != null)
				{
					foreach (PlayerStatsBase playerStatsBase in this.Stats)
					{
						num += playerStatsBase.LoseCount;
					}
				}
				return num;
			}
		}

		public int Experience { get; set; }

		public string LastPlayerName { get; set; }

		public string Username { get; set; }

		public int UserId { get; set; }

		public bool IsUsingClanSigil { get; set; }

		public string LastRegion { get; set; }

		public string[] LastGameTypes { get; set; }

		public DateTime? LastLogin { get; set; }

		public int Playtime { get; set; }

		public string ShownBadgeId { get; set; }

		public int Gold { get; set; }

		public bool IsMuted { get; set; }

		[JsonIgnore]
		public int Level
		{
			get
			{
				return new PlayerDataExperience(this.Experience).Level;
			}
		}

		[JsonIgnore]
		public int ExperienceToNextLevel
		{
			get
			{
				return new PlayerDataExperience(this.Experience).ExperienceToNextLevel;
			}
		}

		[JsonIgnore]
		public int ExperienceInCurrentLevel
		{
			get
			{
				return new PlayerDataExperience(this.Experience).ExperienceInCurrentLevel;
			}
		}

		public void FillWith(PlayerId playerId, PlayerId ownerPlayerId, BodyProperties bodyProperties, bool isFemale, string sigil, int experience, string lastPlayerName, string username, int userId, string lastRegion, string[] lastGameTypes, DateTime? lastLogin, int playtime, string shownBadgeId, int gold, PlayerStatsBase[] stats, bool shouldLog, bool isUsingClanSigil)
		{
			this.PlayerId = playerId;
			this.OwnerPlayerId = ownerPlayerId;
			this.BodyProperties = bodyProperties;
			this.IsFemale = isFemale;
			this.Sigil = sigil;
			this.IsUsingClanSigil = isUsingClanSigil;
			this.Experience = experience;
			this.LastPlayerName = lastPlayerName;
			this.Username = username;
			this.UserId = userId;
			this.LastRegion = lastRegion;
			this.LastGameTypes = lastGameTypes;
			this.LastLogin = lastLogin;
			this.Playtime = playtime;
			this.ShownBadgeId = shownBadgeId;
			this.Gold = gold;
			this.Stats = stats;
		}

		public void FillWithNewPlayer(PlayerId playerId, PlayerId ownerPlayerId, string[] gameTypes)
		{
			this.Stats = new PlayerStatsBase[0];
			this.PlayerId = playerId;
			this.OwnerPlayerId = ownerPlayerId;
			this.Sigil = "11.8.1.4345.4345.770.774.1.0.0.158.7.5.512.512.770.769.1.0.0";
			this.IsUsingClanSigil = false;
			this.LastGameTypes = gameTypes;
			this.Username = null;
			this.UserId = -1;
			this.Gold = 0;
			BodyProperties bodyProperties;
			if (BodyProperties.FromString("<BodyProperties version='4' age='36.35' weight='0.1025' build='0.7'  key='001C380CC000234B88E68BBA1372B7578B7BB5D788BC567878966669835754B604F926450F67798C000000000000000000000000000000000000000000DC10C4' />", out bodyProperties))
			{
				this.BodyProperties = bodyProperties;
			}
		}

		public bool HasGameStats(string gameType)
		{
			return this.GetGameStats(gameType) != null;
		}

		public PlayerStatsBase GetGameStats(string gameType)
		{
			if (this.Stats != null)
			{
				foreach (PlayerStatsBase playerStatsBase in this.Stats)
				{
					if (playerStatsBase.GameType == gameType)
					{
						return playerStatsBase;
					}
				}
			}
			return null;
		}

		public void UpdateGameStats(PlayerStatsBase playerGameTypeStats)
		{
			bool flag = false;
			if (this.Stats != null)
			{
				for (int i = 0; i < this.Stats.Length; i++)
				{
					if (this.Stats[i].GameType == playerGameTypeStats.GameType)
					{
						this.Stats[i] = playerGameTypeStats;
						flag = true;
					}
				}
			}
			if (!flag)
			{
				List<PlayerStatsBase> list = new List<PlayerStatsBase>();
				if (this.Stats != null)
				{
					list.AddRange(this.Stats);
				}
				list.Add(playerGameTypeStats);
				this.Stats = list.ToArray();
			}
		}

		private const string DefaultBodyProperties1 = "<BodyProperties version='4' age='36.35' weight='0.1025' build='0.7'  key='001C380CC000234B88E68BBA1372B7578B7BB5D788BC567878966669835754B604F926450F67798C000000000000000000000000000000000000000000DC10C4' />";

		private const string DefaultBodyProperties2 = "<BodyProperties version='4' age='46.35' weight='0.1025' build='0.7'  key='001C380CC000234B88E68BBA1372B7578B7BB5D788BC567878966669835754B604F926450F67798C000000000000000000000000000000000000000000DC10C4' />";

		public const string DefaultSigil = "11.8.1.4345.4345.770.774.1.0.0.158.7.5.512.512.770.769.1.0.0";

		private BodyProperties _bodyProperties;
	}
}
