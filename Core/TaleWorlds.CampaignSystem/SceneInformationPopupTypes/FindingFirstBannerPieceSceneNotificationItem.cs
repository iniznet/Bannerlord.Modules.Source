using System;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.SceneInformationPopupTypes
{
	public class FindingFirstBannerPieceSceneNotificationItem : SceneNotificationData
	{
		public Hero PlayerHero { get; }

		public override string SceneID
		{
			get
			{
				return "scn_first_banner_piece_notification";
			}
		}

		public override TextObject TitleText
		{
			get
			{
				GameTexts.SetVariable("DAY_OF_YEAR", CampaignSceneNotificationHelper.GetFormalDayAndSeasonText(this._creationCampaignTime));
				GameTexts.SetVariable("YEAR", this._creationCampaignTime.GetYear);
				return GameTexts.FindText("str_first_banner_piece_found", null);
			}
		}

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

		public FindingFirstBannerPieceSceneNotificationItem(Hero playerHero, Action onCloseAction = null)
		{
			this.PlayerHero = playerHero;
			this._creationCampaignTime = CampaignTime.Now;
			this._onCloseAction = onCloseAction;
		}

		private readonly Action _onCloseAction;

		private readonly CampaignTime _creationCampaignTime;
	}
}
