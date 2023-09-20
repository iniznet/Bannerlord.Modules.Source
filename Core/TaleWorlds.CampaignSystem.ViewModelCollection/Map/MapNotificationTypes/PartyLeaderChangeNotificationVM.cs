using System;
using TaleWorlds.CampaignSystem.MapNotificationTypes;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapNotificationTypes
{
	// Token: 0x02000041 RID: 65
	public class PartyLeaderChangeNotificationVM : MapNotificationItemBaseVM
	{
		// Token: 0x06000547 RID: 1351 RVA: 0x0001A8D4 File Offset: 0x00018AD4
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

		// Token: 0x06000548 RID: 1352 RVA: 0x0001A94C File Offset: 0x00018B4C
		private void OnPartyLeaderChangeOfferCanceled(MobileParty party)
		{
			if (Campaign.Current.CampaignInformationManager.InformationDataExists<PartyLeaderChangeNotification>((PartyLeaderChangeNotification x) => x.Party == party))
			{
				base.ExecuteRemove();
			}
		}

		// Token: 0x06000549 RID: 1353 RVA: 0x0001A989 File Offset: 0x00018B89
		public override void OnFinalize()
		{
			base.OnFinalize();
			CampaignEventDispatcher.Instance.RemoveListeners(this);
			if (!this._playerInspectedNotification)
			{
				CampaignEventDispatcher.Instance.OnPartyLeaderChangeOfferCanceled(this._party);
			}
		}

		// Token: 0x0400023B RID: 571
		private bool _playerInspectedNotification;

		// Token: 0x0400023C RID: 572
		private readonly MobileParty _party;

		// Token: 0x0400023D RID: 573
		private TextObject _decisionPopupTitleText = new TextObject("{=nFl0ufe3}A party without a leader", null);

		// Token: 0x0400023E RID: 574
		private TextObject _partyLeaderChangePopupText = new TextObject("{=OMqHwpXF}One of your parties has lost its leader. It will disband after a day has passed. You can assign a new clan member to lead it, if you wish to keep the party.{newline}{newline}Do you want to assign a new leader?", null);
	}
}
