using System;
using System.Collections.Generic;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x02000053 RID: 83
	public class CampaignOptions
	{
		// Token: 0x170001A8 RID: 424
		// (get) Token: 0x060007E2 RID: 2018 RVA: 0x0002192F File Offset: 0x0001FB2F
		private static CampaignOptions _current
		{
			get
			{
				Campaign campaign = Campaign.Current;
				if (campaign == null)
				{
					return null;
				}
				return campaign.Options;
			}
		}

		// Token: 0x170001A9 RID: 425
		// (get) Token: 0x060007E3 RID: 2019 RVA: 0x00021941 File Offset: 0x0001FB41
		// (set) Token: 0x060007E4 RID: 2020 RVA: 0x00021953 File Offset: 0x0001FB53
		public static bool IsLifeDeathCycleDisabled
		{
			get
			{
				CampaignOptions current = CampaignOptions._current;
				return current != null && current._isLifeDeathCycleDisabled;
			}
			set
			{
				if (CampaignOptions._current != null)
				{
					CampaignOptions._current._isLifeDeathCycleDisabled = value;
				}
			}
		}

		// Token: 0x170001AA RID: 426
		// (get) Token: 0x060007E5 RID: 2021 RVA: 0x00021967 File Offset: 0x0001FB67
		// (set) Token: 0x060007E6 RID: 2022 RVA: 0x00021979 File Offset: 0x0001FB79
		public static bool AutoAllocateClanMemberPerks
		{
			get
			{
				CampaignOptions current = CampaignOptions._current;
				return current != null && current._autoAllocateClanMemberPerks;
			}
			set
			{
				if (CampaignOptions._current != null)
				{
					CampaignOptions._current._autoAllocateClanMemberPerks = value;
				}
			}
		}

		// Token: 0x170001AB RID: 427
		// (get) Token: 0x060007E7 RID: 2023 RVA: 0x0002198D File Offset: 0x0001FB8D
		// (set) Token: 0x060007E8 RID: 2024 RVA: 0x0002199F File Offset: 0x0001FB9F
		public static bool IsIronmanMode
		{
			get
			{
				CampaignOptions current = CampaignOptions._current;
				return current != null && current._isIronmanMode;
			}
			set
			{
				if (CampaignOptions._current != null)
				{
					CampaignOptions._current._isIronmanMode = value;
				}
			}
		}

		// Token: 0x170001AC RID: 428
		// (get) Token: 0x060007E9 RID: 2025 RVA: 0x000219B3 File Offset: 0x0001FBB3
		// (set) Token: 0x060007EA RID: 2026 RVA: 0x000219C5 File Offset: 0x0001FBC5
		public static CampaignOptions.Difficulty PlayerTroopsReceivedDamage
		{
			get
			{
				CampaignOptions current = CampaignOptions._current;
				if (current == null)
				{
					return CampaignOptions.Difficulty.Realistic;
				}
				return current._playerTroopsReceivedDamage;
			}
			set
			{
				if (CampaignOptions._current != null)
				{
					CampaignOptions._current._playerTroopsReceivedDamage = value;
				}
			}
		}

		// Token: 0x170001AD RID: 429
		// (get) Token: 0x060007EB RID: 2027 RVA: 0x000219D9 File Offset: 0x0001FBD9
		// (set) Token: 0x060007EC RID: 2028 RVA: 0x000219EB File Offset: 0x0001FBEB
		public static CampaignOptions.Difficulty PlayerReceivedDamage
		{
			get
			{
				CampaignOptions current = CampaignOptions._current;
				if (current == null)
				{
					return CampaignOptions.Difficulty.Realistic;
				}
				return current._playerReceivedDamage;
			}
			set
			{
				if (CampaignOptions._current != null)
				{
					CampaignOptions._current._playerReceivedDamage = value;
				}
			}
		}

		// Token: 0x170001AE RID: 430
		// (get) Token: 0x060007ED RID: 2029 RVA: 0x000219FF File Offset: 0x0001FBFF
		// (set) Token: 0x060007EE RID: 2030 RVA: 0x00021A11 File Offset: 0x0001FC11
		public static CampaignOptions.Difficulty RecruitmentDifficulty
		{
			get
			{
				CampaignOptions current = CampaignOptions._current;
				if (current == null)
				{
					return CampaignOptions.Difficulty.Realistic;
				}
				return current._recruitmentDifficulty;
			}
			set
			{
				if (CampaignOptions._current != null)
				{
					CampaignOptions._current._recruitmentDifficulty = value;
				}
			}
		}

		// Token: 0x170001AF RID: 431
		// (get) Token: 0x060007EF RID: 2031 RVA: 0x00021A25 File Offset: 0x0001FC25
		// (set) Token: 0x060007F0 RID: 2032 RVA: 0x00021A37 File Offset: 0x0001FC37
		public static CampaignOptions.Difficulty PlayerMapMovementSpeed
		{
			get
			{
				CampaignOptions current = CampaignOptions._current;
				if (current == null)
				{
					return CampaignOptions.Difficulty.Realistic;
				}
				return current._playerMapMovementSpeed;
			}
			set
			{
				if (CampaignOptions._current != null)
				{
					CampaignOptions._current._playerMapMovementSpeed = value;
				}
			}
		}

		// Token: 0x170001B0 RID: 432
		// (get) Token: 0x060007F1 RID: 2033 RVA: 0x00021A4B File Offset: 0x0001FC4B
		// (set) Token: 0x060007F2 RID: 2034 RVA: 0x00021A5D File Offset: 0x0001FC5D
		public static CampaignOptions.Difficulty CombatAIDifficulty
		{
			get
			{
				CampaignOptions current = CampaignOptions._current;
				if (current == null)
				{
					return CampaignOptions.Difficulty.Realistic;
				}
				return current._combatAIDifficulty;
			}
			set
			{
				if (CampaignOptions._current != null)
				{
					CampaignOptions._current._combatAIDifficulty = value;
				}
			}
		}

		// Token: 0x170001B1 RID: 433
		// (get) Token: 0x060007F3 RID: 2035 RVA: 0x00021A71 File Offset: 0x0001FC71
		// (set) Token: 0x060007F4 RID: 2036 RVA: 0x00021A83 File Offset: 0x0001FC83
		public static CampaignOptions.Difficulty PersuasionSuccessChance
		{
			get
			{
				CampaignOptions current = CampaignOptions._current;
				if (current == null)
				{
					return CampaignOptions.Difficulty.Realistic;
				}
				return current._persuasionSuccessChance;
			}
			set
			{
				if (CampaignOptions._current != null)
				{
					CampaignOptions._current._persuasionSuccessChance = value;
				}
			}
		}

		// Token: 0x170001B2 RID: 434
		// (get) Token: 0x060007F5 RID: 2037 RVA: 0x00021A97 File Offset: 0x0001FC97
		// (set) Token: 0x060007F6 RID: 2038 RVA: 0x00021AA9 File Offset: 0x0001FCA9
		public static CampaignOptions.Difficulty ClanMemberDeathChance
		{
			get
			{
				CampaignOptions current = CampaignOptions._current;
				if (current == null)
				{
					return CampaignOptions.Difficulty.Realistic;
				}
				return current._clanMemberDeathChance;
			}
			set
			{
				if (CampaignOptions._current != null)
				{
					CampaignOptions._current._clanMemberDeathChance = value;
				}
			}
		}

		// Token: 0x170001B3 RID: 435
		// (get) Token: 0x060007F7 RID: 2039 RVA: 0x00021ABD File Offset: 0x0001FCBD
		// (set) Token: 0x060007F8 RID: 2040 RVA: 0x00021ACF File Offset: 0x0001FCCF
		public static CampaignOptions.Difficulty BattleDeath
		{
			get
			{
				CampaignOptions current = CampaignOptions._current;
				if (current == null)
				{
					return CampaignOptions.Difficulty.Realistic;
				}
				return current._battleDeath;
			}
			set
			{
				if (CampaignOptions._current != null)
				{
					CampaignOptions._current._battleDeath = value;
				}
			}
		}

		// Token: 0x170001B4 RID: 436
		// (get) Token: 0x060007F9 RID: 2041 RVA: 0x00021AE3 File Offset: 0x0001FCE3
		// (set) Token: 0x060007FA RID: 2042 RVA: 0x00021AF5 File Offset: 0x0001FCF5
		public static bool IsSiegeTestBuild
		{
			get
			{
				CampaignOptions current = CampaignOptions._current;
				return current != null && current._isSiegeTestBuild;
			}
			set
			{
				if (CampaignOptions._current != null)
				{
					CampaignOptions._current._isSiegeTestBuild = value;
				}
			}
		}

		// Token: 0x060007FB RID: 2043 RVA: 0x00021B0C File Offset: 0x0001FD0C
		public CampaignOptions()
		{
			this._playerTroopsReceivedDamage = CampaignOptions.Difficulty.VeryEasy;
			this._playerReceivedDamage = CampaignOptions.Difficulty.VeryEasy;
			this._recruitmentDifficulty = CampaignOptions.Difficulty.VeryEasy;
			this._playerMapMovementSpeed = CampaignOptions.Difficulty.VeryEasy;
			this._combatAIDifficulty = CampaignOptions.Difficulty.VeryEasy;
			this._persuasionSuccessChance = CampaignOptions.Difficulty.VeryEasy;
			this._clanMemberDeathChance = CampaignOptions.Difficulty.VeryEasy;
			this._battleDeath = CampaignOptions.Difficulty.VeryEasy;
			this._isLifeDeathCycleDisabled = false;
			this._autoAllocateClanMemberPerks = false;
			this._isIronmanMode = false;
			this._isSiegeTestBuild = false;
		}

		// Token: 0x060007FC RID: 2044 RVA: 0x00021B73 File Offset: 0x0001FD73
		internal static void AutoGeneratedStaticCollectObjectsCampaignOptions(object o, List<object> collectedObjects)
		{
			((CampaignOptions)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		// Token: 0x060007FD RID: 2045 RVA: 0x00021B81 File Offset: 0x0001FD81
		protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
		}

		// Token: 0x060007FE RID: 2046 RVA: 0x00021B83 File Offset: 0x0001FD83
		internal static object AutoGeneratedGetMemberValue_autoAllocateClanMemberPerks(object o)
		{
			return ((CampaignOptions)o)._autoAllocateClanMemberPerks;
		}

		// Token: 0x060007FF RID: 2047 RVA: 0x00021B95 File Offset: 0x0001FD95
		internal static object AutoGeneratedGetMemberValue_playerTroopsReceivedDamage(object o)
		{
			return ((CampaignOptions)o)._playerTroopsReceivedDamage;
		}

		// Token: 0x06000800 RID: 2048 RVA: 0x00021BA7 File Offset: 0x0001FDA7
		internal static object AutoGeneratedGetMemberValue_playerReceivedDamage(object o)
		{
			return ((CampaignOptions)o)._playerReceivedDamage;
		}

		// Token: 0x06000801 RID: 2049 RVA: 0x00021BB9 File Offset: 0x0001FDB9
		internal static object AutoGeneratedGetMemberValue_recruitmentDifficulty(object o)
		{
			return ((CampaignOptions)o)._recruitmentDifficulty;
		}

		// Token: 0x06000802 RID: 2050 RVA: 0x00021BCB File Offset: 0x0001FDCB
		internal static object AutoGeneratedGetMemberValue_playerMapMovementSpeed(object o)
		{
			return ((CampaignOptions)o)._playerMapMovementSpeed;
		}

		// Token: 0x06000803 RID: 2051 RVA: 0x00021BDD File Offset: 0x0001FDDD
		internal static object AutoGeneratedGetMemberValue_isSiegeTestBuild(object o)
		{
			return ((CampaignOptions)o)._isSiegeTestBuild;
		}

		// Token: 0x06000804 RID: 2052 RVA: 0x00021BEF File Offset: 0x0001FDEF
		internal static object AutoGeneratedGetMemberValue_combatAIDifficulty(object o)
		{
			return ((CampaignOptions)o)._combatAIDifficulty;
		}

		// Token: 0x06000805 RID: 2053 RVA: 0x00021C01 File Offset: 0x0001FE01
		internal static object AutoGeneratedGetMemberValue_isLifeDeathCycleDisabled(object o)
		{
			return ((CampaignOptions)o)._isLifeDeathCycleDisabled;
		}

		// Token: 0x06000806 RID: 2054 RVA: 0x00021C13 File Offset: 0x0001FE13
		internal static object AutoGeneratedGetMemberValue_persuasionSuccessChance(object o)
		{
			return ((CampaignOptions)o)._persuasionSuccessChance;
		}

		// Token: 0x06000807 RID: 2055 RVA: 0x00021C25 File Offset: 0x0001FE25
		internal static object AutoGeneratedGetMemberValue_clanMemberDeathChance(object o)
		{
			return ((CampaignOptions)o)._clanMemberDeathChance;
		}

		// Token: 0x06000808 RID: 2056 RVA: 0x00021C37 File Offset: 0x0001FE37
		internal static object AutoGeneratedGetMemberValue_isIronmanMode(object o)
		{
			return ((CampaignOptions)o)._isIronmanMode;
		}

		// Token: 0x06000809 RID: 2057 RVA: 0x00021C49 File Offset: 0x0001FE49
		internal static object AutoGeneratedGetMemberValue_battleDeath(object o)
		{
			return ((CampaignOptions)o)._battleDeath;
		}

		// Token: 0x040002A9 RID: 681
		[SaveableField(4)]
		private bool _autoAllocateClanMemberPerks;

		// Token: 0x040002AA RID: 682
		[SaveableField(5)]
		private CampaignOptions.Difficulty _playerTroopsReceivedDamage;

		// Token: 0x040002AB RID: 683
		[SaveableField(7)]
		private CampaignOptions.Difficulty _playerReceivedDamage;

		// Token: 0x040002AC RID: 684
		[SaveableField(8)]
		private CampaignOptions.Difficulty _recruitmentDifficulty;

		// Token: 0x040002AD RID: 685
		[SaveableField(9)]
		private CampaignOptions.Difficulty _playerMapMovementSpeed;

		// Token: 0x040002AE RID: 686
		[SaveableField(10)]
		private bool _isSiegeTestBuild;

		// Token: 0x040002AF RID: 687
		[SaveableField(11)]
		private CampaignOptions.Difficulty _combatAIDifficulty;

		// Token: 0x040002B0 RID: 688
		[SaveableField(12)]
		private bool _isLifeDeathCycleDisabled;

		// Token: 0x040002B1 RID: 689
		[SaveableField(13)]
		private CampaignOptions.Difficulty _persuasionSuccessChance;

		// Token: 0x040002B2 RID: 690
		[SaveableField(14)]
		private CampaignOptions.Difficulty _clanMemberDeathChance;

		// Token: 0x040002B3 RID: 691
		[SaveableField(15)]
		private bool _isIronmanMode;

		// Token: 0x040002B4 RID: 692
		[SaveableField(17)]
		private CampaignOptions.Difficulty _battleDeath;

		// Token: 0x0200049B RID: 1179
		public enum Difficulty : short
		{
			// Token: 0x040013F0 RID: 5104
			VeryEasy,
			// Token: 0x040013F1 RID: 5105
			Easy,
			// Token: 0x040013F2 RID: 5106
			Realistic
		}
	}
}
