using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.KillFeed.General;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.KillFeed.Personal;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.KillFeed
{
	public class MPKillFeedVM : ViewModel
	{
		public MPKillFeedVM()
		{
			this.GeneralCasualty = new MPGeneralKillNotificationVM();
			this.PersonalCasualty = new MPPersonalKillNotificationVM();
		}

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

		private void OnPersonalAssist(string victimAgentName)
		{
			this.PersonalCasualty.OnPersonalAssist(victimAgentName);
		}

		public void OnPersonalDamage(int damageAmount, bool isFatal, bool isMountDamage, bool isFriendlyDamage, bool isHeadshot, string killedAgentName)
		{
			this.PersonalCasualty.OnPersonalHit(damageAmount, isFatal, isMountDamage, isFriendlyDamage, isHeadshot, killedAgentName);
		}

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

		private MPGeneralKillNotificationVM _generalCasualty;

		private MPPersonalKillNotificationVM _personalCasualty;
	}
}
