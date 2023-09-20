using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Credits
{
	// Token: 0x0200013E RID: 318
	public class CreditsItemWidget : Widget
	{
		// Token: 0x060010B5 RID: 4277 RVA: 0x0002EC9F File Offset: 0x0002CE9F
		public CreditsItemWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060010B6 RID: 4278 RVA: 0x0002ECA8 File Offset: 0x0002CEA8
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this._initialized)
			{
				this.RefreshItemWidget();
				this._initialized = true;
			}
		}

		// Token: 0x060010B7 RID: 4279 RVA: 0x0002ECC8 File Offset: 0x0002CEC8
		private void RefreshItemWidget()
		{
			if (!string.IsNullOrEmpty(this.ItemType))
			{
				if (this.CategoryWidget != null)
				{
					this.CategoryWidget.IsVisible = this.ItemType == "Category";
				}
				if (this.SectionWidget != null)
				{
					this.SectionWidget.IsVisible = this.ItemType == "Section";
				}
				if (this.EntryWidget != null)
				{
					this.EntryWidget.IsVisible = this.ItemType == "Entry";
				}
				if (this.EmptyLineWidget != null)
				{
					this.EmptyLineWidget.IsVisible = this.ItemType == "EmptyLine";
				}
				if (this.ImageWidget != null)
				{
					this.ImageWidget.IsVisible = this.ItemType == "Image";
					if (this.ImageWidget.Sprite != null)
					{
						this.ImageWidget.SuggestedWidth = (float)this.ImageWidget.Sprite.Width;
						this.ImageWidget.SuggestedHeight = (float)this.ImageWidget.Sprite.Height;
					}
				}
			}
		}

		// Token: 0x170005E5 RID: 1509
		// (get) Token: 0x060010B8 RID: 4280 RVA: 0x0002EDD9 File Offset: 0x0002CFD9
		// (set) Token: 0x060010B9 RID: 4281 RVA: 0x0002EDE1 File Offset: 0x0002CFE1
		[Editor(false)]
		public string ItemType
		{
			get
			{
				return this._itemType;
			}
			set
			{
				if (this._itemType != value)
				{
					this._itemType = value;
					base.OnPropertyChanged<string>(value, "ItemType");
				}
			}
		}

		// Token: 0x170005E6 RID: 1510
		// (get) Token: 0x060010BA RID: 4282 RVA: 0x0002EE04 File Offset: 0x0002D004
		// (set) Token: 0x060010BB RID: 4283 RVA: 0x0002EE0C File Offset: 0x0002D00C
		[Editor(false)]
		public Widget CategoryWidget
		{
			get
			{
				return this._categoryWidget;
			}
			set
			{
				if (this._categoryWidget != value)
				{
					this._categoryWidget = value;
					base.OnPropertyChanged<Widget>(value, "CategoryWidget");
				}
			}
		}

		// Token: 0x170005E7 RID: 1511
		// (get) Token: 0x060010BC RID: 4284 RVA: 0x0002EE2A File Offset: 0x0002D02A
		// (set) Token: 0x060010BD RID: 4285 RVA: 0x0002EE32 File Offset: 0x0002D032
		[Editor(false)]
		public Widget ImageWidget
		{
			get
			{
				return this._imageWidget;
			}
			set
			{
				if (this._imageWidget != value)
				{
					this._imageWidget = value;
					base.OnPropertyChanged<Widget>(value, "ImageWidget");
				}
			}
		}

		// Token: 0x170005E8 RID: 1512
		// (get) Token: 0x060010BE RID: 4286 RVA: 0x0002EE50 File Offset: 0x0002D050
		// (set) Token: 0x060010BF RID: 4287 RVA: 0x0002EE58 File Offset: 0x0002D058
		[Editor(false)]
		public Widget SectionWidget
		{
			get
			{
				return this._sectionWidget;
			}
			set
			{
				if (this._sectionWidget != value)
				{
					this._sectionWidget = value;
					base.OnPropertyChanged<Widget>(value, "SectionWidget");
				}
			}
		}

		// Token: 0x170005E9 RID: 1513
		// (get) Token: 0x060010C0 RID: 4288 RVA: 0x0002EE76 File Offset: 0x0002D076
		// (set) Token: 0x060010C1 RID: 4289 RVA: 0x0002EE7E File Offset: 0x0002D07E
		[Editor(false)]
		public Widget EntryWidget
		{
			get
			{
				return this._entryWidget;
			}
			set
			{
				if (this._entryWidget != value)
				{
					this._entryWidget = value;
					base.OnPropertyChanged<Widget>(value, "EntryWidget");
				}
			}
		}

		// Token: 0x170005EA RID: 1514
		// (get) Token: 0x060010C2 RID: 4290 RVA: 0x0002EE9C File Offset: 0x0002D09C
		// (set) Token: 0x060010C3 RID: 4291 RVA: 0x0002EEA4 File Offset: 0x0002D0A4
		[Editor(false)]
		public Widget EmptyLineWidget
		{
			get
			{
				return this._emptyLineWidget;
			}
			set
			{
				if (this._emptyLineWidget != value)
				{
					this._emptyLineWidget = value;
					base.OnPropertyChanged<Widget>(value, "EmptyLineWidget");
				}
			}
		}

		// Token: 0x040007AC RID: 1964
		private bool _initialized;

		// Token: 0x040007AD RID: 1965
		private string _itemType;

		// Token: 0x040007AE RID: 1966
		private Widget _categoryWidget;

		// Token: 0x040007AF RID: 1967
		private Widget _sectionWidget;

		// Token: 0x040007B0 RID: 1968
		private Widget _entryWidget;

		// Token: 0x040007B1 RID: 1969
		private Widget _emptyLineWidget;

		// Token: 0x040007B2 RID: 1970
		private Widget _imageWidget;
	}
}
