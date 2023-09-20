using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Barter
{
	// Token: 0x0200016F RID: 367
	public class BarterItemVisualBrushWidget : BrushWidget
	{
		// Token: 0x060012B5 RID: 4789 RVA: 0x000339CE File Offset: 0x00031BCE
		public BarterItemVisualBrushWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060012B6 RID: 4790 RVA: 0x000339E4 File Offset: 0x00031BE4
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

		// Token: 0x060012B7 RID: 4791 RVA: 0x00033AAC File Offset: 0x00031CAC
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

		// Token: 0x060012B8 RID: 4792 RVA: 0x00033B0C File Offset: 0x00031D0C
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

		// Token: 0x060012B9 RID: 4793 RVA: 0x00033E04 File Offset: 0x00032004
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

		// Token: 0x17000699 RID: 1689
		// (get) Token: 0x060012BA RID: 4794 RVA: 0x00033E9C File Offset: 0x0003209C
		// (set) Token: 0x060012BB RID: 4795 RVA: 0x00033EA4 File Offset: 0x000320A4
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

		// Token: 0x1700069A RID: 1690
		// (get) Token: 0x060012BC RID: 4796 RVA: 0x00033EC2 File Offset: 0x000320C2
		// (set) Token: 0x060012BD RID: 4797 RVA: 0x00033ECA File Offset: 0x000320CA
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

		// Token: 0x1700069B RID: 1691
		// (get) Token: 0x060012BE RID: 4798 RVA: 0x00033EE8 File Offset: 0x000320E8
		// (set) Token: 0x060012BF RID: 4799 RVA: 0x00033EF0 File Offset: 0x000320F0
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

		// Token: 0x1700069C RID: 1692
		// (get) Token: 0x060012C0 RID: 4800 RVA: 0x00033F0E File Offset: 0x0003210E
		// (set) Token: 0x060012C1 RID: 4801 RVA: 0x00033F16 File Offset: 0x00032116
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

		// Token: 0x1700069D RID: 1693
		// (get) Token: 0x060012C2 RID: 4802 RVA: 0x00033F34 File Offset: 0x00032134
		// (set) Token: 0x060012C3 RID: 4803 RVA: 0x00033F3C File Offset: 0x0003213C
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

		// Token: 0x1700069E RID: 1694
		// (get) Token: 0x060012C4 RID: 4804 RVA: 0x00033F5A File Offset: 0x0003215A
		// (set) Token: 0x060012C5 RID: 4805 RVA: 0x00033F62 File Offset: 0x00032162
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

		// Token: 0x1700069F RID: 1695
		// (get) Token: 0x060012C6 RID: 4806 RVA: 0x00033F85 File Offset: 0x00032185
		// (set) Token: 0x060012C7 RID: 4807 RVA: 0x00033F8D File Offset: 0x0003218D
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

		// Token: 0x04000891 RID: 2193
		private bool _imageDetermined;

		// Token: 0x04000892 RID: 2194
		private string _type = "";

		// Token: 0x04000893 RID: 2195
		private string _fiefImagePath;

		// Token: 0x04000894 RID: 2196
		private bool _hasVisualIdentifier;

		// Token: 0x04000895 RID: 2197
		private BrushWidget _spriteWidget;

		// Token: 0x04000896 RID: 2198
		private MaskedTextureWidget _maskedTextureWidget;

		// Token: 0x04000897 RID: 2199
		private ImageIdentifierWidget _imageIdentifierWidget;

		// Token: 0x04000898 RID: 2200
		private Widget _spriteClipWidget;
	}
}
