using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.TroopSuppliers
{
	// Token: 0x020000A9 RID: 169
	public class PartyGroupTroopSupplier : IMissionTroopSupplier
	{
		// Token: 0x170004D5 RID: 1237
		// (get) Token: 0x06001193 RID: 4499 RVA: 0x00050951 File Offset: 0x0004EB51
		// (set) Token: 0x06001194 RID: 4500 RVA: 0x00050959 File Offset: 0x0004EB59
		internal MapEventSide PartyGroup { get; private set; }

		// Token: 0x06001195 RID: 4501 RVA: 0x00050964 File Offset: 0x0004EB64
		public PartyGroupTroopSupplier(MapEvent mapEvent, BattleSideEnum side, FlattenedTroopRoster priorTroops = null, Func<UniqueTroopDescriptor, MapEventParty, bool> customAllocationConditions = null)
		{
			this._customAllocationConditions = customAllocationConditions;
			this.PartyGroup = mapEvent.GetMapEventSide(side);
			this._initialTroopCount = this.PartyGroup.TroopCount;
			this.PartyGroup.MakeReadyForMission(priorTroops);
		}

		// Token: 0x06001196 RID: 4502 RVA: 0x000509B0 File Offset: 0x0004EBB0
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

		// Token: 0x06001197 RID: 4503 RVA: 0x00050A20 File Offset: 0x0004EC20
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

		// Token: 0x06001198 RID: 4504 RVA: 0x00050A68 File Offset: 0x0004EC68
		public BasicCharacterObject GetGeneralCharacter()
		{
			return this.PartyGroup.LeaderParty.General;
		}

		// Token: 0x170004D6 RID: 1238
		// (get) Token: 0x06001199 RID: 4505 RVA: 0x00050A7A File Offset: 0x0004EC7A
		public int NumRemovedTroops
		{
			get
			{
				return this._numWounded + this._numKilled + this._numRouted;
			}
		}

		// Token: 0x170004D7 RID: 1239
		// (get) Token: 0x0600119A RID: 4506 RVA: 0x00050A90 File Offset: 0x0004EC90
		public int NumTroopsNotSupplied
		{
			get
			{
				return this._initialTroopCount - this._numAllocated;
			}
		}

		// Token: 0x170004D8 RID: 1240
		// (get) Token: 0x0600119B RID: 4507 RVA: 0x00050A9F File Offset: 0x0004EC9F
		public bool AnyTroopRemainsToBeSupplied
		{
			get
			{
				return this._anyTroopRemainsToBeSupplied;
			}
		}

		// Token: 0x0600119C RID: 4508 RVA: 0x00050AA8 File Offset: 0x0004ECA8
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

		// Token: 0x0600119D RID: 4509 RVA: 0x00050B38 File Offset: 0x0004ED38
		public void OnTroopWounded(UniqueTroopDescriptor troopDescriptor)
		{
			this._numWounded++;
			this.PartyGroup.OnTroopWounded(troopDescriptor);
		}

		// Token: 0x0600119E RID: 4510 RVA: 0x00050B54 File Offset: 0x0004ED54
		public void OnTroopKilled(UniqueTroopDescriptor troopDescriptor)
		{
			this._numKilled++;
			this.PartyGroup.OnTroopKilled(troopDescriptor);
		}

		// Token: 0x0600119F RID: 4511 RVA: 0x00050B70 File Offset: 0x0004ED70
		public void OnTroopRouted(UniqueTroopDescriptor troopDescriptor)
		{
			this._numRouted++;
			this.PartyGroup.OnTroopRouted(troopDescriptor);
		}

		// Token: 0x060011A0 RID: 4512 RVA: 0x00050B8C File Offset: 0x0004ED8C
		internal CharacterObject GetTroop(UniqueTroopDescriptor troopDescriptor)
		{
			return this.PartyGroup.GetAllocatedTroop(troopDescriptor) ?? this.PartyGroup.GetReadyTroop(troopDescriptor);
		}

		// Token: 0x060011A1 RID: 4513 RVA: 0x00050BAC File Offset: 0x0004EDAC
		public PartyBase GetParty(UniqueTroopDescriptor troopDescriptor)
		{
			PartyBase partyBase = this.PartyGroup.GetAllocatedTroopParty(troopDescriptor);
			if (partyBase == null)
			{
				partyBase = this.PartyGroup.GetReadyTroopParty(troopDescriptor);
			}
			return partyBase;
		}

		// Token: 0x060011A2 RID: 4514 RVA: 0x00050BD7 File Offset: 0x0004EDD7
		public void OnTroopScoreHit(UniqueTroopDescriptor descriptor, BasicCharacterObject attackedCharacter, int damage, bool isFatal, bool isTeamKill, WeaponComponentData attackerWeapon)
		{
			this.PartyGroup.OnTroopScoreHit(descriptor, (CharacterObject)attackedCharacter, damage, isFatal, isTeamKill, attackerWeapon, false);
		}

		// Token: 0x0400061B RID: 1563
		private readonly int _initialTroopCount;

		// Token: 0x0400061C RID: 1564
		private int _numAllocated;

		// Token: 0x0400061D RID: 1565
		private int _numWounded;

		// Token: 0x0400061E RID: 1566
		private int _numKilled;

		// Token: 0x0400061F RID: 1567
		private int _numRouted;

		// Token: 0x04000620 RID: 1568
		private Func<UniqueTroopDescriptor, MapEventParty, bool> _customAllocationConditions;

		// Token: 0x04000621 RID: 1569
		private bool _anyTroopRemainsToBeSupplied = true;
	}
}
