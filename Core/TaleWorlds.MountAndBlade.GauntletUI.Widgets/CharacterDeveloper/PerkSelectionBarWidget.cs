using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.CharacterDeveloper
{
	// Token: 0x02000162 RID: 354
	public class PerkSelectionBarWidget : Widget
	{
		// Token: 0x06001233 RID: 4659 RVA: 0x000325F6 File Offset: 0x000307F6
		public PerkSelectionBarWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06001234 RID: 4660 RVA: 0x00032614 File Offset: 0x00030814
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

		// Token: 0x06001235 RID: 4661 RVA: 0x00032854 File Offset: 0x00030A54
		private float GetXPositionOfLevelOnBar(float level)
		{
			return Mathf.Clamp(level / ((float)this.MaxLevel + 25f) * base.Size.X * base._inverseScaleToUse, 0f, base.Size.X * base._inverseScaleToUse);
		}

		// Token: 0x1700066A RID: 1642
		// (get) Token: 0x06001236 RID: 4662 RVA: 0x00032894 File Offset: 0x00030A94
		// (set) Token: 0x06001237 RID: 4663 RVA: 0x0003289C File Offset: 0x00030A9C
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

		// Token: 0x1700066B RID: 1643
		// (get) Token: 0x06001238 RID: 4664 RVA: 0x000328BA File Offset: 0x00030ABA
		// (set) Token: 0x06001239 RID: 4665 RVA: 0x000328C2 File Offset: 0x00030AC2
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

		// Token: 0x1700066C RID: 1644
		// (get) Token: 0x0600123A RID: 4666 RVA: 0x000328E0 File Offset: 0x00030AE0
		// (set) Token: 0x0600123B RID: 4667 RVA: 0x000328E8 File Offset: 0x00030AE8
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

		// Token: 0x1700066D RID: 1645
		// (get) Token: 0x0600123C RID: 4668 RVA: 0x00032906 File Offset: 0x00030B06
		// (set) Token: 0x0600123D RID: 4669 RVA: 0x0003290E File Offset: 0x00030B0E
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

		// Token: 0x1700066E RID: 1646
		// (get) Token: 0x0600123E RID: 4670 RVA: 0x0003292C File Offset: 0x00030B2C
		// (set) Token: 0x0600123F RID: 4671 RVA: 0x00032934 File Offset: 0x00030B34
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

		// Token: 0x1700066F RID: 1647
		// (get) Token: 0x06001240 RID: 4672 RVA: 0x00032952 File Offset: 0x00030B52
		// (set) Token: 0x06001241 RID: 4673 RVA: 0x0003295A File Offset: 0x00030B5A
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

		// Token: 0x17000670 RID: 1648
		// (get) Token: 0x06001242 RID: 4674 RVA: 0x00032978 File Offset: 0x00030B78
		// (set) Token: 0x06001243 RID: 4675 RVA: 0x00032980 File Offset: 0x00030B80
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

		// Token: 0x17000671 RID: 1649
		// (get) Token: 0x06001244 RID: 4676 RVA: 0x0003299E File Offset: 0x00030B9E
		// (set) Token: 0x06001245 RID: 4677 RVA: 0x000329A6 File Offset: 0x00030BA6
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

		// Token: 0x17000672 RID: 1650
		// (get) Token: 0x06001246 RID: 4678 RVA: 0x000329C4 File Offset: 0x00030BC4
		// (set) Token: 0x06001247 RID: 4679 RVA: 0x000329CC File Offset: 0x00030BCC
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

		// Token: 0x17000673 RID: 1651
		// (get) Token: 0x06001248 RID: 4680 RVA: 0x000329EA File Offset: 0x00030BEA
		// (set) Token: 0x06001249 RID: 4681 RVA: 0x000329F2 File Offset: 0x00030BF2
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

		// Token: 0x04000856 RID: 2134
		private float _perkWidgetWidth = -1f;

		// Token: 0x04000857 RID: 2135
		private Widget _perksList;

		// Token: 0x04000858 RID: 2136
		private Widget _progressClip;

		// Token: 0x04000859 RID: 2137
		private Widget _fullLearningRateClip;

		// Token: 0x0400085A RID: 2138
		private Widget _fullLearningRateClipInnerContent;

		// Token: 0x0400085B RID: 2139
		private Widget _percentageIndicatorWidget;

		// Token: 0x0400085C RID: 2140
		private Widget _seperatorContainer;

		// Token: 0x0400085D RID: 2141
		private TextWidget _percentageIndicatorTextWidget;

		// Token: 0x0400085E RID: 2142
		private int _maxLevel;

		// Token: 0x0400085F RID: 2143
		private int _fullLearningRateLevel;

		// Token: 0x04000860 RID: 2144
		private int _level = -1;
	}
}
