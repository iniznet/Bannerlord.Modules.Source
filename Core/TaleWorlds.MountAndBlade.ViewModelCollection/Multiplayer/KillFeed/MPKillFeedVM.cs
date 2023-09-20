using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.KillFeed.General;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.KillFeed.Personal;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.KillFeed
{
	// Token: 0x020000AF RID: 175
	public class MPKillFeedVM : ViewModel
	{
		// Token: 0x060010AD RID: 4269 RVA: 0x000372BF File Offset: 0x000354BF
		public MPKillFeedVM()
		{
			this.GeneralCasualty = new MPGeneralKillNotificationVM();
			this.PersonalCasualty = new MPPersonalKillNotificationVM();
		}

		// Token: 0x060010AE RID: 4270 RVA: 0x000372E0 File Offset: 0x000354E0
		public void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, bool isPersonalFeedEnabled)
		{
			Agent assistedAgent = this.GetAssistedAgent(affectedAgent, affectorAgent);
			if (assistedAgent != null && assistedAgent.IsMainAgent && isPersonalFeedEnabled)
			{
				string text = affectedAgent.Name;
				if (affectedAgent.MissionPeer != null)
				{
					text = affectedAgent.MissionPeer.DisplayedName;
				}
				this.OnPersonalAssist(text);
			}
			this.GeneralCasualty.OnAgentRemoved(affectedAgent, affectorAgent, assistedAgent);
		}

		// Token: 0x060010AF RID: 4271 RVA: 0x00037336 File Offset: 0x00035536
		private void OnPersonalAssist(string victimAgentName)
		{
			this.PersonalCasualty.OnPersonalAssist(victimAgentName);
		}

		// Token: 0x060010B0 RID: 4272 RVA: 0x00037344 File Offset: 0x00035544
		public void OnPersonalDamage(int damageAmount, bool isFatal, bool isMountDamage, bool isFriendlyDamage, bool isHeadshot, string killedAgentName)
		{
			this.PersonalCasualty.OnPersonalHit(damageAmount, isFatal, isMountDamage, isFriendlyDamage, isHeadshot, killedAgentName);
		}

		// Token: 0x060010B1 RID: 4273 RVA: 0x0003735A File Offset: 0x0003555A
		private Agent GetAssistedAgent(Agent affectedAgent, Agent affectorAgent)
		{
			if (affectedAgent == null)
			{
				return null;
			}
			Agent.Hitter assistingHitter = affectedAgent.GetAssistingHitter((affectorAgent != null) ? affectorAgent.MissionPeer : null);
			if (assistingHitter == null)
			{
				return null;
			}
			MissionPeer hitterPeer = assistingHitter.HitterPeer;
			if (hitterPeer == null)
			{
				return null;
			}
			return hitterPeer.ControlledAgent;
		}

		// Token: 0x1700055E RID: 1374
		// (get) Token: 0x060010B2 RID: 4274 RVA: 0x00037389 File Offset: 0x00035589
		// (set) Token: 0x060010B3 RID: 4275 RVA: 0x00037391 File Offset: 0x00035591
		[DataSourceProperty]
		public MPGeneralKillNotificationVM GeneralCasualty
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
					base.OnPropertyChangedWithValue<MPGeneralKillNotificationVM>(value, "GeneralCasualty");
				}
			}
		}

		// Token: 0x1700055F RID: 1375
		// (get) Token: 0x060010B4 RID: 4276 RVA: 0x000373AF File Offset: 0x000355AF
		// (set) Token: 0x060010B5 RID: 4277 RVA: 0x000373B7 File Offset: 0x000355B7
		[DataSourceProperty]
		public MPPersonalKillNotificationVM PersonalCasualty
		{
			get
			{
				return this._personalCasualty;
			}
			set
			{
				if (value != this._personalCasualty)
				{
					this._personalCasualty = value;
					base.OnPropertyChangedWithValue<MPPersonalKillNotificationVM>(value, "PersonalCasualty");
				}
			}
		}

		// Token: 0x040007F0 RID: 2032
		private MPGeneralKillNotificationVM _generalCasualty;

		// Token: 0x040007F1 RID: 2033
		private MPPersonalKillNotificationVM _personalCasualty;
	}
}
