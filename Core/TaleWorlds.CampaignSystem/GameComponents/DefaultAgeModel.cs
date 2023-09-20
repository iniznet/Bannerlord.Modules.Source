using System;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	public class DefaultAgeModel : AgeModel
	{
		public override int BecomeInfantAge
		{
			get
			{
				return 3;
			}
		}

		public override int BecomeChildAge
		{
			get
			{
				return 6;
			}
		}

		public override int BecomeTeenagerAge
		{
			get
			{
				return 14;
			}
		}

		public override int HeroComesOfAge
		{
			get
			{
				return 18;
			}
		}

		public override int BecomeOldAge
		{
			get
			{
				return 47;
			}
		}

		public override int MaxAge
		{
			get
			{
				return 128;
			}
		}

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

		public const string TavernVisitorTag = "TavernVisitor";

		public const string TavernDrinkerTag = "TavernDrinker";

		public const string SlowTownsmanTag = "SlowTownsman";

		public const string TownsfolkCarryingStuffTag = "TownsfolkCarryingStuff";

		public const string BroomsWomanTag = "BroomsWoman";

		public const string DancerTag = "Dancer";

		public const string BeggarTag = "Beggar";

		public const string ChildTag = "Child";

		public const string TeenagerTag = "Teenager";

		public const string InfantTag = "Infant";

		public const string NotaryTag = "Notary";

		public const string BarberTag = "Barber";

		public const string AlleyGangMemberTag = "AlleyGangMember";
	}
}
