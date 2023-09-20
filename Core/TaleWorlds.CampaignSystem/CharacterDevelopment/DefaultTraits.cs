using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.CharacterDevelopment
{
	public class DefaultTraits
	{
		private static DefaultTraits Instance
		{
			get
			{
				return Campaign.Current.DefaultTraits;
			}
		}

		public static TraitObject Frequency
		{
			get
			{
				return DefaultTraits.Instance._traitFrequency;
			}
		}

		public static TraitObject Mercy
		{
			get
			{
				return DefaultTraits.Instance._traitMercy;
			}
		}

		public static TraitObject Valor
		{
			get
			{
				return DefaultTraits.Instance._traitValor;
			}
		}

		public static TraitObject Honor
		{
			get
			{
				return DefaultTraits.Instance._traitHonor;
			}
		}

		public static TraitObject Generosity
		{
			get
			{
				return DefaultTraits.Instance._traitGenerosity;
			}
		}

		public static TraitObject Calculating
		{
			get
			{
				return DefaultTraits.Instance._traitCalculating;
			}
		}

		public static TraitObject PersonaCurt
		{
			get
			{
				return DefaultTraits.Instance._traitPersonaCurt;
			}
		}

		public static TraitObject PersonaEarnest
		{
			get
			{
				return DefaultTraits.Instance._traitPersonaEarnest;
			}
		}

		public static TraitObject PersonaIronic
		{
			get
			{
				return DefaultTraits.Instance._traitPersonaIronic;
			}
		}

		public static TraitObject PersonaSoftspoken
		{
			get
			{
				return DefaultTraits.Instance._traitPersonaSoftspoken;
			}
		}

		public static TraitObject Surgery
		{
			get
			{
				return DefaultTraits.Instance._traitSurgery;
			}
		}

		public static TraitObject SergeantCommandSkills
		{
			get
			{
				return DefaultTraits.Instance._traitSergeantCommandSkills;
			}
		}

		public static TraitObject RogueSkills
		{
			get
			{
				return DefaultTraits.Instance._traitRogueSkills;
			}
		}

		public static TraitObject Siegecraft
		{
			get
			{
				return DefaultTraits.Instance._traitEngineerSkills;
			}
		}

		public static TraitObject ScoutSkills
		{
			get
			{
				return DefaultTraits.Instance._traitScoutSkills;
			}
		}

		public static TraitObject Blacksmith
		{
			get
			{
				return DefaultTraits.Instance._traitBlacksmith;
			}
		}

		public static TraitObject Fighter
		{
			get
			{
				return DefaultTraits.Instance._traitFighter;
			}
		}

		public static TraitObject Commander
		{
			get
			{
				return DefaultTraits.Instance._traitCommander;
			}
		}

		public static TraitObject Politician
		{
			get
			{
				return DefaultTraits.Instance._traitPolitician;
			}
		}

		public static TraitObject Manager
		{
			get
			{
				return DefaultTraits.Instance._traitManager;
			}
		}

		public static TraitObject Trader
		{
			get
			{
				return DefaultTraits.Instance._traitTraderSkills;
			}
		}

		public static TraitObject KnightFightingSkills
		{
			get
			{
				return DefaultTraits.Instance._traitKnightFightingSkills;
			}
		}

		public static TraitObject CavalryFightingSkills
		{
			get
			{
				return DefaultTraits.Instance._traitCavalryFightingSkills;
			}
		}

		public static TraitObject HorseArcherFightingSkills
		{
			get
			{
				return DefaultTraits.Instance._traitHorseArcherFightingSkills;
			}
		}

		public static TraitObject HopliteFightingSkills
		{
			get
			{
				return DefaultTraits.Instance._traitHopliteFightingSkills;
			}
		}

		public static TraitObject ArcherFIghtingSkills
		{
			get
			{
				return DefaultTraits.Instance._traitArcherFIghtingSkills;
			}
		}

		public static TraitObject CrossbowmanStyle
		{
			get
			{
				return DefaultTraits.Instance._traitCrossbowmanStyle;
			}
		}

		public static TraitObject PeltastFightingSkills
		{
			get
			{
				return DefaultTraits.Instance._traitPeltastFightingSkills;
			}
		}

		public static TraitObject HuscarlFightingSkills
		{
			get
			{
				return DefaultTraits.Instance._traitHuscarlFightingSkills;
			}
		}

		public static TraitObject BossFightingSkills
		{
			get
			{
				return DefaultTraits.Instance._traitBossFightingSkills;
			}
		}

		public static TraitObject WandererEquipment
		{
			get
			{
				return DefaultTraits.Instance._traitWandererEquipment;
			}
		}

		public static TraitObject GentryEquipment
		{
			get
			{
				return DefaultTraits.Instance._traitGentryEquipment;
			}
		}

		public static TraitObject MuscularBuild
		{
			get
			{
				return DefaultTraits.Instance._traitMuscularBuild;
			}
		}

		public static TraitObject HeavyBuild
		{
			get
			{
				return DefaultTraits.Instance._traitHeavyBuild;
			}
		}

		public static TraitObject LightBuild
		{
			get
			{
				return DefaultTraits.Instance._traitLightBuild;
			}
		}

		public static TraitObject OutOfShapeBuild
		{
			get
			{
				return DefaultTraits.Instance._traitOutOfShapeBuild;
			}
		}

		public static TraitObject RomanHair
		{
			get
			{
				return DefaultTraits.Instance._traitRomanHair;
			}
		}

		public static TraitObject FrankishHair
		{
			get
			{
				return DefaultTraits.Instance._traitFrankishHair;
			}
		}

		public static TraitObject CelticHair
		{
			get
			{
				return DefaultTraits.Instance._traitCelticHair;
			}
		}

		public static TraitObject RusHair
		{
			get
			{
				return DefaultTraits.Instance._traitRusHair;
			}
		}

		public static TraitObject ArabianHair
		{
			get
			{
				return DefaultTraits.Instance._traitArabianHair;
			}
		}

		public static TraitObject SteppeHair
		{
			get
			{
				return DefaultTraits.Instance._traitSteppeHair;
			}
		}

		public static TraitObject Thug
		{
			get
			{
				return DefaultTraits.Instance._traitThug;
			}
		}

		public static TraitObject Thief
		{
			get
			{
				return DefaultTraits.Instance._traitThief;
			}
		}

		public static TraitObject Gambler
		{
			get
			{
				return DefaultTraits.Instance._traitGambler;
			}
		}

		public static TraitObject Smuggler
		{
			get
			{
				return DefaultTraits.Instance._traitSmuggler;
			}
		}

		public static TraitObject Egalitarian
		{
			get
			{
				return DefaultTraits.Instance._traitEgalitarian;
			}
		}

		public static TraitObject Oligarchic
		{
			get
			{
				return DefaultTraits.Instance._traitOligarchic;
			}
		}

		public static TraitObject Authoritarian
		{
			get
			{
				return DefaultTraits.Instance._traitAuthoritarian;
			}
		}

		public static IEnumerable<TraitObject> Personality
		{
			get
			{
				return DefaultTraits.Instance._personality;
			}
		}

		public static IEnumerable<TraitObject> SkillCategories
		{
			get
			{
				return DefaultTraits.Instance._skillCategories;
			}
		}

		public DefaultTraits()
		{
			this.RegisterAll();
			this._personality = new TraitObject[] { this._traitMercy, this._traitValor, this._traitHonor, this._traitGenerosity, this._traitCalculating };
			this._skillCategories = new TraitObject[] { this._traitCommander, this._traitFighter, this._traitPolitician, this._traitManager };
		}

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

		private TraitObject Create(string stringId)
		{
			return Game.Current.ObjectManager.RegisterPresumedObject<TraitObject>(new TraitObject(stringId));
		}

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

		private const int MaxPersonalityTraitValue = 2;

		private const int MinPersonalityTraitValue = -2;

		private const int MaxHiddenTraitValue = 20;

		private const int MinHiddenTraitValue = 0;

		private TraitObject _traitMercy;

		private TraitObject _traitValor;

		private TraitObject _traitHonor;

		private TraitObject _traitGenerosity;

		private TraitObject _traitCalculating;

		private TraitObject _traitPersonaCurt;

		private TraitObject _traitPersonaEarnest;

		private TraitObject _traitPersonaIronic;

		private TraitObject _traitPersonaSoftspoken;

		private TraitObject _traitEgalitarian;

		private TraitObject _traitOligarchic;

		private TraitObject _traitAuthoritarian;

		private TraitObject _traitSurgery;

		private TraitObject _traitTracking;

		private TraitObject _traitSergeantCommandSkills;

		private TraitObject _traitRogueSkills;

		private TraitObject _traitEngineerSkills;

		private TraitObject _traitBlacksmith;

		private TraitObject _traitScoutSkills;

		private TraitObject _traitTraderSkills;

		private TraitObject _traitHeavyBuild;

		private TraitObject _traitMuscularBuild;

		private TraitObject _traitOutOfShapeBuild;

		private TraitObject _traitLightBuild;

		private TraitObject _traitOneEyed;

		private TraitObject _traitScarred;

		private TraitObject _traitRomanHair;

		private TraitObject _traitCelticHair;

		private TraitObject _traitRusHair;

		private TraitObject _traitArabianHair;

		private TraitObject _traitFrankishHair;

		private TraitObject _traitSteppeHair;

		private TraitObject _traitIsMercenary;

		private TraitObject _traitFrequency;

		private TraitObject _traitCommander;

		private TraitObject _traitFighter;

		private TraitObject _traitPolitician;

		private TraitObject _traitManager;

		private TraitObject _traitKnightFightingSkills;

		private TraitObject _traitCavalryFightingSkills;

		private TraitObject _traitHorseArcherFightingSkills;

		private TraitObject _traitArcherFIghtingSkills;

		private TraitObject _traitCrossbowmanStyle;

		private TraitObject _traitPeltastFightingSkills;

		private TraitObject _traitHopliteFightingSkills;

		private TraitObject _traitHuscarlFightingSkills;

		private TraitObject _traitBossFightingSkills;

		private TraitObject _traitWandererEquipment;

		private TraitObject _traitGentryEquipment;

		private TraitObject _traitThug;

		private TraitObject _traitThief;

		private TraitObject _traitGambler;

		private TraitObject _traitSmuggler;

		private readonly TraitObject[] _personality;

		private readonly TraitObject[] _skillCategories;
	}
}
