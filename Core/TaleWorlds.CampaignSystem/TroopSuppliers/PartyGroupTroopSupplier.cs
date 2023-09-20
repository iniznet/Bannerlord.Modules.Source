using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.TroopSuppliers
{
	public class PartyGroupTroopSupplier : IMissionTroopSupplier
	{
		internal MapEventSide PartyGroup { get; private set; }

		public PartyGroupTroopSupplier(MapEvent mapEvent, BattleSideEnum side, FlattenedTroopRoster priorTroops = null, Func<UniqueTroopDescriptor, MapEventParty, bool> customAllocationConditions = null)
		{
			this._customAllocationConditions = customAllocationConditions;
			this.PartyGroup = mapEvent.GetMapEventSide(side);
			this._initialTroopCount = this.PartyGroup.TroopCount;
			this.PartyGroup.MakeReadyForMission(priorTroops);
		}

		public IEnumerable<IAgentOriginBase> SupplyTroops(int numberToAllocate)
		{
			List<UniqueTroopDescriptor> list = null;
			this.PartyGroup.AllocateTroops(ref list, numberToAllocate, this._customAllocationConditions);
			PartyGroupAgentOrigin[] array = new PartyGroupAgentOrigin[list.Count];
			this._numAllocated += list.Count;
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new PartyGroupAgentOrigin(this, list[i], i);
			}
			if (array.Length < numberToAllocate)
			{
				this._anyTroopRemainsToBeSupplied = false;
			}
			return array;
		}

		public IEnumerable<IAgentOriginBase> GetAllTroops()
		{
			List<UniqueTroopDescriptor> list = null;
			this.PartyGroup.GetAllTroops(ref list);
			PartyGroupAgentOrigin[] array = new PartyGroupAgentOrigin[list.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new PartyGroupAgentOrigin(this, list[i], i);
			}
			return array;
		}

		public BasicCharacterObject GetGeneralCharacter()
		{
			return this.PartyGroup.LeaderParty.General;
		}

		public int NumRemovedTroops
		{
			get
			{
				return this._numWounded + this._numKilled + this._numRouted;
			}
		}

		public int NumTroopsNotSupplied
		{
			get
			{
				return this._initialTroopCount - this._numAllocated;
			}
		}

		public bool AnyTroopRemainsToBeSupplied
		{
			get
			{
				return this._anyTroopRemainsToBeSupplied;
			}
		}

		public int GetNumberOfPlayerControllableTroops()
		{
			int num = 0;
			foreach (MapEventParty mapEventParty in this.PartyGroup.Parties)
			{
				PartyBase party = mapEventParty.Party;
				if (PartyGroupAgentOrigin.IsPartyUnderPlayerCommand(party) || (party.Side == PartyBase.MainParty.Side && this.PartyGroup.MapEvent.IsPlayerSergeant()))
				{
					num += party.NumberOfHealthyMembers;
				}
			}
			return num;
		}

		public void OnTroopWounded(UniqueTroopDescriptor troopDescriptor)
		{
			this._numWounded++;
			this.PartyGroup.OnTroopWounded(troopDescriptor);
		}

		public void OnTroopKilled(UniqueTroopDescriptor troopDescriptor)
		{
			this._numKilled++;
			this.PartyGroup.OnTroopKilled(troopDescriptor);
		}

		public void OnTroopRouted(UniqueTroopDescriptor troopDescriptor)
		{
			this._numRouted++;
			this.PartyGroup.OnTroopRouted(troopDescriptor);
		}

		internal CharacterObject GetTroop(UniqueTroopDescriptor troopDescriptor)
		{
			return this.PartyGroup.GetAllocatedTroop(troopDescriptor) ?? this.PartyGroup.GetReadyTroop(troopDescriptor);
		}

		public PartyBase GetParty(UniqueTroopDescriptor troopDescriptor)
		{
			PartyBase partyBase = this.PartyGroup.GetAllocatedTroopParty(troopDescriptor);
			if (partyBase == null)
			{
				partyBase = this.PartyGroup.GetReadyTroopParty(troopDescriptor);
			}
			return partyBase;
		}

		public void OnTroopScoreHit(UniqueTroopDescriptor descriptor, BasicCharacterObject attackedCharacter, int damage, bool isFatal, bool isTeamKill, WeaponComponentData attackerWeapon)
		{
			this.PartyGroup.OnTroopScoreHit(descriptor, (CharacterObject)attackedCharacter, damage, isFatal, isTeamKill, attackerWeapon, false);
		}

		private readonly int _initialTroopCount;

		private int _numAllocated;

		private int _numWounded;

		private int _numKilled;

		private int _numRouted;

		private Func<UniqueTroopDescriptor, MapEventParty, bool> _customAllocationConditions;

		private bool _anyTroopRemainsToBeSupplied = true;
	}
}
