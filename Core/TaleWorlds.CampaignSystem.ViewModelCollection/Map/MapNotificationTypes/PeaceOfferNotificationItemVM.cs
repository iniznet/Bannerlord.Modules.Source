using System;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.CampaignSystem.MapNotificationTypes;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapNotificationTypes
{
	public class PeaceOfferNotificationItemVM : MapNotificationItemBaseVM
	{
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

		private void OnPeaceOfferCancelled(IFaction opponentFaction)
		{
			if (Campaign.Current.CampaignInformationManager.InformationDataExists<PeaceOfferMapNotification>((PeaceOfferMapNotification x) => x == base.Data))
			{
				base.ExecuteRemove();
				this._opponentFaction = null;
			}
		}

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

		private IFaction _opponentFaction;

		private int _tributeAmount;

		private bool _playerInspectedNotification;
	}
}
