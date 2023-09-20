using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.CharacterDevelopment
{
	// Token: 0x02000351 RID: 849
	public class DefaultTraits
	{
		// Token: 0x17000B50 RID: 2896
		// (get) Token: 0x06002F95 RID: 12181 RVA: 0x000CA4F3 File Offset: 0x000C86F3
		private static DefaultTraits Instance
		{
			get
			{
				return Campaign.Current.DefaultTraits;
			}
		}

		// Token: 0x17000B51 RID: 2897
		// (get) Token: 0x06002F96 RID: 12182 RVA: 0x000CA4FF File Offset: 0x000C86FF
		public static TraitObject Frequency
		{
			get
			{
				return DefaultTraits.Instance._traitFrequency;
			}
		}

		// Token: 0x17000B52 RID: 2898
		// (get) Token: 0x06002F97 RID: 12183 RVA: 0x000CA50B File Offset: 0x000C870B
		public static TraitObject Mercy
		{
			get
			{
				return DefaultTraits.Instance._traitMercy;
			}
		}

		// Token: 0x17000B53 RID: 2899
		// (get) Token: 0x06002F98 RID: 12184 RVA: 0x000CA517 File Offset: 0x000C8717
		public static TraitObject Valor
		{
			get
			{
				return DefaultTraits.Instance._traitValor;
			}
		}

		// Token: 0x17000B54 RID: 2900
		// (get) Token: 0x06002F99 RID: 12185 RVA: 0x000CA523 File Offset: 0x000C8723
		public static TraitObject Honor
		{
			get
			{
				return DefaultTraits.Instance._traitHonor;
			}
		}

		// Token: 0x17000B55 RID: 2901
		// (get) Token: 0x06002F9A RID: 12186 RVA: 0x000CA52F File Offset: 0x000C872F
		public static TraitObject Generosity
		{
			get
			{
				return DefaultTraits.Instance._traitGenerosity;
			}
		}

		// Token: 0x17000B56 RID: 2902
		// (get) Token: 0x06002F9B RID: 12187 RVA: 0x000CA53B File Offset: 0x000C873B
		public static TraitObject Calculating
		{
			get
			{
				return DefaultTraits.Instance._traitCalculating;
			}
		}

		// Token: 0x17000B57 RID: 2903
		// (get) Token: 0x06002F9C RID: 12188 RVA: 0x000CA547 File Offset: 0x000C8747
		public static TraitObject PersonaCurt
		{
			get
			{
				return DefaultTraits.Instance._traitPersonaCurt;
			}
		}

		// Token: 0x17000B58 RID: 2904
		// (get) Token: 0x06002F9D RID: 12189 RVA: 0x000CA553 File Offset: 0x000C8753
		public static TraitObject PersonaEarnest
		{
			get
			{
				return DefaultTraits.Instance._traitPersonaEarnest;
			}
		}

		// Token: 0x17000B59 RID: 2905
		// (get) Token: 0x06002F9E RID: 12190 RVA: 0x000CA55F File Offset: 0x000C875F
		public static TraitObject PersonaIronic
		{
			get
			{
				return DefaultTraits.Instance._traitPersonaIronic;
			}
		}

		// Token: 0x17000B5A RID: 2906
		// (get) Token: 0x06002F9F RID: 12191 RVA: 0x000CA56B File Offset: 0x000C876B
		public static TraitObject PersonaSoftspoken
		{
			get
			{
				return DefaultTraits.Instance._traitPersonaSoftspoken;
			}
		}

		// Token: 0x17000B5B RID: 2907
		// (get) Token: 0x06002FA0 RID: 12192 RVA: 0x000CA577 File Offset: 0x000C8777
		public static TraitObject Surgery
		{
			get
			{
				return DefaultTraits.Instance._traitSurgery;
			}
		}

		// Token: 0x17000B5C RID: 2908
		// (get) Token: 0x06002FA1 RID: 12193 RVA: 0x000CA583 File Offset: 0x000C8783
		public static TraitObject SergeantCommandSkills
		{
			get
			{
				return DefaultTraits.Instance._traitSergeantCommandSkills;
			}
		}

		// Token: 0x17000B5D RID: 2909
		// (get) Token: 0x06002FA2 RID: 12194 RVA: 0x000CA58F File Offset: 0x000C878F
		public static TraitObject RogueSkills
		{
			get
			{
				return DefaultTraits.Instance._traitRogueSkills;
			}
		}

		// Token: 0x17000B5E RID: 2910
		// (get) Token: 0x06002FA3 RID: 12195 RVA: 0x000CA59B File Offset: 0x000C879B
		public static TraitObject Siegecraft
		{
			get
			{
				return DefaultTraits.Instance._traitEngineerSkills;
			}
		}

		// Token: 0x17000B5F RID: 2911
		// (get) Token: 0x06002FA4 RID: 12196 RVA: 0x000CA5A7 File Offset: 0x000C87A7
		public static TraitObject ScoutSkills
		{
			get
			{
				return DefaultTraits.Instance._traitScoutSkills;
			}
		}

		// Token: 0x17000B60 RID: 2912
		// (get) Token: 0x06002FA5 RID: 12197 RVA: 0x000CA5B3 File Offset: 0x000C87B3
		public static TraitObject Blacksmith
		{
			get
			{
				return DefaultTraits.Instance._traitBlacksmith;
			}
		}

		// Token: 0x17000B61 RID: 2913
		// (get) Token: 0x06002FA6 RID: 12198 RVA: 0x000CA5BF File Offset: 0x000C87BF
		public static TraitObject Fighter
		{
			get
			{
				return DefaultTraits.Instance._traitFighter;
			}
		}

		// Token: 0x17000B62 RID: 2914
		// (get) Token: 0x06002FA7 RID: 12199 RVA: 0x000CA5CB File Offset: 0x000C87CB
		public static TraitObject Commander
		{
			get
			{
				return DefaultTraits.Instance._traitCommander;
			}
		}

		// Token: 0x17000B63 RID: 2915
		// (get) Token: 0x06002FA8 RID: 12200 RVA: 0x000CA5D7 File Offset: 0x000C87D7
		public static TraitObject Politician
		{
			get
			{
				return DefaultTraits.Instance._traitPolitician;
			}
		}

		// Token: 0x17000B64 RID: 2916
		// (get) Token: 0x06002FA9 RID: 12201 RVA: 0x000CA5E3 File Offset: 0x000C87E3
		public static TraitObject Manager
		{
			get
			{
				return DefaultTraits.Instance._traitManager;
			}
		}

		// Token: 0x17000B65 RID: 2917
		// (get) Token: 0x06002FAA RID: 12202 RVA: 0x000CA5EF File Offset: 0x000C87EF
		public static TraitObject Trader
		{
			get
			{
				return DefaultTraits.Instance._traitTraderSkills;
			}
		}

		// Token: 0x17000B66 RID: 2918
		// (get) Token: 0x06002FAB RID: 12203 RVA: 0x000CA5FB File Offset: 0x000C87FB
		public static TraitObject KnightFightingSkills
		{
			get
			{
				return DefaultTraits.Instance._traitKnightFightingSkills;
			}
		}

		// Token: 0x17000B67 RID: 2919
		// (get) Token: 0x06002FAC RID: 12204 RVA: 0x000CA607 File Offset: 0x000C8807
		public static TraitObject CavalryFightingSkills
		{
			get
			{
				return DefaultTraits.Instance._traitCavalryFightingSkills;
			}
		}

		// Token: 0x17000B68 RID: 2920
		// (get) Token: 0x06002FAD RID: 12205 RVA: 0x000CA613 File Offset: 0x000C8813
		public static TraitObject HorseArcherFightingSkills
		{
			get
			{
				return DefaultTraits.Instance._traitHorseArcherFightingSkills;
			}
		}

		// Token: 0x17000B69 RID: 2921
		// (get) Token: 0x06002FAE RID: 12206 RVA: 0x000CA61F File Offset: 0x000C881F
		public static TraitObject HopliteFightingSkills
		{
			get
			{
				return DefaultTraits.Instance._traitHopliteFightingSkills;
			}
		}

		// Token: 0x17000B6A RID: 2922
		// (get) Token: 0x06002FAF RID: 12207 RVA: 0x000CA62B File Offset: 0x000C882B
		public static TraitObject ArcherFIghtingSkills
		{
			get
			{
				return DefaultTraits.Instance._traitArcherFIghtingSkills;
			}
		}

		// Token: 0x17000B6B RID: 2923
		// (get) Token: 0x06002FB0 RID: 12208 RVA: 0x000CA637 File Offset: 0x000C8837
		public static TraitObject CrossbowmanStyle
		{
			get
			{
				return DefaultTraits.Instance._traitCrossbowmanStyle;
			}
		}

		// Token: 0x17000B6C RID: 2924
		// (get) Token: 0x06002FB1 RID: 12209 RVA: 0x000CA643 File Offset: 0x000C8843
		public static TraitObject PeltastFightingSkills
		{
			get
			{
				return DefaultTraits.Instance._traitPeltastFightingSkills;
			}
		}

		// Token: 0x17000B6D RID: 2925
		// (get) Token: 0x06002FB2 RID: 12210 RVA: 0x000CA64F File Offset: 0x000C884F
		public static TraitObject HuscarlFightingSkills
		{
			get
			{
				return DefaultTraits.Instance._traitHuscarlFightingSkills;
			}
		}

		// Token: 0x17000B6E RID: 2926
		// (get) Token: 0x06002FB3 RID: 12211 RVA: 0x000CA65B File Offset: 0x000C885B
		public static TraitObject BossFightingSkills
		{
			get
			{
				return DefaultTraits.Instance._traitBossFightingSkills;
			}
		}

		// Token: 0x17000B6F RID: 2927
		// (get) Token: 0x06002FB4 RID: 12212 RVA: 0x000CA667 File Offset: 0x000C8867
		public static TraitObject WandererEquipment
		{
			get
			{
				return DefaultTraits.Instance._traitWandererEquipment;
			}
		}

		// Token: 0x17000B70 RID: 2928
		// (get) Token: 0x06002FB5 RID: 12213 RVA: 0x000CA673 File Offset: 0x000C8873
		public static TraitObject GentryEquipment
		{
			get
			{
				return DefaultTraits.Instance._traitGentryEquipment;
			}
		}

		// Token: 0x17000B71 RID: 2929
		// (get) Token: 0x06002FB6 RID: 12214 RVA: 0x000CA67F File Offset: 0x000C887F
		public static TraitObject MuscularBuild
		{
			get
			{
				return DefaultTraits.Instance._traitMuscularBuild;
			}
		}

		// Token: 0x17000B72 RID: 2930
		// (get) Token: 0x06002FB7 RID: 12215 RVA: 0x000CA68B File Offset: 0x000C888B
		public static TraitObject HeavyBuild
		{
			get
			{
				return DefaultTraits.Instance._traitHeavyBuild;
			}
		}

		// Token: 0x17000B73 RID: 2931
		// (get) Token: 0x06002FB8 RID: 12216 RVA: 0x000CA697 File Offset: 0x000C8897
		public static TraitObject LightBuild
		{
			get
			{
				return DefaultTraits.Instance._traitLightBuild;
			}
		}

		// Token: 0x17000B74 RID: 2932
		// (get) Token: 0x06002FB9 RID: 12217 RVA: 0x000CA6A3 File Offset: 0x000C88A3
		public static TraitObject OutOfShapeBuild
		{
			get
			{
				return DefaultTraits.Instance._traitOutOfShapeBuild;
			}
		}

		// Token: 0x17000B75 RID: 2933
		// (get) Token: 0x06002FBA RID: 12218 RVA: 0x000CA6AF File Offset: 0x000C88AF
		public static TraitObject RomanHair
		{
			get
			{
				return DefaultTraits.Instance._traitRomanHair;
			}
		}

		// Token: 0x17000B76 RID: 2934
		// (get) Token: 0x06002FBB RID: 12219 RVA: 0x000CA6BB File Offset: 0x000C88BB
		public static TraitObject FrankishHair
		{
			get
			{
				return DefaultTraits.Instance._traitFrankishHair;
			}
		}

		// Token: 0x17000B77 RID: 2935
		// (get) Token: 0x06002FBC RID: 12220 RVA: 0x000CA6C7 File Offset: 0x000C88C7
		public static TraitObject CelticHair
		{
			get
			{
				return DefaultTraits.Instance._traitCelticHair;
			}
		}

		// Token: 0x17000B78 RID: 2936
		// (get) Token: 0x06002FBD RID: 12221 RVA: 0x000CA6D3 File Offset: 0x000C88D3
		public static TraitObject RusHair
		{
			get
			{
				return DefaultTraits.Instance._traitRusHair;
			}
		}

		// Token: 0x17000B79 RID: 2937
		// (get) Token: 0x06002FBE RID: 12222 RVA: 0x000CA6DF File Offset: 0x000C88DF
		public static TraitObject ArabianHair
		{
			get
			{
				return DefaultTraits.Instance._traitArabianHair;
			}
		}

		// Token: 0x17000B7A RID: 2938
		// (get) Token: 0x06002FBF RID: 12223 RVA: 0x000CA6EB File Offset: 0x000C88EB
		public static TraitObject SteppeHair
		{
			get
			{
				return DefaultTraits.Instance._traitSteppeHair;
			}
		}

		// Token: 0x17000B7B RID: 2939
		// (get) Token: 0x06002FC0 RID: 12224 RVA: 0x000CA6F7 File Offset: 0x000C88F7
		public static TraitObject Thug
		{
			get
			{
				return DefaultTraits.Instance._traitThug;
			}
		}

		// Token: 0x17000B7C RID: 2940
		// (get) Token: 0x06002FC1 RID: 12225 RVA: 0x000CA703 File Offset: 0x000C8903
		public static TraitObject Thief
		{
			get
			{
				return DefaultTraits.Instance._traitThief;
			}
		}

		// Token: 0x17000B7D RID: 2941
		// (get) Token: 0x06002FC2 RID: 12226 RVA: 0x000CA70F File Offset: 0x000C890F
		public static TraitObject Gambler
		{
			get
			{
				return DefaultTraits.Instance._traitGambler;
			}
		}

		// Token: 0x17000B7E RID: 2942
		// (get) Token: 0x06002FC3 RID: 12227 RVA: 0x000CA71B File Offset: 0x000C891B
		public static TraitObject Smuggler
		{
			get
			{
				return DefaultTraits.Instance._traitSmuggler;
			}
		}

		// Token: 0x17000B7F RID: 2943
		// (get) Token: 0x06002FC4 RID: 12228 RVA: 0x000CA727 File Offset: 0x000C8927
		public static TraitObject Egalitarian
		{
			get
			{
				return DefaultTraits.Instance._traitEgalitarian;
			}
		}

		// Token: 0x17000B80 RID: 2944
		// (get) Token: 0x06002FC5 RID: 12229 RVA: 0x000CA733 File Offset: 0x000C8933
		public static TraitObject Oligarchic
		{
			get
			{
				return DefaultTraits.Instance._traitOligarchic;
			}
		}

		// Token: 0x17000B81 RID: 2945
		// (get) Token: 0x06002FC6 RID: 12230 RVA: 0x000CA73F File Offset: 0x000C893F
		public static TraitObject Authoritarian
		{
			get
			{
				return DefaultTraits.Instance._traitAuthoritarian;
			}
		}

		// Token: 0x17000B82 RID: 2946
		// (get) Token: 0x06002FC7 RID: 12231 RVA: 0x000CA74B File Offset: 0x000C894B
		public static IEnumerable<TraitObject> Personality
		{
			get
			{
				return DefaultTraits.Instance._personality;
			}
		}

		// Token: 0x17000B83 RID: 2947
		// (get) Token: 0x06002FC8 RID: 12232 RVA: 0x000CA757 File Offset: 0x000C8957
		public static IEnumerable<TraitObject> SkillCategories
		{
			get
			{
				return DefaultTraits.Instance._skillCategories;
			}
		}

		// Token: 0x06002FC9 RID: 12233 RVA: 0x000CA764 File Offset: 0x000C8964
		public DefaultTraits()
		{
			this.RegisterAll();
			this._personality = new TraitObject[] { this._traitMercy, this._traitValor, this._traitHonor, this._traitGenerosity, this._traitCalculating };
			this._skillCategories = new TraitObject[] { this._traitCommander, this._traitFighter, this._traitPolitician, this._traitManager };
		}

		// Token: 0x06002FCA RID: 12234 RVA: 0x000CA7E8 File Offset: 0x000C89E8
		public void RegisterAll()
		{
			this._traitFrequency = this.Create("Frequency");
			this._traitMercy = this.Create("Mercy");
			this._traitValor = this.Create("Valor");
			this._traitHonor = this.Create("Honor");
			this._traitGenerosity = this.Create("Generosity");
			this._traitCalculating = this.Create("Calculating");
			this._traitPersonaCurt = this.Create("curt");
			this._traitPersonaIronic = this.Create("ironic");
			this._traitPersonaEarnest = this.Create("earnest");
			this._traitPersonaSoftspoken = this.Create("softspoken");
			this._traitCommander = this.Create("Commander");
			this._traitFighter = this.Create("BalancedFightingSkills");
			this._traitPolitician = this.Create("Politician");
			this._traitManager = this.Create("Manager");
			this._traitTraderSkills = this.Create("Trader");
			this._traitSurgery = this.Create("Surgeon");
			this._traitTracking = this.Create("Tracking");
			this._traitBlacksmith = this.Create("Blacksmith");
			this._traitSergeantCommandSkills = this.Create("SergeantCommandSkills");
			this._traitEngineerSkills = this.Create("EngineerSkills");
			this._traitRogueSkills = this.Create("RogueSkills");
			this._traitScoutSkills = this.Create("ScoutSkills");
			this._traitKnightFightingSkills = this.Create("KnightFightingSkills");
			this._traitCavalryFightingSkills = this.Create("CavalryFightingSkills");
			this._traitHorseArcherFightingSkills = this.Create("HorseArcherFightingSkills");
			this._traitHopliteFightingSkills = this.Create("HopliteFightingSkills");
			this._traitArcherFIghtingSkills = this.Create("ArcherFIghtingSkills");
			this._traitCrossbowmanStyle = this.Create("CrossbowmanStyle");
			this._traitPeltastFightingSkills = this.Create("PeltastFightingSkills");
			this._traitHuscarlFightingSkills = this.Create("HuscarlFightingSkills");
			this._traitBossFightingSkills = this.Create("BossFightingSkills");
			this._traitHeavyBuild = this.Create("HeavyBuild");
			this._traitMuscularBuild = this.Create("MuscularBuild");
			this._traitOutOfShapeBuild = this.Create("OutOfShapeBuild");
			this._traitLightBuild = this.Create("LightBuild");
			this._traitScarred = this.Create("Scarred");
			this._traitOneEyed = this.Create("OneEyed");
			this._traitRomanHair = this.Create("RomanHair");
			this._traitCelticHair = this.Create("CelticHair");
			this._traitArabianHair = this.Create("ArabianHair");
			this._traitRusHair = this.Create("RusHair");
			this._traitFrankishHair = this.Create("FrankishHair");
			this._traitSteppeHair = this.Create("SteppeHair");
			this._traitWandererEquipment = this.Create("WandererEquipment");
			this._traitGentryEquipment = this.Create("GentryEquipment");
			this._traitThug = this.Create("Thug");
			this._traitThief = this.Create("Thief");
			this._traitGambler = this.Create("Gambler");
			this._traitSmuggler = this.Create("Smuggler");
			this._traitIsMercenary = this.Create("Mercenary");
			this._traitEgalitarian = this.Create("Egalitarian");
			this._traitOligarchic = this.Create("Oligarchic");
			this._traitAuthoritarian = this.Create("Authoritarian");
			this.InitializeAll();
		}

		// Token: 0x06002FCB RID: 12235 RVA: 0x000CAB80 File Offset: 0x000C8D80
		private TraitObject Create(string stringId)
		{
			return Game.Current.ObjectManager.RegisterPresumedObject<TraitObject>(new TraitObject(stringId));
		}

		// Token: 0x06002FCC RID: 12236 RVA: 0x000CAB98 File Offset: 0x000C8D98
		private void InitializeAll()
		{
			this._traitFrequency.Initialize(new TextObject("{=vsoyhPnl}Frequency", null), new TextObject("{=!}Frequency Description", null), true, 0, 20);
			this._traitMercy.Initialize(new TextObject("{=2I2uKJlw}Mercy", null), new TextObject("{=Au7VCWTa}Mercy represents your general aversion to suffering and your willingness to help strangers or even enemies.", null), false, -2, 2);
			this._traitValor.Initialize(new TextObject("{=toQLHG6x}Valor", null), new TextObject("{=Ugm9nO49}Valor represents your reputation for risking your life to win glory or wealth or advance your cause.", null), false, -2, 2);
			this._traitHonor.Initialize(new TextObject("{=0oGz5rVx}Honor", null), new TextObject("{=1vYgkaaK}Honor represents your reputation for respecting your formal commitments, like keeping your word and obeying the law.", null), false, -2, 2);
			this._traitGenerosity.Initialize(new TextObject("{=IuWu5Bu7}Generosity", null), new TextObject("{=IKzqzPDS}Generosity represents your loyalty to your kin and those who serve you, and your gratitude to those who have done you a favor.", null), false, -2, 2);
			this._traitCalculating.Initialize(new TextObject("{=5sMBbn7y}Calculating", null), new TextObject("{=QKjF5gTR}Calculating represents your ability to control your emotions for the sake of your long-term interests.", null), false, -2, 2);
			this._traitPersonaCurt.Initialize(new TextObject("{=!}PersonaCurt", null), new TextObject("{=!}PersonaCurt Description", null), false, -2, 2);
			this._traitPersonaIronic.Initialize(new TextObject("{=!}PersonaIronic", null), new TextObject("{=!}PersonaIronic Description", null), false, -2, 2);
			this._traitPersonaEarnest.Initialize(new TextObject("{=!}PersonaEarnest", null), new TextObject("{=!}PersonaEarnest Description", null), false, -2, 2);
			this._traitPersonaSoftspoken.Initialize(new TextObject("{=!}PersonaSoftspoken", null), new TextObject("{=!}PersonaSoftspoken Description", null), false, -2, 2);
			this._traitCommander.Initialize(new TextObject("{=RvKwdXWs}Commander", null), new TextObject("{=!}Commander Description", null), true, 0, 20);
			this._traitFighter.Initialize(new TextObject("{=!}BalancedFightingSkills", null), new TextObject("{=!}BalancedFightingSkills Description", null), true, 0, 20);
			this._traitPolitician.Initialize(new TextObject("{=4Ybbhzjw}Politician", null), new TextObject("{=!}Politician Description", null), true, 0, 20);
			this._traitManager.Initialize(new TextObject("{=6RYVOb0c}Manager", null), new TextObject("{=!}Manager Description", null), true, 0, 20);
			this._traitSurgery.Initialize(new TextObject("{=QBPrRdQJ}Surgeon", null), new TextObject("{=!}Surgeon Description", null), true, 0, 20);
			this._traitTracking.Initialize(new TextObject("{=dx0hmeH6}Tracking", null), new TextObject("{=!}Tracking Description", null), true, 0, 20);
			this._traitBlacksmith.Initialize(new TextObject("{=bNnQt4jN}Blacksmith", null), new TextObject("{=!}Blacksmith Description", null), true, 0, 20);
			this._traitSergeantCommandSkills.Initialize(new TextObject("{=!}SergeantCommandSkills", null), new TextObject("{=!}SergeantCommandSkills Description", null), true, 0, 20);
			this._traitEngineerSkills.Initialize(new TextObject("{=!}EngineerSkills", null), new TextObject("{=!}EngineerSkills Description", null), true, 0, 20);
			this._traitRogueSkills.Initialize(new TextObject("{=!}RogueSkills", null), new TextObject("{=!}RogueSkills Description", null), true, 0, 20);
			this._traitScoutSkills.Initialize(new TextObject("{=!}ScoutSkills", null), new TextObject("{=!}ScoutSkills Description", null), true, 0, 20);
			this._traitTraderSkills.Initialize(new TextObject("{=!}TraderSkills", null), new TextObject("{=!}Trader Description", null), true, 0, 20);
			this._traitKnightFightingSkills.Initialize(new TextObject("{=!}KnightFightingSkills", null), new TextObject("{=!}KnightFightingSkills Description", null), true, 0, 20);
			this._traitCavalryFightingSkills.Initialize(new TextObject("{=!}CavalryFightingSkills", null), new TextObject("{=!}CavalryFightingSkills Description", null), true, 0, 20);
			this._traitHorseArcherFightingSkills.Initialize(new TextObject("{=!}HorseArcherFightingSkills", null), new TextObject("{=!}HorseArcherFightingSkills Description", null), true, 0, 20);
			this._traitHopliteFightingSkills.Initialize(new TextObject("{=!}HopliteFightingSkills", null), new TextObject("{=!}HopliteFightingSkills Description", null), true, 0, 20);
			this._traitArcherFIghtingSkills.Initialize(new TextObject("{=!}ArcherFIghtingSkills", null), new TextObject("{=!}ArcherFIghtingSkills Description", null), true, 0, 20);
			this._traitCrossbowmanStyle.Initialize(new TextObject("{=!}CrossbowmanStyle", null), new TextObject("{=!}CrossbowmanStyle Description", null), true, 0, 20);
			this._traitPeltastFightingSkills.Initialize(new TextObject("{=!}PeltastFightingSkills", null), new TextObject("{=!}PeltastFightingSkills Description", null), true, 0, 20);
			this._traitHuscarlFightingSkills.Initialize(new TextObject("{=!}HuscarlFightingSkills", null), new TextObject("{=!}HuscarlFightingSkills Description", null), true, 0, 20);
			this._traitBossFightingSkills.Initialize(new TextObject("{=!}BossFightingSkills", null), new TextObject("{=!}BossFightingSkills Description", null), true, 0, 20);
			this._traitHeavyBuild.Initialize(new TextObject("{=!}HeavyBuild", null), new TextObject("{=!}HeavyBuild Description", null), true, 0, 20);
			this._traitMuscularBuild.Initialize(new TextObject("{=!}MuscularBuild", null), new TextObject("{=!}MuscularBuild Description", null), true, 0, 20);
			this._traitOutOfShapeBuild.Initialize(new TextObject("{=!}OutOfShapeBuild", null), new TextObject("{=!}OutOfShapeBuild Description", null), true, 0, 20);
			this._traitLightBuild.Initialize(new TextObject("{=!}LightBuild", null), new TextObject("{=!}LightBuild Description", null), true, 0, 20);
			this._traitScarred.Initialize(new TextObject("{=!}Scarred", null), new TextObject("{=!}Scarred Description", null), true, 0, 20);
			this._traitOneEyed.Initialize(new TextObject("{=!}OneEyed", null), new TextObject("{=!}OneEyed Description", null), true, 0, 20);
			this._traitRomanHair.Initialize(new TextObject("{=!}RomanHair", null), new TextObject("{=!}RomanHair Description", null), true, 0, 20);
			this._traitCelticHair.Initialize(new TextObject("{=!}CelticHair", null), new TextObject("{=!}CelticHair Description", null), true, 0, 20);
			this._traitArabianHair.Initialize(new TextObject("{=!}ArabianHair", null), new TextObject("{=!}ArabianHair Description", null), true, 0, 20);
			this._traitRusHair.Initialize(new TextObject("{=!}RusHair", null), new TextObject("{=!}RusHair Description", null), true, 0, 20);
			this._traitFrankishHair.Initialize(new TextObject("{=!}FrankishHair", null), new TextObject("{=!}FrankishHair Description", null), true, 0, 20);
			this._traitSteppeHair.Initialize(new TextObject("{=!}SteppeHair", null), new TextObject("{=!}SteppeHair Description", null), true, 0, 20);
			this._traitWandererEquipment.Initialize(new TextObject("{=!}WandererEquipment", null), new TextObject("{=!}WandererEquipment Description", null), true, 0, 20);
			this._traitGentryEquipment.Initialize(new TextObject("{=!}GentryEquipment", null), new TextObject("{=!}GentryEquipment Description", null), true, 0, 20);
			this._traitThug.Initialize(new TextObject("{=thugtrait}Thug", null), new TextObject("{=Fjnw9ooa}Indicates a gang member specialized in extortion", null), true, 0, 20);
			this._traitThief.Initialize(new TextObject("{=eWRlQ4XU}Thief", null), new TextObject("{=dCbdYqVw}Indicates a gang member specialized in theft and fencing", null), true, 0, 20);
			this._traitGambler.Initialize(new TextObject("{=LgSTrjuk}Gambler", null), new TextObject("{=0Euf4iPM}Indicates a gang member specialized in running gambling dens", null), true, 0, 20);
			this._traitSmuggler.Initialize(new TextObject("{=eeWx1yYd}Smuggler", null), new TextObject("{=87c7IhkZ}Indicates a gang member specialized in smuggling", null), true, 0, 20);
			this._traitIsMercenary.Initialize(new TextObject("{=zCQaPTOK}Mercenary", null), new TextObject("{=!}Mercenary Description", null), false, 0, 20);
			this._traitEgalitarian.Initialize(new TextObject("{=HMFb1gaq}Egalitarian", null), new TextObject("{=!}Egalitarian Description", null), false, 0, 20);
			this._traitOligarchic.Initialize(new TextObject("{=hR6Zo6pD}Oligarchic", null), new TextObject("{=!}Oligarchic Description", null), false, 0, 20);
			this._traitAuthoritarian.Initialize(new TextObject("{=NaMPa4ML}Authoritarian", null), new TextObject("{=!}Authoritarian Description", null), false, 0, 20);
		}

		// Token: 0x04000FA2 RID: 4002
		private const int MaxPersonalityTraitValue = 2;

		// Token: 0x04000FA3 RID: 4003
		private const int MinPersonalityTraitValue = -2;

		// Token: 0x04000FA4 RID: 4004
		private const int MaxHiddenTraitValue = 20;

		// Token: 0x04000FA5 RID: 4005
		private const int MinHiddenTraitValue = 0;

		// Token: 0x04000FA6 RID: 4006
		private TraitObject _traitMercy;

		// Token: 0x04000FA7 RID: 4007
		private TraitObject _traitValor;

		// Token: 0x04000FA8 RID: 4008
		private TraitObject _traitHonor;

		// Token: 0x04000FA9 RID: 4009
		private TraitObject _traitGenerosity;

		// Token: 0x04000FAA RID: 4010
		private TraitObject _traitCalculating;

		// Token: 0x04000FAB RID: 4011
		private TraitObject _traitPersonaCurt;

		// Token: 0x04000FAC RID: 4012
		private TraitObject _traitPersonaEarnest;

		// Token: 0x04000FAD RID: 4013
		private TraitObject _traitPersonaIronic;

		// Token: 0x04000FAE RID: 4014
		private TraitObject _traitPersonaSoftspoken;

		// Token: 0x04000FAF RID: 4015
		private TraitObject _traitEgalitarian;

		// Token: 0x04000FB0 RID: 4016
		private TraitObject _traitOligarchic;

		// Token: 0x04000FB1 RID: 4017
		private TraitObject _traitAuthoritarian;

		// Token: 0x04000FB2 RID: 4018
		private TraitObject _traitSurgery;

		// Token: 0x04000FB3 RID: 4019
		private TraitObject _traitTracking;

		// Token: 0x04000FB4 RID: 4020
		private TraitObject _traitSergeantCommandSkills;

		// Token: 0x04000FB5 RID: 4021
		private TraitObject _traitRogueSkills;

		// Token: 0x04000FB6 RID: 4022
		private TraitObject _traitEngineerSkills;

		// Token: 0x04000FB7 RID: 4023
		private TraitObject _traitBlacksmith;

		// Token: 0x04000FB8 RID: 4024
		private TraitObject _traitScoutSkills;

		// Token: 0x04000FB9 RID: 4025
		private TraitObject _traitTraderSkills;

		// Token: 0x04000FBA RID: 4026
		private TraitObject _traitHeavyBuild;

		// Token: 0x04000FBB RID: 4027
		private TraitObject _traitMuscularBuild;

		// Token: 0x04000FBC RID: 4028
		private TraitObject _traitOutOfShapeBuild;

		// Token: 0x04000FBD RID: 4029
		private TraitObject _traitLightBuild;

		// Token: 0x04000FBE RID: 4030
		private TraitObject _traitOneEyed;

		// Token: 0x04000FBF RID: 4031
		private TraitObject _traitScarred;

		// Token: 0x04000FC0 RID: 4032
		private TraitObject _traitRomanHair;

		// Token: 0x04000FC1 RID: 4033
		private TraitObject _traitCelticHair;

		// Token: 0x04000FC2 RID: 4034
		private TraitObject _traitRusHair;

		// Token: 0x04000FC3 RID: 4035
		private TraitObject _traitArabianHair;

		// Token: 0x04000FC4 RID: 4036
		private TraitObject _traitFrankishHair;

		// Token: 0x04000FC5 RID: 4037
		private TraitObject _traitSteppeHair;

		// Token: 0x04000FC6 RID: 4038
		private TraitObject _traitIsMercenary;

		// Token: 0x04000FC7 RID: 4039
		private TraitObject _traitFrequency;

		// Token: 0x04000FC8 RID: 4040
		private TraitObject _traitCommander;

		// Token: 0x04000FC9 RID: 4041
		private TraitObject _traitFighter;

		// Token: 0x04000FCA RID: 4042
		private TraitObject _traitPolitician;

		// Token: 0x04000FCB RID: 4043
		private TraitObject _traitManager;

		// Token: 0x04000FCC RID: 4044
		private TraitObject _traitKnightFightingSkills;

		// Token: 0x04000FCD RID: 4045
		private TraitObject _traitCavalryFightingSkills;

		// Token: 0x04000FCE RID: 4046
		private TraitObject _traitHorseArcherFightingSkills;

		// Token: 0x04000FCF RID: 4047
		private TraitObject _traitArcherFIghtingSkills;

		// Token: 0x04000FD0 RID: 4048
		private TraitObject _traitCrossbowmanStyle;

		// Token: 0x04000FD1 RID: 4049
		private TraitObject _traitPeltastFightingSkills;

		// Token: 0x04000FD2 RID: 4050
		private TraitObject _traitHopliteFightingSkills;

		// Token: 0x04000FD3 RID: 4051
		private TraitObject _traitHuscarlFightingSkills;

		// Token: 0x04000FD4 RID: 4052
		private TraitObject _traitBossFightingSkills;

		// Token: 0x04000FD5 RID: 4053
		private TraitObject _traitWandererEquipment;

		// Token: 0x04000FD6 RID: 4054
		private TraitObject _traitGentryEquipment;

		// Token: 0x04000FD7 RID: 4055
		private TraitObject _traitThug;

		// Token: 0x04000FD8 RID: 4056
		private TraitObject _traitThief;

		// Token: 0x04000FD9 RID: 4057
		private TraitObject _traitGambler;

		// Token: 0x04000FDA RID: 4058
		private TraitObject _traitSmuggler;

		// Token: 0x04000FDB RID: 4059
		private readonly TraitObject[] _personality;

		// Token: 0x04000FDC RID: 4060
		private readonly TraitObject[] _skillCategories;
	}
}
