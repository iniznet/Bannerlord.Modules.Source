using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby.Armory
{
	public class MultiplayerArmoryCosmeticsSectionWidget : Widget
	{
		public MultiplayerArmoryCosmeticsSectionWidget(UIContext context)
			: base(context)
		{
		}

		private void AnimateTauntAssignmentStates(float dt)
		{
			this._tauntAssignmentStateTimer += dt;
			float num;
			if (this._tauntAssignmentStateTimer < this.TauntAssignmentStateAnimationDuration)
			{
				num = this._tauntAssignmentStateTimer / this.TauntAssignmentStateAnimationDuration;
				base.EventManager.AddLateUpdateAction(this, new Action<float>(this.AnimateTauntAssignmentStates), 1);
			}
			else
			{
				num = 1f;
			}
			float num2 = (this.IsTauntAssignmentActive ? 1f : this.TauntAssignmentStateAlpha);
			float num3 = (this.IsTauntAssignmentActive ? this.TauntAssignmentStateAlpha : 1f);
			float num4 = MathF.Lerp(num2, num3, num, 1E-05f);
			this.SetWidgetAlpha(this.TopSectionParent, num4);
			this.SetWidgetAlpha(this.BottomSectionParent, num4);
			this.SetWidgetAlpha(this.SortControlsParent, num4);
			this.SetWidgetAlpha(this.CategorySeparatorWidget, num4);
		}

		private void SetWidgetAlpha(Widget widget, float alpha)
		{
			if (widget != null)
			{
				widget.IsVisible = alpha != 0f;
				widget.SetGlobalAlphaRecursively(alpha);
			}
		}

		public bool IsTauntAssignmentActive
		{
			get
			{
				return this._isTauntAssignmentActive;
			}
			set
			{
				if (value != this._isTauntAssignmentActive)
				{
					this._isTauntAssignmentActive = value;
					base.OnPropertyChanged(value, "IsTauntAssignmentActive");
					this._tauntAssignmentStateTimer = 0f;
					base.EventManager.AddLateUpdateAction(this, new Action<float>(this.AnimateTauntAssignmentStates), 1);
				}
			}
		}

		public float TauntAssignmentStateAnimationDuration
		{
			get
			{
				return this._tauntAssignmentStateAnimationDuration;
			}
			set
			{
				if (value != this._tauntAssignmentStateAnimationDuration)
				{
					this._tauntAssignmentStateAnimationDuration = value;
					base.OnPropertyChanged(value, "TauntAssignmentStateAnimationDuration");
				}
			}
		}

		public float TauntAssignmentStateAlpha
		{
			get
			{
				return this._tauntAssignmentStateAlpha;
			}
			set
			{
				if (value != this._tauntAssignmentStateAlpha)
				{
					this._tauntAssignmentStateAlpha = value;
					base.OnPropertyChanged(value, "TauntAssignmentStateAlpha");
				}
			}
		}

		public Widget TopSectionParent
		{
			get
			{
				return this._topSectionParent;
			}
			set
			{
				if (value != this._topSectionParent)
				{
					this._topSectionParent = value;
					base.OnPropertyChanged<Widget>(value, "TopSectionParent");
				}
			}
		}

		public Widget BottomSectionParent
		{
			get
			{
				return this._bottomSectionParent;
			}
			set
			{
				if (value != this._bottomSectionParent)
				{
					this._bottomSectionParent = value;
					base.OnPropertyChanged<Widget>(value, "BottomSectionParent");
				}
			}
		}

		public Widget SortControlsParent
		{
			get
			{
				return this._sortControlsParent;
			}
			set
			{
				if (value != this._sortControlsParent)
				{
					this._sortControlsParent = value;
					base.OnPropertyChanged<Widget>(value, "SortControlsParent");
				}
			}
		}

		public Widget CategorySeparatorWidget
		{
			get
			{
				return this._categorySeparatorWidget;
			}
			set
			{
				if (value != this._categorySeparatorWidget)
				{
					this._categorySeparatorWidget = value;
					base.OnPropertyChanged<Widget>(value, "CategorySeparatorWidget");
				}
			}
		}

		private float _tauntAssignmentStateTimer;

		private bool _isTauntAssignmentActive;

		private float _tauntAssignmentStateAnimationDuration;

		private float _tauntAssignmentStateAlpha;

		private Widget _topSectionParent;

		private Widget _bottomSectionParent;

		private Widget _sortControlsParent;

		private Widget _categorySeparatorWidget;
	}
}
