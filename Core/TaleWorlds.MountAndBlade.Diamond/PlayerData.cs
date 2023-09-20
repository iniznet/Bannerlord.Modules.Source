using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.Diamond.MultiplayerBadges;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x0200013B RID: 315
	[Serializable]
	public class PlayerData
	{
		// Token: 0x17000293 RID: 659
		// (get) Token: 0x06000782 RID: 1922 RVA: 0x0000C316 File Offset: 0x0000A516
		// (set) Token: 0x06000783 RID: 1923 RVA: 0x0000C31E File Offset: 0x0000A51E
		public PlayerId PlayerId { get; private set; }

		// Token: 0x17000294 RID: 660
		// (get) Token: 0x06000784 RID: 1924 RVA: 0x0000C327 File Offset: 0x0000A527
		// (set) Token: 0x06000785 RID: 1925 RVA: 0x0000C32F File Offset: 0x0000A52F
		public PlayerId OwnerPlayerId { get; private set; }

		// Token: 0x17000295 RID: 661
		// (get) Token: 0x06000786 RID: 1926 RVA: 0x0000C338 File Offset: 0x0000A538
		// (set) Token: 0x06000787 RID: 1927 RVA: 0x0000C340 File Offset: 0x0000A540
		public string Sigil { get; set; }

		// Token: 0x17000296 RID: 662
		// (get) Token: 0x06000788 RID: 1928 RVA: 0x0000C349 File Offset: 0x0000A549
		// (set) Token: 0x06000789 RID: 1929 RVA: 0x0000C351 File Offset: 0x0000A551
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

		// Token: 0x0600078A RID: 1930 RVA: 0x0000C35A File Offset: 0x0000A55A
		private void SetBodyProperties(BodyProperties bodyProperties)
		{
			this._bodyProperties = bodyProperties.ClampForMultiplayer();
		}

		// Token: 0x17000297 RID: 663
		// (get) Token: 0x0600078B RID: 1931 RVA: 0x0000C36C File Offset: 0x0000A56C
		public string BodyPropertiesAsString
		{
			get
			{
				return this.BodyProperties.ToString();
			}
		}

		// Token: 0x17000298 RID: 664
		// (get) Token: 0x0600078C RID: 1932 RVA: 0x0000C38D File Offset: 0x0000A58D
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

		// Token: 0x17000299 RID: 665
		// (get) Token: 0x0600078D RID: 1933 RVA: 0x0000C3A5 File Offset: 0x0000A5A5
		// (set) Token: 0x0600078E RID: 1934 RVA: 0x0000C3AD File Offset: 0x0000A5AD
		public PlayerStatsBase[] Stats { get; set; }

		// Token: 0x1700029A RID: 666
		// (get) Token: 0x0600078F RID: 1935 RVA: 0x0000C3B6 File Offset: 0x0000A5B6
		// (set) Token: 0x06000790 RID: 1936 RVA: 0x0000C3BE File Offset: 0x0000A5BE
		public int Race { get; set; }

		// Token: 0x1700029B RID: 667
		// (get) Token: 0x06000791 RID: 1937 RVA: 0x0000C3C7 File Offset: 0x0000A5C7
		// (set) Token: 0x06000792 RID: 1938 RVA: 0x0000C3CF File Offset: 0x0000A5CF
		public bool IsFemale { get; set; }

		// Token: 0x1700029C RID: 668
		// (get) Token: 0x06000793 RID: 1939 RVA: 0x0000C3D8 File Offset: 0x0000A5D8
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

		// Token: 0x1700029D RID: 669
		// (get) Token: 0x06000794 RID: 1940 RVA: 0x0000C414 File Offset: 0x0000A614
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

		// Token: 0x1700029E RID: 670
		// (get) Token: 0x06000795 RID: 1941 RVA: 0x0000C450 File Offset: 0x0000A650
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

		// Token: 0x1700029F RID: 671
		// (get) Token: 0x06000796 RID: 1942 RVA: 0x0000C48C File Offset: 0x0000A68C
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

		// Token: 0x170002A0 RID: 672
		// (get) Token: 0x06000797 RID: 1943 RVA: 0x0000C4C8 File Offset: 0x0000A6C8
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

		// Token: 0x170002A1 RID: 673
		// (get) Token: 0x06000798 RID: 1944 RVA: 0x0000C502 File Offset: 0x0000A702
		// (set) Token: 0x06000799 RID: 1945 RVA: 0x0000C50A File Offset: 0x0000A70A
		public int Experience { get; set; }

		// Token: 0x170002A2 RID: 674
		// (get) Token: 0x0600079A RID: 1946 RVA: 0x0000C513 File Offset: 0x0000A713
		// (set) Token: 0x0600079B RID: 1947 RVA: 0x0000C51B File Offset: 0x0000A71B
		public string LastPlayerName { get; set; }

		// Token: 0x170002A3 RID: 675
		// (get) Token: 0x0600079C RID: 1948 RVA: 0x0000C524 File Offset: 0x0000A724
		// (set) Token: 0x0600079D RID: 1949 RVA: 0x0000C52C File Offset: 0x0000A72C
		public string Username { get; set; }

		// Token: 0x170002A4 RID: 676
		// (get) Token: 0x0600079E RID: 1950 RVA: 0x0000C535 File Offset: 0x0000A735
		// (set) Token: 0x0600079F RID: 1951 RVA: 0x0000C53D File Offset: 0x0000A73D
		public int UserId { get; set; }

		// Token: 0x170002A5 RID: 677
		// (get) Token: 0x060007A0 RID: 1952 RVA: 0x0000C546 File Offset: 0x0000A746
		// (set) Token: 0x060007A1 RID: 1953 RVA: 0x0000C54E File Offset: 0x0000A74E
		public bool IsUsingClanSigil { get; set; }

		// Token: 0x170002A6 RID: 678
		// (get) Token: 0x060007A2 RID: 1954 RVA: 0x0000C557 File Offset: 0x0000A757
		// (set) Token: 0x060007A3 RID: 1955 RVA: 0x0000C55F File Offset: 0x0000A75F
		public string LastRegion { get; set; }

		// Token: 0x170002A7 RID: 679
		// (get) Token: 0x060007A4 RID: 1956 RVA: 0x0000C568 File Offset: 0x0000A768
		// (set) Token: 0x060007A5 RID: 1957 RVA: 0x0000C570 File Offset: 0x0000A770
		public string[] LastGameTypes { get; set; }

		// Token: 0x170002A8 RID: 680
		// (get) Token: 0x060007A6 RID: 1958 RVA: 0x0000C579 File Offset: 0x0000A779
		// (set) Token: 0x060007A7 RID: 1959 RVA: 0x0000C581 File Offset: 0x0000A781
		public DateTime? LastLogin { get; set; }

		// Token: 0x170002A9 RID: 681
		// (get) Token: 0x060007A8 RID: 1960 RVA: 0x0000C58A File Offset: 0x0000A78A
		// (set) Token: 0x060007A9 RID: 1961 RVA: 0x0000C592 File Offset: 0x0000A792
		public int Playtime { get; set; }

		// Token: 0x170002AA RID: 682
		// (get) Token: 0x060007AA RID: 1962 RVA: 0x0000C59B File Offset: 0x0000A79B
		// (set) Token: 0x060007AB RID: 1963 RVA: 0x0000C5A3 File Offset: 0x0000A7A3
		public string ShownBadgeId { get; set; }

		// Token: 0x170002AB RID: 683
		// (get) Token: 0x060007AC RID: 1964 RVA: 0x0000C5AC File Offset: 0x0000A7AC
		// (set) Token: 0x060007AD RID: 1965 RVA: 0x0000C5B4 File Offset: 0x0000A7B4
		public int Gold { get; set; }

		// Token: 0x170002AC RID: 684
		// (get) Token: 0x060007AE RID: 1966 RVA: 0x0000C5BD File Offset: 0x0000A7BD
		// (set) Token: 0x060007AF RID: 1967 RVA: 0x0000C5C5 File Offset: 0x0000A7C5
		public bool IsMuted { get; set; }

		// Token: 0x170002AD RID: 685
		// (get) Token: 0x060007B0 RID: 1968 RVA: 0x0000C5D0 File Offset: 0x0000A7D0
		public int Level
		{
			get
			{
				return new PlayerDataExperience(this.Experience).Level;
			}
		}

		// Token: 0x170002AE RID: 686
		// (get) Token: 0x060007B1 RID: 1969 RVA: 0x0000C5F0 File Offset: 0x0000A7F0
		public int ExperienceToNextLevel
		{
			get
			{
				return new PlayerDataExperience(this.Experience).ExperienceToNextLevel;
			}
		}

		// Token: 0x170002AF RID: 687
		// (get) Token: 0x060007B2 RID: 1970 RVA: 0x0000C610 File Offset: 0x0000A810
		public int ExperienceInCurrentLevel
		{
			get
			{
				return new PlayerDataExperience(this.Experience).ExperienceInCurrentLevel;
			}
		}

		// Token: 0x060007B4 RID: 1972 RVA: 0x0000C638 File Offset: 0x0000A838
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

		// Token: 0x060007B5 RID: 1973 RVA: 0x0000C6CC File Offset: 0x0000A8CC
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

		// Token: 0x060007B6 RID: 1974 RVA: 0x0000C736 File Offset: 0x0000A936
		public bool HasGameStats(string gameType)
		{
			return this.GetGameStats(gameType) != null;
		}

		// Token: 0x060007B7 RID: 1975 RVA: 0x0000C744 File Offset: 0x0000A944
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

		// Token: 0x060007B8 RID: 1976 RVA: 0x0000C784 File Offset: 0x0000A984
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

		// Token: 0x04000374 RID: 884
		private const string DefaultBodyProperties1 = "<BodyProperties version='4' age='36.35' weight='0.1025' build='0.7'  key='001C380CC000234B88E68BBA1372B7578B7BB5D788BC567878966669835754B604F926450F67798C000000000000000000000000000000000000000000DC10C4' />";

		// Token: 0x04000375 RID: 885
		private const string DefaultBodyProperties2 = "<BodyProperties version='4' age='46.35' weight='0.1025' build='0.7'  key='001C380CC000234B88E68BBA1372B7578B7BB5D788BC567878966669835754B604F926450F67798C000000000000000000000000000000000000000000DC10C4' />";

		// Token: 0x04000376 RID: 886
		public const string DefaultSigil = "11.8.1.4345.4345.770.774.1.0.0.158.7.5.512.512.770.769.1.0.0";

		// Token: 0x0400037A RID: 890
		private BodyProperties _bodyProperties;
	}
}
