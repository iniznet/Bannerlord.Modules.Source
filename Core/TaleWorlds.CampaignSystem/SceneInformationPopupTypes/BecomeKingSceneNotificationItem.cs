using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.SceneInformationPopupTypes
{
	public class BecomeKingSceneNotificationItem : SceneNotificationData
	{
		public Hero NewLeaderHero { get; }

		public override string SceneID
		{
			get
			{
				return "scn_become_king_notification";
			}
		}

		public override TextObject TitleText
		{
			get
			{
				TextObject textObject;
				if (this.NewLeaderHero.Clan.Kingdom.Culture.StringId.Equals("empire", StringComparison.InvariantCultureIgnoreCase))
				{
					textObject = GameTexts.FindText("str_become_king_empire", null);
				}
				else
				{
					TextObject textObject2 = (this.NewLeaderHero.IsFemale ? GameTexts.FindText("str_liege_title_female", this.NewLeaderHero.Clan.Kingdom.Culture.StringId) : GameTexts.FindText("str_liege_title", this.NewLeaderHero.Clan.Kingdom.Culture.StringId));
					textObject = GameTexts.FindText("str_become_king_nonempire", null);
					textObject.SetTextVariable("TITLE_NAME", textObject2);
				}
				textObject.SetTextVariable("DAY_OF_YEAR", CampaignSceneNotificationHelper.GetFormalDayAndSeasonText(this._creationCampaignTime));
				textObject.SetTextVariable("YEAR", this._creationCampaignTime.GetYear);
				textObject.SetTextVariable("KING_NAME", this.NewLeaderHero.Name);
				textObject.SetTextVariable("IS_KING_MALE", this.NewLeaderHero.IsFemale ? 0 : 1);
				return textObject;
			}
		}

		public override IEnumerable<SceneNotificationData.SceneNotificationCharacter> GetSceneNotificationCharacters()
		{
			Equipment equipment = this.NewLeaderHero.CharacterObject.Equipment.Clone(true);
			List<SceneNotificationData.SceneNotificationCharacter> list = new List<SceneNotificationData.SceneNotificationCharacter>();
			list.Add(new SceneNotificationData.SceneNotificationCharacter(this.NewLeaderHero.CharacterObject, equipment, default(BodyProperties), false, uint.MaxValue, uint.MaxValue, false));
			for (int i = 0; i < 14; i++)
			{
				CharacterObject characterObject = (this.IsAudienceFemale(i) ? this.NewLeaderHero.Clan.Kingdom.Culture.Townswoman : this.NewLeaderHero.Clan.Kingdom.Culture.Townsman);
				Equipment equipment2 = characterObject.FirstCivilianEquipment.Clone(false);
				CampaignSceneNotificationHelper.RemoveWeaponsFromEquipment(ref equipment2, true, false);
				uint color = BannerManager.Instance.ReadOnlyColorPalette.GetRandomElementInefficiently<KeyValuePair<int, BannerColor>>().Value.Color;
				uint color2 = BannerManager.Instance.ReadOnlyColorPalette.GetRandomElementInefficiently<KeyValuePair<int, BannerColor>>().Value.Color;
				list.Add(new SceneNotificationData.SceneNotificationCharacter(characterObject, equipment2, characterObject.GetBodyProperties(equipment2, MBRandom.RandomInt(100)), false, color, color2, false));
			}
			for (int j = 0; j < 2; j++)
			{
				list.Add(CampaignSceneNotificationHelper.GetBodyguardOfCulture(this.NewLeaderHero.Clan.Kingdom.MapFaction.Culture));
			}
			foreach (Hero hero in CampaignSceneNotificationHelper.GetMilitaryAudienceForHero(this.NewLeaderHero, false, false).Take(4))
			{
				Equipment equipment3 = hero.CivilianEquipment.Clone(false);
				CampaignSceneNotificationHelper.RemoveWeaponsFromEquipment(ref equipment3, false, false);
				list.Add(CampaignSceneNotificationHelper.CreateNotificationCharacterFromHero(hero, equipment3, false, default(BodyProperties), uint.MaxValue, uint.MaxValue, false));
			}
			return list;
		}

		public override IEnumerable<Banner> GetBanners()
		{
			return new List<Banner>
			{
				this.NewLeaderHero.Clan.Kingdom.Banner,
				this.NewLeaderHero.Clan.Kingdom.Banner
			};
		}

		public BecomeKingSceneNotificationItem(Hero newLeaderHero)
		{
			this.NewLeaderHero = newLeaderHero;
			this._creationCampaignTime = CampaignTime.Now;
		}

		private bool IsAudienceFemale(int indexOfAudience)
		{
			return indexOfAudience == 2 || indexOfAudience == 5 || indexOfAudience - 11 <= 2;
		}

		private const int NumberOfAudience = 14;

		private const int NumberOfGuards = 2;

		private const int NumberOfCompanions = 4;

		private readonly CampaignTime _creationCampaignTime;
	}
}
