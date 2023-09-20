using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.SceneInformationPopupTypes
{
	public class JoinKingdomSceneNotificationItem : SceneNotificationData
	{
		public Clan NewMemberClan { get; }

		public Kingdom KingdomToUse { get; }

		public override string SceneID
		{
			get
			{
				return "scn_cutscene_factionjoin";
			}
		}

		public override SceneNotificationData.RelevantContextType RelevantContext
		{
			get
			{
				return SceneNotificationData.RelevantContextType.Map;
			}
		}

		public override TextObject TitleText
		{
			get
			{
				GameTexts.SetVariable("CLAN_NAME", this.NewMemberClan.Name);
				GameTexts.SetVariable("DAY_OF_YEAR", CampaignSceneNotificationHelper.GetFormalDayAndSeasonText(this._creationCampaignTime));
				GameTexts.SetVariable("YEAR", this._creationCampaignTime.GetYear);
				GameTexts.SetVariable("KINGDOM_FORMALNAME", CampaignSceneNotificationHelper.GetFormalNameForKingdom(this.KingdomToUse));
				return GameTexts.FindText("str_new_faction_member", null);
			}
		}

		public override IEnumerable<Banner> GetBanners()
		{
			return new List<Banner>
			{
				this.KingdomToUse.Banner,
				this.KingdomToUse.Banner
			};
		}

		public override IEnumerable<SceneNotificationData.SceneNotificationCharacter> GetSceneNotificationCharacters()
		{
			List<SceneNotificationData.SceneNotificationCharacter> list = new List<SceneNotificationData.SceneNotificationCharacter>();
			Hero leader = this.NewMemberClan.Leader;
			Equipment equipment = leader.CivilianEquipment.Clone(false);
			CampaignSceneNotificationHelper.RemoveWeaponsFromEquipment(ref equipment, true, false);
			list.Add(CampaignSceneNotificationHelper.CreateNotificationCharacterFromHero(leader, equipment, false, default(BodyProperties), uint.MaxValue, uint.MaxValue, false));
			foreach (Hero hero in CampaignSceneNotificationHelper.GetMilitaryAudienceForKingdom(this.KingdomToUse, true).Take(5))
			{
				Equipment equipment2 = hero.CivilianEquipment.Clone(false);
				CampaignSceneNotificationHelper.RemoveWeaponsFromEquipment(ref equipment2, true, false);
				list.Add(CampaignSceneNotificationHelper.CreateNotificationCharacterFromHero(hero, equipment2, false, default(BodyProperties), uint.MaxValue, uint.MaxValue, false));
			}
			return list;
		}

		public JoinKingdomSceneNotificationItem(Clan newMember, Kingdom kingdom)
		{
			this.NewMemberClan = newMember;
			this.KingdomToUse = kingdom;
			this._creationCampaignTime = CampaignTime.Now;
		}

		private const int NumberOfKingdomMembers = 5;

		private readonly CampaignTime _creationCampaignTime;
	}
}
