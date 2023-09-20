using System;

namespace TaleWorlds.GauntletUI.BaseTypes
{
	public class SelectedStateBrushWidget : BrushWidget
	{
		public SelectedStateBrushWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this._isBrushStatesRegistered)
			{
				this.RegisterBrushStatesOfWidget();
				this._isBrushStatesRegistered = true;
			}
			if (this._isDirty)
			{
				this.SetState(this.SelectedState);
				this._isDirty = false;
			}
		}

		[Editor(false)]
		public string SelectedState
		{
			get
			{
				return this._selectedState;
			}
			set
			{
				if (this._selectedState != value)
				{
					this._selectedState = value;
					base.OnPropertyChanged<string>(value, "SelectedState");
					this._isDirty = true;
				}
			}
		}

		private bool _isDirty = true;

		private bool _isBrushStatesRegistered;

		private string _selectedState = "Default";
	}
}
