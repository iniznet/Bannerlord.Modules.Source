using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement
{
	// Token: 0x020000FB RID: 251
	public class ClanCardSelectionPopupItemVM : ViewModel
	{
		// Token: 0x170007E9 RID: 2025
		// (get) Token: 0x0600175E RID: 5982 RVA: 0x0005668E File Offset: 0x0005488E
		public object Identifier { get; }

		// Token: 0x170007EA RID: 2026
		// (get) Token: 0x0600175F RID: 5983 RVA: 0x00056696 File Offset: 0x00054896
		public TextObject ActionResultText { get; }

		// Token: 0x06001760 RID: 5984 RVA: 0x000566A0 File Offset: 0x000548A0
		public ClanCardSelectionPopupItemVM(in ClanCardSelectionItemInfo info, Action<ClanCardSelectionPopupItemVM> onSelected)
		{
			this.Identifier = info.Identifier;
			this._onSelected = onSelected;
			this.ActionResultText = info.ActionResult;
			this._titleText = info.Title;
			this._disabledReasonText = info.DisabledReason;
			this._specialActionText = info.SpecialActionText;
			this.DisabledHint = new HintViewModel();
			this.Properties = new MBBindingList<ClanCardSelectionPopupItemPropertyVM>();
			if (info.Properties != null)
			{
				foreach (ClanCardSelectionItemPropertyInfo clanCardSelectionItemPropertyInfo in info.Properties)
				{
					this.Properties.Add(new ClanCardSelectionPopupItemPropertyVM(clanCardSelectionItemPropertyInfo));
				}
			}
			this.IsDisabled = info.IsDisabled;
			this.IsSpecialActionItem = info.IsSpecialActionItem;
			this.HasSprite = !string.IsNullOrEmpty(info.SpriteName);
			this.HasImage = info.Image != null;
			this.SpriteType = info.SpriteType.ToString();
			this.SpriteName = info.SpriteName ?? string.Empty;
			this.SpriteLabel = info.SpriteLabel ?? string.Empty;
			this.Image = (this.HasImage ? new ImageIdentifierVM(info.Image) : new ImageIdentifierVM(ImageIdentifierType.Null));
		}

		// Token: 0x06001761 RID: 5985 RVA: 0x000567FC File Offset: 0x000549FC
		public override void RefreshValues()
		{
			base.RefreshValues();
			TextObject titleText = this._titleText;
			this.Title = ((titleText != null) ? titleText.ToString() : null) ?? string.Empty;
			TextObject specialActionText = this._specialActionText;
			this.SpecialAction = ((specialActionText != null) ? specialActionText.ToString() : null) ?? string.Empty;
			HintViewModel disabledHint = this.DisabledHint;
			string text = "{=!}";
			string text2;
			if (!this.IsDisabled)
			{
				text2 = null;
			}
			else
			{
				TextObject disabledReasonText = this._disabledReasonText;
				text2 = ((disabledReasonText != null) ? disabledReasonText.ToString() : null);
			}
			disabledHint.HintText = new TextObject(text + text2, null);
			this.Properties.ApplyActionOnAllItems(delegate(ClanCardSelectionPopupItemPropertyVM x)
			{
				x.RefreshValues();
			});
		}

		// Token: 0x06001762 RID: 5986 RVA: 0x000568B3 File Offset: 0x00054AB3
		public void ExecuteSelect()
		{
			Action<ClanCardSelectionPopupItemVM> onSelected = this._onSelected;
			if (onSelected == null)
			{
				return;
			}
			onSelected(this);
		}

		// Token: 0x170007EB RID: 2027
		// (get) Token: 0x06001763 RID: 5987 RVA: 0x000568C6 File Offset: 0x00054AC6
		// (set) Token: 0x06001764 RID: 5988 RVA: 0x000568CE File Offset: 0x00054ACE
		[DataSourceProperty]
		public ImageIdentifierVM Image
		{
			get
			{
				return this._image;
			}
			set
			{
				if (value != this._image)
				{
					this._image = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "Image");
				}
			}
		}

		// Token: 0x170007EC RID: 2028
		// (get) Token: 0x06001765 RID: 5989 RVA: 0x000568EC File Offset: 0x00054AEC
		// (set) Token: 0x06001766 RID: 5990 RVA: 0x000568F4 File Offset: 0x00054AF4
		[DataSourceProperty]
		public MBBindingList<ClanCardSelectionPopupItemPropertyVM> Properties
		{
			get
			{
				return this._properties;
			}
			set
			{
				if (value != this._properties)
				{
					this._properties = value;
					base.OnPropertyChangedWithValue<MBBindingList<ClanCardSelectionPopupItemPropertyVM>>(value, "Properties");
				}
			}
		}

		// Token: 0x170007ED RID: 2029
		// (get) Token: 0x06001767 RID: 5991 RVA: 0x00056912 File Offset: 0x00054B12
		// (set) Token: 0x06001768 RID: 5992 RVA: 0x0005691A File Offset: 0x00054B1A
		[DataSourceProperty]
		public HintViewModel DisabledHint
		{
			get
			{
				return this._disabledHint;
			}
			set
			{
				if (value != this._disabledHint)
				{
					this._disabledHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "DisabledHint");
				}
			}
		}

		// Token: 0x170007EE RID: 2030
		// (get) Token: 0x06001769 RID: 5993 RVA: 0x00056938 File Offset: 0x00054B38
		// (set) Token: 0x0600176A RID: 5994 RVA: 0x00056940 File Offset: 0x00054B40
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

		// Token: 0x170007EF RID: 2031
		// (get) Token: 0x0600176B RID: 5995 RVA: 0x00056963 File Offset: 0x00054B63
		// (set) Token: 0x0600176C RID: 5996 RVA: 0x0005696B File Offset: 0x00054B6B
		[DataSourceProperty]
		public string SpriteType
		{
			get
			{
				return this._spriteType;
			}
			set
			{
				if (value != this._spriteType)
				{
					this._spriteType = value;
					base.OnPropertyChangedWithValue<string>(value, "SpriteType");
				}
			}
		}

		// Token: 0x170007F0 RID: 2032
		// (get) Token: 0x0600176D RID: 5997 RVA: 0x0005698E File Offset: 0x00054B8E
		// (set) Token: 0x0600176E RID: 5998 RVA: 0x00056996 File Offset: 0x00054B96
		[DataSourceProperty]
		public string SpriteName
		{
			get
			{
				return this._spriteName;
			}
			set
			{
				if (value != this._spriteName)
				{
					this._spriteName = value;
					base.OnPropertyChangedWithValue<string>(value, "SpriteName");
				}
			}
		}

		// Token: 0x170007F1 RID: 2033
		// (get) Token: 0x0600176F RID: 5999 RVA: 0x000569B9 File Offset: 0x00054BB9
		// (set) Token: 0x06001770 RID: 6000 RVA: 0x000569C1 File Offset: 0x00054BC1
		[DataSourceProperty]
		public string SpriteLabel
		{
			get
			{
				return this._spriteLabel;
			}
			set
			{
				if (value != this._spriteLabel)
				{
					this._spriteLabel = value;
					base.OnPropertyChangedWithValue<string>(value, "SpriteLabel");
				}
			}
		}

		// Token: 0x170007F2 RID: 2034
		// (get) Token: 0x06001771 RID: 6001 RVA: 0x000569E4 File Offset: 0x00054BE4
		// (set) Token: 0x06001772 RID: 6002 RVA: 0x000569EC File Offset: 0x00054BEC
		[DataSourceProperty]
		public string SpecialAction
		{
			get
			{
				return this._specialAction;
			}
			set
			{
				if (value != this._specialAction)
				{
					this._specialAction = value;
					base.OnPropertyChangedWithValue<string>(value, "SpecialAction");
				}
			}
		}

		// Token: 0x170007F3 RID: 2035
		// (get) Token: 0x06001773 RID: 6003 RVA: 0x00056A0F File Offset: 0x00054C0F
		// (set) Token: 0x06001774 RID: 6004 RVA: 0x00056A17 File Offset: 0x00054C17
		[DataSourceProperty]
		public bool HasImage
		{
			get
			{
				return this._hasImage;
			}
			set
			{
				if (value != this._hasImage)
				{
					this._hasImage = value;
					base.OnPropertyChangedWithValue(value, "HasImage");
				}
			}
		}

		// Token: 0x170007F4 RID: 2036
		// (get) Token: 0x06001775 RID: 6005 RVA: 0x00056A35 File Offset: 0x00054C35
		// (set) Token: 0x06001776 RID: 6006 RVA: 0x00056A3D File Offset: 0x00054C3D
		[DataSourceProperty]
		public bool HasSprite
		{
			get
			{
				return this._hasSprite;
			}
			set
			{
				if (value != this._hasSprite)
				{
					this._hasSprite = value;
					base.OnPropertyChangedWithValue(value, "HasSprite");
				}
			}
		}

		// Token: 0x170007F5 RID: 2037
		// (get) Token: 0x06001777 RID: 6007 RVA: 0x00056A5B File Offset: 0x00054C5B
		// (set) Token: 0x06001778 RID: 6008 RVA: 0x00056A63 File Offset: 0x00054C63
		[DataSourceProperty]
		public bool IsSpecialActionItem
		{
			get
			{
				return this._isSpecialActionItem;
			}
			set
			{
				if (value != this._isSpecialActionItem)
				{
					this._isSpecialActionItem = value;
					base.OnPropertyChangedWithValue(value, "IsSpecialActionItem");
				}
			}
		}

		// Token: 0x170007F6 RID: 2038
		// (get) Token: 0x06001779 RID: 6009 RVA: 0x00056A81 File Offset: 0x00054C81
		// (set) Token: 0x0600177A RID: 6010 RVA: 0x00056A89 File Offset: 0x00054C89
		[DataSourceProperty]
		public bool IsDisabled
		{
			get
			{
				return this._isDisabled;
			}
			set
			{
				if (value != this._isDisabled)
				{
					this._isDisabled = value;
					base.OnPropertyChangedWithValue(value, "IsDisabled");
				}
			}
		}

		// Token: 0x170007F7 RID: 2039
		// (get) Token: 0x0600177B RID: 6011 RVA: 0x00056AA7 File Offset: 0x00054CA7
		// (set) Token: 0x0600177C RID: 6012 RVA: 0x00056AAF File Offset: 0x00054CAF
		[DataSourceProperty]
		public bool IsSelected
		{
			get
			{
				return this._isSelected;
			}
			set
			{
				if (value != this._isSelected)
				{
					this._isSelected = value;
					base.OnPropertyChangedWithValue(value, "IsSelected");
				}
			}
		}

		// Token: 0x04000B07 RID: 2823
		private readonly TextObject _titleText;

		// Token: 0x04000B08 RID: 2824
		private readonly TextObject _disabledReasonText;

		// Token: 0x04000B09 RID: 2825
		private readonly TextObject _specialActionText;

		// Token: 0x04000B0A RID: 2826
		private readonly Action<ClanCardSelectionPopupItemVM> _onSelected;

		// Token: 0x04000B0B RID: 2827
		private ImageIdentifierVM _image;

		// Token: 0x04000B0C RID: 2828
		private MBBindingList<ClanCardSelectionPopupItemPropertyVM> _properties;

		// Token: 0x04000B0D RID: 2829
		private HintViewModel _disabledHint;

		// Token: 0x04000B0E RID: 2830
		private string _title;

		// Token: 0x04000B0F RID: 2831
		private string _spriteType;

		// Token: 0x04000B10 RID: 2832
		private string _spriteName;

		// Token: 0x04000B11 RID: 2833
		private string _spriteLabel;

		// Token: 0x04000B12 RID: 2834
		private string _specialAction;

		// Token: 0x04000B13 RID: 2835
		private bool _hasImage;

		// Token: 0x04000B14 RID: 2836
		private bool _hasSprite;

		// Token: 0x04000B15 RID: 2837
		private bool _isSpecialActionItem;

		// Token: 0x04000B16 RID: 2838
		private bool _isDisabled;

		// Token: 0x04000B17 RID: 2839
		private bool _isSelected;
	}
}
