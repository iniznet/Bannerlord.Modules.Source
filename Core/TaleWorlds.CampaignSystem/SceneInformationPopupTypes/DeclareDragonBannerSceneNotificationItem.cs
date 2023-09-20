using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.CampaignSystem.SceneInformationPopupTypes
{
	public class DeclareDragonBannerSceneNotificationItem : SceneNotificationData
	{
		public bool PlayerWantsToRestore { get; }

		public override string SceneID
		{
			get
			{
				return "scn_cutscene_declare_dragon_banner";
			}
		}

		public override TextObject TitleText
		{
			get
			{
				GameTexts.SetVariable("PLAYER_WANTS_RESTORE", this.PlayerWantsToRestore ? 1 : 0);
				GameTexts.SetVariable("DAY_OF_YEAR", CampaignSceneNotificationHelper.GetFormalDayAndSeasonText(this._creationCampaignTime));
				GameTexts.SetVariable("YEAR", this._creationCampaignTime.GetYear);
				return GameTexts.FindText("str_declare_dragon_banner", null);
			}
		}

		public override IEnumerable<SceneNotificationData.SceneNotificationCharacter> GetSceneNotificationCharacters()
		{
			List<SceneNotificationData.SceneNotificationCharacter> list = new List<SceneNotificationData.SceneNotificationCharacter>();
			IOrderedEnumerable<Hero> orderedEnumerable = from h in Hero.MainHero.Clan.Heroes
				where !h.IsChild && h.IsAlive && h != Hero.MainHero
				orderby h.Level
				select h;
			for (int i = 0; i < 17; i++)
			{
				SceneNotificationData.SceneNotificationCharacter characterAtIndex = this.GetCharacterAtIndex(i, orderedEnumerable);
				list.Add(characterAtIndex);
			}
			return list;
		}

		public override IEnumerable<Banner> GetBanners()
		{
			return new List<Banner> { Hero.MainHero.ClanBanner };
		}

		public DeclareDragonBannerSceneNotificationItem(bool playerWantsToRestore)
		{
			this.PlayerWantsToRestore = playerWantsToRestore;
			this._creationCampaignTime = CampaignTime.Now;
		}

		private SceneNotificationData.SceneNotificationCharacter GetCharacterAtIndex(int index, IOrderedEnumerable<Hero> clanHeroesPool)
		{
			bool flag = false;
			int num = -1;
			string text = string.Empty;
			switch (index)
			{
			case 0:
				text = "battanian_picked_warrior";
				num = 0;
				break;
			case 1:
				text = "imperial_infantryman";
				break;
			case 2:
				text = "imperial_veteran_infantryman";
				break;
			case 3:
				text = "sturgian_warrior";
				num = 1;
				break;
			case 4:
				text = "imperial_menavliaton";
				break;
			case 5:
				text = "sturgian_ulfhednar";
				num = 2;
				break;
			case 6:
				text = "aserai_recruit";
				break;
			case 7:
				text = "aserai_skirmisher";
				break;
			case 8:
				text = "aserai_veteran_faris";
				break;
			case 9:
				text = "imperial_legionary";
				num = 3;
				break;
			case 10:
				text = "mountain_bandits_bandit";
				break;
			case 11:
				text = "mountain_bandits_chief";
				break;
			case 12:
				text = "forest_people_tier_3";
				num = 4;
				break;
			case 13:
				text = "mountain_bandits_raider";
				break;
			case 14:
				flag = true;
				break;
			case 15:
				text = "vlandian_pikeman";
				break;
			case 16:
				text = "vlandian_voulgier";
				break;
			}
			uint num2 = uint.MaxValue;
			uint num3 = uint.MaxValue;
			CharacterObject characterObject;
			if (flag)
			{
				characterObject = CharacterObject.PlayerCharacter;
				num2 = Hero.MainHero.MapFaction.Color;
				num3 = Hero.MainHero.MapFaction.Color2;
			}
			else if (num != -1 && clanHeroesPool.ElementAtOrDefault(num) != null)
			{
				Hero hero = clanHeroesPool.ElementAtOrDefault(num);
				characterObject = hero.CharacterObject;
				num2 = hero.MapFaction.Color;
				num3 = hero.MapFaction.Color2;
			}
			else
			{
				characterObject = MBObjectManager.Instance.GetObject<CharacterObject>(text);
			}
			Equipment equipment = characterObject.FirstBattleEquipment.Clone(false);
			CampaignSceneNotificationHelper.RemoveWeaponsFromEquipment(ref equipment, true, false);
			return new SceneNotificationData.SceneNotificationCharacter(characterObject, equipment, default(BodyProperties), false, num2, num3, false);
		}

		private const int NumberOfCharacters = 17;

		private readonly CampaignTime _creationCampaignTime;
	}
}
