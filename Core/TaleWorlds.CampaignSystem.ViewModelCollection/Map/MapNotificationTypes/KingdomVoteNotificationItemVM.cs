using System;
using System.Linq;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.CampaignSystem.MapNotificationTypes;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapNotificationTypes
{
	public class KingdomVoteNotificationItemVM : MapNotificationItemBaseVM
	{
		public KingdomVoteNotificationItemVM(KingdomDecisionMapNotification data)
			: base(data)
		{
			KingdomVoteNotificationItemVM <>4__this = this;
			this._decision = data.Decision;
			this._kingdomOfDecision = data.KingdomOfDecision;
			base.NotificationIdentifier = "vote";
			CampaignEvents.OnClanChangedKingdomEvent.AddNonSerializedListener(this, new Action<Clan, Kingdom, Kingdom, ChangeKingdomAction.ChangeKingdomActionDetail, bool>(this.OnClanChangedKingdom));
			CampaignEvents.KingdomDecisionCancelled.AddNonSerializedListener(this, new Action<KingdomDecision, bool>(this.OnDecisionCancelled));
			CampaignEvents.KingdomDecisionConcluded.AddNonSerializedListener(this, new Action<KingdomDecision, DecisionOutcome, bool>(this.OnDecisionConcluded));
			this._onInspect = new Action(this.OnInspect);
			this._onInspectOpenKingdom = delegate
			{
				<>4__this.NavigationHandler.OpenKingdom(data.Decision);
			};
		}

		private void OnInspect()
		{
			if (!this._decision.ShouldBeCancelled())
			{
				Kingdom kingdom = Clan.PlayerClan.Kingdom;
				if (kingdom != null && kingdom.UnresolvedDecisions.Any((KingdomDecision d) => d == this._decision))
				{
					this._onInspectOpenKingdom();
					return;
				}
			}
			InformationManager.ShowInquiry(new InquiryData("", new TextObject("{=i9OsCshW}This kingdom decision is not relevant anymore.", null).ToString(), true, false, GameTexts.FindText("str_ok", null).ToString(), "", null, null, "", 0f, null, null, null), false, false);
			base.ExecuteRemove();
		}

		private void OnDecisionConcluded(KingdomDecision decision, DecisionOutcome arg2, bool arg3)
		{
			if (decision == this._decision)
			{
				base.ExecuteRemove();
			}
		}

		private void OnDecisionCancelled(KingdomDecision decision, bool arg2)
		{
			if (decision == this._decision)
			{
				base.ExecuteRemove();
			}
		}

		private void OnClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail, bool showNotification)
		{
			if (clan == Clan.PlayerClan)
			{
				base.ExecuteRemove();
			}
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			CampaignEvents.OnClanChangedKingdomEvent.ClearListeners(this);
		}

		private KingdomDecision _decision;

		private Kingdom _kingdomOfDecision;

		private Action _onInspectOpenKingdom;
	}
}
