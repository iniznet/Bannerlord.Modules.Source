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
	// Token: 0x0200003B RID: 59
	public class KingdomVoteNotificationItemVM : MapNotificationItemBaseVM
	{
		// Token: 0x06000512 RID: 1298 RVA: 0x0001A0F0 File Offset: 0x000182F0
		public KingdomVoteNotificationItemVM(KingdomDecisionMapNotification data)
			: base(data)
		{
			KingdomVoteNotificationItemVM <>4__this = this;
			this._decision = data.Decision;
			this._kingdomOfDecision = data.KingdomOfDecision;
			base.NotificationIdentifier = "vote";
			CampaignEvents.ClanChangedKingdom.AddNonSerializedListener(this, new Action<Clan, Kingdom, Kingdom, ChangeKingdomAction.ChangeKingdomActionDetail, bool>(this.OnClanChangedKingdom));
			CampaignEvents.KingdomDecisionCancelled.AddNonSerializedListener(this, new Action<KingdomDecision, bool>(this.OnDecisionCancelled));
			CampaignEvents.KingdomDecisionConcluded.AddNonSerializedListener(this, new Action<KingdomDecision, DecisionOutcome, bool>(this.OnDecisionConcluded));
			this._onInspect = new Action(this.OnInspect);
			this._onInspectOpenKingdom = delegate
			{
				<>4__this.NavigationHandler.OpenKingdom(data.KingdomOfDecision);
			};
		}

		// Token: 0x06000513 RID: 1299 RVA: 0x0001A1B4 File Offset: 0x000183B4
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

		// Token: 0x06000514 RID: 1300 RVA: 0x0001A253 File Offset: 0x00018453
		private void OnDecisionConcluded(KingdomDecision decision, DecisionOutcome arg2, bool arg3)
		{
			if (decision == this._decision)
			{
				base.ExecuteRemove();
			}
		}

		// Token: 0x06000515 RID: 1301 RVA: 0x0001A264 File Offset: 0x00018464
		private void OnDecisionCancelled(KingdomDecision decision, bool arg2)
		{
			if (decision == this._decision)
			{
				base.ExecuteRemove();
			}
		}

		// Token: 0x06000516 RID: 1302 RVA: 0x0001A275 File Offset: 0x00018475
		private void OnClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail, bool showNotification)
		{
			if (clan == Clan.PlayerClan)
			{
				base.ExecuteRemove();
			}
		}

		// Token: 0x06000517 RID: 1303 RVA: 0x0001A285 File Offset: 0x00018485
		public override void OnFinalize()
		{
			base.OnFinalize();
			CampaignEvents.ClanChangedKingdom.ClearListeners(this);
		}

		// Token: 0x04000221 RID: 545
		private KingdomDecision _decision;

		// Token: 0x04000222 RID: 546
		private Kingdom _kingdomOfDecision;

		// Token: 0x04000223 RID: 547
		private Action _onInspectOpenKingdom;
	}
}
