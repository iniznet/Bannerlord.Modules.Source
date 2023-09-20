﻿using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.MapEvents
{
	// Token: 0x020002C2 RID: 706
	public class ForceSuppliesEventComponent : MapEventComponent
	{
		// Token: 0x06002947 RID: 10567 RVA: 0x000B0AAB File Offset: 0x000AECAB
		internal static void AutoGeneratedStaticCollectObjectsForceSuppliesEventComponent(object o, List<object> collectedObjects)
		{
			((ForceSuppliesEventComponent)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		// Token: 0x06002948 RID: 10568 RVA: 0x000B0AB9 File Offset: 0x000AECB9
		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		// Token: 0x06002949 RID: 10569 RVA: 0x000B0AC2 File Offset: 0x000AECC2
		protected ForceSuppliesEventComponent(MapEvent mapEvent)
			: base(mapEvent)
		{
		}

		// Token: 0x0600294A RID: 10570 RVA: 0x000B0ACC File Offset: 0x000AECCC
		public static ForceSuppliesEventComponent CreateForceSuppliesEvent(PartyBase attackerParty, PartyBase defenderParty)
		{
			MapEvent mapEvent = new MapEvent();
			ForceSuppliesEventComponent forceSuppliesEventComponent = new ForceSuppliesEventComponent(mapEvent);
			mapEvent.Initialize(attackerParty, defenderParty, forceSuppliesEventComponent, MapEvent.BattleTypes.IsForcingSupplies);
			Campaign.Current.MapEventManager.OnMapEventCreated(mapEvent);
			return forceSuppliesEventComponent;
		}

		// Token: 0x0600294B RID: 10571 RVA: 0x000B0B01 File Offset: 0x000AED01
		public static ForceSuppliesEventComponent CreateComponentForOldSaves(MapEvent mapEvent)
		{
			return new ForceSuppliesEventComponent(mapEvent);
		}

		// Token: 0x0600294C RID: 10572 RVA: 0x000B0B09 File Offset: 0x000AED09
		protected override void OnInitialize()
		{
			ChangeVillageStateAction.ApplyBySettingToBeingForcedForSupplies(base.MapEvent.MapEventSettlement, base.MapEvent.AttackerSide.LeaderParty.MobileParty);
		}

		// Token: 0x0600294D RID: 10573 RVA: 0x000B0B30 File Offset: 0x000AED30
		protected override void OnFinalize()
		{
			CampaignEventDispatcher.Instance.ForceSuppliesCompleted((base.MapEvent.BattleState == BattleState.AttackerVictory) ? BattleSideEnum.Attacker : BattleSideEnum.Defender, this);
			ChangeVillageStateAction.ApplyBySettingToNormal(base.MapEvent.MapEventSettlement);
		}
	}
}
