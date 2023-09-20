using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	public class BasicBattleAgentOrigin : IAgentOriginBase
	{
		bool IAgentOriginBase.IsUnderPlayersCommand
		{
			get
			{
				return false;
			}
		}

		uint IAgentOriginBase.FactionColor
		{
			get
			{
				return 0U;
			}
		}

		uint IAgentOriginBase.FactionColor2
		{
			get
			{
				return 0U;
			}
		}

		IBattleCombatant IAgentOriginBase.BattleCombatant
		{
			get
			{
				return null;
			}
		}

		int IAgentOriginBase.UniqueSeed
		{
			get
			{
				return 0;
			}
		}

		int IAgentOriginBase.Seed
		{
			get
			{
				return 0;
			}
		}

		Banner IAgentOriginBase.Banner
		{
			get
			{
				return null;
			}
		}

		BasicCharacterObject IAgentOriginBase.Troop
		{
			get
			{
				return this._troop;
			}
		}

		public BasicBattleAgentOrigin(BasicCharacterObject troop)
		{
			this._troop = troop;
		}

		void IAgentOriginBase.SetWounded()
		{
		}

		void IAgentOriginBase.SetKilled()
		{
		}

		void IAgentOriginBase.SetRouted()
		{
		}

		void IAgentOriginBase.OnAgentRemoved(float agentHealth)
		{
		}

		void IAgentOriginBase.OnScoreHit(BasicCharacterObject victim, BasicCharacterObject captain, int damage, bool isFatal, bool isTeamKill, WeaponComponentData attackerWeapon)
		{
		}

		void IAgentOriginBase.SetBanner(Banner banner)
		{
		}

		private BasicCharacterObject _troop;
	}
}
