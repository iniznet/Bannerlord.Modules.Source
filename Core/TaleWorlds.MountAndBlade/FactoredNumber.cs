using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public struct FactoredNumber
	{
		public float ResultNumber
		{
			get
			{
				return MathF.Clamp(this.BaseNumber + this.BaseNumber * this._sumOfFactors, this.LimitMinValue, this.LimitMaxValue);
			}
		}

		public float BaseNumber { get; private set; }

		public float LimitMinValue
		{
			get
			{
				return this._limitMinValue;
			}
		}

		public float LimitMaxValue
		{
			get
			{
				return this._limitMaxValue;
			}
		}

		public FactoredNumber(float baseNumber = 0f)
		{
			this.BaseNumber = baseNumber;
			this._sumOfFactors = 0f;
			this._limitMinValue = float.MinValue;
			this._limitMaxValue = float.MaxValue;
		}

		public void Add(float value)
		{
			if (value.ApproximatelyEqualsTo(0f, 1E-05f))
			{
				return;
			}
			this.BaseNumber += value;
		}

		public void AddFactor(float value)
		{
			if (value.ApproximatelyEqualsTo(0f, 1E-05f))
			{
				return;
			}
			this._sumOfFactors += value;
		}

		public void LimitMin(float minValue)
		{
			this._limitMinValue = minValue;
		}

		public void LimitMax(float maxValue)
		{
			this._limitMaxValue = maxValue;
		}

		public void Clamp(float minValue, float maxValue)
		{
			this.LimitMin(minValue);
			this.LimitMax(maxValue);
		}

		private float _limitMinValue;

		private float _limitMaxValue;

		private float _sumOfFactors;
	}
}
