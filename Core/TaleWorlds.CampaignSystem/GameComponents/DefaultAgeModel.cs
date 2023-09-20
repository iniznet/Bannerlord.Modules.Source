using System;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	// Token: 0x020000EC RID: 236
	public class DefaultAgeModel : AgeModel
	{
		// Token: 0x170005AA RID: 1450
		// (get) Token: 0x06001439 RID: 5177 RVA: 0x00059891 File Offset: 0x00057A91
		public override int BecomeInfantAge
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x170005AB RID: 1451
		// (get) Token: 0x0600143A RID: 5178 RVA: 0x00059894 File Offset: 0x00057A94
		public override int BecomeChildAge
		{
			get
			{
				return 6;
			}
		}

		// Token: 0x170005AC RID: 1452
		// (get) Token: 0x0600143B RID: 5179 RVA: 0x00059897 File Offset: 0x00057A97
		public override int BecomeTeenagerAge
		{
			get
			{
				return 14;
			}
		}

		// Token: 0x170005AD RID: 1453
		// (get) Token: 0x0600143C RID: 5180 RVA: 0x0005989B File Offset: 0x00057A9B
		public override int HeroComesOfAge
		{
			get
			{
				return 18;
			}
		}

		// Token: 0x170005AE RID: 1454
		// (get) Token: 0x0600143D RID: 5181 RVA: 0x0005989F File Offset: 0x00057A9F
		public override int BecomeOldAge
		{
			get
			{
				return 47;
			}
		}

		// Token: 0x170005AF RID: 1455
		// (get) Token: 0x0600143E RID: 5182 RVA: 0x000598A3 File Offset: 0x00057AA3
		public override int MaxAge
		{
			get
			{
				return 128;
			}
		}

		// Token: 0x0600143F RID: 5183 RVA: 0x000598AC File Offset: 0x00057AAC
		public override void GetAgeLimitForLocation(CharacterObject character, out int minimumAge, out int maximumAge, string additionalTags = "")
		{
			if (character.Occupation == Occupation.TavernWench)
			{
				minimumAge = 20;
				maximumAge = 28;
				return;
			}
			if (character.Occupation == Occupation.Townsfolk)
			{
				if (additionalTags == "TavernVisitor")
				{
					minimumAge = 20;
					maximumAge = 60;
					return;
				}
				if (additionalTags == "TavernDrinker")
				{
					minimumAge = 20;
					maximumAge = 40;
					return;
				}
				if (additionalTags == "SlowTownsman")
				{
					minimumAge = 50;
					maximumAge = 70;
					return;
				}
				if (additionalTags == "TownsfolkCarryingStuff")
				{
					minimumAge = 20;
					maximumAge = 40;
					return;
				}
				if (additionalTags == "BroomsWoman")
				{
					minimumAge = 30;
					maximumAge = 45;
					return;
				}
				if (additionalTags == "Dancer")
				{
					minimumAge = 20;
					maximumAge = 28;
					return;
				}
				if (additionalTags == "Beggar")
				{
					minimumAge = 60;
					maximumAge = 90;
					return;
				}
				if (additionalTags == "Child")
				{
					minimumAge = this.BecomeChildAge;
					maximumAge = this.BecomeTeenagerAge;
					return;
				}
				if (additionalTags == "Teenager")
				{
					minimumAge = this.BecomeTeenagerAge;
					maximumAge = this.HeroComesOfAge;
					return;
				}
				if (additionalTags == "Infant")
				{
					minimumAge = this.BecomeInfantAge;
					maximumAge = this.BecomeChildAge;
					return;
				}
				if (additionalTags == "Notary" || additionalTags == "Barber")
				{
					minimumAge = 30;
					maximumAge = 80;
					return;
				}
				minimumAge = this.HeroComesOfAge;
				maximumAge = 70;
				return;
			}
			else if (character.Occupation == Occupation.Villager)
			{
				if (additionalTags == "TownsfolkCarryingStuff")
				{
					minimumAge = 20;
					maximumAge = 40;
					return;
				}
				if (additionalTags == "Child")
				{
					minimumAge = this.BecomeChildAge;
					maximumAge = this.BecomeTeenagerAge;
					return;
				}
				if (additionalTags == "Teenager")
				{
					minimumAge = this.BecomeTeenagerAge;
					maximumAge = this.HeroComesOfAge;
					return;
				}
				if (additionalTags == "Infant")
				{
					minimumAge = this.BecomeInfantAge;
					maximumAge = this.BecomeChildAge;
					return;
				}
				minimumAge = this.HeroComesOfAge;
				maximumAge = 70;
				return;
			}
			else
			{
				if (character.Occupation == Occupation.TavernGameHost)
				{
					minimumAge = 30;
					maximumAge = 40;
					return;
				}
				if (character.Occupation == Occupation.Musician)
				{
					minimumAge = 20;
					maximumAge = 40;
					return;
				}
				if (character.Occupation == Occupation.ArenaMaster)
				{
					minimumAge = 30;
					maximumAge = 60;
					return;
				}
				if (character.Occupation == Occupation.ShopWorker)
				{
					minimumAge = 18;
					maximumAge = 50;
					return;
				}
				if (character.Occupation == Occupation.Tavernkeeper)
				{
					minimumAge = 40;
					maximumAge = 80;
					return;
				}
				if (character.Occupation == Occupation.RansomBroker)
				{
					minimumAge = 30;
					maximumAge = 60;
					return;
				}
				if (character.Occupation == Occupation.Blacksmith || character.Occupation == Occupation.GoodsTrader || character.Occupation == Occupation.HorseTrader || character.Occupation == Occupation.Armorer || character.Occupation == Occupation.Weaponsmith)
				{
					minimumAge = 30;
					maximumAge = 80;
					return;
				}
				if (additionalTags == "AlleyGangMember")
				{
					minimumAge = 30;
					maximumAge = 40;
					return;
				}
				minimumAge = this.HeroComesOfAge;
				maximumAge = this.MaxAge;
				return;
			}
		}

		// Token: 0x06001440 RID: 5184 RVA: 0x00059B68 File Offset: 0x00057D68
		public override float GetSkillScalingModifierForAge(Hero hero, SkillObject skill, bool isByNaturalGrowth)
		{
			if (!isByNaturalGrowth)
			{
				return 1f;
			}
			float age = hero.Age;
			float num = 0f;
			if (age >= (float)this.BecomeChildAge && age < (float)this.BecomeTeenagerAge)
			{
				num = 0.2f;
			}
			else if (age >= (float)this.BecomeTeenagerAge && age < (float)this.HeroComesOfAge)
			{
				num = 0.5f;
			}
			else if (age >= (float)this.HeroComesOfAge)
			{
				if (skill == DefaultSkills.Riding)
				{
					num = 1f;
				}
				else
				{
					num = 0.8f;
				}
			}
			return num;
		}

		// Token: 0x04000716 RID: 1814
		public const string TavernVisitorTag = "TavernVisitor";

		// Token: 0x04000717 RID: 1815
		public const string TavernDrinkerTag = "TavernDrinker";

		// Token: 0x04000718 RID: 1816
		public const string SlowTownsmanTag = "SlowTownsman";

		// Token: 0x04000719 RID: 1817
		public const string TownsfolkCarryingStuffTag = "TownsfolkCarryingStuff";

		// Token: 0x0400071A RID: 1818
		public const string BroomsWomanTag = "BroomsWoman";

		// Token: 0x0400071B RID: 1819
		public const string DancerTag = "Dancer";

		// Token: 0x0400071C RID: 1820
		public const string BeggarTag = "Beggar";

		// Token: 0x0400071D RID: 1821
		public const string ChildTag = "Child";

		// Token: 0x0400071E RID: 1822
		public const string TeenagerTag = "Teenager";

		// Token: 0x0400071F RID: 1823
		public const string InfantTag = "Infant";

		// Token: 0x04000720 RID: 1824
		public const string NotaryTag = "Notary";

		// Token: 0x04000721 RID: 1825
		public const string BarberTag = "Barber";

		// Token: 0x04000722 RID: 1826
		public const string AlleyGangMemberTag = "AlleyGangMember";
	}
}
