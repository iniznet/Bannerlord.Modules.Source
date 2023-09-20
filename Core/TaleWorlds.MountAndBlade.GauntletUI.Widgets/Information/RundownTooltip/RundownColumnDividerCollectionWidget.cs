using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Information.RundownTooltip
{
	public class RundownColumnDividerCollectionWidget : ListPanel
	{
		public float DividerWidth { get; set; }

		public RundownColumnDividerCollectionWidget(UIContext context)
			: base(context)
		{
		}

		public void Refresh(IReadOnlyList<float> columnWidths)
		{
			base.RemoveAllChildren();
			for (int i = 0; i < columnWidths.Count - 1; i++)
			{
				Widget widget = this.CreateFixedSpaceWidget(columnWidths[i] * base._inverseScaleToUse - this.DividerWidth);
				base.AddChild(widget);
				base.AddChild(this.CreateDividerWidget());
			}
			base.AddChild(this.CreateStretchedSpaceWidget());
		}

		private Widget CreateFixedSpaceWidget(float width)
		{
			return new Widget(base.Context)
			{
				WidthSizePolicy = SizePolicy.Fixed,
				HeightSizePolicy = SizePolicy.StretchToParent,
				SuggestedWidth = width
			};
		}

		private Widget CreateStretchedSpaceWidget()
		{
			return new Widget(base.Context)
			{
				WidthSizePolicy = SizePolicy.StretchToParent,
				HeightSizePolicy = SizePolicy.StretchToParent
			};
		}

		private Widget CreateDividerWidget()
		{
			return new Widget(base.Context)
			{
				WidthSizePolicy = SizePolicy.Fixed,
				HeightSizePolicy = SizePolicy.StretchToParent,
				SuggestedWidth = this.DividerWidth,
				Sprite = this.DividerSprite,
				Color = this.DividerColor
			};
		}

		[Editor(false)]
		public Sprite DividerSprite
		{
			get
			{
				return this._dividerSprite;
			}
			set
			{
				if (value != this._dividerSprite)
				{
					this._dividerSprite = value;
					base.OnPropertyChanged<Sprite>(value, "DividerSprite");
				}
			}
		}

		[Editor(false)]
		public Color DividerColor
		{
			get
			{
				return this._dividerColor;
			}
			set
			{
				if (value != this._dividerColor)
				{
					this._dividerColor = value;
					base.OnPropertyChanged(value, "DividerColor");
				}
			}
		}

		private Sprite _dividerSprite;

		private Color _dividerColor = new Color(1f, 1f, 1f, 1f);
	}
}
