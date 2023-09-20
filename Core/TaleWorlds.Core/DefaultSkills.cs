using System;
using TaleWorlds.Localization;

namespace TaleWorlds.Core
{
	public class DefaultSkills
	{
		private static DefaultSkills Instance
		{
			get
			{
				return Game.Current.DefaultSkills;
			}
		}

		public static SkillObject OneHanded
		{
			get
			{
				return DefaultSkills.Instance._skillOneHanded;
			}
		}

		public static SkillObject TwoHanded
		{
			get
			{
				return DefaultSkills.Instance._skillTwoHanded;
			}
		}

		public static SkillObject Polearm
		{
			get
			{
				return DefaultSkills.Instance._skillPolearm;
			}
		}

		public static SkillObject Bow
		{
			get
			{
				return DefaultSkills.Instance._skillBow;
			}
		}

		public static SkillObject Crossbow
		{
			get
			{
				return DefaultSkills.Instance._skillCrossbow;
			}
		}

		public static SkillObject Throwing
		{
			get
			{
				return DefaultSkills.Instance._skillThrowing;
			}
		}

		public static SkillObject Riding
		{
			get
			{
				return DefaultSkills.Instance._skillRiding;
			}
		}

		public static SkillObject Athletics
		{
			get
			{
				return DefaultSkills.Instance._skillAthletics;
			}
		}

		public static SkillObject Crafting
		{
			get
			{
				return DefaultSkills.Instance._skillCrafting;
			}
		}

		public static SkillObject Tactics
		{
			get
			{
				return DefaultSkills.Instance._skillTactics;
			}
		}

		public static SkillObject Scouting
		{
			get
			{
				return DefaultSkills.Instance._skillScouting;
			}
		}

		public static SkillObject Roguery
		{
			get
			{
				return DefaultSkills.Instance._skillRoguery;
			}
		}

		public static SkillObject Charm
		{
			get
			{
				return DefaultSkills.Instance._skillCharm;
			}
		}

		public static SkillObject Leadership
		{
			get
			{
				return DefaultSkills.Instance._skillLeadership;
			}
		}

		public static SkillObject Trade
		{
			get
			{
				return DefaultSkills.Instance._skillTrade;
			}
		}

		public static SkillObject Steward
		{
			get
			{
				return DefaultSkills.Instance._skillSteward;
			}
		}

		public static SkillObject Medicine
		{
			get
			{
				return DefaultSkills.Instance._skillMedicine;
			}
		}

		public static SkillObject Engineering
		{
			get
			{
				return DefaultSkills.Instance._skillEngineering;
			}
		}

		private SkillObject Create(string stringId)
		{
			return Game.Current.ObjectManager.RegisterPresumedObject<SkillObject>(new SkillObject(stringId));
		}

