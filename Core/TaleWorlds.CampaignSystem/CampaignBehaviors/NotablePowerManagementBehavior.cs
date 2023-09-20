using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	public class NotablePowerManagementBehavior : CampaignBehaviorBase
	{
		public override void RegisterEvents()
		{
			CampaignEvents.HeroCreated.AddNonSerializedListener(this, new Action<Hero, bool>(this.OnHeroCreated));
			CampaignEvents.HeroKilledEvent.AddNonSerializedListener(this, new Action<Hero, Hero, KillCharacterAction.KillCharacterActionDetail, bool>(this.OnHeroKilled));
			CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.DailyTick));
			CampaignEvents.RaidCompletedEvent.AddNonSerializedListener(this, new Action<BattleSideEnum, RaidEventComponent>(this.OnRaidCompleted));
			CampaignEvents.OnGameLoadFinishedEvent.AddNonSerializedListener(this, new Action(this.OnGameLoadFinished));
		}

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

		private void OnHeroCreated(Hero hero, bool isMaternal)
		{
			if (hero.IsNotable)
			{
				this._notables.Add(hero);
				hero.AddPower((float)Campaign.Current.Models.NotablePowerModel.GetInitialPower());
			}
		}

		private void OnHeroKilled(Hero hero, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification)
		{
			if (hero.IsNotable && this._notables.Contains(hero))
			{
				this._notables.Remove(hero);
			}
		}

		private void DailyTick()
		{
			foreach (Hero hero in this._notables)
			{
				hero.AddPower(Campaign.Current.Models.NotablePowerModel.CalculateDailyPowerChangeForHero(hero, false).ResultNumber);
			}
		}

		private void OnRaidCompleted(BattleSideEnum winnerSide, RaidEventComponent mapEvent)
		{
			foreach (Hero hero in mapEvent.MapEventSettlement.Notables)
			{
				hero.AddPower(-5f);
			}
		}

		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<List<Hero>>("_notables", ref this._notables);
		}

		private List<Hero> _notables = new List<Hero>();
	}
}
