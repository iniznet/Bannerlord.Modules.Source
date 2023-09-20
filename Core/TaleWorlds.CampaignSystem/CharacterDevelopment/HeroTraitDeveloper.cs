﻿using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.CharacterDevelopment
{
	// Token: 0x02000349 RID: 841
	public class HeroTraitDeveloper : PropertyOwner<PropertyObject>
	{
		// Token: 0x06002ED9 RID: 11993 RVA: 0x000C0CD8 File Offset: 0x000BEED8
		internal static void AutoGeneratedStaticCollectObjectsHeroTraitDeveloper(object o, List<object> collectedObjects)
		{
			((HeroTraitDeveloper)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		// Token: 0x06002EDA RID: 11994 RVA: 0x000C0CE6 File Offset: 0x000BEEE6
		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this.Hero);
		}

		// Token: 0x06002EDB RID: 11995 RVA: 0x000C0CFB File Offset: 0x000BEEFB
		internal static object AutoGeneratedGetMemberValueHero(object o)
		{
			return ((HeroTraitDeveloper)o).Hero;
		}

		// Token: 0x17000B23 RID: 2851
		// (get) Token: 0x06002EDC RID: 11996 RVA: 0x000C0D08 File Offset: 0x000BEF08
		// (set) Token: 0x06002EDD RID: 11997 RVA: 0x000C0D10 File Offset: 0x000BEF10
		[SaveableProperty(0)]
		internal Hero Hero { get; private set; }

		// Token: 0x06002EDE RID: 11998 RVA: 0x000C0D19 File Offset: 0x000BEF19
		internal HeroTraitDeveloper(Hero hero)
		{
			this.Hero = hero;
			this.UpdateTraitXPAccordingToTraitLevels();
		}

		// Token: 0x06002EDF RID: 11999 RVA: 0x000C0D30 File Offset: 0x000BEF30
		public void AddTraitXp(TraitObject trait, int xpAmount)
		{
			xpAmount += base.GetPropertyValue(trait);
			int num;
			int num2;
			Campaign.Current.Models.CharacterDevelopmentModel.GetTraitLevelForTraitXp(this.Hero, trait, xpAmount, out num, out num2);
			base.SetPropertyValue(trait, num2);
			if (num != this.Hero.GetTraitLevel(trait))
			{
				this.Hero.SetTraitLevel(trait, num);
			}
		}

		// Token: 0x06002EE0 RID: 12000 RVA: 0x000C0D8C File Offset: 0x000BEF8C
		public void UpdateTraitXPAccordingToTraitLevels()
		{
			foreach (TraitObject traitObject in TraitObject.All)
			{
				int traitLevel = this.Hero.GetTraitLevel(traitObject);
				if (traitLevel != 0)
				{
					int traitXpRequiredForTraitLevel = Campaign.Current.Models.CharacterDevelopmentModel.GetTraitXpRequiredForTraitLevel(traitObject, traitLevel);
					base.SetPropertyValue(traitObject, traitXpRequiredForTraitLevel);
				}
			}
		}
	}
}
