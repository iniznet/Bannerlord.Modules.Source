using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Information
{
	public class TooltipPropertyWidget : Widget
	{
		public bool IsTwoColumn { get; private set; }

		public TooltipPropertyWidget.TooltipPropertyFlags PropertyModifierAsFlag { get; private set; }

		private bool _allBrushesInitialized
		{
			get
			{
				return this.SubtextBrush != null && this.ValueTextBrush != null && this.DescriptionTextBrush != null && this.ValueNameTextBrush != null && this.RundownSeperatorSprite != null && this.DefaultSeperatorSprite != null && this.TitleBackgroundSprite != null;
			}
		}

		public bool IsMultiLine
		{
			get
			{
				return this._isMultiLine;
			}
		}

		public bool IsBattleMode
		{
			get
			{
				return this._isBattleMode;
			}
		}

		public bool IsBattleModeOver
		{
			get
			{
				return this._isBattleModeOver;
			}
		}

		public bool IsCost
		{
			get
			{
				return this._isCost;
			}
		}

		public bool IsRelation
		{
			get
			{
				return this._isRelation;
			}
		}

		public TooltipPropertyWidget(UIContext context)
			: base(context)
		{
			this._isMultiLine = false;
			this._isBattleMode = false;
			this._isBattleModeOver = false;
		}

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

		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			if (this._firstFrame)
			{
				this.RefreshText();
				this._firstFrame = false;
			}
		}

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

		private const int HeaderSize = 30;

		private const int DefaultSize = 15;

		private const int SubTextSize = 10;

		private bool _isMultiLine;

		private bool _isBattleMode;

		private bool _isBattleModeOver;

		private bool _isCost;

		private bool _isRundownSeperator;

		private bool _isDefaultSeperator;

		private bool _isRundownResult;

		private bool _isTitle;

		private bool _isSubtext;

		private bool _isEmptySpace;

		private bool _isRelation;

		private bool _useCustomColor;

		private Sprite RundownSeperatorSprite;

		private Sprite DefaultSeperatorSprite;

		private Sprite TitleBackgroundSprite;

		private Sprite _currentSprite;

		private float _maxValueLabelSizeX;

		private bool _firstFrame = true;

		private bool _modifyDefinitionColor = true;

		private Color _textColor;

		private RichTextWidget _definitionLabel;

		private RichTextWidget _valueLabel;

		private Widget _definitionLabelContainer;

		private Widget _valueLabelContainer;

		private ListPanel _valueBackgroundSpriteWidget;

		private int _textHeight;

		private Brush _titleTextBrush;

		private Brush _subtextBrush;

		private Brush _valueTextBrush;

		private Brush _descriptionTextBrush;

		private Brush _valueNameTextBrush;

		private string _rundownSeperatorSpriteName;

		private string _defaultSeperatorSpriteName;

		private string _titleBackgroundSpriteName;

		private string _definitionText;

		private string _valueText;

		private int _propertyModifier;

		[Flags]
		public enum TooltipPropertyFlags
		{
			None = 0,
			MultiLine = 1,
			BattleMode = 2,
			BattleModeOver = 4,
			WarFirstEnemy = 8,
			WarFirstAlly = 16,
			WarFirstNeutral = 32,
			WarSecondEnemy = 64,
			WarSecondAlly = 128,
			WarSecondNeutral = 256,
			RundownSeperator = 512,
			DefaultSeperator = 1024,
			Cost = 2048,
			Title = 4096,
			RundownResult = 8192
		}
	}
}
