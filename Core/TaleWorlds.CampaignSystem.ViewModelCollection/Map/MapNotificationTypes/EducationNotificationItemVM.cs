using System;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.MapNotificationTypes;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapNotificationTypes
{
	// Token: 0x02000038 RID: 56
	public class EducationNotificationItemVM : MapNotificationItemBaseVM
	{
		// Token: 0x0600050A RID: 1290 RVA: 0x00019EBC File Offset: 0x000180BC
		public EducationNotificationItemVM(EducationMapNotification data)
			: base(data)
		{
			base.NotificationIdentifier = "education";
			base.ForceInspection = true;
			this._child = data.Child;
			this._age = data.Age;
			this._onInspect = new Action(this.OnInspect);
			CampaignEvents.ChildEducationCompletedEvent.AddNonSerializedListener(this, new Action<Hero, int>(this.OnEducationCompletedForChild));
		}

		// Token: 0x0600050B RID: 1291 RVA: 0x00019F24 File Offset: 0x00018124
		private void OnInspect()
		{
			EducationMapNotification educationMapNotification = (EducationMapNotification)base.Data;
			if (educationMapNotification != null && !educationMapNotification.IsValid())
			{
				InformationManager.ShowInquiry(new InquiryData("", new TextObject("{=wGWYNYYX}This education stage is no longer relevant.", null).ToString(), true, false, GameTexts.FindText("str_ok", null).ToString(), "", null, null, "", 0f, null, null, null), false, false);
				base.ExecuteRemove();
				return;
			}
			Game.Current.GameStateManager.PushState(Game.Current.GameStateManager.CreateState<EducationState>(new object[] { this._child }), 0);
		}

		// Token: 0x0600050C RID: 1292 RVA: 0x00019FC4 File Offset: 0x000181C4
		private void OnEducationCompletedForChild(Hero child, int age)
		{
			if (child == this._child && age >= this._age)
			{
				base.ExecuteRemove();
			}
		}

		// Token: 0x0600050D RID: 1293 RVA: 0x00019FDE File Offset: 0x000181DE
		public override void OnFinalize()
		{
			base.OnFinalize();
			CampaignEvents.ChildEducationCompletedEvent.ClearListeners(this);
		}

		// Token: 0x0400021F RID: 543
		private readonly Hero _child;

		// Token: 0x04000220 RID: 544
		private readonly int _age;
	}
}
