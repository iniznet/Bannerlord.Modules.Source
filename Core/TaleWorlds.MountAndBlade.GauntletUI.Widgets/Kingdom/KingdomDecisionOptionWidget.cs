using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Kingdom
{
	public class KingdomDecisionOptionWidget : Widget
	{
		public Widget SealVisualWidget { get; set; }

		public DecisionSupportStrengthListPanel StrengthWidget { get; set; }

		public bool IsPlayerSupporter { get; set; }

		public bool IsAbstain { get; set; }

		public float SealStartWidth { get; set; } = 232f;

		public float SealStartHeight { get; set; } = 232f;

		public float SealEndWidth { get; set; } = 140f;

		public float SealEndHeight { get; set; } = 140f;

		public float SealAnimLength { get; set; } = 0.2f;

		public KingdomDecisionOptionWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			this.StrengthWidget.IsVisible = !this.IsAbstain && this.IsPlayerSupporter && this.IsOptionSelected && !this.IsKingsOption && !this._isKingsDecisionDone;
			if (this._animStartTime != -1f && base.EventManager.Time - this._animStartTime < this.SealAnimLength)
			{
				this.SealVisualWidget.IsVisible = true;
				float num = (base.EventManager.Time - this._animStartTime) / this.SealAnimLength;
				this.SealVisualWidget.SuggestedWidth = Mathf.Lerp(this.SealStartWidth, this.SealEndWidth, num);
				this.SealVisualWidget.SuggestedHeight = Mathf.Lerp(this.SealStartHeight, this.SealEndHeight, num);
				this.SealVisualWidget.SetGlobalAlphaRecursively(Mathf.Lerp(0f, 1f, num));
			}
		}

		internal void OnKingsDecisionDone()
		{
			this._isKingsDecisionDone = true;
		}

		internal void OnFinalDone()
		{
			this._isKingsDecisionDone = false;
			this._animStartTime = -1f;
		}

		private void OnSelectionChange(bool value)
		{
			if (!this.IsPlayerSupporter)
			{
				this.SealVisualWidget.IsVisible = value;
				this.SealVisualWidget.SetGlobalAlphaRecursively(0.2f);
				return;
			}
			this.SealVisualWidget.IsVisible = false;
		}

		private void HandleKingsOption()
		{
			this._animStartTime = base.EventManager.Time;
		}

		[Editor(false)]
		public bool IsOptionSelected
		{
			get
			{
				return this._isOptionSelected;
			}
			set
			{
				if (this._isOptionSelected != value)
				{
					this._isOptionSelected = value;
					base.OnPropertyChanged(value, "IsOptionSelected");
					this.OnSelectionChange(value);
					base.GamepadNavigationIndex = (value ? (-1) : 0);
				}
			}
		}

		[Editor(false)]
		public bool IsKingsOption
		{
			get
			{
				return this._isKingsOption;
			}
			set
			{
				if (this._isKingsOption != value)
				{
					this._isKingsOption = value;
					base.OnPropertyChanged(value, "IsKingsOption");
					this.HandleKingsOption();
				}
			}
		}

		private float _animStartTime = -1f;

		private bool _isKingsDecisionDone;

		private bool _isOptionSelected;

		public bool _isKingsOption;
	}
}
