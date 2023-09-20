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
	// Token: 0x020003C5 RID: 965
	public class PregnancyCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x060039D1 RID: 14801 RVA: 0x00109BA0 File Offset: 0x00107DA0
		public override void RegisterEvents()
		{
			CampaignEvents.HeroKilledEvent.AddNonSerializedListener(this, new Action<Hero, Hero, KillCharacterAction.KillCharacterActionDetail, bool>(this.OnHeroKilled));
			CampaignEvents.DailyTickHeroEvent.AddNonSerializedListener(this, new Action<Hero>(this.DailyTickHero));
			CampaignEvents.OnChildConceivedEvent.AddNonSerializedListener(this, new Action<Hero>(this.ChildConceived));
		}

		// Token: 0x060039D2 RID: 14802 RVA: 0x00109BF4 File Offset: 0x00107DF4
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

		// Token: 0x060039D3 RID: 14803 RVA: 0x00109C90 File Offset: 0x00107E90
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

		// Token: 0x060039D4 RID: 14804 RVA: 0x00109CD9 File Offset: 0x00107ED9
		private void RefreshSpouseVisit(Hero hero)
		{
			if (this.CheckAreNearby(hero, hero.Spouse) && MBRandom.RandomFloat <= Campaign.Current.Models.PregnancyModel.GetDailyChanceOfPregnancyForHero(hero))
			{
				MakePregnantAction.Apply(hero);
			}
		}

		// Token: 0x060039D5 RID: 14805 RVA: 0x00109D0C File Offset: 0x00107F0C
		private bool CheckAreNearby(Hero hero, Hero spouse)
		{
			Settlement settlement;
			MobileParty mobileParty;
			this.GetLocation(hero, out settlement, out mobileParty);
			Settlement settlement2;
			MobileParty mobileParty2;
			this.GetLocation(spouse, out settlement2, out mobileParty2);
			return (settlement != null && settlement == settlement2) || (hero.Clan != Hero.MainHero.Clan && MBRandom.RandomFloat < 0.2f);
		}

		// Token: 0x060039D6 RID: 14806 RVA: 0x00109D59 File Offset: 0x00107F59
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

		// Token: 0x060039D7 RID: 14807 RVA: 0x00109D98 File Offset: 0x00107F98
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

		// Token: 0x060039D8 RID: 14808 RVA: 0x00109ED0 File Offset: 0x001080D0
		private void ChildConceived(Hero mother)
		{
			this._heroPregnancies.Add(new PregnancyCampaignBehavior.Pregnancy(mother, mother.Spouse, CampaignTime.DaysFromNow(Campaign.Current.Models.PregnancyModel.PregnancyDurationInDays)));
		}

		// Token: 0x060039D9 RID: 14809 RVA: 0x00109F04 File Offset: 0x00108104
		public void OnHeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification)
		{
			if (victim.IsFemale && this._heroPregnancies.Any((PregnancyCampaignBehavior.Pregnancy pregnancy) => pregnancy.Mother == victim))
			{
				this._heroPregnancies.RemoveAll((PregnancyCampaignBehavior.Pregnancy pregnancy) => pregnancy.Mother == victim);
			}
		}

		// Token: 0x060039DA RID: 14810 RVA: 0x00109F5C File Offset: 0x0010815C
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<List<PregnancyCampaignBehavior.Pregnancy>>("_heroPregnancies", ref this._heroPregnancies);
		}

		// Token: 0x040011DA RID: 4570
		private List<PregnancyCampaignBehavior.Pregnancy> _heroPregnancies = new List<PregnancyCampaignBehavior.Pregnancy>();

		// Token: 0x0200070F RID: 1807
		public class PregnancyCampaignBehaviorTypeDefiner : CampaignBehaviorBase.SaveableCampaignBehaviorTypeDefiner
		{
			// Token: 0x060055BE RID: 21950 RVA: 0x0016D7C6 File Offset: 0x0016B9C6
			public PregnancyCampaignBehaviorTypeDefiner()
				: base(110000)
			{
			}

			// Token: 0x060055BF RID: 21951 RVA: 0x0016D7D3 File Offset: 0x0016B9D3
			protected override void DefineClassTypes()
			{
				base.AddClassDefinition(typeof(PregnancyCampaignBehavior.Pregnancy), 2, null);
			}

			// Token: 0x060055C0 RID: 21952 RVA: 0x0016D7E7 File Offset: 0x0016B9E7
			protected override void DefineContainerDefinitions()
			{
				base.ConstructContainerDefinition(typeof(List<PregnancyCampaignBehavior.Pregnancy>));
			}
		}

		// Token: 0x02000710 RID: 1808
		internal class Pregnancy
		{
			// Token: 0x060055C1 RID: 21953 RVA: 0x0016D7F9 File Offset: 0x0016B9F9
			public Pregnancy(Hero pregnantHero, Hero father, CampaignTime dueDate)
			{
				this.Mother = pregnantHero;
				this.Father = father;
				this.DueDate = dueDate;
			}

			// Token: 0x060055C2 RID: 21954 RVA: 0x0016D816 File Offset: 0x0016BA16
			internal static void AutoGeneratedStaticCollectObjectsPregnancy(object o, List<object> collectedObjects)
			{
				((PregnancyCampaignBehavior.Pregnancy)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			// Token: 0x060055C3 RID: 21955 RVA: 0x0016D824 File Offset: 0x0016BA24
			protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				collectedObjects.Add(this.Mother);
				collectedObjects.Add(this.Father);
				CampaignTime.AutoGeneratedStaticCollectObjectsCampaignTime(this.DueDate, collectedObjects);
			}

			// Token: 0x060055C4 RID: 21956 RVA: 0x0016D84F File Offset: 0x0016BA4F
			internal static object AutoGeneratedGetMemberValueMother(object o)
			{
				return ((PregnancyCampaignBehavior.Pregnancy)o).Mother;
			}

			// Token: 0x060055C5 RID: 21957 RVA: 0x0016D85C File Offset: 0x0016BA5C
			internal static object AutoGeneratedGetMemberValueFather(object o)
			{
				return ((PregnancyCampaignBehavior.Pregnancy)o).Father;
			}

			// Token: 0x060055C6 RID: 21958 RVA: 0x0016D869 File Offset: 0x0016BA69
			internal static object AutoGeneratedGetMemberValueDueDate(object o)
			{
				return ((PregnancyCampaignBehavior.Pregnancy)o).DueDate;
			}

			// Token: 0x04001D2B RID: 7467
			[SaveableField(1)]
			public readonly Hero Mother;

			// Token: 0x04001D2C RID: 7468
			[SaveableField(2)]
			public readonly Hero Father;

			// Token: 0x04001D2D RID: 7469
			[SaveableField(3)]
			public readonly CampaignTime DueDate;
		}
	}
}
