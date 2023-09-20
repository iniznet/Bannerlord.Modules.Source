using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission
{
	public class FormationFocusedMarkerWidget : BrushWidget
	{
		public int NormalSize { get; set; } = 55;

		public int FocusedSize { get; set; } = 60;

		public FormationFocusedMarkerWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			this.UpdateVisibility();
			if (base.IsVisible)
			{
				this.UpdateSize();
			}
		}

		private void UpdateVisibility()
		{
			base.IsVisible = this.IsTargetingAFormation || (this.IsFormationTargetRelevant && this.IsCenterOfFocus);
		}

		private void UpdateSize()
		{
			float num4;
			if (this.IsCenterOfFocus)
			{
				float num = (float)(this.IsTargetingAFormation ? (this.FocusedSize + 3) : this.FocusedSize);
				float num2 = MathF.Sin(base.EventManager.Time * 5f);
				num2 = (num2 + 1f) / 2f;
				float num3 = (num - (float)this.NormalSize) * num2;
				num4 = (float)this.NormalSize + num3;
			}
			else
			{
				num4 = (float)this.NormalSize;
			}
			base.ScaledSuggestedHeight = num4 * base._scaleToUse;
			base.ScaledSuggestedWidth = num4 * base._scaleToUse;
		}

		private void UpdateState()
		{
			this.SetState(this.IsTargetingAFormation ? "Targeting" : "Default");
		}

		public bool IsCenterOfFocus
		{
			get
			{
				return this._isCenterOfFocus;
			}
			set
			{
				if (this._isCenterOfFocus != value)
				{
					this._isCenterOfFocus = value;
					base.OnPropertyChanged(value, "IsCenterOfFocus");
				}
			}
		}

		public bool IsFormationTargetRelevant
		{
			get
			{
				return this._isFormationTargetRelevant;
			}
			set
			{
				if (this._isFormationTargetRelevant != value)
				{
					this._isFormationTargetRelevant = value;
					base.OnPropertyChanged(value, "IsFormationTargetRelevant");
				}
			}
		}

		public bool IsTargetingAFormation
		{
			get
			{
				return this._isTargetingAFormation;
			}
			set
			{
				if (this._isTargetingAFormation != value)
				{
					this._isTargetingAFormation = value;
					base.OnPropertyChanged(value, "IsTargetingAFormation");
					this.UpdateState();
				}
			}
		}

		private bool _isCenterOfFocus;

		private bool _isFormationTargetRelevant;

		private bool _isTargetingAFormation;
	}
}
