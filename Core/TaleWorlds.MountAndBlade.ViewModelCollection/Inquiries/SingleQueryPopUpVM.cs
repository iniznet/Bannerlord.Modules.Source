using System;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Inquiries
{
	// Token: 0x020000D3 RID: 211
	public class SingleQueryPopUpVM : PopUpBaseVM
	{
		// Token: 0x060013B7 RID: 5047 RVA: 0x00040CED File Offset: 0x0003EEED
		public SingleQueryPopUpVM(Action closeQuery)
			: base(closeQuery)
		{
			base.ButtonOkHint = new HintViewModel();
			base.ButtonCancelHint = new HintViewModel();
		}

		// Token: 0x060013B8 RID: 5048 RVA: 0x00040D0C File Offset: 0x0003EF0C
		public override void OnTick(float dt)
		{
			base.OnTick(dt);
			if (this._data != null)
			{
				if (this._data.ExpireTime > 0f)
				{
					if (this._queryTimer > this._data.ExpireTime)
					{
						Action timeoutAction = this._data.TimeoutAction;
						if (timeoutAction != null)
						{
							timeoutAction();
						}
						base.CloseQuery();
					}
					else
					{
						this._queryTimer += dt;
						this.RemainingQueryTime = this._data.ExpireTime - this._queryTimer;
					}
				}
				this.UpdateButtonEnabledStates();
			}
		}

		// Token: 0x060013B9 RID: 5049 RVA: 0x00040D97 File Offset: 0x0003EF97
		public override void ExecuteAffirmativeAction()
		{
			Action affirmativeAction = this._data.AffirmativeAction;
			if (affirmativeAction != null)
			{
				affirmativeAction();
			}
			base.CloseQuery();
		}

		// Token: 0x060013BA RID: 5050 RVA: 0x00040DB5 File Offset: 0x0003EFB5
		public override void ExecuteNegativeAction()
		{
			Action negativeAction = this._data.NegativeAction;
			if (negativeAction != null)
			{
				negativeAction();
			}
			base.CloseQuery();
		}

		// Token: 0x060013BB RID: 5051 RVA: 0x00040DD3 File Offset: 0x0003EFD3
		public override void OnClearData()
		{
			base.OnClearData();
			this._data = null;
		}

		// Token: 0x060013BC RID: 5052 RVA: 0x00040DE4 File Offset: 0x0003EFE4
		private void UpdateButtonEnabledStates()
		{
			if (this._data.GetIsAffirmativeOptionEnabled != null)
			{
				ValueTuple<bool, string> valueTuple = this._data.GetIsAffirmativeOptionEnabled();
				base.IsButtonOkEnabled = valueTuple.Item1;
				if (!string.Equals(this._lastButtonOkHint, valueTuple.Item2, StringComparison.OrdinalIgnoreCase))
				{
					base.ButtonOkHint.HintText = (string.IsNullOrEmpty(valueTuple.Item2) ? TextObject.Empty : new TextObject("{=!}" + valueTuple.Item2, null));
					this._lastButtonOkHint = valueTuple.Item2;
				}
			}
			else
			{
				base.IsButtonOkEnabled = true;
				this._lastButtonOkHint = string.Empty;
			}
			if (this._data.GetIsNegativeOptionEnabled != null)
			{
				ValueTuple<bool, string> valueTuple2 = this._data.GetIsNegativeOptionEnabled();
				base.IsButtonCancelEnabled = valueTuple2.Item1;
				if (!string.Equals(this._lastButtonCancelHint, valueTuple2.Item2, StringComparison.OrdinalIgnoreCase))
				{
					base.ButtonCancelHint.HintText = (string.IsNullOrEmpty(valueTuple2.Item2) ? TextObject.Empty : new TextObject("{=!}" + valueTuple2.Item2, null));
				}
				this._lastButtonCancelHint = valueTuple2.Item2;
				return;
			}
			base.IsButtonCancelEnabled = true;
			this._lastButtonCancelHint = string.Empty;
		}

		// Token: 0x060013BD RID: 5053 RVA: 0x00040F18 File Offset: 0x0003F118
		public void SetData(InquiryData data)
		{
			this._data = data;
			base.TitleText = this._data.TitleText;
			base.PopUpLabel = this._data.Text;
			base.ButtonOkLabel = this._data.AffirmativeText;
			base.ButtonCancelLabel = this._data.NegativeText;
			base.IsButtonOkShown = this._data.IsAffirmativeOptionShown;
			base.IsButtonCancelShown = this._data.IsNegativeOptionShown;
			this.IsTimerShown = this._data.ExpireTime > 0f;
			base.IsButtonOkEnabled = true;
			base.IsButtonCancelEnabled = true;
			this.UpdateButtonEnabledStates();
			this._queryTimer = 0f;
			this.TotalQueryTime = (float)MathF.Round(this._data.ExpireTime);
		}

		// Token: 0x17000682 RID: 1666
		// (get) Token: 0x060013BE RID: 5054 RVA: 0x00040FE0 File Offset: 0x0003F1E0
		// (set) Token: 0x060013BF RID: 5055 RVA: 0x00040FE8 File Offset: 0x0003F1E8
		[DataSourceProperty]
		public float RemainingQueryTime
		{
			get
			{
				return this._remainingQueryTime;
			}
			set
			{
				if (value != this._remainingQueryTime)
				{
					this._remainingQueryTime = value;
					base.OnPropertyChangedWithValue(value, "RemainingQueryTime");
				}
			}
		}

		// Token: 0x17000683 RID: 1667
		// (get) Token: 0x060013C0 RID: 5056 RVA: 0x00041006 File Offset: 0x0003F206
		// (set) Token: 0x060013C1 RID: 5057 RVA: 0x0004100E File Offset: 0x0003F20E
		[DataSourceProperty]
		public float TotalQueryTime
		{
			get
			{
				return this._totalQueryTime;
			}
			set
			{
				if (value != this._totalQueryTime)
				{
					this._totalQueryTime = value;
					base.OnPropertyChangedWithValue(value, "TotalQueryTime");
				}
			}
		}

		// Token: 0x17000684 RID: 1668
		// (get) Token: 0x060013C2 RID: 5058 RVA: 0x0004102C File Offset: 0x0003F22C
		// (set) Token: 0x060013C3 RID: 5059 RVA: 0x00041034 File Offset: 0x0003F234
		[DataSourceProperty]
		public bool IsTimerShown
		{
			get
			{
				return this._isTimerShown;
			}
			set
			{
				if (value != this._isTimerShown)
				{
					this._isTimerShown = value;
					base.OnPropertyChangedWithValue(value, "IsTimerShown");
				}
			}
		}

		// Token: 0x04000978 RID: 2424
		private InquiryData _data;

		// Token: 0x04000979 RID: 2425
		private float _queryTimer;

		// Token: 0x0400097A RID: 2426
		private string _lastButtonOkHint;

		// Token: 0x0400097B RID: 2427
		private string _lastButtonCancelHint;

		// Token: 0x0400097C RID: 2428
		private float _remainingQueryTime;

		// Token: 0x0400097D RID: 2429
		private float _totalQueryTime;

		// Token: 0x0400097E RID: 2430
		private bool _isTimerShown;
	}
}
