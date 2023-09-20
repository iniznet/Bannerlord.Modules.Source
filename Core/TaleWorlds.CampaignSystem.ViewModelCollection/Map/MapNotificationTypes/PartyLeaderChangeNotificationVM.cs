using System;
using TaleWorlds.CampaignSystem.MapNotificationTypes;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapNotificationTypes
{
	public class PartyLeaderChangeNotificationVM : MapNotificationItemBaseVM
	{
		public PartyLeaderChangeNotificationVM(PartyLeaderChangeNotification data)
			: base(data)
		{
			this._party = data.Party;
			base.NotificationIdentifier = "death";
			this._onInspect = delegate
			{
				InformationManager.ShowInquiry(new InquiryData(this._decisionPopupTitleText.ToString(), this._partyLeaderChangePopupText.ToString(), true, true, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), delegate
				{
					INavigationHandler navigationHandler = base.NavigationHandler;
					if (navigationHandler == null)
					{
						return;
					}
					navigationHandler.OpenClan(this._party.Party);
				}, null, "", 0f, null, null, null), false, false);
				Campaign.Current.TimeControlMode = CampaignTimeControlMode.Stop;
				this._playerInspectedNotification = true;
				base.ExecuteRemove();
			};
			CampaignEvents.OnPartyLeaderChangeOfferCanceledEvent.AddNonSerializedListener(this, new Action<MobileParty>(this.OnPartyLeaderChangeOfferCanceled));
		}

		private void OnPartyLeaderChangeOfferCanceled(MobileParty party)
		{
			if (Campaign.Current.CampaignInformationManager.InformationDataExists<PartyLeaderChangeNotification>((PartyLeaderChangeNotification x) => x.Party == party))
			{
				base.ExecuteRemove();
			}
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			CampaignEventDispatcher.Instance.RemoveListeners(this);
			if (!this._playerInspectedNotification)
			{
				CampaignEventDispatcher.Instance.OnPartyLeaderChangeOfferCanceled(this._party);
			}
		}

		private bool _playerInspectedNotification;

		private readonly MobileParty _party;

		private TextObject _decisionPopupTitleText = new TextObject("{=nFl0ufe3}A party without a leader", null);

		private TextObject _partyLeaderChangePopupText = new TextObject("{=OMqHwpXF}One of your parties has lost its leader. It will disband after a day has passed. You can assign a new clan member to lead it, if you wish to keep the party.{newline}{newline}Do you want to assign a new leader?", null);
	}
}
