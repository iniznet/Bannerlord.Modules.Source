using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.HUD.DamageFeed
{
	// Token: 0x020000EF RID: 239
	public class MissionAgentDamageFeedVM : ViewModel
	{
		// Token: 0x06001537 RID: 5431 RVA: 0x000451B3 File Offset: 0x000433B3
		public MissionAgentDamageFeedVM()
		{
			this._takenDamageText = new TextObject("{=meFS5F4V}-{DAMAGE}", null);
			this.FeedList = new MBBindingList<MissionAgentDamageFeedItemVM>();
			CombatLogManager.OnGenerateCombatLog += this.CombatLogManagerOnPrintCombatLog;
		}

		// Token: 0x06001538 RID: 5432 RVA: 0x000451E8 File Offset: 0x000433E8
		public override void OnFinalize()
		{
			CombatLogManager.OnGenerateCombatLog -= this.CombatLogManagerOnPrintCombatLog;
			base.OnFinalize();
		}

		// Token: 0x06001539 RID: 5433 RVA: 0x00045204 File Offset: 0x00043404
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

		// Token: 0x0600153A RID: 5434 RVA: 0x00045265 File Offset: 0x00043465
		private void RemoveItem(MissionAgentDamageFeedItemVM item)
		{
			this.FeedList.Remove(item);
		}

		// Token: 0x170006F8 RID: 1784
		// (get) Token: 0x0600153B RID: 5435 RVA: 0x00045274 File Offset: 0x00043474
		// (set) Token: 0x0600153C RID: 5436 RVA: 0x0004527C File Offset: 0x0004347C
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

		// Token: 0x04000A29 RID: 2601
		private readonly TextObject _takenDamageText;

		// Token: 0x04000A2A RID: 2602
		private MBBindingList<MissionAgentDamageFeedItemVM> _feedList;
	}
}
