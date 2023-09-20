using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000207 RID: 519
	public struct FactoredNumber
	{
		// Token: 0x170005AE RID: 1454
		// (get) Token: 0x06001CA0 RID: 7328 RVA: 0x00065EE3 File Offset: 0x000640E3
		public float ResultNumber
		{
			get
			{
				return MathF.Clamp(this.BaseNumber + this.BaseNumber * this._sumOfFactors, this.LimitMinValue, this.LimitMaxValue);
			}
		}

		// Token: 0x170005AF RID: 1455
		// (get) Token: 0x06001CA1 RID: 7329 RVA: 0x00065F0A File Offset: 0x0006410A
		// (set) Token: 0x06001CA2 RID: 7330 RVA: 0x00065F12 File Offset: 0x00064112
		public float BaseNumber { get; private set; }

		// Token: 0x170005B0 RID: 1456
		// (get) Token: 0x06001CA3 RID: 7331 RVA: 0x00065F1B File Offset: 0x0006411B
		public float LimitMinValue
		{
			get
			{
				return this._limitMinValue;
			}
		}

		// Token: 0x170005B1 RID: 1457
		// (get) Token: 0x06001CA4 RID: 7332 RVA: 0x00065F24 File Offset: 0x00064124
		public float LimitMaxValue
		{
			get
			{
				return this._limitMaxValue;
			}
		}

		// Token: 0x06001CA5 RID: 7333 RVA: 0x00065F2D File Offset: 0x0006412D
		public FactoredNumber(float baseNumber = 0f)
		{
			this.BaseNumber = baseNumber;
			this._sumOfFactors = 0f;
			this._limitMinValue = float.MinValue;
			this._limitMaxValue = float.MaxValue;
		}

		// Token: 0x06001CA6 RID: 7334 RVA: 0x00065F57 File Offset: 0x00064157
		public void Add(float value)
		{
			if (value.ApproximatelyEqualsTo(0f, 1E-05f))
			{
				return;
			}
			this.BaseNumber += value;
		}

		// Token: 0x06001CA7 RID: 7335 RVA: 0x00065F7A File Offset: 0x0006417A
		public void AddFactor(float value)
		{
			if (value.ApproximatelyEqualsTo(0f, 1E-05f))
			{
				return;
			}
			this._sumOfFactors += value;
		}

		// Token: 0x06001CA8 RID: 7336 RVA: 0x00065F9D File Offset: 0x0006419D
		public void LimitMin(float minValue)
		{
			this._limitMinValue = minValue;
		}

		// Token: 0x06001CA9 RID: 7337 RVA: 0x00065FA6 File Offset: 0x000641A6
		public void LimitMax(float maxValue)
		{
			this._limitMaxValue = maxValue;
		}

		// Token: 0x06001CAA RID: 7338 RVA: 0x00065FAF File Offset: 0x000641AF
		public void Clamp(float minValue, float maxValue)
		{
			this.LimitMin(minValue);
			this.LimitMax(maxValue);
		}

		// Token: 0x04000976 RID: 2422
		private float _limitMinValue;

		// Token: 0x04000977 RID: 2423
		private float _limitMaxValue;

		// Token: 0x04000978 RID: 2424
		private float _sumOfFactors;
	}
}
