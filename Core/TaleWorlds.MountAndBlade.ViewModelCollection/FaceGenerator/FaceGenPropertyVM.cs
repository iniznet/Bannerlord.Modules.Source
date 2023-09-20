using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.FaceGenerator
{
	// Token: 0x02000106 RID: 262
	public class FaceGenPropertyVM : ViewModel
	{
		// Token: 0x17000779 RID: 1913
		// (get) Token: 0x060016FB RID: 5883 RVA: 0x0004ACF2 File Offset: 0x00048EF2
		public int KeyTimePoint { get; }

		// Token: 0x060016FC RID: 5884 RVA: 0x0004ACFC File Offset: 0x00048EFC
		public FaceGenPropertyVM(int keyNo, double min, double max, TextObject name, int keyTimePoint, int tabId, double value, Action<int, float, bool, bool> updateFace, Action addCommand, Action resetSliderPrevValuesCommand, bool isEnabled = true, bool isDiscrete = false)
		{
			this._calledFromInit = true;
			this._updateFace = updateFace;
			this._addCommand = addCommand;
			this._nameObj = name;
			this._resetSliderPrevValuesCommand = resetSliderPrevValuesCommand;
			this.KeyNo = keyNo;
			this.Min = (float)min;
			this.Max = (float)max;
			this.KeyTimePoint = keyTimePoint;
			this.TabID = tabId;
			this._initialValue = (float)value;
			this.Value = (float)value;
			this.PrevValue = -1.0;
			this.IsEnabled = isEnabled;
			this.IsDiscrete = isDiscrete;
			this._calledFromInit = false;
			this.RefreshValues();
		}

		// Token: 0x060016FD RID: 5885 RVA: 0x0004ADB8 File Offset: 0x00048FB8
		public void Reset()
		{
			this._updateOnValueChange = false;
			this.Value = this._initialValue;
			this._updateOnValueChange = true;
		}

		// Token: 0x060016FE RID: 5886 RVA: 0x0004ADD4 File Offset: 0x00048FD4
		public void Randomize()
		{
			this._updateOnValueChange = false;
			float num = 0.5f * (MBRandom.RandomFloat + MBRandom.RandomFloat);
			this.Value = num * (this.Max - this.Min) + this.Min;
			this._updateOnValueChange = true;
		}

		// Token: 0x060016FF RID: 5887 RVA: 0x0004AE1D File Offset: 0x0004901D
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = this._nameObj.ToString();
		}

		// Token: 0x1700077A RID: 1914
		// (get) Token: 0x06001700 RID: 5888 RVA: 0x0004AE36 File Offset: 0x00049036
		// (set) Token: 0x06001701 RID: 5889 RVA: 0x0004AE3E File Offset: 0x0004903E
		[DataSourceProperty]
		public float Min
		{
			get
			{
				return this._min;
			}
			set
			{
				if (value != this._min)
				{
					this._min = value;
					base.OnPropertyChangedWithValue(value, "Min");
				}
			}
		}

		// Token: 0x1700077B RID: 1915
		// (get) Token: 0x06001702 RID: 5890 RVA: 0x0004AE5C File Offset: 0x0004905C
		// (set) Token: 0x06001703 RID: 5891 RVA: 0x0004AE64 File Offset: 0x00049064
		[DataSourceProperty]
		public int TabID
		{
			get
			{
				return this._tabID;
			}
			set
			{
				if (value != this._tabID)
				{
					this._tabID = value;
					base.OnPropertyChangedWithValue(value, "TabID");
				}
			}
		}

		// Token: 0x1700077C RID: 1916
		// (get) Token: 0x06001704 RID: 5892 RVA: 0x0004AE82 File Offset: 0x00049082
		// (set) Token: 0x06001705 RID: 5893 RVA: 0x0004AE8A File Offset: 0x0004908A
		[DataSourceProperty]
		public float Max
		{
			get
			{
				return this._max;
			}
			set
			{
				if (value != this._max)
				{
					this._max = value;
					base.OnPropertyChangedWithValue(value, "Max");
				}
			}
		}

		// Token: 0x1700077D RID: 1917
		// (get) Token: 0x06001706 RID: 5894 RVA: 0x0004AEA8 File Offset: 0x000490A8
		// (set) Token: 0x06001707 RID: 5895 RVA: 0x0004AEB0 File Offset: 0x000490B0
		[DataSourceProperty]
		public float Value
		{
			get
			{
				return this._value;
			}
			set
			{
				if ((double)MathF.Abs(value - this._value) > ((this.KeyNo == -16) ? 0.006000000052154064 : 0.06))
				{
					if (!this._calledFromInit && this.PrevValue < 0.0 && this._updateOnValueChange)
					{
						this._addCommand();
					}
					this._resetSliderPrevValuesCommand();
					if (this.KeyNo >= 0)
					{
						this.PrevValue = (double)this._value;
					}
					this._value = value;
					base.OnPropertyChangedWithValue(value, "Value");
					Action<int, float, bool, bool> updateFace = this._updateFace;
					if (updateFace == null)
					{
						return;
					}
					updateFace(this.KeyNo, value, this._calledFromInit, this._updateOnValueChange);
				}
			}
		}

		// Token: 0x1700077E RID: 1918
		// (get) Token: 0x06001708 RID: 5896 RVA: 0x0004AF71 File Offset: 0x00049171
		// (set) Token: 0x06001709 RID: 5897 RVA: 0x0004AF79 File Offset: 0x00049179
		[DataSourceProperty]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (value != this._name)
				{
					this._name = value;
					base.OnPropertyChangedWithValue<string>(value, "Name");
				}
			}
		}

		// Token: 0x1700077F RID: 1919
		// (get) Token: 0x0600170A RID: 5898 RVA: 0x0004AF9C File Offset: 0x0004919C
		// (set) Token: 0x0600170B RID: 5899 RVA: 0x0004AFA4 File Offset: 0x000491A4
		[DataSourceProperty]
		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}
			set
			{
				if (value != this._isEnabled)
				{
					this._isEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsEnabled");
				}
			}
		}

		// Token: 0x17000780 RID: 1920
		// (get) Token: 0x0600170C RID: 5900 RVA: 0x0004AFC2 File Offset: 0x000491C2
		// (set) Token: 0x0600170D RID: 5901 RVA: 0x0004AFCA File Offset: 0x000491CA
		[DataSourceProperty]
		public bool IsDiscrete
		{
			get
			{
				return this._isDiscrete;
			}
			set
			{
				if (value != this._isDiscrete)
				{
					this._isDiscrete = value;
					base.OnPropertyChangedWithValue(value, "IsDiscrete");
				}
			}
		}

		// Token: 0x04000AE2 RID: 2786
		public int KeyNo;

		// Token: 0x04000AE3 RID: 2787
		public double PrevValue = -1.0;

		// Token: 0x04000AE4 RID: 2788
		private bool _updateOnValueChange = true;

		// Token: 0x04000AE5 RID: 2789
		private readonly TextObject _nameObj;

		// Token: 0x04000AE6 RID: 2790
		private readonly Action<int, float, bool, bool> _updateFace;

		// Token: 0x04000AE7 RID: 2791
		private readonly Action _resetSliderPrevValuesCommand;

		// Token: 0x04000AE8 RID: 2792
		private readonly Action _addCommand;

		// Token: 0x04000AE9 RID: 2793
		private readonly bool _calledFromInit;

		// Token: 0x04000AEA RID: 2794
		private readonly float _initialValue;

		// Token: 0x04000AEB RID: 2795
		private int _tabID = -1;

		// Token: 0x04000AEC RID: 2796
		private string _name;

		// Token: 0x04000AED RID: 2797
		private float _value;

		// Token: 0x04000AEE RID: 2798
		private float _max;

		// Token: 0x04000AEF RID: 2799
		private float _min;

		// Token: 0x04000AF0 RID: 2800
		private bool _isEnabled;

		// Token: 0x04000AF1 RID: 2801
		private bool _isDiscrete;
	}
}
