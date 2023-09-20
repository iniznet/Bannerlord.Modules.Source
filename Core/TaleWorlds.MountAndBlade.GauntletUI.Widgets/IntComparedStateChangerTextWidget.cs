using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	public class IntComparedStateChangerTextWidget : TextWidget
	{
		public IntComparedStateChangerTextWidget(UIContext context)
			: base(context)
		{
		}

		private void UpdateState()
		{
			if (string.IsNullOrEmpty(this.TrueState) || string.IsNullOrEmpty(this.FalseState))
			{
				return;
			}
			bool flag = false;
			if (this.ComparisonType == IntComparedStateChangerTextWidget.ComparisonTypes.Equals)
			{
				flag = this.FirstValue == this.SecondValue;
			}
			else if (this.ComparisonType == IntComparedStateChangerTextWidget.ComparisonTypes.NotEquals)
			{
				flag = this.FirstValue != this.SecondValue;
			}
			else if (this.ComparisonType == IntComparedStateChangerTextWidget.ComparisonTypes.LessThan)
			{
				flag = this.FirstValue < this.SecondValue;
			}
			else if (this.ComparisonType == IntComparedStateChangerTextWidget.ComparisonTypes.GreaterThan)
			{
				flag = this.FirstValue > this.SecondValue;
			}
			else if (this.ComparisonType == IntComparedStateChangerTextWidget.ComparisonTypes.GreaterThanOrEqual)
			{
				flag = this.FirstValue >= this.SecondValue;
			}
			else if (this.ComparisonType == IntComparedStateChangerTextWidget.ComparisonTypes.LessThanOrEqual)
			{
				flag = this.FirstValue <= this.SecondValue;
			}
			this.SetState(flag ? this.TrueState : this.FalseState);
		}

		public IntComparedStateChangerTextWidget.ComparisonTypes ComparisonType
		{
			get
			{
				return this._comparisonType;
			}
			set
			{
				if (value != this._comparisonType)
				{
					this._comparisonType = value;
					this.UpdateState();
				}
			}
		}

		public int FirstValue
		{
			get
			{
				return this._firstValue;
			}
			set
			{
				if (value != this._firstValue)
				{
					this._firstValue = value;
					this.UpdateState();
				}
			}
		}

		public int SecondValue
		{
			get
			{
				return this._secondValue;
			}
			set
			{
				if (value != this._secondValue)
				{
					this._secondValue = value;
					this.UpdateState();
				}
			}
		}

		public string TrueState
		{
			get
			{
				return this._trueState;
			}
			set
			{
				if (value != this._trueState)
				{
					this._trueState = value;
					this.UpdateState();
				}
			}
		}

		public string FalseState
		{
			get
			{
				return this._falseState;
			}
			set
			{
				if (value != this._falseState)
				{
					this._falseState = value;
					this.UpdateState();
				}
			}
		}

		private IntComparedStateChangerTextWidget.ComparisonTypes _comparisonType;

		private int _firstValue;

		private int _secondValue;

		private string _trueState;

		private string _falseState;

		public enum ComparisonTypes
		{
			Equals,
			NotEquals,
			GreaterThan,
			LessThan,
			GreaterThanOrEqual,
			LessThanOrEqual
		}
	}
}
