using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.CharacterDeveloper
{
	public class PerkSelectionBarWidget : Widget
	{
		public PerkSelectionBarWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this.PerksList != null)
			{
				for (int i = 0; i < this.PerksList.ChildCount; i++)
				{
					PerkItemButtonWidget perkItemButtonWidget = this.PerksList.GetChild(i) as PerkItemButtonWidget;
					if (this._perkWidgetWidth != perkItemButtonWidget.Size.X)
					{
						this._perkWidgetWidth = perkItemButtonWidget.Size.X;
					}
					perkItemButtonWidget.PositionXOffset = this.GetXPositionOfLevelOnBar((float)perkItemButtonWidget.Level) - this._perkWidgetWidth / 2f * base._inverseScaleToUse;
					if (perkItemButtonWidget.AlternativeType == 0)
					{
						perkItemButtonWidget.PositionYOffset = 45f;
					}
					else if (perkItemButtonWidget.AlternativeType == 1)
					{
						perkItemButtonWidget.PositionYOffset = 5f;
					}
					else if (perkItemButtonWidget.AlternativeType == 2)
					{
						perkItemButtonWidget.PositionYOffset = (float)((int)Mathf.Round(perkItemButtonWidget.Size.Y * base._inverseScaleToUse));
					}
				}
			}
			if (this.PercentageIndicatorWidget != null)
			{
				float xpositionOfLevelOnBar = this.GetXPositionOfLevelOnBar((float)this.Level);
				float num = xpositionOfLevelOnBar - this.PercentageIndicatorWidget.Size.X / 2f * base._inverseScaleToUse;
				this.PercentageIndicatorWidget.PositionXOffset = num;
				if (this.FullLearningRateClip != null)
				{
					float num2 = this.GetXPositionOfLevelOnBar((float)this.FullLearningRateLevel) - xpositionOfLevelOnBar;
					this.FullLearningRateClip.SuggestedWidth = ((num2 >= 0f) ? num2 : 0f);
					this.FullLearningRateClip.PositionXOffset = this.PercentageIndicatorWidget.PositionXOffset + this.PercentageIndicatorWidget.Size.X / 2f * base._inverseScaleToUse;
					this.FullLearningRateClipInnerContent.PositionXOffset = -this.FullLearningRateClip.PositionXOffset;
				}
				this.ProgressClip.SuggestedWidth = num + this.PercentageIndicatorWidget.Size.X / 2f * base._inverseScaleToUse;
			}
			using (List<Widget>.Enumerator enumerator = this.SeperatorContainer.Children.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					CharacterDeveloperSkillVerticalSeperatorWidget characterDeveloperSkillVerticalSeperatorWidget;
					if ((characterDeveloperSkillVerticalSeperatorWidget = enumerator.Current as CharacterDeveloperSkillVerticalSeperatorWidget) != null)
					{
						characterDeveloperSkillVerticalSeperatorWidget.PositionXOffset = this.GetXPositionOfLevelOnBar((float)characterDeveloperSkillVerticalSeperatorWidget.SkillValue);
					}
				}
			}
		}

		private float GetXPositionOfLevelOnBar(float level)
		{
			return Mathf.Clamp(level / ((float)this.MaxLevel + 25f) * base.Size.X * base._inverseScaleToUse, 0f, base.Size.X * base._inverseScaleToUse);
		}

		public Widget ProgressClip
		{
			get
			{
				return this._progressClip;
			}
			set
			{
				if (this._progressClip != value)
				{
					this._progressClip = value;
					base.OnPropertyChanged<Widget>(value, "ProgressClip");
				}
			}
		}

		public Widget PercentageIndicatorWidget
		{
			get
			{
				return this._percentageIndicatorWidget;
			}
			set
			{
				if (this._percentageIndicatorWidget != value)
				{
					this._percentageIndicatorWidget = value;
					base.OnPropertyChanged<Widget>(value, "PercentageIndicatorWidget");
				}
			}
		}

		public Widget FullLearningRateClip
		{
			get
			{
				return this._fullLearningRateClip;
			}
			set
			{
				if (this._fullLearningRateClip != value)
				{
					this._fullLearningRateClip = value;
					base.OnPropertyChanged<Widget>(value, "FullLearningRateClip");
				}
			}
		}

		public Widget SeperatorContainer
		{
			get
			{
				return this._seperatorContainer;
			}
			set
			{
				if (this._seperatorContainer != value)
				{
					this._seperatorContainer = value;
					base.OnPropertyChanged<Widget>(value, "SeperatorContainer");
				}
			}
		}

		public Widget FullLearningRateClipInnerContent
		{
			get
			{
				return this._fullLearningRateClipInnerContent;
			}
			set
			{
				if (this._fullLearningRateClipInnerContent != value)
				{
					this._fullLearningRateClipInnerContent = value;
					base.OnPropertyChanged<Widget>(value, "FullLearningRateClipInnerContent");
				}
			}
		}

		public Widget PerksList
		{
			get
			{
				return this._perksList;
			}
			set
			{
				if (this._perksList != value)
				{
					this._perksList = value;
					base.OnPropertyChanged<Widget>(value, "PerksList");
				}
			}
		}

		public TextWidget PercentageIndicatorTextWidget
		{
			get
			{
				return this._percentageIndicatorTextWidget;
			}
			set
			{
				if (this._percentageIndicatorTextWidget != value)
				{
					this._percentageIndicatorTextWidget = value;
					base.OnPropertyChanged<TextWidget>(value, "PercentageIndicatorTextWidget");
				}
			}
		}

		public int MaxLevel
		{
			get
			{
				return this._maxLevel;
			}
			set
			{
				if (this._maxLevel != value)
				{
					this._maxLevel = value;
					base.OnPropertyChanged(value, "MaxLevel");
				}
			}
		}

		public int FullLearningRateLevel
		{
			get
			{
				return this._fullLearningRateLevel;
			}
			set
			{
				if (this._fullLearningRateLevel != value)
				{
					this._fullLearningRateLevel = value;
					base.OnPropertyChanged(value, "FullLearningRateLevel");
				}
			}
		}

		public int Level
		{
			get
			{
				return this._level;
			}
			set
			{
				if (this._level != value)
				{
					this._level = value;
					base.OnPropertyChanged(value, "Level");
				}
			}
		}

		private float _perkWidgetWidth = -1f;

		private Widget _perksList;

		private Widget _progressClip;

		private Widget _fullLearningRateClip;

		private Widget _fullLearningRateClipInnerContent;

		private Widget _percentageIndicatorWidget;

		private Widget _seperatorContainer;

		private TextWidget _percentageIndicatorTextWidget;

		private int _maxLevel;

		private int _fullLearningRateLevel;

		private int _level = -1;
	}
}
