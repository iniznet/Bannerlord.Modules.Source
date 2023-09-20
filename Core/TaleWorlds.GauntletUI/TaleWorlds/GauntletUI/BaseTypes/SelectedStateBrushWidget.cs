using System;

namespace TaleWorlds.GauntletUI.BaseTypes
{
	// Token: 0x02000066 RID: 102
	public class SelectedStateBrushWidget : BrushWidget
	{
		// Token: 0x0600069C RID: 1692 RVA: 0x0001D658 File Offset: 0x0001B858
		public SelectedStateBrushWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x0600069D RID: 1693 RVA: 0x0001D673 File Offset: 0x0001B873
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

		// Token: 0x170001E6 RID: 486
		// (get) Token: 0x0600069E RID: 1694 RVA: 0x0001D6AC File Offset: 0x0001B8AC
		// (set) Token: 0x0600069F RID: 1695 RVA: 0x0001D6B4 File Offset: 0x0001B8B4
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

		// Token: 0x04000323 RID: 803
		private bool _isDirty = true;

		// Token: 0x04000324 RID: 804
		private bool _isBrushStatesRegistered;

		// Token: 0x04000325 RID: 805
		private string _selectedState = "Default";
	}
}
