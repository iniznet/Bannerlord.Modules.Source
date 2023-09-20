using System;
using TaleWorlds.Localization;

namespace TaleWorlds.Core
{
	// Token: 0x02000051 RID: 81
	public class DefaultSkills
	{
		// Token: 0x17000228 RID: 552
		// (get) Token: 0x060005F9 RID: 1529 RVA: 0x00015F87 File Offset: 0x00014187
		private static DefaultSkills Instance
		{
			get
			{
				return Game.Current.DefaultSkills;
			}
		}

		// Token: 0x17000229 RID: 553
		// (get) Token: 0x060005FA RID: 1530 RVA: 0x00015F93 File Offset: 0x00014193
		public static SkillObject OneHanded
		{
			get
			{
				return DefaultSkills.Instance._skillOneHanded;
			}
		}

		// Token: 0x1700022A RID: 554
		// (get) Token: 0x060005FB RID: 1531 RVA: 0x00015F9F File Offset: 0x0001419F
		public static SkillObject TwoHanded
		{
			get
			{
				return DefaultSkills.Instance._skillTwoHanded;
			}
		}

		// Token: 0x1700022B RID: 555
		// (get) Token: 0x060005FC RID: 1532 RVA: 0x00015FAB File Offset: 0x000141AB
		public static SkillObject Polearm
		{
			get
			{
				return DefaultSkills.Instance._skillPolearm;
			}
		}

		// Token: 0x1700022C RID: 556
		// (get) Token: 0x060005FD RID: 1533 RVA: 0x00015FB7 File Offset: 0x000141B7
		public static SkillObject Bow
		{
			get
			{
				return DefaultSkills.Instance._skillBow;
			}
		}

		// Token: 0x1700022D RID: 557
		// (get) Token: 0x060005FE RID: 1534 RVA: 0x00015FC3 File Offset: 0x000141C3
		public static SkillObject Crossbow
		{
			get
			{
				return DefaultSkills.Instance._skillCrossbow;
			}
		}

		// Token: 0x1700022E RID: 558
		// (get) Token: 0x060005FF RID: 1535 RVA: 0x00015FCF File Offset: 0x000141CF
		public static SkillObject Throwing
		{
			get
			{
				return DefaultSkills.Instance._skillThrowing;
			}
		}

		// Token: 0x1700022F RID: 559
		// (get) Token: 0x06000600 RID: 1536 RVA: 0x00015FDB File Offset: 0x000141DB
		public static SkillObject Riding
		{
			get
			{
				return DefaultSkills.Instance._skillRiding;
			}
		}

		// Token: 0x17000230 RID: 560
		// (get) Token: 0x06000601 RID: 1537 RVA: 0x00015FE7 File Offset: 0x000141E7
		public static SkillObject Athletics
		{
			get
			{
				return DefaultSkills.Instance._skillAthletics;
			}
		}

		// Token: 0x17000231 RID: 561
		// (get) Token: 0x06000602 RID: 1538 RVA: 0x00015FF3 File Offset: 0x000141F3
		public static SkillObject Crafting
		{
			get
			{
				return DefaultSkills.Instance._skillCrafting;
			}
		}

		// Token: 0x17000232 RID: 562
		// (get) Token: 0x06000603 RID: 1539 RVA: 0x00015FFF File Offset: 0x000141FF
		public static SkillObject Tactics
		{
			get
			{
				return DefaultSkills.Instance._skillTactics;
			}
		}

		// Token: 0x17000233 RID: 563
		// (get) Token: 0x06000604 RID: 1540 RVA: 0x0001600B File Offset: 0x0001420B
		public static SkillObject Scouting
		{
			get
			{
				return DefaultSkills.Instance._skillScouting;
			}
		}

		// Token: 0x17000234 RID: 564
		// (get) Token: 0x06000605 RID: 1541 RVA: 0x00016017 File Offset: 0x00014217
		public static SkillObject Roguery
		{
			get
			{
				return DefaultSkills.Instance._skillRoguery;
			}
		}

