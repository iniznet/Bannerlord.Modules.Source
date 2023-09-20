using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.SceneInformationPopupTypes
{
	public class FindingThirdBannerPieceSceneNotificationItem : SceneNotificationData
	{
		public override string SceneID
		{
			get
			{
				return "scn_third_banner_piece_notification";
			}
		}

		public override bool IsAffirmativeOptionShown { get; }

		public override TextObject TitleText
		{
			get
			{
				GameTexts.SetVariable("DAY_OF_YEAR", CampaignSceneNotificationHelper.GetFormalDayAndSeasonText(this._creationCampaignTime));
				GameTexts.SetVariable("YEAR", this._creationCampaignTime.GetYear);
				return GameTexts.FindText("str_third_banner_piece_found", null);
			}
		}

		public override TextObject AffirmativeTitleText
		{
			get
			{
				return GameTexts.FindText("str_third_banner_piece_found_assembled", null);
			}
		}

		public override TextObject AffirmativeText
		{
			get
			{
				return new TextObject("{=6mgapvxb}Assemble", null);
			}
		}

		public override TextObject AffirmativeDescriptionText
		{
			get
			{
				return new TextObject("{=IRLB42FY}Assemble the dragon banner!", null);
			}
		}

		public override IEnumerable<Banner> GetBanners()
		{
			return new List<Banner> { Hero.MainHero.ClanBanner };
		}

		public FindingThirdBannerPieceSceneNotificationItem()
		{
			this.IsAffirmativeOptionShown = true;
			this._creationCampaignTime = CampaignTime.Now;
		}

		private readonly CampaignTime _creationCampaignTime;
	}
}
