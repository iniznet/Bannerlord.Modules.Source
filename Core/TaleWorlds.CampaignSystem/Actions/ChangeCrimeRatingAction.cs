using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.Actions
{
	public static class ChangeCrimeRatingAction
	{
		private static void ApplyInternal(IFaction faction, float deltaCrimeRating, bool showNotification)
		{
			float num = MBMath.ClampFloat(faction.MainHeroCrimeRating + deltaCrimeRating, 0f, Campaign.Current.Models.CrimeModel.GetMaxCrimeRating());
			deltaCrimeRating = num - faction.MainHeroCrimeRating;
			if (showNotification && !deltaCrimeRating.ApproximatelyEqualsTo(0f, 1E-05f))
			{
				TextObject textObject = new TextObject("{=hwq0RMRN}Your criminal rating with {FACTION_NAME} has {?IS_INCREASED}increased{?}decreased{\\?} by {CHANGE} to {NEW_RATING}", null);
				textObject.SetTextVariable("CHANGE", MathF.Round(MathF.Abs(deltaCrimeRating)));
				textObject.SetTextVariable("IS_INCREASED", (deltaCrimeRating > 0f) ? 1 : 0);
				textObject.SetTextVariable("FACTION_NAME", faction.Name);
				textObject.SetTextVariable("NEW_RATING", MathF.Round(num));
				MBInformationManager.AddQuickInformation(textObject, 0, null, "");
			}
			faction.MainHeroCrimeRating = num;
			if (num > (float)Campaign.Current.Models.CrimeModel.DeclareWarCrimeRatingThreshold && Hero.MainHero.MapFaction.Leader == Hero.MainHero && !faction.IsAtWarWith(Hero.MainHero.MapFaction) && Hero.MainHero.MapFaction != faction)
			{
				ChangeRelationAction.ApplyPlayerRelation(faction.Leader, -10, true, true);
				DeclareWarAction.ApplyByCrimeRatingChange(faction, Hero.MainHero.MapFaction);
			}
			CampaignEventDispatcher.Instance.OnCrimeRatingChanged(faction, deltaCrimeRating);
		}

		public static void Apply(IFaction faction, float deltaCrimeRating, bool showNotification = true)
		{
			ChangeCrimeRatingAction.ApplyInternal(faction, deltaCrimeRating, showNotification);
		}
	}
}
