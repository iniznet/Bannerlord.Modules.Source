using System;
using System.Collections.Generic;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace TaleWorlds.Core.ViewModelCollection
{
	// Token: 0x02000003 RID: 3
	public class BattleResultVM : ViewModel
	{
		// Token: 0x06000012 RID: 18 RVA: 0x000021C4 File Offset: 0x000003C4
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

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000013 RID: 19 RVA: 0x0000221F File Offset: 0x0000041F
		// (set) Token: 0x06000014 RID: 20 RVA: 0x00002227 File Offset: 0x00000427
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

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000015 RID: 21 RVA: 0x0000224A File Offset: 0x0000044A
		// (set) Token: 0x06000016 RID: 22 RVA: 0x00002252 File Offset: 0x00000452
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

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000017 RID: 23 RVA: 0x00002270 File Offset: 0x00000470
		// (set) Token: 0x06000018 RID: 24 RVA: 0x00002278 File Offset: 0x00000478
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

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000019 RID: 25 RVA: 0x00002296 File Offset: 0x00000496
		// (set) Token: 0x0600001A RID: 26 RVA: 0x0000229E File Offset: 0x0000049E
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

		// Token: 0x04000002 RID: 2
		private string _text;

		// Token: 0x04000003 RID: 3
		private BasicTooltipViewModel _hint;

		// Token: 0x04000004 RID: 4
		private ImageIdentifierVM _deadLordPortrait;

		// Token: 0x04000005 RID: 5
		private ImageIdentifierVM _deadLordClanBanner;
	}
}
