using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.ViewModelCollection.HUD.KillFeed.General;
using TaleWorlds.MountAndBlade.ViewModelCollection.HUD.KillFeed.Personal;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.HUD.KillFeed
{
	// Token: 0x020000E5 RID: 229
	public class SPKillFeedVM : ViewModel
	{
		// Token: 0x060014CC RID: 5324 RVA: 0x00043DD7 File Offset: 0x00041FD7
		public SPKillFeedVM()
		{
			this.GeneralCasualty = new SPGeneralKillNotificationVM();
			this.PersonalFeed = new SPPersonalKillNotificationVM();
		}

		// Token: 0x060014CD RID: 5325 RVA: 0x00043DF5 File Offset: 0x00041FF5
		public void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, bool isHeadshot)
		{
			this.GeneralCasualty.OnAgentRemoved(affectedAgent, affectorAgent, null, isHeadshot);
		}

		// Token: 0x060014CE RID: 5326 RVA: 0x00043E06 File Offset: 0x00042006
		public void OnPersonalKill(int damageAmount, bool isMountDamage, bool isFriendlyFire, bool isHeadshot, string killedAgentName, bool isUnconscious)
		{
			this.PersonalFeed.OnPersonalKill(damageAmount, isMountDamage, isFriendlyFire, isHeadshot, killedAgentName, isUnconscious);
		}

		// Token: 0x060014CF RID: 5327 RVA: 0x00043E1C File Offset: 0x0004201C
		public void OnPersonalDamage(int totalDamage, bool isVictimAgentMount, bool isFriendlyFire, string victimAgentName)
		{
			this.PersonalFeed.OnPersonalHit(totalDamage, isVictimAgentMount, isFriendlyFire, victimAgentName);
		}

		// Token: 0x170006D7 RID: 1751
		// (get) Token: 0x060014D0 RID: 5328 RVA: 0x00043E2E File Offset: 0x0004202E
		// (set) Token: 0x060014D1 RID: 5329 RVA: 0x00043E36 File Offset: 0x00042036
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

		// Token: 0x170006D8 RID: 1752
		// (get) Token: 0x060014D2 RID: 5330 RVA: 0x00043E54 File Offset: 0x00042054
		// (set) Token: 0x060014D3 RID: 5331 RVA: 0x00043E5C File Offset: 0x0004205C
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

		// Token: 0x040009F2 RID: 2546
		private SPGeneralKillNotificationVM _generalCasualty;

		// Token: 0x040009F3 RID: 2547
		private SPPersonalKillNotificationVM _personalFeed;
	}
}
