using System;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Settlements;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	internal class VillageTradeBoundCampaignBehavior : CampaignBehaviorBase
	{
		public override void RegisterEvents()
		{
			CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnNewGameCreated));
			CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener(this, new Action<Settlement, bool, Hero, Hero, Hero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail>(this.OnSettlementOwnerChanged));
			CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnGameLoaded));
			CampaignEvents.WarDeclared.AddNonSerializedListener(this, new Action<IFaction, IFaction, DeclareWarAction.DeclareWarDetail>(this.WarDeclared));
			CampaignEvents.MakePeace.AddNonSerializedListener(this, new Action<IFaction, IFaction, MakePeaceAction.MakePeaceDetail>(this.OnMakePeace));
			CampaignEvents.ClanChangedKingdom.AddNonSerializedListener(this, new Action<Clan, Kingdom, Kingdom, ChangeKingdomAction.ChangeKingdomActionDetail, bool>(this.ClanChangedKingdom));
			CampaignEvents.OnClanDestroyedEvent.AddNonSerializedListener(this, new Action<Clan>(this.OnClanDestroyed));
		}

		private void OnClanDestroyed(Clan obj)
		{
			this.UpdateTradeBounds();
		}

		public override void SyncData(IDataStore dataStore)
		{
		}

		private void ClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail, bool showNotification = true)
		{
			this.UpdateTradeBounds();
		}

		private void OnGameLoaded(CampaignGameStarter obj)
		{
			this.UpdateTradeBounds();
		}

		private void OnMakePeace(IFaction faction1, IFaction faction2, MakePeaceAction.MakePeaceDetail detail)
		{
			this.UpdateTradeBounds();
		}

		private void WarDeclared(IFaction faction1, IFaction faction2, DeclareWarAction.DeclareWarDetail declareWarDetail)
		{
			this.UpdateTradeBounds();
		}

		private void OnSettlementOwnerChanged(Settlement settlement, bool openToClaim, Hero newOwner, Hero oldOwner, Hero capturerHero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
		{
			this.UpdateTradeBounds();
		}

		public void OnNewGameCreated(CampaignGameStarter campaignGameStarter)
		{
			this.UpdateTradeBounds();
		}

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

		private void TryToAssignTradeBoundForVillage(Village village)
		{
			Settlement settlement = SettlementHelper.FindNearestSettlement((Settlement x) => x.IsTown && x.Town.MapFaction == village.Settlement.MapFaction, village.Settlement);
			if (settlement != null && Campaign.Current.Models.MapDistanceModel.GetDistance(settlement, village.Settlement) < 150f)
			{
				village.TradeBound = settlement;
				return;
			}
			Settlement settlement2 = SettlementHelper.FindNearestSettlement((Settlement x) => x.IsTown && x.Town.MapFaction != village.Settlement.MapFaction && !x.Town.MapFaction.IsAtWarWith(village.Settlement.MapFaction) && Campaign.Current.Models.MapDistanceModel.GetDistance(x, village.Settlement) <= 150f, village.Settlement);
			if (settlement2 != null && Campaign.Current.Models.MapDistanceModel.GetDistance(settlement2, village.Settlement) < 150f)
			{
				village.TradeBound = settlement2;
				return;
			}
			village.TradeBound = null;
		}

		public const float TradeBoundDistanceLimit = 150f;
	}
}
