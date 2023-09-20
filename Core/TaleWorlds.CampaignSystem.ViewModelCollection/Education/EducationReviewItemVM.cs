using System;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Education
{
	public class EducationReviewItemVM : ViewModel
	{
		public void UpdateWith(string gainText)
		{
			this.GainText = gainText;
		}

		[DataSourceProperty]
		public string Title
		{
			get
			{
				return this._title;
			}
			set
			{
				if (value != this._title)
				{
					this._title = value;
					base.OnPropertyChangedWithValue<string>(value, "Title");
				}
			}
		}

		[DataSourceProperty]
		public string GainText
		{
			get
			{
				return this._gainText;
			}
			set
			{
				if (value != this._gainText)
				{
					this._gainText = value;
					base.OnPropertyChangedWithValue<string>(value, "GainText");
				}
			}
		}

		private string _title;

		private string _gainText;
	}
}
