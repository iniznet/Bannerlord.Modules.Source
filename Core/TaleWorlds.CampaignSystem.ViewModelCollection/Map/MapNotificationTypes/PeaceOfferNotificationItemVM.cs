using System;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.CampaignSystem.MapNotificationTypes;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapNotificationTypes
{
	// Token: 0x02000043 RID: 67
	public class PeaceOfferNotificationItemVM : MapNotificationItemBaseVM
	{
		// Token: 0x06000550 RID: 1360 RVA: 0x0001AB1C File Offset: 0x00018D1C
		public PeaceOfferNotificationItemVM(PeaceOfferMapNotification data)
			: base(data)
		{
			PeaceOfferNotificationItemVM <>4__this = this;
			this._opponentFaction = data.OpponentFaction;
			this._tributeAmount = data.TributeAmount;
			this._onInspect = delegate
			{
				CampaignEventDispatcher.Instance.OnPeaceOfferedToPlayer(data.OpponentFaction, data.TributeAmount);
				<>4__this._playerInspectedNotification = true;
				<>4__this.ExecuteRemove();
			};
			CampaignEvents.OnPeaceOfferCancelledEvent.AddNonSerializedListener(this, new Action<IFaction>(this.OnPeaceOfferCancelled));
			base.NotificationIdentifier = "ransom";
		}

		// Token: 0x06000551 RID: 1361 RVA: 0x0001AB9F File Offset: 0x00018D9F
		private void OnPeaceOfferCancelled(IFaction opponentFaction)
		{
			if (Campaign.Current.CampaignInformationManager.InformationDataExists<PeaceOfferMapNotification>((PeaceOfferMapNotification x) => x == base.Data))
			{
				base.ExecuteRemove();
				this._opponentFaction = null;
			}
		}

		// Token: 0x06000552 RID: 1362 RVA: 0x0001ABCC File Offset: 0x00018DCC
		public override void OnFinalize()
		{
			base.OnFinalize();
			CampaignEventDispatcher.Instance.RemoveListeners(this);
			if (!this._playerInspectedNotification && Hero.MainHero.MapFaction.Leader != Hero.MainHero)
			{
				bool flag = false;
				foreach (KingdomDecision kingdomDecision in ((Kingdom)Hero.MainHero.MapFaction).UnresolvedDecisions)
				{
					if (kingdomDecision is MakePeaceKingdomDecision && ((MakePeaceKingdomDecision)kingdomDecision).ProposerClan.MapFaction == Hero.MainHero.MapFaction && ((MakePeaceKingdomDecision)kingdomDecision).FactionToMakePeaceWith == this._opponentFaction)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					MakePeaceKingdomDecision makePeaceKingdomDecision = new MakePeaceKingdomDecision(Hero.MainHero.MapFaction.Leader.Clan, this._opponentFaction, -this._tributeAmount, true);
					((Kingdom)Hero.MainHero.MapFaction).AddDecision(makePeaceKingdomDecision, false);
				}
			}
		}

		// Token: 0x04000240 RID: 576
		private IFaction _opponentFaction;

		// Token: 0x04000241 RID: 577
		private int _tributeAmount;

		// Token: 0x04000242 RID: 578
		private bool _playerInspectedNotification;
	}
}
