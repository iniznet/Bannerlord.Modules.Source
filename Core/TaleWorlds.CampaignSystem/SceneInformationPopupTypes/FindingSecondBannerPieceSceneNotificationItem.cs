using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.SceneInformationPopupTypes
{
	// Token: 0x020000B5 RID: 181
	public class FindingSecondBannerPieceSceneNotificationItem : SceneNotificationData
	{
		// Token: 0x170004F5 RID: 1269
		// (get) Token: 0x060011E5 RID: 4581 RVA: 0x00051F82 File Offset: 0x00050182
		public Hero PlayerHero { get; }

		// Token: 0x170004F6 RID: 1270
		// (get) Token: 0x060011E6 RID: 4582 RVA: 0x00051F8A File Offset: 0x0005018A
		public override string SceneID
		{
			get
			{
				return "scn_second_banner_piece_notification";
			}
		}

		// Token: 0x170004F7 RID: 1271
		// (get) Token: 0x060011E7 RID: 4583 RVA: 0x00051F94 File Offset: 0x00050194
		public override TextObject TitleText
		{
			get
			{
				GameTexts.SetVariable("DAY_OF_YEAR", CampaignSceneNotificationHelper.GetFormalDayAndSeasonText(this._creationCampaignTime));
				GameTexts.SetVariable("YEAR", this._creationCampaignTime.GetYear);
				return GameTexts.FindText("str_second_banner_piece_found", null);
			}
		}

		// Token: 0x060011E8 RID: 4584 RVA: 0x00051FD9 File Offset: 0x000501D9
		public override IEnumerable<Banner> GetBanners()
		{
			return new List<Banner> { this.PlayerHero.ClanBanner };
		}

		// Token: 0x060011E9 RID: 4585 RVA: 0x00051FF1 File Offset: 0x000501F1
		public FindingSecondBannerPieceSceneNotificationItem(Hero playerHero)
		{
			this.PlayerHero = playerHero;
			this._creationCampaignTime = CampaignTime.Now;
		}

		// Token: 0x0400063F RID: 1599
		private readonly CampaignTime _creationCampaignTime;
	}
}
