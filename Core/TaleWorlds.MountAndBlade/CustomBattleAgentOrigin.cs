using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	public class CustomBattleAgentOrigin : IAgentOriginBase
	{
		public CustomBattleCombatant CustomBattleCombatant { get; private set; }

		IBattleCombatant IAgentOriginBase.BattleCombatant
		{
			get
			{
				return this.CustomBattleCombatant;
			}
		}

		public BasicCharacterObject Troop { get; private set; }

		public int Rank { get; private set; }

		public Banner Banner
		{
			get
			{
				return this.CustomBattleCombatant.Banner;
			}
		}

		public bool IsUnderPlayersCommand
		{
			get
			{
				return this._isPlayerSide;
			}
		}

		public uint FactionColor
		{
			get
			{
				return this.CustomBattleCombatant.BasicCulture.Color;
			}
		}

		public uint FactionColor2
		{
			get
			{
				return this.CustomBattleCombatant.BasicCulture.Color2;
			}
		}

		public int Seed
		{
			get
			{
				return this.Troop.GetDefaultFaceSeed(this.Rank);
			}
		}

		public int UniqueSeed
		{
			get
			{
				return this._descriptor.UniqueSeed;
			}
		}

		public CustomBattleAgentOrigin(CustomBattleCombatant customBattleCombatant, BasicCharacterObject characterObject, CustomBattleTroopSupplier troopSupplier, bool isPlayerSide, int rank = -1, UniqueTroopDescriptor uniqueNo = default(UniqueTroopDescriptor))
		{
			this.CustomBattleCombatant = customBattleCombatant;
			this.Troop = characterObject;
			this._descriptor = ((!uniqueNo.IsValid) ? new UniqueTroopDescriptor(Game.Current.NextUniqueTroopSeed) : uniqueNo);
			this.Rank = ((rank == -1) ? MBRandom.RandomInt(10000) : rank);
			this._troopSupplier = troopSupplier;
			this._isPlayerSide = isPlayerSide;
		}

		public void SetWounded()
		{
			if (!this._isRemoved)
			{
				this._troopSupplier.OnTroopWounded();
				this._isRemoved = true;
			}
		}

		public void SetKilled()
		{
			if (!this._isRemoved)
			{
				this._troopSupplier.OnTroopKilled();
				this._isRemoved = true;
			}
		}

		public void SetRouted()
		{
			if (!this._isRemoved)
			{
				this._troopSupplier.OnTroopRouted();
				this._isRemoved = true;
			}
		}

		public void OnAgentRemoved(float agentHealth)
		{
		}

		void IAgentOriginBase.OnScoreHit(BasicCharacterObject victim, BasicCharacterObject captain, int damage, bool isFatal, bool isTeamKill, WeaponComponentData attackerWeapon)
		{
		}

		public void SetBanner(Banner banner)
		{
			throw new NotImplementedException();
		}

		private readonly UniqueTroopDescriptor _descriptor;

		private readonly bool _isPlayerSide;

		private CustomBattleTroopSupplier _troopSupplier;

		private bool _isRemoved;
	}
}
