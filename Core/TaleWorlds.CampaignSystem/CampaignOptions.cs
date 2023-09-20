using System;
using System.Collections.Generic;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem
{
	public class CampaignOptions
	{
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
		}

		internal static void AutoGeneratedStaticCollectObjectsCampaignOptions(object o, List<object> collectedObjects)
		{
			((CampaignOptions)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
		}

		internal static object AutoGeneratedGetMemberValue_autoAllocateClanMemberPerks(object o)
		{
			return ((CampaignOptions)o)._autoAllocateClanMemberPerks;
		}

		internal static object AutoGeneratedGetMemberValue_playerTroopsReceivedDamage(object o)
		{
			return ((CampaignOptions)o)._playerTroopsReceivedDamage;
		}

		internal static object AutoGeneratedGetMemberValue_playerReceivedDamage(object o)
		{
			return ((CampaignOptions)o)._playerReceivedDamage;
		}

		internal static object AutoGeneratedGetMemberValue_recruitmentDifficulty(object o)
		{
			return ((CampaignOptions)o)._recruitmentDifficulty;
		}

		internal static object AutoGeneratedGetMemberValue_playerMapMovementSpeed(object o)
		{
			return ((CampaignOptions)o)._playerMapMovementSpeed;
		}

		internal static object AutoGeneratedGetMemberValue_combatAIDifficulty(object o)
		{
			return ((CampaignOptions)o)._combatAIDifficulty;
		}

		internal static object AutoGeneratedGetMemberValue_isLifeDeathCycleDisabled(object o)
		{
			return ((CampaignOptions)o)._isLifeDeathCycleDisabled;
		}

		internal static object AutoGeneratedGetMemberValue_persuasionSuccessChance(object o)
		{
			return ((CampaignOptions)o)._persuasionSuccessChance;
		}

		internal static object AutoGeneratedGetMemberValue_clanMemberDeathChance(object o)
		{
			return ((CampaignOptions)o)._clanMemberDeathChance;
		}

		internal static object AutoGeneratedGetMemberValue_isIronmanMode(object o)
		{
			return ((CampaignOptions)o)._isIronmanMode;
		}

		internal static object AutoGeneratedGetMemberValue_battleDeath(object o)
		{
			return ((CampaignOptions)o)._battleDeath;
		}

		[SaveableField(4)]
		private bool _autoAllocateClanMemberPerks;

		[SaveableField(5)]
		private CampaignOptions.Difficulty _playerTroopsReceivedDamage;

		[SaveableField(7)]
		private CampaignOptions.Difficulty _playerReceivedDamage;

		[SaveableField(8)]
		private CampaignOptions.Difficulty _recruitmentDifficulty;

		[SaveableField(9)]
		private CampaignOptions.Difficulty _playerMapMovementSpeed;

		[SaveableField(11)]
		private CampaignOptions.Difficulty _combatAIDifficulty;

		[SaveableField(12)]
		private bool _isLifeDeathCycleDisabled;

		[SaveableField(13)]
		private CampaignOptions.Difficulty _persuasionSuccessChance;

		[SaveableField(14)]
		private CampaignOptions.Difficulty _clanMemberDeathChance;

		[SaveableField(15)]
		private bool _isIronmanMode;

		[SaveableField(17)]
		private CampaignOptions.Difficulty _battleDeath;

		public enum Difficulty : short
		{
			VeryEasy,
			Easy,
			Realistic
		}
	}
}
