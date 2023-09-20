using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.HUD.DamageFeed
{
	// Token: 0x020000EE RID: 238
	public class MissionAgentDamageFeedItemVM : ViewModel
	{
		// Token: 0x06001533 RID: 5427 RVA: 0x00045164 File Offset: 0x00043364
		public MissionAgentDamageFeedItemVM(string feedText, Action<MissionAgentDamageFeedItemVM> onRemove)
		{
			this._onRemove = onRemove;
			this.FeedText = feedText;
		}

		// Token: 0x06001534 RID: 5428 RVA: 0x0004517A File Offset: 0x0004337A
		public void ExecuteRemove()
		{
			this._onRemove(this);
		}

		// Token: 0x170006F7 RID: 1783
		// (get) Token: 0x06001535 RID: 5429 RVA: 0x00045188 File Offset: 0x00043388
		// (set) Token: 0x06001536 RID: 5430 RVA: 0x00045190 File Offset: 0x00043390
		[DataSourceProperty]
		public string FeedText
		{
			get
			{
				return this._feedText;
			}
			set
			{
				if (value != this._feedText)
				{
					this._feedText = value;
					base.OnPropertyChangedWithValue<string>(value, "FeedText");
				}
			}
		}

		// Token: 0x04000A27 RID: 2599
		private readonly Action<MissionAgentDamageFeedItemVM> _onRemove;

		// Token: 0x04000A28 RID: 2600
		private string _feedText;
	}
}
