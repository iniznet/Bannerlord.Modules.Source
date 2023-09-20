using System;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Items
{
	// Token: 0x020000C7 RID: 199
	public class EncyclopediaHistoryEventVM : EncyclopediaLinkVM
	{
		// Token: 0x06001313 RID: 4883 RVA: 0x000497BC File Offset: 0x000479BC
		public EncyclopediaHistoryEventVM(IEncyclopediaLog log)
		{
			this._log = log;
			this.RefreshValues();
		}

		// Token: 0x06001314 RID: 4884 RVA: 0x000497D4 File Offset: 0x000479D4
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.HistoryEventTimeText = this._log.GameTime.ToString();
			this.HistoryEventText = this._log.GetEncyclopediaText().ToString();
		}

		// Token: 0x06001315 RID: 4885 RVA: 0x0004981C File Offset: 0x00047A1C
		public void ExecuteLink(string link)
		{
			Campaign.Current.EncyclopediaManager.GoToLink(link);
		}

		// Token: 0x1700065F RID: 1631
		// (get) Token: 0x06001316 RID: 4886 RVA: 0x0004982E File Offset: 0x00047A2E
		// (set) Token: 0x06001317 RID: 4887 RVA: 0x00049836 File Offset: 0x00047A36
		[DataSourceProperty]
		public string HistoryEventTimeText
		{
			get
			{
				return this._historyEventTimeText;
			}
			set
			{
				if (value != this._historyEventTimeText)
				{
					this._historyEventTimeText = value;
					base.OnPropertyChangedWithValue<string>(value, "HistoryEventTimeText");
				}
			}
		}

		// Token: 0x17000660 RID: 1632
		// (get) Token: 0x06001318 RID: 4888 RVA: 0x00049859 File Offset: 0x00047A59
		// (set) Token: 0x06001319 RID: 4889 RVA: 0x00049861 File Offset: 0x00047A61
		[DataSourceProperty]
		public string HistoryEventText
		{
			get
			{
				return this._historyEventText;
			}
			set
			{
				if (value != this._historyEventText)
				{
					this._historyEventText = value;
					base.OnPropertyChangedWithValue<string>(value, "HistoryEventText");
				}
			}
		}

		// Token: 0x040008D7 RID: 2263
		private readonly IEncyclopediaLog _log;

		// Token: 0x040008D8 RID: 2264
		private string _historyEventText;

		// Token: 0x040008D9 RID: 2265
		private string _historyEventTimeText;
	}
}