		private void InitializeAll()
		{
			this._skillOneHanded.Initialize(new TextObject("{=PiHpR4QL}One Handed", null), new TextObject("{=yEkSSqIm}Mastery of fighting with one-handed weapons either with a shield or without.", null), SkillObject.SkillTypeEnum.Personal).SetAttribute(DefaultCharacterAttributes.Vigor);
			this._skillTwoHanded.Initialize(new TextObject("{=t78atYqH}Two Handed", null), new TextObject("{=eoLbkhsY}Mastery of fighting with two-handed weapons of average length such as bigger axes and swords.", null), SkillObject.SkillTypeEnum.Personal).SetAttribute(DefaultCharacterAttributes.Vigor);
			this._skillPolearm.Initialize(new TextObject("{=haax8kMa}Polearm", null), new TextObject("{=iKmXX7i3}Mastery of the spear, lance, staff and other polearms, both one-handed and two-handed.", null), SkillObject.SkillTypeEnum.Personal).SetAttribute(DefaultCharacterAttributes.Vigor);
			this._skillBow.Initialize(new TextObject("{=5rj7xQE4}Bow", null), new TextObject("{=FLf5J3su}Familarity with bows and physical conditioning to shoot with them effectively.", null), SkillObject.SkillTypeEnum.Personal).SetAttribute(DefaultCharacterAttributes.Control);
			this._skillCrossbow.Initialize(new TextObject("{=TTWL7RLe}Crossbow", null), new TextObject("{=haV3nLYA}Knowledge of operating and maintaining crossbows.", null), SkillObject.SkillTypeEnum.Personal).SetAttribute(DefaultCharacterAttributes.Control);
			this._skillThrowing.Initialize(new TextObject("{=2wclahIJ}Throwing", null), new TextObject("{=NwTpATW5}Mastery of throwing projectiles accurately and with power.", null), SkillObject.SkillTypeEnum.Personal).SetAttribute(DefaultCharacterAttributes.Control);
			this._skillRiding.Initialize(new TextObject("{=p9i3zRm9}Riding", null), new TextObject("{=H9Zamrao}The ability to control a horse, to keep your balance when it moves suddenly or unexpectedly, as well as general knowledge of horses, including their care and breeding.", null), SkillObject.SkillTypeEnum.Personal).SetAttribute(DefaultCharacterAttributes.Endurance);
			this._skillAthletics.Initialize(new TextObject("{=skZS2UlW}Athletics", null), new TextObject("{=bVD9j0wI}Physical fitness, speed and balance.", null), SkillObject.SkillTypeEnum.Personal).SetAttribute(DefaultCharacterAttributes.Endurance);
			this._skillCrafting.Initialize(new TextObject("{=smithingskill}Smithing", null), new TextObject("{=xWbkjccP}The knowledge of how to forge metal, match handle to blade, turn poles, sew scales, and other skills useful in the assembly of weapons and armor", null), SkillObject.SkillTypeEnum.Personal).SetAttribute(DefaultCharacterAttributes.Endurance);
			this._skillScouting.Initialize(new TextObject("{=LJ6Krlbr}Scouting", null), new TextObject("{=kmBxaJZd}Knowledge of how to scan the wilderness for life. You can follow tracks, spot movement in the undergrowth, and spot an enemy across the valley from a flash of light on spearpoints or a dustcloud.", null), SkillObject.SkillTypeEnum.Party).SetAttribute(DefaultCharacterAttributes.Cunning);
			this._skillTactics.Initialize(new TextObject("{=m8o51fc7}Tactics", null), new TextObject("{=FQOFDrAu}Your judgment of how troops will perform in contact. This allows you to make a good prediction of when an unorthodox tactic will work, and when it won't.", null), SkillObject.SkillTypeEnum.Personal).SetAttribute(DefaultCharacterAttributes.Cunning);
			this._skillRoguery.Initialize(new TextObject("{=V0ZMJ0PX}Roguery", null), new TextObject("{=81YLbLok}Experience with the darker side of human life. You can tell when a guard wants a bribe, you know how to intimidate someone, and have a good sense of what you can and can't get away with.", null), SkillObject.SkillTypeEnum.Personal).SetAttribute(DefaultCharacterAttributes.Cunning);
			this._skillCharm.Initialize(new TextObject("{=EGeY1gfs}Charm", null), new TextObject("{=VajIVjkc}The ability to make a person like and trust you. You can make a good guess at people's motivations and the kinds of arguments to which they'll respond.", null), SkillObject.SkillTypeEnum.Personal).SetAttribute(DefaultCharacterAttributes.Social);
			this._skillLeadership.Initialize(new TextObject("{=HsLfmEmb}Leadership", null), new TextObject("{=97EmbcHQ}The ability to inspire. You can fill individuals with confidence and stir up enthusiasm and courage in larger groups.", null), SkillObject.SkillTypeEnum.Personal).SetAttribute(DefaultCharacterAttributes.Social);
			this._skillTrade.Initialize(new TextObject("{=GmcgoiGy}Trade", null), new TextObject("{=lsJMCkZy}Familiarity with the most common goods in the marketplace and their prices, as well as the ability to spot defective goods or tell if you've been shortchanged in quantity", null), SkillObject.SkillTypeEnum.Party).SetAttribute(DefaultCharacterAttributes.Social);
			this._skillSteward.Initialize(new TextObject("{=stewardskill}Steward", null), new TextObject("{=2K0iVRkW}Ability to organize a group and manage logistics. This helps you to run an estate or administer a town, and can increase the size of a party that you lead or in which you serve as quartermaster.", null), SkillObject.SkillTypeEnum.Party).SetAttribute(DefaultCharacterAttributes.Intelligence);
			this._skillMedicine.Initialize(new TextObject("{=JKH59XNp}Medicine", null), new TextObject("{=igg5sEh3}Knowledge of how to staunch bleeding, to set broken bones, to remove embedded weapons and clean wounds to prevent infection, and to apply poultices to relieve pain and soothe inflammation.", null), SkillObject.SkillTypeEnum.Party).SetAttribute(DefaultCharacterAttributes.Intelligence);
			this._skillEngineering.Initialize(new TextObject("{=engineeringskill}Engineering", null), new TextObject("{=hbaMnpVR}Knowledge of how to make things that can withstand powerful forces without collapsing. Useful for building both structures and the devices that knock them down.", null), SkillObject.SkillTypeEnum.Party).SetAttribute(DefaultCharacterAttributes.Intelligence);
		}

		public DefaultSkills()
		{
			this.RegisterAll();
		}

		private void RegisterAll()
		{
			this._skillOneHanded = this.Create("OneHanded");
			this._skillTwoHanded = this.Create("TwoHanded");
			this._skillPolearm = this.Create("Polearm");
			this._skillBow = this.Create("Bow");
			this._skillCrossbow = this.Create("Crossbow");
			this._skillThrowing = this.Create("Throwing");
			this._skillRiding = this.Create("Riding");
			this._skillAthletics = this.Create("Athletics");
			this._skillCrafting = this.Create("Crafting");
			this._skillTactics = this.Create("Tactics");
			this._skillScouting = this.Create("Scouting");
			this._skillRoguery = this.Create("Roguery");
			this._skillCharm = this.Create("Charm");
			this._skillTrade = this.Create("Trade");
			this._skillSteward = this.Create("Steward");
			this._skillLeadership = this.Create("Leadership");
			this._skillMedicine = this.Create("Medicine");
			this._skillEngineering = this.Create("Engineering");
			this.InitializeAll();
		}

		private SkillObject _skillEngineering;

		private SkillObject _skillMedicine;

		private SkillObject _skillLeadership;

		private SkillObject _skillSteward;

		private SkillObject _skillTrade;

		private SkillObject _skillCharm;

		private SkillObject _skillRoguery;

		private SkillObject _skillScouting;

		private SkillObject _skillTactics;

		private SkillObject _skillCrafting;

		private SkillObject _skillAthletics;

		private SkillObject _skillRiding;

		private SkillObject _skillThrowing;

		private SkillObject _skillCrossbow;

		private SkillObject _skillBow;

		private SkillObject _skillPolearm;

		private SkillObject _skillTwoHanded;

		private SkillObject _skillOneHanded;
	}
}
