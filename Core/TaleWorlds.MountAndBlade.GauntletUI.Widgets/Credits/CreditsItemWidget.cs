using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Credits
{
	public class CreditsItemWidget : Widget
	{
		public CreditsItemWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this._initialized)
			{
				this.RefreshItemWidget();
				this._initialized = true;
			}
		}

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

		private bool _initialized;

		private string _itemType;

		private Widget _categoryWidget;

		private Widget _sectionWidget;

		private Widget _entryWidget;

		private Widget _emptyLineWidget;

		private Widget _imageWidget;
	}
}
