using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Barter
{
	public class BarterItemVisualBrushWidget : BrushWidget
	{
		public BarterItemVisualBrushWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnParallelUpdate(float dt)
		{
			base.OnParallelUpdate(dt);
			if (!this._imageDetermined)
			{
				this.RegisterStatesOfWidgetFromBrush(this.SpriteWidget);
				this.UpdateVisual();
				this._imageDetermined = true;
			}
			if (this._imageDetermined && this.Type == "fief_barterable")
			{
				this.SpriteClipWidget.ClipContents = true;
				this.SpriteWidget.WidthSizePolicy = SizePolicy.Fixed;
				this.SpriteWidget.HeightSizePolicy = SizePolicy.Fixed;
				this.SpriteWidget.ScaledSuggestedHeight = this.SpriteClipWidget.Size.X;
				this.SpriteWidget.ScaledSuggestedWidth = this.SpriteClipWidget.Size.X;
				this.SpriteWidget.PositionYOffset = 18f;
				this.SpriteWidget.VerticalAlignment = VerticalAlignment.Center;
			}
		}

		private void RegisterStatesOfWidgetFromBrush(BrushWidget widget)
		{
			if (widget != null)
			{
				foreach (BrushLayer brushLayer in widget.ReadOnlyBrush.Layers)
				{
					widget.AddState(brushLayer.Name);
				}
			}
		}

		private void UpdateVisual()
		{
			Sprite sprite = null;
			this.SpriteWidget.IsVisible = false;
			this.MaskedTextureWidget.IsVisible = false;
			this.ImageIdentifierWidget.IsVisible = false;
			string type = this.Type;
			uint num = <PrivateImplementationDetails>.ComputeStringHash(type);
			if (num <= 2080743372U)
			{
				if (num <= 403518212U)
				{
					if (num != 189982571U)
					{
						if (num != 284088421U)
						{
							if (num != 403518212U)
							{
								goto IL_294;
							}
							if (!(type == "fief_barterable"))
							{
								goto IL_294;
							}
							sprite = base.EventManager.Context.SpriteData.GetSprite(this.FiefImagePath + "_t");
							this.SpriteWidget.Brush = base.EventManager.Context.DefaultBrush;
							this.SpriteWidget.IsVisible = true;
							goto IL_2A0;
						}
						else
						{
							if (!(type == "lift_siege_barterable"))
							{
								goto IL_294;
							}
							goto IL_294;
						}
					}
					else if (!(type == "join_faction_barterable"))
					{
						goto IL_294;
					}
				}
				else if (num <= 1289251258U)
				{
					if (num != 806661062U)
					{
						if (num != 1289251258U)
						{
							goto IL_294;
						}
						if (!(type == "marriage_barterable"))
						{
							goto IL_294;
						}
						goto IL_286;
					}
					else
					{
						if (!(type == "war_barterable"))
						{
							goto IL_294;
						}
						goto IL_294;
					}
				}
				else if (num != 1654682144U)
				{
					if (num != 2080743372U)
					{
						goto IL_294;
					}
					if (!(type == "leave_faction_barterable"))
					{
						goto IL_294;
					}
				}
				else
				{
					if (!(type == "start_siege_barterable"))
					{
						goto IL_294;
					}
					goto IL_294;
				}
			}
			else if (num <= 2639715379U)
			{
				if (num != 2166136261U)
				{
					if (num != 2342284176U)
					{
						if (num != 2639715379U)
						{
							goto IL_294;
						}
						if (!(type == "item_barterable"))
						{
							goto IL_294;
						}
						goto IL_286;
					}
					else
					{
						if (!(type == "set_prisoner_free_barterable"))
						{
							goto IL_294;
						}
						goto IL_286;
					}
				}
				else
				{
					if (type != null && type.Length != 0)
					{
						goto IL_294;
					}
					goto IL_294;
				}
			}
			else if (num <= 3787227692U)
			{
				if (num != 3249789840U)
				{
					if (num != 3787227692U)
					{
						goto IL_294;
					}
					if (!(type == "safe_passage_barterable"))
					{
						goto IL_294;
					}
					goto IL_294;
				}
				else if (!(type == "mercenary_join_faction_barterable"))
				{
					goto IL_294;
				}
			}
			else if (num != 3835993774U)
			{
				if (num != 3957684540U)
				{
					goto IL_294;
				}
				if (!(type == "gold_barterable"))
				{
					goto IL_294;
				}
				goto IL_294;
			}
			else
			{
				if (!(type == "peace_barterable"))
				{
					goto IL_294;
				}
				goto IL_294;
			}
			this.MaskedTextureWidget.IsVisible = true;
			goto IL_2A0;
			IL_286:
			this.ImageIdentifierWidget.IsVisible = true;
			goto IL_2A0;
			IL_294:
			this.SpriteWidget.IsVisible = true;
			IL_2A0:
			if (this.SpriteWidget.ContainsState(this.Type))
			{
				this.SpriteWidget.SetState(this.Type);
			}
			if (sprite != null)
			{
				this.SetWidgetSpriteForAllStyles(this.SpriteWidget, sprite);
			}
			this.SpriteClipWidget.IsVisible = this.SpriteWidget.IsVisible;
		}

		private void SetWidgetSpriteForAllStyles(BrushWidget widget, Sprite sprite)
		{
			widget.Sprite = sprite;
			foreach (Style style in widget.Brush.Styles)
			{
				foreach (StyleLayer styleLayer in style.Layers)
				{
					styleLayer.Sprite = sprite;
				}
			}
		}

		[Editor(false)]
		public BrushWidget SpriteWidget
		{
			get
			{
				return this._spriteWidget;
			}
			set
			{
				if (this._spriteWidget != value)
				{
					this._spriteWidget = value;
					base.OnPropertyChanged<BrushWidget>(value, "SpriteWidget");
				}
			}
		}

		[Editor(false)]
		public Widget SpriteClipWidget
		{
			get
			{
				return this._spriteClipWidget;
			}
			set
			{
				if (this._spriteClipWidget != value)
				{
					this._spriteClipWidget = value;
					base.OnPropertyChanged<Widget>(value, "SpriteClipWidget");
				}
			}
		}

		[Editor(false)]
		public ImageIdentifierWidget ImageIdentifierWidget
		{
			get
			{
				return this._imageIdentifierWidget;
			}
			set
			{
				if (this._imageIdentifierWidget != value)
				{
					this._imageIdentifierWidget = value;
					base.OnPropertyChanged<ImageIdentifierWidget>(value, "ImageIdentifierWidget");
				}
			}
		}

		[Editor(false)]
		public MaskedTextureWidget MaskedTextureWidget
		{
			get
			{
				return this._maskedTextureWidget;
			}
			set
			{
				if (this._maskedTextureWidget != value)
				{
					this._maskedTextureWidget = value;
					base.OnPropertyChanged<MaskedTextureWidget>(value, "MaskedTextureWidget");
				}
			}
		}

		[Editor(false)]
		public bool HasVisualIdentifier
		{
			get
			{
				return this._hasVisualIdentifier;
			}
			set
			{
				if (this._hasVisualIdentifier != value)
				{
					this._hasVisualIdentifier = value;
					base.OnPropertyChanged(value, "HasVisualIdentifier");
				}
			}
		}

		[Editor(false)]
		public string Type
		{
			get
			{
				return this._type;
			}
			set
			{
				if (this._type != value)
				{
					this._type = value;
					base.OnPropertyChanged<string>(value, "Type");
				}
			}
		}

		[Editor(false)]
		public string FiefImagePath
		{
			get
			{
				return this._fiefImagePath;
			}
			set
			{
				if (this._fiefImagePath != value)
				{
					this._fiefImagePath = value;
					base.OnPropertyChanged<string>(value, "FiefImagePath");
				}
			}
		}

		private bool _imageDetermined;

		private string _type = "";

		private string _fiefImagePath;

		private bool _hasVisualIdentifier;

		private BrushWidget _spriteWidget;

		private MaskedTextureWidget _maskedTextureWidget;

		private ImageIdentifierWidget _imageIdentifierWidget;

		private Widget _spriteClipWidget;
	}
}
