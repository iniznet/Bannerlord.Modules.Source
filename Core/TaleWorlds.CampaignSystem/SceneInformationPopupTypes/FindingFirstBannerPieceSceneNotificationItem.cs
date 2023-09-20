using System;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.SceneInformationPopupTypes
{
	// Token: 0x020000B4 RID: 180
	public class FindingFirstBannerPieceSceneNotificationItem : SceneNotificationData
	{
		// Token: 0x170004F2 RID: 1266
		// (get) Token: 0x060011E0 RID: 4576 RVA: 0x00051EF4 File Offset: 0x000500F4
		public Hero PlayerHero { get; }

		// Token: 0x170004F3 RID: 1267
		// (get) Token: 0x060011E1 RID: 4577 RVA: 0x00051EFC File Offset: 0x000500FC
		public override string SceneID
		{
			get
			{
				return "scn_first_banner_piece_notification";
			}
		}

		// Token: 0x170004F4 RID: 1268
		// (get) Token: 0x060011E2 RID: 4578 RVA: 0x00051F04 File Offset: 0x00050104
		public override TextObject TitleText
		{
			get
			{
				GameTexts.SetVariable("DAY_OF_YEAR", CampaignSceneNotificationHelper.GetFormalDayAndSeasonText(this._creationCampaignTime));
				GameTexts.SetVariable("YEAR", this._creationCampaignTime.GetYear);
				return GameTexts.FindText("str_first_banner_piece_found", null);
			}
		}

		// Token: 0x060011E3 RID: 4579 RVA: 0x00051F49 File Offset: 0x00050149
		public override void OnCloseAction()
		{
			base.OnCloseAction();
			Action onCloseAction = this._onCloseAction;
			if (onCloseAction == null)
			{
				return;
			}
			onCloseAction();
		}

		// Token: 0x060011E4 RID: 4580 RVA: 0x00051F61 File Offset: 0x00050161
		public FindingFirstBannerPieceSceneNotificationItem(Hero playerHero, Action onCloseAction = null)
		{
			this.PlayerHero = playerHero;
			this._creationCampaignTime = CampaignTime.Now;
			this._onCloseAction = onCloseAction;
		}

		// Token: 0x0400063C RID: 1596
		private readonly Action _onCloseAction;

		// Token: 0x0400063D RID: 1597
		private readonly CampaignTime _creationCampaignTime;
	}
}
