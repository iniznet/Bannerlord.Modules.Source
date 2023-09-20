using System;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.MapNotificationTypes;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapNotificationTypes
{
	public class EducationNotificationItemVM : MapNotificationItemBaseVM
	{
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

		private void OnEducationCompletedForChild(Hero child, int age)
		{
			if (child == this._child && age >= this._age)
			{
				base.ExecuteRemove();
			}
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			CampaignEvents.ChildEducationCompletedEvent.ClearListeners(this);
		}

		private readonly Hero _child;

		private readonly int _age;
	}
}
