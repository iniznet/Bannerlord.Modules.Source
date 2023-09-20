using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Information
{
	// Token: 0x0200012A RID: 298
	public class TooltipPropertyWidget : Widget
	{
		// Token: 0x17000573 RID: 1395
		// (get) Token: 0x06000F89 RID: 3977 RVA: 0x0002B634 File Offset: 0x00029834
		// (set) Token: 0x06000F8A RID: 3978 RVA: 0x0002B63C File Offset: 0x0002983C
		public bool IsTwoColumn { get; private set; }

		// Token: 0x17000574 RID: 1396
		// (get) Token: 0x06000F8B RID: 3979 RVA: 0x0002B645 File Offset: 0x00029845
		// (set) Token: 0x06000F8C RID: 3980 RVA: 0x0002B64D File Offset: 0x0002984D
		public TooltipPropertyWidget.TooltipPropertyFlags PropertyModifierAsFlag { get; private set; }

		// Token: 0x17000575 RID: 1397
		// (get) Token: 0x06000F8D RID: 3981 RVA: 0x0002B656 File Offset: 0x00029856
		private bool _allBrushesInitialized
		{
			get
			{
				return this.SubtextBrush != null && this.ValueTextBrush != null && this.DescriptionTextBrush != null && this.ValueNameTextBrush != null && this.RundownSeperatorSprite != null && this.DefaultSeperatorSprite != null && this.TitleBackgroundSprite != null;
			}
		}

		// Token: 0x17000576 RID: 1398
		// (get) Token: 0x06000F8E RID: 3982 RVA: 0x0002B693 File Offset: 0x00029893
		public bool IsMultiLine
		{
			get
			{
				return this._isMultiLine;
			}
		}

		// Token: 0x17000577 RID: 1399
		// (get) Token: 0x06000F8F RID: 3983 RVA: 0x0002B69B File Offset: 0x0002989B
		public bool IsBattleMode
		{
			get
			{
				return this._isBattleMode;
			}
		}

		// Token: 0x17000578 RID: 1400
		// (get) Token: 0x06000F90 RID: 3984 RVA: 0x0002B6A3 File Offset: 0x000298A3
		public bool IsBattleModeOver
		{
			get
			{
				return this._isBattleModeOver;
			}
		}

		// Token: 0x17000579 RID: 1401
		// (get) Token: 0x06000F91 RID: 3985 RVA: 0x0002B6AB File Offset: 0x000298AB
		public bool IsCost
		{
			get
			{
				return this._isCost;
			}
		}

		// Token: 0x1700057A RID: 1402
		// (get) Token: 0x06000F92 RID: 3986 RVA: 0x0002B6B3 File Offset: 0x000298B3
		public bool IsRelation
		{
			get
			{
				return this._isRelation;
			}
		}

		// Token: 0x06000F93 RID: 3987 RVA: 0x0002B6BB File Offset: 0x000298BB
		public TooltipPropertyWidget(UIContext context)
			: base(context)
		{
			this._isMultiLine = false;
			this._isBattleMode = false;
			this._isBattleModeOver = false;
		}

		// Token: 0x06000F94 RID: 3988 RVA: 0x0002B6E7 File Offset: 0x000298E7
		public void SetBattleScope(bool battleScope)
		{
			if (battleScope)
			{
				this.DefinitionLabel.HorizontalAlignment = HorizontalAlignment.Center;
				this.ValueLabel.HorizontalAlignment = HorizontalAlignment.Center;
				return;
			}
			this.DefinitionLabel.HorizontalAlignment = HorizontalAlignment.Right;
			this.ValueLabel.HorizontalAlignment = HorizontalAlignment.Left;
		}

		// Token: 0x06000F95 RID: 3989 RVA: 0x0002B720 File Offset: 0x00029920
		public void RefreshSize(bool inBattleScope, float battleScopeSize, float maxValueLabelSizeX, float maxDefinitionLabelSizeX, Brush definitionRelationBrush = null, Brush valueRelationBrush = null)
		{
			if (this._isMultiLine || this._isSubtext)
			{
				this.DefinitionLabelContainer.IsVisible = false;
				this.DefinitionLabelContainer.ScaledSuggestedWidth = 0f;
				this.ValueLabel.WidthSizePolicy = SizePolicy.Fixed;
				this.ValueLabelContainer.WidthSizePolicy = SizePolicy.Fixed;
				this.ValueLabel.ScaledSuggestedWidth = base.ParentWidget.Size.X - (base.ScaledMarginLeft + base.ScaledMarginRight);
				this.ValueLabelContainer.ScaledSuggestedWidth = base.ParentWidget.Size.X - (base.ScaledMarginLeft + base.ScaledMarginRight);
			}
			else if (inBattleScope)
			{
				this.DefinitionLabelContainer.ScaledSuggestedWidth = battleScopeSize;
				this.DefinitionLabel.Brush = definitionRelationBrush;
				this.ValueLabelContainer.ScaledSuggestedWidth = battleScopeSize;
				this.ValueLabel.Brush = valueRelationBrush;
				this.ValueLabelContainer.HorizontalAlignment = HorizontalAlignment.Left;
				this.ValueLabel.HorizontalAlignment = HorizontalAlignment.Left;
				this.ValueLabel.Brush.TextHorizontalAlignment = TextHorizontalAlignment.Left;
			}
			else if (!this.IsTwoColumn)
			{
				if (!string.IsNullOrEmpty(this.DefinitionLabel.Text))
				{
					float num = ((this.DefinitionLabel.Size.X > this.ValueLabel.Size.X) ? this.DefinitionLabel.Size.X : this.ValueLabel.Size.X);
					this.DefinitionLabelContainer.ScaledSuggestedWidth = num;
					this.ValueLabelContainer.ScaledSuggestedWidth = num;
				}
				else
				{
					this.DefinitionLabelContainer.ScaledSuggestedWidth = 0f;
					this.DefinitionLabelContainer.IsVisible = false;
					this.ValueLabelContainer.ScaledSuggestedWidth = this.ValueLabel.Size.X;
				}
			}
			this._maxValueLabelSizeX = maxValueLabelSizeX;
			if (this.IsTwoColumn && !this._isMultiLine && (!this._isTitle || (this._isTitle && this.IsTwoColumn)))
			{
				this.ValueLabel.WidthSizePolicy = SizePolicy.Fixed;
				this.ValueLabel.ScaledSuggestedWidth = MathF.Max(53f * base._scaleToUse, this._maxValueLabelSizeX);
				this.ValueLabelContainer.WidthSizePolicy = SizePolicy.Fixed;
				this.ValueLabelContainer.ScaledSuggestedWidth = MathF.Max(53f * base._scaleToUse, this._maxValueLabelSizeX);
			}
			if (this.IsTwoColumn && !this._isMultiLine && this._isTitle)
			{
				this.DefinitionLabel.WidthSizePolicy = SizePolicy.Fixed;
				this.DefinitionLabel.ScaledSuggestedWidth = MathF.Max(53f * base._scaleToUse, maxDefinitionLabelSizeX);
				this.DefinitionLabelContainer.WidthSizePolicy = SizePolicy.Fixed;
				this.DefinitionLabelContainer.ScaledSuggestedWidth = MathF.Max(53f * base._scaleToUse, maxDefinitionLabelSizeX);
			}
		}

		// Token: 0x06000F96 RID: 3990 RVA: 0x0002B9D8 File Offset: 0x00029BD8
		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			if (this._firstFrame)
			{
				this.RefreshText();
				this._firstFrame = false;
			}
		}

		// Token: 0x06000F97 RID: 3991 RVA: 0x0002B9F8 File Offset: 0x00029BF8
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			this.ValueBackgroundSpriteWidget.HeightSizePolicy = SizePolicy.CoverChildren;
			if (this._currentSprite != null)
			{
				if (this.DefinitionLabelContainer.Size.X + this.ValueLabelContainer.Size.X > base.ParentWidget.Size.X)
				{
					this.ValueBackgroundSpriteWidget.WidthSizePolicy = SizePolicy.Fixed;
					this.ValueBackgroundSpriteWidget.ScaledSuggestedWidth = this.DefinitionLabelContainer.Size.X + this.ValueLabelContainer.Size.X;
					base.MarginLeft = 0f;
					base.MarginRight = 0f;
				}
				else
				{
					this.ValueBackgroundSpriteWidget.WidthSizePolicy = SizePolicy.Fixed;
					this.ValueBackgroundSpriteWidget.ScaledSuggestedWidth = base.ParentWidget.Size.X - (base.MarginLeft + base.MarginRight) * base._scaleToUse;
				}
				this.ValueBackgroundSpriteWidget.MinHeight = (float)this._currentSprite.Height;
				if (this._isTitle)
				{
					base.PositionXOffset = -base.MarginLeft;
					if (!this.IsTwoColumn)
					{
						this.ValueLabelContainer.MarginLeft = base.MarginLeft;
						this.ValueBackgroundSpriteWidget.ScaledSuggestedHeight = this.ValueLabel.Size.Y;
						return;
					}
					this.DefinitionLabelContainer.MarginLeft = base.MarginLeft;
					this.DefinitionLabel.HorizontalAlignment = HorizontalAlignment.Left;
					return;
				}
			}
			else
			{
				this.ValueBackgroundSpriteWidget.SuggestedWidth = 0f;
			}
		}

		// Token: 0x06000F98 RID: 3992 RVA: 0x0002BB74 File Offset: 0x00029D74
		private void RefreshText()
		{
			this.DefinitionLabel.Text = this._definitionText;
			this.ValueLabel.Text = this._valueText;
			this.DetermineTypeOfTooltipProperty();
			this.ValueLabelContainer.IsVisible = true;
			this.DefinitionLabelContainer.IsVisible = true;
			this.DefinitionLabel.IsVisible = true;
			this.ValueLabel.IsVisible = true;
			this._currentSprite = null;
			if (this._allBrushesInitialized)
			{
				if (this._isRelation)
				{
					this.DefinitionLabel.Text = "";
					this.ValueLabel.Text = "";
				}
				else if (this._isBattleMode)
				{
					this.DefinitionLabel.Text = "";
					this.ValueLabel.Text = "";
				}
				else if (this._isBattleModeOver)
				{
					this.DefinitionLabel.Text = "";
					this.ValueLabel.Text = "";
				}
				else if (this._isMultiLine)
				{
					this.DefinitionLabelContainer.IsVisible = false;
					this.ValueLabel.Text = this._valueText;
					this.ValueLabel.Brush = this.DescriptionTextBrush;
					this.ValueLabel.WidthSizePolicy = SizePolicy.Fixed;
					this.ValueLabelContainer.WidthSizePolicy = SizePolicy.Fixed;
					this.ValueLabel.SuggestedWidth = 0f;
					this.ValueLabelContainer.SuggestedWidth = 0f;
				}
				else if (this._isCost)
				{
					this.DefinitionLabel.Text = "";
					this.ValueLabel.Text = this._valueText;
					base.HorizontalAlignment = HorizontalAlignment.Center;
					this.ValueLabelContainer.WidthSizePolicy = SizePolicy.CoverChildren;
					this.ValueLabel.WidthSizePolicy = SizePolicy.CoverChildren;
				}
				else if (this._isRundownSeperator)
				{
					this.ValueLabel.IsVisible = false;
					this.DefinitionLabelContainer.IsVisible = false;
					this.ValueBackgroundSpriteWidget.IsVisible = true;
					this.ValueLabelContainer.WidthSizePolicy = SizePolicy.CoverChildren;
					this._currentSprite = this.RundownSeperatorSprite;
					this.ValueBackgroundSpriteWidget.HorizontalAlignment = HorizontalAlignment.Right;
					this.ValueBackgroundSpriteWidget.PositionXOffset = base.Right * base._inverseScaleToUse;
					this.ValueBackgroundSpriteWidget.Sprite = this._currentSprite;
					this.ValueBackgroundSpriteWidget.HeightSizePolicy = SizePolicy.Fixed;
					this.ValueBackgroundSpriteWidget.WidthSizePolicy = SizePolicy.Fixed;
				}
				else if (this._isDefaultSeperator)
				{
					this.ValueLabel.IsVisible = false;
					this.DefinitionLabelContainer.IsVisible = false;
					this.ValueBackgroundSpriteWidget.IsVisible = true;
					this.ValueLabelContainer.WidthSizePolicy = SizePolicy.CoverChildren;
					this._currentSprite = this.DefaultSeperatorSprite;
					this.ValueBackgroundSpriteWidget.HorizontalAlignment = HorizontalAlignment.Right;
					this.ValueBackgroundSpriteWidget.PositionXOffset = base.Right * base._inverseScaleToUse;
					this.ValueBackgroundSpriteWidget.Sprite = this._currentSprite;
					this.ValueBackgroundSpriteWidget.AlphaFactor = 0.5f;
					this.ValueBackgroundSpriteWidget.HeightSizePolicy = SizePolicy.Fixed;
					this.ValueBackgroundSpriteWidget.WidthSizePolicy = SizePolicy.Fixed;
				}
				else if (this._isTitle)
				{
					this.DefinitionLabel.Brush = this.TitleTextBrush;
					this.ValueLabel.Brush = this.TitleTextBrush;
					this.DefinitionLabel.HeightSizePolicy = SizePolicy.CoverChildren;
					this.ValueLabel.HeightSizePolicy = SizePolicy.CoverChildren;
					this.DefinitionLabelContainer.HeightSizePolicy = SizePolicy.CoverChildren;
					this.ValueLabelContainer.HeightSizePolicy = SizePolicy.CoverChildren;
					if (this.IsTwoColumn)
					{
						this.DefinitionLabelContainer.WidthSizePolicy = SizePolicy.CoverChildren;
						this.DefinitionLabelContainer.HorizontalAlignment = HorizontalAlignment.Left;
						this.DefinitionLabel.WidthSizePolicy = SizePolicy.CoverChildren;
						this.DefinitionLabel.HorizontalAlignment = HorizontalAlignment.Left;
						this.DefinitionLabel.Brush.TextHorizontalAlignment = TextHorizontalAlignment.Left;
						this.ValueLabel.WidthSizePolicy = SizePolicy.CoverChildren;
						this.ValueLabelContainer.WidthSizePolicy = SizePolicy.CoverChildren;
						this.ValueLabel.Text = " " + this.ValueLabel.Text;
					}
					else
					{
						this.ValueLabelContainer.WidthSizePolicy = SizePolicy.CoverChildren;
						this.ValueLabel.WidthSizePolicy = SizePolicy.CoverChildren;
					}
					this._currentSprite = this.TitleBackgroundSprite;
					this.ValueBackgroundSpriteWidget.HeightSizePolicy = SizePolicy.CoverChildren;
					this.ValueBackgroundSpriteWidget.Sprite = this._currentSprite;
					this.ValueBackgroundSpriteWidget.IsVisible = true;
				}
				else if (this.IsTwoColumn)
				{
					this.DefinitionLabelContainer.WidthSizePolicy = SizePolicy.CoverChildren;
					this.DefinitionLabel.WidthSizePolicy = SizePolicy.CoverChildren;
					this.DefinitionLabelContainer.HorizontalAlignment = HorizontalAlignment.Right;
					this.DefinitionLabel.HorizontalAlignment = HorizontalAlignment.Right;
					this.ValueLabel.WidthSizePolicy = SizePolicy.CoverChildren;
					this.ValueLabelContainer.WidthSizePolicy = SizePolicy.CoverChildren;
					base.HorizontalAlignment = HorizontalAlignment.Right;
					this.ValueLabel.Text = " " + this.ValueLabel.Text;
					this.DefinitionLabel.Brush = this.ValueNameTextBrush;
					this.ValueLabel.Brush = this.ValueTextBrush;
				}
				else if (this._isSubtext)
				{
					this.DefinitionLabelContainer.IsVisible = false;
					this.ValueLabel.Brush = this.SubtextBrush;
					this.ValueLabelContainer.WidthSizePolicy = SizePolicy.CoverChildren;
					this.ValueLabel.WidthSizePolicy = SizePolicy.CoverChildren;
				}
				else if (this._isEmptySpace)
				{
					this.DefinitionLabel.IsVisible = false;
					this.ValueLabel.Text = " ";
					this.ValueLabel.WidthSizePolicy = SizePolicy.CoverChildren;
					this.ValueLabelContainer.WidthSizePolicy = SizePolicy.CoverChildren;
					if (this.TextHeight > 0)
					{
						this.ValueLabel.Brush.FontSize = 30;
					}
					else if (this.TextHeight < 0)
					{
						this.ValueLabel.Brush.FontSize = 10;
					}
					else
					{
						this.ValueLabel.Brush.FontSize = 15;
					}
				}
				else if (this.DefinitionLabel.Text == string.Empty && this.ValueLabel.Text != string.Empty)
				{
					this.DefinitionLabelContainer.IsVisible = false;
					this.ValueLabelContainer.WidthSizePolicy = SizePolicy.CoverChildren;
					this.ValueLabel.WidthSizePolicy = SizePolicy.CoverChildren;
					this.ValueLabel.Brush = this.DescriptionTextBrush;
					this.ValueLabel.WidthSizePolicy = SizePolicy.CoverChildren;
					this.ValueLabelContainer.WidthSizePolicy = SizePolicy.CoverChildren;
				}
				else
				{
					this.ValueLabel.WidthSizePolicy = SizePolicy.CoverChildren;
					this.DefinitionLabel.WidthSizePolicy = SizePolicy.CoverChildren;
				}
				if (this._useCustomColor)
				{
					this.ValueLabel.Brush.FontColor = this.TextColor;
					this.ValueLabel.Brush.TextAlphaFactor = this.TextColor.Alpha;
				}
				if (this._isRundownResult)
				{
					this.ValueLabel.Brush.FontSize = (int)((float)this.ValueLabel.ReadOnlyBrush.FontSize * 1.3f);
					this.DefinitionLabel.Brush.FontSize = (int)((float)this.DefinitionLabel.ReadOnlyBrush.FontSize * 1.3f);
				}
			}
		}

		// Token: 0x06000F99 RID: 3993 RVA: 0x0002C248 File Offset: 0x0002A448
		private void DetermineTypeOfTooltipProperty()
		{
			this.PropertyModifierAsFlag = (TooltipPropertyWidget.TooltipPropertyFlags)this.PropertyModifier;
			this._isMultiLine = (this.PropertyModifierAsFlag & TooltipPropertyWidget.TooltipPropertyFlags.MultiLine) == TooltipPropertyWidget.TooltipPropertyFlags.MultiLine;
			this._isBattleMode = (this.PropertyModifierAsFlag & TooltipPropertyWidget.TooltipPropertyFlags.BattleMode) == TooltipPropertyWidget.TooltipPropertyFlags.BattleMode;
			this._isBattleModeOver = (this.PropertyModifierAsFlag & TooltipPropertyWidget.TooltipPropertyFlags.BattleModeOver) == TooltipPropertyWidget.TooltipPropertyFlags.BattleModeOver;
			this._isCost = (this.PropertyModifierAsFlag & TooltipPropertyWidget.TooltipPropertyFlags.Cost) == TooltipPropertyWidget.TooltipPropertyFlags.Cost;
			this._isTitle = (this.PropertyModifierAsFlag & TooltipPropertyWidget.TooltipPropertyFlags.Title) == TooltipPropertyWidget.TooltipPropertyFlags.Title;
			this._isRelation = (this.PropertyModifierAsFlag & TooltipPropertyWidget.TooltipPropertyFlags.WarFirstEnemy) == TooltipPropertyWidget.TooltipPropertyFlags.WarFirstEnemy || (this.PropertyModifierAsFlag & TooltipPropertyWidget.TooltipPropertyFlags.WarFirstAlly) == TooltipPropertyWidget.TooltipPropertyFlags.WarFirstAlly || (this.PropertyModifierAsFlag & TooltipPropertyWidget.TooltipPropertyFlags.WarFirstNeutral) == TooltipPropertyWidget.TooltipPropertyFlags.WarFirstNeutral || (this.PropertyModifierAsFlag & TooltipPropertyWidget.TooltipPropertyFlags.WarSecondEnemy) == TooltipPropertyWidget.TooltipPropertyFlags.WarSecondEnemy || (this.PropertyModifierAsFlag & TooltipPropertyWidget.TooltipPropertyFlags.WarSecondAlly) == TooltipPropertyWidget.TooltipPropertyFlags.WarSecondAlly || (this.PropertyModifierAsFlag & TooltipPropertyWidget.TooltipPropertyFlags.WarSecondNeutral) == TooltipPropertyWidget.TooltipPropertyFlags.WarSecondNeutral;
			this._isRundownSeperator = (this.PropertyModifierAsFlag & TooltipPropertyWidget.TooltipPropertyFlags.RundownSeperator) == TooltipPropertyWidget.TooltipPropertyFlags.RundownSeperator;
			this._isDefaultSeperator = (this.PropertyModifierAsFlag & TooltipPropertyWidget.TooltipPropertyFlags.DefaultSeperator) == TooltipPropertyWidget.TooltipPropertyFlags.DefaultSeperator;
			this._isRundownResult = (this.PropertyModifierAsFlag & TooltipPropertyWidget.TooltipPropertyFlags.RundownResult) == TooltipPropertyWidget.TooltipPropertyFlags.RundownResult;
			this.IsTwoColumn = false;
			this._isSubtext = false;
			this._isEmptySpace = false;
			if (!this._isMultiLine && !this._isBattleMode && !this._isBattleModeOver && !this._isCost && !this._isRundownSeperator && !this._isDefaultSeperator)
			{
				this._isEmptySpace = this.DefinitionText == string.Empty && this.ValueText == string.Empty;
				this.IsTwoColumn = this.DefinitionText != string.Empty && this.ValueText != string.Empty && this.TextHeight == 0;
				this._isSubtext = this.DefinitionText == string.Empty && this.ValueText != string.Empty && this.TextHeight < 0;
			}
		}

		// Token: 0x1700057B RID: 1403
		// (get) Token: 0x06000F9A RID: 3994 RVA: 0x0002C460 File Offset: 0x0002A660
		// (set) Token: 0x06000F9B RID: 3995 RVA: 0x0002C468 File Offset: 0x0002A668
		[Editor(false)]
		public string RundownSeperatorSpriteName
		{
			get
			{
				return this._rundownSeperatorSpriteName;
			}
			set
			{
				if (this._rundownSeperatorSpriteName != value)
				{
					this._rundownSeperatorSpriteName = value;
					base.OnPropertyChanged<string>(value, "RundownSeperatorSpriteName");
					this.RundownSeperatorSprite = base.Context.SpriteData.GetSprite(value);
				}
			}
		}

		// Token: 0x1700057C RID: 1404
		// (get) Token: 0x06000F9C RID: 3996 RVA: 0x0002C4A2 File Offset: 0x0002A6A2
		// (set) Token: 0x06000F9D RID: 3997 RVA: 0x0002C4AA File Offset: 0x0002A6AA
		[Editor(false)]
		public string DefaultSeperatorSpriteName
		{
			get
			{
				return this._defaultSeperatorSpriteName;
			}
			set
			{
				if (this._defaultSeperatorSpriteName != value)
				{
					this._defaultSeperatorSpriteName = value;
					base.OnPropertyChanged<string>(value, "DefaultSeperatorSpriteName");
					this.DefaultSeperatorSprite = base.Context.SpriteData.GetSprite(value);
				}
			}
		}

		// Token: 0x1700057D RID: 1405
		// (get) Token: 0x06000F9E RID: 3998 RVA: 0x0002C4E4 File Offset: 0x0002A6E4
		// (set) Token: 0x06000F9F RID: 3999 RVA: 0x0002C4EC File Offset: 0x0002A6EC
		[Editor(false)]
		public string TitleBackgroundSpriteName
		{
			get
			{
				return this._titleBackgroundSpriteName;
			}
			set
			{
				if (this._titleBackgroundSpriteName != value)
				{
					this._titleBackgroundSpriteName = value;
					base.OnPropertyChanged<string>(value, "TitleBackgroundSpriteName");
					this.TitleBackgroundSprite = base.Context.SpriteData.GetSprite(value);
				}
			}
		}

		// Token: 0x1700057E RID: 1406
		// (get) Token: 0x06000FA0 RID: 4000 RVA: 0x0002C526 File Offset: 0x0002A726
		// (set) Token: 0x06000FA1 RID: 4001 RVA: 0x0002C52E File Offset: 0x0002A72E
		[Editor(false)]
		public Brush ValueNameTextBrush
		{
			get
			{
				return this._valueNameTextBrush;
			}
			set
			{
				if (this._valueNameTextBrush != value)
				{
					this._valueNameTextBrush = value;
					base.OnPropertyChanged<Brush>(value, "ValueNameTextBrush");
				}
			}
		}

		// Token: 0x1700057F RID: 1407
		// (get) Token: 0x06000FA2 RID: 4002 RVA: 0x0002C54C File Offset: 0x0002A74C
		// (set) Token: 0x06000FA3 RID: 4003 RVA: 0x0002C554 File Offset: 0x0002A754
		[Editor(false)]
		public Brush TitleTextBrush
		{
			get
			{
				return this._titleTextBrush;
			}
			set
			{
				if (this._titleTextBrush != value)
				{
					this._titleTextBrush = value;
					base.OnPropertyChanged<Brush>(value, "TitleTextBrush");
				}
			}
		}

		// Token: 0x17000580 RID: 1408
		// (get) Token: 0x06000FA4 RID: 4004 RVA: 0x0002C572 File Offset: 0x0002A772
		// (set) Token: 0x06000FA5 RID: 4005 RVA: 0x0002C57A File Offset: 0x0002A77A
		[Editor(false)]
		public Brush SubtextBrush
		{
			get
			{
				return this._subtextBrush;
			}
			set
			{
				if (this._subtextBrush != value)
				{
					this._subtextBrush = value;
					base.OnPropertyChanged<Brush>(value, "SubtextBrush");
				}
			}
		}

		// Token: 0x17000581 RID: 1409
		// (get) Token: 0x06000FA6 RID: 4006 RVA: 0x0002C598 File Offset: 0x0002A798
		// (set) Token: 0x06000FA7 RID: 4007 RVA: 0x0002C5A0 File Offset: 0x0002A7A0
		[Editor(false)]
		public Brush ValueTextBrush
		{
			get
			{
				return this._valueTextBrush;
			}
			set
			{
				if (this._valueTextBrush != value)
				{
					this._valueTextBrush = value;
					base.OnPropertyChanged<Brush>(value, "ValueTextBrush");
				}
			}
		}

		// Token: 0x17000582 RID: 1410
		// (get) Token: 0x06000FA8 RID: 4008 RVA: 0x0002C5BE File Offset: 0x0002A7BE
		// (set) Token: 0x06000FA9 RID: 4009 RVA: 0x0002C5C6 File Offset: 0x0002A7C6
		[Editor(false)]
		public Brush DescriptionTextBrush
		{
			get
			{
				return this._descriptionTextBrush;
			}
			set
			{
				if (this._descriptionTextBrush != value)
				{
					this._descriptionTextBrush = value;
					base.OnPropertyChanged<Brush>(value, "DescriptionTextBrush");
				}
			}
		}

		// Token: 0x17000583 RID: 1411
		// (get) Token: 0x06000FAA RID: 4010 RVA: 0x0002C5E4 File Offset: 0x0002A7E4
		// (set) Token: 0x06000FAB RID: 4011 RVA: 0x0002C5EC File Offset: 0x0002A7EC
		[Editor(false)]
		public bool ModifyDefinitionColor
		{
			get
			{
				return this._modifyDefinitionColor;
			}
			set
			{
				if (this._modifyDefinitionColor != value)
				{
					this._modifyDefinitionColor = value;
					base.OnPropertyChanged(value, "ModifyDefinitionColor");
				}
			}
		}

		// Token: 0x17000584 RID: 1412
		// (get) Token: 0x06000FAC RID: 4012 RVA: 0x0002C60A File Offset: 0x0002A80A
		// (set) Token: 0x06000FAD RID: 4013 RVA: 0x0002C612 File Offset: 0x0002A812
		[Editor(false)]
		public RichTextWidget DefinitionLabel
		{
			get
			{
				return this._definitionLabel;
			}
			set
			{
				if (this._definitionLabel != value)
				{
					this._definitionLabel = value;
					base.OnPropertyChanged<RichTextWidget>(value, "DefinitionLabel");
				}
			}
		}

		// Token: 0x17000585 RID: 1413
		// (get) Token: 0x06000FAE RID: 4014 RVA: 0x0002C630 File Offset: 0x0002A830
		// (set) Token: 0x06000FAF RID: 4015 RVA: 0x0002C638 File Offset: 0x0002A838
		[Editor(false)]
		public RichTextWidget ValueLabel
		{
			get
			{
				return this._valueLabel;
			}
			set
			{
				if (this._valueLabel != value)
				{
					this._valueLabel = value;
					base.OnPropertyChanged<RichTextWidget>(value, "ValueLabel");
				}
			}
		}

		// Token: 0x17000586 RID: 1414
		// (get) Token: 0x06000FB0 RID: 4016 RVA: 0x0002C656 File Offset: 0x0002A856
		// (set) Token: 0x06000FB1 RID: 4017 RVA: 0x0002C65E File Offset: 0x0002A85E
		[Editor(false)]
		public ListPanel ValueBackgroundSpriteWidget
		{
			get
			{
				return this._valueBackgroundSpriteWidget;
			}
			set
			{
				if (this._valueBackgroundSpriteWidget != value)
				{
					this._valueBackgroundSpriteWidget = value;
					base.OnPropertyChanged<ListPanel>(value, "ValueBackgroundSpriteWidget");
				}
			}
		}

		// Token: 0x17000587 RID: 1415
		// (get) Token: 0x06000FB2 RID: 4018 RVA: 0x0002C67C File Offset: 0x0002A87C
		// (set) Token: 0x06000FB3 RID: 4019 RVA: 0x0002C684 File Offset: 0x0002A884
		[Editor(false)]
		public Widget DefinitionLabelContainer
		{
			get
			{
				return this._definitionLabelContainer;
			}
			set
			{
				if (this._definitionLabelContainer != value)
				{
					this._definitionLabelContainer = value;
					base.OnPropertyChanged<Widget>(value, "DefinitionLabelContainer");
				}
			}
		}

		// Token: 0x17000588 RID: 1416
		// (get) Token: 0x06000FB4 RID: 4020 RVA: 0x0002C6A2 File Offset: 0x0002A8A2
		// (set) Token: 0x06000FB5 RID: 4021 RVA: 0x0002C6AA File Offset: 0x0002A8AA
		[Editor(false)]
		public Widget ValueLabelContainer
		{
			get
			{
				return this._valueLabelContainer;
			}
			set
			{
				if (this._valueLabelContainer != value)
				{
					this._valueLabelContainer = value;
					base.OnPropertyChanged<Widget>(value, "ValueLabelContainer");
				}
			}
		}

		// Token: 0x17000589 RID: 1417
		// (get) Token: 0x06000FB6 RID: 4022 RVA: 0x0002C6C8 File Offset: 0x0002A8C8
		// (set) Token: 0x06000FB7 RID: 4023 RVA: 0x0002C6D0 File Offset: 0x0002A8D0
		[Editor(false)]
		public Color TextColor
		{
			get
			{
				return this._textColor;
			}
			set
			{
				if (this._textColor != value)
				{
					this._textColor = value;
					base.OnPropertyChanged(value, "TextColor");
					this._useCustomColor = true;
				}
			}
		}

		// Token: 0x1700058A RID: 1418
		// (get) Token: 0x06000FB8 RID: 4024 RVA: 0x0002C6FA File Offset: 0x0002A8FA
		// (set) Token: 0x06000FB9 RID: 4025 RVA: 0x0002C702 File Offset: 0x0002A902
		[Editor(false)]
		public int TextHeight
		{
			get
			{
				return this._textHeight;
			}
			set
			{
				if (this._textHeight != value)
				{
					this._textHeight = value;
					base.OnPropertyChanged(value, "TextHeight");
				}
			}
		}

		// Token: 0x1700058B RID: 1419
		// (get) Token: 0x06000FBA RID: 4026 RVA: 0x0002C720 File Offset: 0x0002A920
		// (set) Token: 0x06000FBB RID: 4027 RVA: 0x0002C728 File Offset: 0x0002A928
		[Editor(false)]
		public string DefinitionText
		{
			get
			{
				return this._definitionText;
			}
			set
			{
				if (this._definitionText != value)
				{
					this._definitionText = value;
					base.OnPropertyChanged<string>(value, "DefinitionText");
					this._firstFrame = true;
				}
			}
		}

		// Token: 0x1700058C RID: 1420
		// (get) Token: 0x06000FBC RID: 4028 RVA: 0x0002C752 File Offset: 0x0002A952
		// (set) Token: 0x06000FBD RID: 4029 RVA: 0x0002C75A File Offset: 0x0002A95A
		[Editor(false)]
		public string ValueText
		{
			get
			{
				return this._valueText;
			}
			set
			{
				if (this._valueText != value)
				{
					this._valueText = value;
					base.OnPropertyChanged<string>(value, "ValueText");
					this._firstFrame = true;
				}
			}
		}

		// Token: 0x1700058D RID: 1421
		// (get) Token: 0x06000FBE RID: 4030 RVA: 0x0002C784 File Offset: 0x0002A984
		// (set) Token: 0x06000FBF RID: 4031 RVA: 0x0002C78C File Offset: 0x0002A98C
		[Editor(false)]
		public int PropertyModifier
		{
			get
			{
				return this._propertyModifier;
			}
			set
			{
				if (this._propertyModifier != value)
				{
					this._propertyModifier = value;
					base.OnPropertyChanged(value, "PropertyModifier");
				}
			}
		}

		// Token: 0x0400071A RID: 1818
		private const int HeaderSize = 30;

		// Token: 0x0400071B RID: 1819
		private const int DefaultSize = 15;

		// Token: 0x0400071C RID: 1820
		private const int SubTextSize = 10;

		// Token: 0x0400071E RID: 1822
		private bool _isMultiLine;

		// Token: 0x0400071F RID: 1823
		private bool _isBattleMode;

		// Token: 0x04000720 RID: 1824
		private bool _isBattleModeOver;

		// Token: 0x04000721 RID: 1825
		private bool _isCost;

		// Token: 0x04000722 RID: 1826
		private bool _isRundownSeperator;

		// Token: 0x04000723 RID: 1827
		private bool _isDefaultSeperator;

		// Token: 0x04000724 RID: 1828
		private bool _isRundownResult;

		// Token: 0x04000725 RID: 1829
		private bool _isTitle;

		// Token: 0x04000726 RID: 1830
		private bool _isSubtext;

		// Token: 0x04000727 RID: 1831
		private bool _isEmptySpace;

		// Token: 0x04000728 RID: 1832
		private bool _isRelation;

		// Token: 0x0400072A RID: 1834
		private bool _useCustomColor;

		// Token: 0x0400072B RID: 1835
		private Sprite RundownSeperatorSprite;

		// Token: 0x0400072C RID: 1836
		private Sprite DefaultSeperatorSprite;

		// Token: 0x0400072D RID: 1837
		private Sprite TitleBackgroundSprite;

		// Token: 0x0400072E RID: 1838
		private Sprite _currentSprite;

		// Token: 0x0400072F RID: 1839
		private float _maxValueLabelSizeX;

		// Token: 0x04000730 RID: 1840
		private bool _firstFrame = true;

		// Token: 0x04000731 RID: 1841
		private bool _modifyDefinitionColor = true;

		// Token: 0x04000732 RID: 1842
		private Color _textColor;

		// Token: 0x04000733 RID: 1843
		private RichTextWidget _definitionLabel;

		// Token: 0x04000734 RID: 1844
		private RichTextWidget _valueLabel;

		// Token: 0x04000735 RID: 1845
		private Widget _definitionLabelContainer;

		// Token: 0x04000736 RID: 1846
		private Widget _valueLabelContainer;

		// Token: 0x04000737 RID: 1847
		private ListPanel _valueBackgroundSpriteWidget;

		// Token: 0x04000738 RID: 1848
		private int _textHeight;

		// Token: 0x04000739 RID: 1849
		private Brush _titleTextBrush;

		// Token: 0x0400073A RID: 1850
		private Brush _subtextBrush;

		// Token: 0x0400073B RID: 1851
		private Brush _valueTextBrush;

		// Token: 0x0400073C RID: 1852
		private Brush _descriptionTextBrush;

		// Token: 0x0400073D RID: 1853
		private Brush _valueNameTextBrush;

		// Token: 0x0400073E RID: 1854
		private string _rundownSeperatorSpriteName;

		// Token: 0x0400073F RID: 1855
		private string _defaultSeperatorSpriteName;

		// Token: 0x04000740 RID: 1856
		private string _titleBackgroundSpriteName;

		// Token: 0x04000741 RID: 1857
		private string _definitionText;

		// Token: 0x04000742 RID: 1858
		private string _valueText;

		// Token: 0x04000743 RID: 1859
		private int _propertyModifier;

		// Token: 0x0200019C RID: 412
		[Flags]
		public enum TooltipPropertyFlags
		{
			// Token: 0x04000920 RID: 2336
			None = 0,
			// Token: 0x04000921 RID: 2337
			MultiLine = 1,
			// Token: 0x04000922 RID: 2338
			BattleMode = 2,
			// Token: 0x04000923 RID: 2339
			BattleModeOver = 4,
			// Token: 0x04000924 RID: 2340
			WarFirstEnemy = 8,
			// Token: 0x04000925 RID: 2341
			WarFirstAlly = 16,
			// Token: 0x04000926 RID: 2342
			WarFirstNeutral = 32,
			// Token: 0x04000927 RID: 2343
			WarSecondEnemy = 64,
			// Token: 0x04000928 RID: 2344
			WarSecondAlly = 128,
			// Token: 0x04000929 RID: 2345
			WarSecondNeutral = 256,
			// Token: 0x0400092A RID: 2346
			RundownSeperator = 512,
			// Token: 0x0400092B RID: 2347
			DefaultSeperator = 1024,
			// Token: 0x0400092C RID: 2348
			Cost = 2048,
			// Token: 0x0400092D RID: 2349
			Title = 4096,
			// Token: 0x0400092E RID: 2350
			RundownResult = 8192
		}
	}
}
