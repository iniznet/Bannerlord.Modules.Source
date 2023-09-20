using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace StoryMode.StoryModeObjects
{
	// Token: 0x02000017 RID: 23
	public class StoryModeHeroes
	{
		// Token: 0x1700002C RID: 44
		// (get) Token: 0x060000A4 RID: 164 RVA: 0x00005016 File Offset: 0x00003216
		public static Hero ElderBrother
		{
			get
			{
				return StoryModeManager.Current.StoryModeHeroes._elderBrother;
			}
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x060000A5 RID: 165 RVA: 0x00005027 File Offset: 0x00003227
		public static Hero LittleBrother
		{
			get
			{
				return StoryModeManager.Current.StoryModeHeroes._littleBrother;
			}
		}

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x060000A6 RID: 166 RVA: 0x00005038 File Offset: 0x00003238
		public static Hero LittleSister
		{
			get
			{
				return StoryModeManager.Current.StoryModeHeroes._littleSister;
			}
		}

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x060000A7 RID: 167 RVA: 0x00005049 File Offset: 0x00003249
		public static Hero Tacitus
		{
			get
			{
				return StoryModeManager.Current.StoryModeHeroes._tacitus;
			}
		}

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x060000A8 RID: 168 RVA: 0x0000505A File Offset: 0x0000325A
		public static Hero Radagos
		{
			get
			{
				return StoryModeManager.Current.StoryModeHeroes._radagos;
			}
		}

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x060000A9 RID: 169 RVA: 0x0000506B File Offset: 0x0000326B
		public static Hero ImperialMentor
		{
			get
			{
				return StoryModeManager.Current.StoryModeHeroes._imperialMentor;
			}
		}

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x060000AA RID: 170 RVA: 0x0000507C File Offset: 0x0000327C
		public static Hero AntiImperialMentor
		{
			get
			{
				return StoryModeManager.Current.StoryModeHeroes._antiImperialMentor;
			}
		}

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x060000AB RID: 171 RVA: 0x0000508D File Offset: 0x0000328D
		public static Hero RadagosHencman
		{
			get
			{
				return StoryModeManager.Current.StoryModeHeroes._radagosHenchman;
			}
		}

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x060000AC RID: 172 RVA: 0x0000509E File Offset: 0x0000329E
		public static Clan RadiersClan
		{
			get
			{
				return Clan.BanditFactions.FirstOrDefault((Clan x) => x.StringId == "mountain_bandits");
			}
		}

		// Token: 0x060000AD RID: 173 RVA: 0x000050C9 File Offset: 0x000032C9
		internal StoryModeHeroes()
		{
			this.RegisterAll();
		}

		// Token: 0x060000AE RID: 174 RVA: 0x000050D8 File Offset: 0x000032D8
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

		// Token: 0x04000031 RID: 49
		private const string BrotherStringId = "tutorial_npc_brother";

		// Token: 0x04000032 RID: 50
		private const string LittleBrotherStringId = "storymode_little_brother";

		// Token: 0x04000033 RID: 51
		private const string LittleSisterStringId = "storymode_little_sister";

		// Token: 0x04000034 RID: 52
		private const string TacitusStringId = "tutorial_npc_tacitus";

		// Token: 0x04000035 RID: 53
		private const string RadagosStringId = "tutorial_npc_radagos";

		// Token: 0x04000036 RID: 54
		private const string IstianaStringId = "storymode_imperial_mentor_istiana";

		// Token: 0x04000037 RID: 55
		private const string ArzagosStringId = "storymode_imperial_mentor_arzagos";

		// Token: 0x04000038 RID: 56
		private const string GalterStringId = "radagos_henchman";

		// Token: 0x04000039 RID: 57
		private Hero _elderBrother;

		// Token: 0x0400003A RID: 58
		private Hero _littleBrother;

		// Token: 0x0400003B RID: 59
		private Hero _littleSister;

		// Token: 0x0400003C RID: 60
		private Hero _tacitus;

		// Token: 0x0400003D RID: 61
		private Hero _radagos;

		// Token: 0x0400003E RID: 62
		private Hero _imperialMentor;

		// Token: 0x0400003F RID: 63
		private Hero _antiImperialMentor;

		// Token: 0x04000040 RID: 64
		private Hero _radagosHenchman;
	}
}
