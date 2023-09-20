using System;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Inquiries
{
	public class SingleQueryPopUpVM : PopUpBaseVM
	{
		public SingleQueryPopUpVM(Action closeQuery)
			: base(closeQuery)
		{
			base.ButtonOkHint = new HintViewModel();
			base.ButtonCancelHint = new HintViewModel();
		}

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

		public override void ExecuteAffirmativeAction()
		{
			Action affirmativeAction = this._data.AffirmativeAction;
			if (affirmativeAction != null)
			{
				affirmativeAction();
			}
			base.CloseQuery();
		}

		public override void ExecuteNegativeAction()
		{
			Action negativeAction = this._data.NegativeAction;
			if (negativeAction != null)
			{
				negativeAction();
			}
			base.CloseQuery();
		}

		public override void OnClearData()
		{
			base.OnClearData();
			this._data = null;
		}

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

		private InquiryData _data;

		private float _queryTimer;

		private string _lastButtonOkHint;

		private string _lastButtonCancelHint;

		private float _remainingQueryTime;

		private float _totalQueryTime;

		private bool _isTimerShown;
	}
}
