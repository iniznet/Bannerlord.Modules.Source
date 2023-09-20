using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.ViewModelCollection.HUD.KillFeed.General;
using TaleWorlds.MountAndBlade.ViewModelCollection.HUD.KillFeed.Personal;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.HUD.KillFeed
{
	public class SPKillFeedVM : ViewModel
	{
		public SPKillFeedVM()
		{
			this.GeneralCasualty = new SPGeneralKillNotificationVM();
			this.PersonalFeed = new SPPersonalKillNotificationVM();
		}

		public void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, bool isHeadshot)
		{
			this.GeneralCasualty.OnAgentRemoved(affectedAgent, affectorAgent, null, isHeadshot);
		}

		public void OnPersonalKill(int damageAmount, bool isMountDamage, bool isFriendlyFire, bool isHeadshot, string killedAgentName, bool isUnconscious)
		{
			this.PersonalFeed.OnPersonalKill(damageAmount, isMountDamage, isFriendlyFire, isHeadshot, killedAgentName, isUnconscious);
		}

		public void OnPersonalDamage(int totalDamage, bool isVictimAgentMount, bool isFriendlyFire, string victimAgentName)
		{
			this.PersonalFeed.OnPersonalHit(totalDamage, isVictimAgentMount, isFriendlyFire, victimAgentName);
		}

		[DataSourceProperty]
		public SPGeneralKillNotificationVM GeneralCasualty
		{
			get
			{
				return this._generalCasualty;
			}
			set
			{
				if (value != this._generalCasualty)
				{
					this._generalCasualty = value;
					base.OnPropertyChangedWithValue<SPGeneralKillNotificationVM>(value, "GeneralCasualty");
				}
			}
		}

		[DataSourceProperty]
		public SPPersonalKillNotificationVM PersonalFeed
		{
			get
			{
				return this._personalFeed;
			}
			set
			{
				if (value != this._personalFeed)
				{
					this._personalFeed = value;
					base.OnPropertyChangedWithValue<SPPersonalKillNotificationVM>(value, "PersonalFeed");
				}
			}
		}

		private SPGeneralKillNotificationVM _generalCasualty;

		private SPPersonalKillNotificationVM _personalFeed;
	}
}
