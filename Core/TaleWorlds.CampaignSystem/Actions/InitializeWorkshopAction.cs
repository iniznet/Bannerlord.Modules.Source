using System;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.Actions
{
	public static class InitializeWorkshopAction
	{
		public static void ApplyByNewGame(Workshop workshop, Hero workshopOwner, WorkshopType workshopType)
		{
			workshop.InitializeWorkshop(workshopOwner, workshopType);
			TextObject textObject;
			TextObject textObject2;
			NameGenerator.Current.GenerateHeroNameAndHeroFullName(workshopOwner, out textObject, out textObject2, true);
			workshopOwner.SetName(textObject2, textObject);
			CampaignEventDispatcher.Instance.OnWorkshopInitialized(workshop);
		}
	}
}
