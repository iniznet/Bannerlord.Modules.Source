using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.HUD.DamageFeed
{
	public class MissionAgentDamageFeedVM : ViewModel
	{
		public MissionAgentDamageFeedVM()
		{
			this._takenDamageText = new TextObject("{=meFS5F4V}-{DAMAGE}", null);
			this.FeedList = new MBBindingList<MissionAgentDamageFeedItemVM>();
			CombatLogManager.OnGenerateCombatLog += this.CombatLogManagerOnPrintCombatLog;
		}

		public override void OnFinalize()
		{
			CombatLogManager.OnGenerateCombatLog -= this.CombatLogManagerOnPrintCombatLog;
			base.OnFinalize();
		}

		private void CombatLogManagerOnPrintCombatLog(CombatLogData logData)
		{
			if (!logData.IsVictimAgentMine || logData.TotalDamage <= 0)
			{
				return;
			}
			this._takenDamageText.SetTextVariable("DAMAGE", logData.TotalDamage);
			MissionAgentDamageFeedItemVM missionAgentDamageFeedItemVM = new MissionAgentDamageFeedItemVM(this._takenDamageText.ToString(), new Action<MissionAgentDamageFeedItemVM>(this.RemoveItem));
			this.FeedList.Add(missionAgentDamageFeedItemVM);
		}

		private void RemoveItem(MissionAgentDamageFeedItemVM item)
		{
			this.FeedList.Remove(item);
		}

		[DataSourceProperty]
		public MBBindingList<MissionAgentDamageFeedItemVM> FeedList
		{
			get
			{
				return this._feedList;
			}
			set
			{
				if (value != this._feedList)
				{
					this._feedList = value;
					base.OnPropertyChangedWithValue<MBBindingList<MissionAgentDamageFeedItemVM>>(value, "FeedList");
				}
			}
		}

		private readonly TextObject _takenDamageText;

		private MBBindingList<MissionAgentDamageFeedItemVM> _feedList;
	}
}
