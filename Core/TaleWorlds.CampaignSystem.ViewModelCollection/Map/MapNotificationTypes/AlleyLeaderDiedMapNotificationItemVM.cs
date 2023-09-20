using System;
using TaleWorlds.CampaignSystem.MapNotificationTypes;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapNotificationTypes
{
	// Token: 0x02000033 RID: 51
	public class AlleyLeaderDiedMapNotificationItemVM : MapNotificationItemBaseVM
	{
		// Token: 0x060004FC RID: 1276 RVA: 0x00019B1A File Offset: 0x00017D1A
		public AlleyLeaderDiedMapNotificationItemVM(AlleyLeaderDiedMapNotification data)
			: base(data)
		{
			this._alley = data.Alley;
			base.NotificationIdentifier = "alley_leader_died";
			this._onInspect = new Action(this.CreateAlleyLeaderDiedPopUp);
		}

		// Token: 0x060004FD RID: 1277 RVA: 0x00019B4C File Offset: 0x00017D4C
		private void CreateAlleyLeaderDiedPopUp()
		{
			object obj = new TextObject("{=6QoSHiWC}An alley without a leader", null);
			TextObject textObject = new TextObject("{=FzbeSkBb}One of your alleys has lost its leader or is lacking troops. It will be abandoned after {DAYS} days have passed. You can assign a new clan member from the clan screen or travel to the alley to add more troops, if you wish to keep it. Any troops left in the alley will be lost when it is abandoned.", null);
			textObject.SetTextVariable("DAYS", (int)Campaign.Current.Models.AlleyModel.DestroyAlleyAfterDaysWhenLeaderIsDeath.ToDays);
			TextObject textObject2 = new TextObject("{=jVLJTuwl}Learn more", null);
			InformationManager.ShowInquiry(new InquiryData(obj.ToString(), textObject.ToString(), true, true, textObject2.ToString(), GameTexts.FindText("str_dismiss", null).ToString(), new Action(this.OpenClanScreenAfterAlleyLeaderDeath), new Action(base.ExecuteRemove), "", 0f, null, null, null), false, false);
		}

		// Token: 0x060004FE RID: 1278 RVA: 0x00019BF9 File Offset: 0x00017DF9
		private void OpenClanScreenAfterAlleyLeaderDeath()
		{
			if (base.NavigationHandler != null && this._alley != null)
			{
				base.NavigationHandler.OpenClan(this._alley);
				base.ExecuteRemove();
			}
		}

		// Token: 0x0400021C RID: 540
		private Alley _alley;
	}
}
