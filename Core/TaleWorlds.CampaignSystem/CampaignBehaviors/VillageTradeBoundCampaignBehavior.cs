using System;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Settlements;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x020003E3 RID: 995
	internal class VillageTradeBoundCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x06003C85 RID: 15493 RVA: 0x0011FF24 File Offset: 0x0011E124
		public override void RegisterEvents()
		{
			CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnNewGameCreated));
			CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener(this, new Action<Settlement, bool, Hero, Hero, Hero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail>(this.OnSettlementOwnerChanged));
			CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnGameLoaded));
			CampaignEvents.WarDeclared.AddNonSerializedListener(this, new Action<IFaction, IFaction, DeclareWarAction.DeclareWarDetail>(this.WarDeclared));
			CampaignEvents.MakePeace.AddNonSerializedListener(this, new Action<IFaction, IFaction, MakePeaceAction.MakePeaceDetail>(this.OnMakePeace));
			CampaignEvents.ClanChangedKingdom.AddNonSerializedListener(this, new Action<Clan, Kingdom, Kingdom, ChangeKingdomAction.ChangeKingdomActionDetail, bool>(this.ClanChangedKingdom));
		}

		// Token: 0x06003C86 RID: 15494 RVA: 0x0011FFBB File Offset: 0x0011E1BB
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x06003C87 RID: 15495 RVA: 0x0011FFBD File Offset: 0x0011E1BD
		private void ClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail, bool showNotification = true)
		{
			this.UpdateTradeBounds();
		}

		// Token: 0x06003C88 RID: 15496 RVA: 0x0011FFC5 File Offset: 0x0011E1C5
		private void OnGameLoaded(CampaignGameStarter obj)
		{
			this.UpdateTradeBounds();
		}

		// Token: 0x06003C89 RID: 15497 RVA: 0x0011FFCD File Offset: 0x0011E1CD
		private void OnMakePeace(IFaction faction1, IFaction faction2, MakePeaceAction.MakePeaceDetail detail)
		{
			this.UpdateTradeBounds();
		}

		// Token: 0x06003C8A RID: 15498 RVA: 0x0011FFD5 File Offset: 0x0011E1D5
		private void WarDeclared(IFaction faction1, IFaction faction2, DeclareWarAction.DeclareWarDetail declareWarDetail)
		{
			this.UpdateTradeBounds();
		}

		// Token: 0x06003C8B RID: 15499 RVA: 0x0011FFDD File Offset: 0x0011E1DD
		private void OnSettlementOwnerChanged(Settlement settlement, bool openToClaim, Hero newOwner, Hero oldOwner, Hero capturerHero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
		{
			this.UpdateTradeBounds();
		}

		// Token: 0x06003C8C RID: 15500 RVA: 0x0011FFE5 File Offset: 0x0011E1E5
		public void OnNewGameCreated(CampaignGameStarter campaignGameStarter)
		{
			this.UpdateTradeBounds();
		}

		// Token: 0x06003C8D RID: 15501 RVA: 0x0011FFF0 File Offset: 0x0011E1F0
		private void UpdateTradeBounds()
		{
			foreach (Town town in Campaign.Current.AllCastles)
			{
				foreach (Village village in town.Villages)
				{
					this.TryToAssignTradeBoundForVillage(village);
				}
			}
		}

		// Token: 0x06003C8E RID: 15502 RVA: 0x00120080 File Offset: 0x0011E280
		private void TryToAssignTradeBoundForVillage(Village village)
		{
			Settlement settlement = SettlementHelper.FindNearestSettlement((Settlement x) => x.IsTown && x.Town.MapFaction == village.Settlement.MapFaction, village.Settlement);
			if (settlement != null)
			{
				if (Campaign.Current.Models.MapDistanceModel.GetDistance(settlement, village.Settlement) <= 150f)
				{
					village.TradeBound = settlement;
					return;
				}
				Settlement settlement2 = SettlementHelper.FindNearestSettlement((Settlement x) => x.IsTown && x.Town.MapFaction != village.Settlement.MapFaction && !x.Town.MapFaction.IsAtWarWith(village.Settlement.MapFaction) && Campaign.Current.Models.MapDistanceModel.GetDistance(x, village.Settlement) <= 150f, village.Settlement);
				if (settlement2 != null)
				{
					village.TradeBound = settlement2;
					return;
				}
				village.TradeBound = null;
			}
		}

		// Token: 0x04001258 RID: 4696
		public const float TradeBoundDistanceLimit = 150f;
	}
}
