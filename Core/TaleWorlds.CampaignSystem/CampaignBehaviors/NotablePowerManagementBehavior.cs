using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x020003AF RID: 943
	public class NotablePowerManagementBehavior : CampaignBehaviorBase
	{
		// Token: 0x0600385E RID: 14430 RVA: 0x0010023C File Offset: 0x000FE43C
		public override void RegisterEvents()
		{
			CampaignEvents.HeroCreated.AddNonSerializedListener(this, new Action<Hero, bool>(this.OnHeroCreated));
			CampaignEvents.HeroKilledEvent.AddNonSerializedListener(this, new Action<Hero, Hero, KillCharacterAction.KillCharacterActionDetail, bool>(this.OnHeroKilled));
			CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.DailyTick));
			CampaignEvents.RaidCompletedEvent.AddNonSerializedListener(this, new Action<BattleSideEnum, RaidEventComponent>(this.OnRaidCompleted));
			CampaignEvents.OnGameLoadFinishedEvent.AddNonSerializedListener(this, new Action(this.OnGameLoadFinished));
		}

		// Token: 0x0600385F RID: 14431 RVA: 0x001002BC File Offset: 0x000FE4BC
		private void OnGameLoadFinished()
		{
			if (this._notables == null)
			{
				this._notables = new List<Hero>();
				foreach (Hero hero in Hero.AllAliveHeroes)
				{
					if (hero.IsNotable)
					{
						this._notables.Add(hero);
					}
				}
			}
		}

		// Token: 0x06003860 RID: 14432 RVA: 0x00100330 File Offset: 0x000FE530
		private void OnHeroCreated(Hero hero, bool isMaternal)
		{
			if (hero.IsNotable)
			{
				this._notables.Add(hero);
				hero.AddPower((float)Campaign.Current.Models.NotablePowerModel.GetInitialPower());
			}
		}

		// Token: 0x06003861 RID: 14433 RVA: 0x00100361 File Offset: 0x000FE561
		private void OnHeroKilled(Hero hero, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification)
		{
			if (hero.IsNotable && this._notables.Contains(hero))
			{
				this._notables.Remove(hero);
			}
		}

		// Token: 0x06003862 RID: 14434 RVA: 0x00100388 File Offset: 0x000FE588
		private void DailyTick()
		{
			foreach (Hero hero in this._notables)
			{
				hero.AddPower(Campaign.Current.Models.NotablePowerModel.CalculateDailyPowerChangeForHero(hero, false).ResultNumber);
			}
		}

		// Token: 0x06003863 RID: 14435 RVA: 0x001003F8 File Offset: 0x000FE5F8
		private void OnRaidCompleted(BattleSideEnum winnerSide, RaidEventComponent mapEvent)
		{
			foreach (Hero hero in mapEvent.MapEventSettlement.Notables)
			{
				hero.AddPower(-5f);
			}
		}

		// Token: 0x06003864 RID: 14436 RVA: 0x00100454 File Offset: 0x000FE654
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<List<Hero>>("_notables", ref this._notables);
		}

		// Token: 0x040011A6 RID: 4518
		private List<Hero> _notables = new List<Hero>();
	}
}
