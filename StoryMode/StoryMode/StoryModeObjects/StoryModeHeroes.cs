using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace StoryMode.StoryModeObjects
{
	public class StoryModeHeroes
	{
		public static Hero ElderBrother
		{
			get
			{
				return StoryModeManager.Current.StoryModeHeroes._elderBrother;
			}
		}

		public static Hero LittleBrother
		{
			get
			{
				return StoryModeManager.Current.StoryModeHeroes._littleBrother;
			}
		}

		public static Hero LittleSister
		{
			get
			{
				return StoryModeManager.Current.StoryModeHeroes._littleSister;
			}
		}

		public static Hero Tacitus
		{
			get
			{
				return StoryModeManager.Current.StoryModeHeroes._tacitus;
			}
		}

		public static Hero Radagos
		{
			get
			{
				return StoryModeManager.Current.StoryModeHeroes._radagos;
			}
		}

		public static Hero ImperialMentor
		{
			get
			{
				return StoryModeManager.Current.StoryModeHeroes._imperialMentor;
			}
		}

		public static Hero AntiImperialMentor
		{
			get
			{
				return StoryModeManager.Current.StoryModeHeroes._antiImperialMentor;
			}
		}

		public static Hero RadagosHencman
		{
			get
			{
				return StoryModeManager.Current.StoryModeHeroes._radagosHenchman;
			}
		}

		internal StoryModeHeroes()
		{
			this.RegisterAll();
		}

		private void RegisterAll()
		{
			Clan clan = Campaign.Current.CampaignObjectManager.Find<Clan>("player_faction");
			CharacterObject @object = Game.Current.ObjectManager.GetObject<CharacterObject>("main_hero_mother");
			CharacterObject object2 = Game.Current.ObjectManager.GetObject<CharacterObject>("main_hero_father");
			if (HeroCreator.CreateBasicHero(MBObjectManager.Instance.GetObject<CharacterObject>("tutorial_npc_brother"), ref this._elderBrother, "tutorial_npc_brother"))
			{
				this._elderBrother.Clan = clan;
				TextObject textObject = GameTexts.FindText("str_player_brother_name", @object.Culture.StringId);
				this._elderBrother.SetName(textObject, textObject);
				this._elderBrother.Mother = @object.HeroObject;
				this._elderBrother.Father = object2.HeroObject;
			}
			if (HeroCreator.CreateBasicHero(MBObjectManager.Instance.GetObject<CharacterObject>("storymode_little_brother"), ref this._littleBrother, "storymode_little_brother"))
			{
				TextObject textObject2 = GameTexts.FindText("str_player_little_brother_name", @object.Culture.StringId);
				this._littleBrother.SetName(textObject2, textObject2);
				this._littleBrother.Mother = @object.HeroObject;
				this._littleBrother.Father = object2.HeroObject;
			}
			if (HeroCreator.CreateBasicHero(MBObjectManager.Instance.GetObject<CharacterObject>("storymode_little_sister"), ref this._littleSister, "storymode_little_sister"))
			{
				TextObject textObject3 = GameTexts.FindText("str_player_little_sister_name", @object.Culture.StringId);
				this._littleSister.SetName(textObject3, textObject3);
				this._littleSister.Mother = @object.HeroObject;
				this._littleSister.Father = object2.HeroObject;
			}
			HeroCreator.CreateBasicHero(MBObjectManager.Instance.GetObject<CharacterObject>("tutorial_npc_tacitus"), ref this._tacitus, "tutorial_npc_tacitus");
			HeroCreator.CreateBasicHero(MBObjectManager.Instance.GetObject<CharacterObject>("tutorial_npc_radagos"), ref this._radagos, "tutorial_npc_radagos");
			HeroCreator.CreateBasicHero(MBObjectManager.Instance.GetObject<CharacterObject>("storymode_imperial_mentor_istiana"), ref this._imperialMentor, "storymode_imperial_mentor_istiana");
			HeroCreator.CreateBasicHero(MBObjectManager.Instance.GetObject<CharacterObject>("storymode_imperial_mentor_arzagos"), ref this._antiImperialMentor, "storymode_imperial_mentor_arzagos");
			HeroCreator.CreateBasicHero(MBObjectManager.Instance.GetObject<CharacterObject>("radagos_henchman"), ref this._radagosHenchman, "radagos_henchman");
		}

		private const string BrotherStringId = "tutorial_npc_brother";

		private const string LittleBrotherStringId = "storymode_little_brother";

		private const string LittleSisterStringId = "storymode_little_sister";

		private const string TacitusStringId = "tutorial_npc_tacitus";

		private const string RadagosStringId = "tutorial_npc_radagos";

		private const string IstianaStringId = "storymode_imperial_mentor_istiana";

		private const string ArzagosStringId = "storymode_imperial_mentor_arzagos";

		private const string GalterStringId = "radagos_henchman";

		private Hero _elderBrother;

		private Hero _littleBrother;

		private Hero _littleSister;

		private Hero _tacitus;

		private Hero _radagos;

		private Hero _imperialMentor;

		private Hero _antiImperialMentor;

		private Hero _radagosHenchman;
	}
}
