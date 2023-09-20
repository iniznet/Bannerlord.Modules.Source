using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.HUD.DamageFeed
{
	public class MissionAgentDamageFeedItemVM : ViewModel
	{
		public MissionAgentDamageFeedItemVM(string feedText, Action<MissionAgentDamageFeedItemVM> onRemove)
		{
			this._onRemove = onRemove;
			this.FeedText = feedText;
		}

		public void ExecuteRemove()
		{
			this._onRemove(this);
		}

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

		private readonly Action<MissionAgentDamageFeedItemVM> _onRemove;

		private string _feedText;
	}
}
