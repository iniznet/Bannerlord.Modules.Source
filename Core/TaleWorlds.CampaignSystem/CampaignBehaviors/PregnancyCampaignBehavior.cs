using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	public class PregnancyCampaignBehavior : CampaignBehaviorBase
	{
		public override void RegisterEvents()
		{
			CampaignEvents.HeroKilledEvent.AddNonSerializedListener(this, new Action<Hero, Hero, KillCharacterAction.KillCharacterActionDetail, bool>(this.OnHeroKilled));
			CampaignEvents.DailyTickHeroEvent.AddNonSerializedListener(this, new Action<Hero>(this.DailyTickHero));
			CampaignEvents.OnChildConceivedEvent.AddNonSerializedListener(this, new Action<Hero>(this.ChildConceived));
		}

		private void DailyTickHero(Hero hero)
		{
			if (hero.IsFemale && !CampaignOptions.IsLifeDeathCycleDisabled && hero.IsAlive && hero.Age > (float)Campaign.Current.Models.AgeModel.HeroComesOfAge && (hero.Clan == null || !hero.Clan.IsRebelClan))
			{
				if (hero.Age > 18f && hero.Spouse != null && hero.Spouse.IsAlive && !hero.IsPregnant)
				{
					this.RefreshSpouseVisit(hero);
				}
				if (hero.IsPregnant)
				{
					this.CheckOffspringsToDeliver(hero);
				}
			}
		}

		private void CheckOffspringsToDeliver(Hero hero)
		{
			PregnancyCampaignBehavior.Pregnancy pregnancy = this._heroPregnancies.Find((PregnancyCampaignBehavior.Pregnancy x) => x.Mother == hero);
			if (pregnancy == null)
			{
				hero.IsPregnant = false;
				return;
			}
			this.CheckOffspringsToDeliver(pregnancy);
		}

		private void RefreshSpouseVisit(Hero hero)
		{
			if (this.CheckAreNearby(hero, hero.Spouse) && MBRandom.RandomFloat <= Campaign.Current.Models.PregnancyModel.GetDailyChanceOfPregnancyForHero(hero))
			{
				MakePregnantAction.Apply(hero);
			}
		}

		private bool CheckAreNearby(Hero hero, Hero spouse)
		{
			Settlement settlement;
			MobileParty mobileParty;
			this.GetLocation(hero, out settlement, out mobileParty);
			Settlement settlement2;
			MobileParty mobileParty2;
			this.GetLocation(spouse, out settlement2, out mobileParty2);
			return (settlement != null && settlement == settlement2) || (mobileParty != null && mobileParty == mobileParty2) || (hero.Clan != Hero.MainHero.Clan && MBRandom.RandomFloat < 0.2f);
		}

		private void GetLocation(Hero hero, out Settlement heroSettlement, out MobileParty heroParty)
		{
			heroSettlement = hero.CurrentSettlement;
			heroParty = hero.PartyBelongedTo;
			MobileParty mobileParty = heroParty;
			if (((mobileParty != null) ? mobileParty.AttachedTo : null) != null)
			{
				heroParty = heroParty.AttachedTo;
			}
			if (heroSettlement == null)
			{
				MobileParty mobileParty2 = heroParty;
				heroSettlement = ((mobileParty2 != null) ? mobileParty2.CurrentSettlement : null);
			}
		}

		private void CheckOffspringsToDeliver(PregnancyCampaignBehavior.Pregnancy pregnancy)
		{
			PregnancyModel pregnancyModel = Campaign.Current.Models.PregnancyModel;
			if (!pregnancy.DueDate.IsFuture && pregnancy.Mother.IsAlive)
			{
				Hero mother = pregnancy.Mother;
				bool flag = MBRandom.RandomFloat <= pregnancyModel.DeliveringTwinsProbability;
				List<Hero> list = new List<Hero>();
				int num = (flag ? 2 : 1);
				int num2 = 0;
				for (int i = 0; i < num; i++)
				{
					if (MBRandom.RandomFloat > pregnancyModel.StillbirthProbability)
					{
						bool flag2 = MBRandom.RandomFloat <= pregnancyModel.DeliveringFemaleOffspringProbability;
						Hero hero = HeroCreator.DeliverOffSpring(mother, pregnancy.Father, flag2);
						list.Add(hero);
					}
					else
					{
						TextObject textObject = new TextObject("{=pw4cUPEn}{MOTHER.LINK} has delivered stillborn.", null);
						StringHelpers.SetCharacterProperties("MOTHER", mother.CharacterObject, textObject, false);
						InformationManager.DisplayMessage(new InformationMessage(textObject.ToString()));
						num2++;
					}
				}
				CampaignEventDispatcher.Instance.OnGivenBirth(mother, list, num2);
				mother.IsPregnant = false;
				this._heroPregnancies.Remove(pregnancy);
				if (mother != Hero.MainHero && MBRandom.RandomFloat <= pregnancyModel.MaternalMortalityProbabilityInLabor)
				{
					KillCharacterAction.ApplyInLabor(mother, true);
				}
			}
		}

		private void ChildConceived(Hero mother)
		{
			this._heroPregnancies.Add(new PregnancyCampaignBehavior.Pregnancy(mother, mother.Spouse, CampaignTime.DaysFromNow(Campaign.Current.Models.PregnancyModel.PregnancyDurationInDays)));
		}

		public void OnHeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification)
		{
			if (victim.IsFemale && this._heroPregnancies.Any((PregnancyCampaignBehavior.Pregnancy pregnancy) => pregnancy.Mother == victim))
			{
				this._heroPregnancies.RemoveAll((PregnancyCampaignBehavior.Pregnancy pregnancy) => pregnancy.Mother == victim);
			}
		}

		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<List<PregnancyCampaignBehavior.Pregnancy>>("_heroPregnancies", ref this._heroPregnancies);
		}

		private List<PregnancyCampaignBehavior.Pregnancy> _heroPregnancies = new List<PregnancyCampaignBehavior.Pregnancy>();

		public class PregnancyCampaignBehaviorTypeDefiner : SaveableTypeDefiner
		{
			public PregnancyCampaignBehaviorTypeDefiner()
				: base(110000)
			{
			}

			protected override void DefineClassTypes()
			{
				base.AddClassDefinition(typeof(PregnancyCampaignBehavior.Pregnancy), 2, null);
			}

			protected override void DefineContainerDefinitions()
			{
				base.ConstructContainerDefinition(typeof(List<PregnancyCampaignBehavior.Pregnancy>));
			}
		}

		internal class Pregnancy
		{
			public Pregnancy(Hero pregnantHero, Hero father, CampaignTime dueDate)
			{
				this.Mother = pregnantHero;
				this.Father = father;
				this.DueDate = dueDate;
			}

			internal static void AutoGeneratedStaticCollectObjectsPregnancy(object o, List<object> collectedObjects)
			{
				((PregnancyCampaignBehavior.Pregnancy)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				collectedObjects.Add(this.Mother);
				collectedObjects.Add(this.Father);
				CampaignTime.AutoGeneratedStaticCollectObjectsCampaignTime(this.DueDate, collectedObjects);
			}

			internal static object AutoGeneratedGetMemberValueMother(object o)
			{
				return ((PregnancyCampaignBehavior.Pregnancy)o).Mother;
			}

			internal static object AutoGeneratedGetMemberValueFather(object o)
			{
				return ((PregnancyCampaignBehavior.Pregnancy)o).Father;
			}

			internal static object AutoGeneratedGetMemberValueDueDate(object o)
			{
				return ((PregnancyCampaignBehavior.Pregnancy)o).DueDate;
			}

			[SaveableField(1)]
			public readonly Hero Mother;

			[SaveableField(2)]
			public readonly Hero Father;

			[SaveableField(3)]
			public readonly CampaignTime DueDate;
		}
	}
}