		// Token: 0x17000235 RID: 565
		// (get) Token: 0x06000606 RID: 1542 RVA: 0x00016023 File Offset: 0x00014223
		public static SkillObject Charm
		{
			get
			{
				return DefaultSkills.Instance._skillCharm;
			}
		}

		// Token: 0x17000236 RID: 566
		// (get) Token: 0x06000607 RID: 1543 RVA: 0x0001602F File Offset: 0x0001422F
		public static SkillObject Leadership
		{
			get
			{
				return DefaultSkills.Instance._skillLeadership;
			}
		}

		// Token: 0x17000237 RID: 567
		// (get) Token: 0x06000608 RID: 1544 RVA: 0x0001603B File Offset: 0x0001423B
		public static SkillObject Trade
		{
			get
			{
				return DefaultSkills.Instance._skillTrade;
			}
		}

		// Token: 0x17000238 RID: 568
		// (get) Token: 0x06000609 RID: 1545 RVA: 0x00016047 File Offset: 0x00014247
		public static SkillObject Steward
		{
			get
			{
				return DefaultSkills.Instance._skillSteward;
			}
		}

		// Token: 0x17000239 RID: 569
		// (get) Token: 0x0600060A RID: 1546 RVA: 0x00016053 File Offset: 0x00014253
		public static SkillObject Medicine
		{
			get
			{
				return DefaultSkills.Instance._skillMedicine;
			}
		}

		// Token: 0x1700023A RID: 570
		// (get) Token: 0x0600060B RID: 1547 RVA: 0x0001605F File Offset: 0x0001425F
		public static SkillObject Engineering
		{
			get
			{
				return DefaultSkills.Instance._skillEngineering;
			}
		}

		// Token: 0x0600060C RID: 1548 RVA: 0x0001606B File Offset: 0x0001426B
		private SkillObject Create(string stringId)
		{
			return Game.Current.ObjectManager.RegisterPresumedObject<SkillObject>(new SkillObject(stringId));
		}

		// Token: 0x0600060D RID: 1549 RVA: 0x00016084 File Offset: 0x00014284
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

		// Token: 0x0600060E RID: 1550 RVA: 0x000163A9 File Offset: 0x000145A9
		public DefaultSkills()
		{
			this.RegisterAll();
		}

		// Token: 0x0600060F RID: 1551 RVA: 0x000163B8 File Offset: 0x000145B8
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

		// Token: 0x04000301 RID: 769
		public const int MaxAssumedValue = 250;

		// Token: 0x04000302 RID: 770
		private SkillObject _skillEngineering;

		// Token: 0x04000303 RID: 771
		private SkillObject _skillMedicine;

		// Token: 0x04000304 RID: 772
		private SkillObject _skillLeadership;

		// Token: 0x04000305 RID: 773
		private SkillObject _skillSteward;

		// Token: 0x04000306 RID: 774
		private SkillObject _skillTrade;

		// Token: 0x04000307 RID: 775
		private SkillObject _skillCharm;

		// Token: 0x04000308 RID: 776
		private SkillObject _skillRoguery;

		// Token: 0x04000309 RID: 777
		private SkillObject _skillScouting;

		// Token: 0x0400030A RID: 778
		private SkillObject _skillTactics;

		// Token: 0x0400030B RID: 779
		private SkillObject _skillCrafting;

		// Token: 0x0400030C RID: 780
		private SkillObject _skillAthletics;

		// Token: 0x0400030D RID: 781
		private SkillObject _skillRiding;

		// Token: 0x0400030E RID: 782
		private SkillObject _skillThrowing;

		// Token: 0x0400030F RID: 783
		private SkillObject _skillCrossbow;

		// Token: 0x04000310 RID: 784
		private SkillObject _skillBow;

		// Token: 0x04000311 RID: 785
		private SkillObject _skillPolearm;

		// Token: 0x04000312 RID: 786
		private SkillObject _skillTwoHanded;

		// Token: 0x04000313 RID: 787
		private SkillObject _skillOneHanded;
	}
}
