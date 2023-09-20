using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.SceneInformationPopupTypes
{
	public class FindingSecondBannerPieceSceneNotificationItem : SceneNotificationData
	{
		public Hero PlayerHero { get; }

		public override string SceneID
		{
			get
			{
				return "scn_second_banner_piece_notification";
			}
		}

		public override TextObject TitleText
		{
			get
			{
				GameTexts.SetVariable("DAY_OF_YEAR", CampaignSceneNotificationHelper.GetFormalDayAndSeasonText(this._creationCampaignTime));
				GameTexts.SetVariable("YEAR", this._creationCampaignTime.GetYear);
				return GameTexts.FindText("str_second_banner_piece_found", null);
			}
		}

		public override IEnumerable<Banner> GetBanners()
		{
			return new List<Banner> { this.PlayerHero.ClanBanner };
		}

		public FindingSecondBannerPieceSceneNotificationItem(Hero playerHero)
		{
			this.PlayerHero = playerHero;
			this._creationCampaignTime = CampaignTime.Now;
		}

		private readonly CampaignTime _creationCampaignTime;
	}
}
