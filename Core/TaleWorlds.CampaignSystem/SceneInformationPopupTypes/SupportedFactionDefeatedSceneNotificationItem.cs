using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.SceneInformationPopupTypes
{
	public class SupportedFactionDefeatedSceneNotificationItem : SceneNotificationData
	{
		public Kingdom Faction { get; }

		public bool PlayerWantsRestore { get; }

		public override string SceneID
		{
			get
			{
				return "scn_supported_faction_defeated_notification";
			}
		}

		public override TextObject TitleText
		{
			get
			{
				GameTexts.SetVariable("FORMAL_NAME", CampaignSceneNotificationHelper.GetFormalNameForKingdom(this.Faction));
				GameTexts.SetVariable("PLAYER_WANTS_RESTORE", this.PlayerWantsRestore ? 1 : 0);
				GameTexts.SetVariable("DAY_OF_YEAR", CampaignSceneNotificationHelper.GetFormalDayAndSeasonText(this._creationCampaignTime));
				GameTexts.SetVariable("YEAR", this._creationCampaignTime.GetYear);
				return GameTexts.FindText("str_supported_faction_defeated", null);
			}
		}

		public override IEnumerable<Banner> GetBanners()
		{
			return new List<Banner>
			{
				this.Faction.Banner,
				this.Faction.Banner
			};
		}

		public SupportedFactionDefeatedSceneNotificationItem(Kingdom faction, bool playerWantsRestore)
		{
			this.Faction = faction;
			this.PlayerWantsRestore = playerWantsRestore;
			this._creationCampaignTime = CampaignTime.Now;
		}

		private readonly CampaignTime _creationCampaignTime;
	}
}
