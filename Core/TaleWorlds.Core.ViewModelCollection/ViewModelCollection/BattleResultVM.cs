using System;
using System.Collections.Generic;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace TaleWorlds.Core.ViewModelCollection
{
	public class BattleResultVM : ViewModel
	{
		public BattleResultVM(string text, Func<List<TooltipProperty>> propertyFunc, CharacterCode deadHeroCode = null)
		{
			this.Text = text;
			this.Hint = new BasicTooltipViewModel(propertyFunc);
			if (deadHeroCode != null)
			{
				this.DeadLordPortrait = new ImageIdentifierVM(deadHeroCode);
				this.DeadLordClanBanner = new ImageIdentifierVM(BannerCode.CreateFrom(deadHeroCode.Banner), true);
				return;
			}
			this.DeadLordPortrait = null;
			this.DeadLordClanBanner = null;
		}

		[DataSourceProperty]
		public string Text
		{
			get
			{
				return this._text;
			}
			set
			{
				if (value != this._text)
				{
					this._text = value;
					base.OnPropertyChangedWithValue<string>(value, "Text");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel Hint
		{
			get
			{
				return this._hint;
			}
			set
			{
				if (value != this._hint)
				{
					this._hint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "Hint");
				}
			}
		}

		[DataSourceProperty]
		public ImageIdentifierVM DeadLordPortrait
		{
			get
			{
				return this._deadLordPortrait;
			}
			set
			{
				if (value != this._deadLordPortrait)
				{
					this._deadLordPortrait = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "DeadLordPortrait");
				}
			}
		}

		[DataSourceProperty]
		public ImageIdentifierVM DeadLordClanBanner
		{
			get
			{
				return this._deadLordClanBanner;
			}
			set
			{
				if (value != this._deadLordClanBanner)
				{
					this._deadLordClanBanner = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "DeadLordClanBanner");
				}
			}
		}

		private string _text;

		private BasicTooltipViewModel _hint;

		private ImageIdentifierVM _deadLordPortrait;

		private ImageIdentifierVM _deadLordClanBanner;
	}
}
