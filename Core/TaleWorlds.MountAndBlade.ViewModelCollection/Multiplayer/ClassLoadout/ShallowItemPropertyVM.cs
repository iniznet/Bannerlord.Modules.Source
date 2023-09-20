using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.ClassLoadout
{
	// Token: 0x020000CF RID: 207
	public class ShallowItemPropertyVM : ViewModel
	{
		// Token: 0x06001368 RID: 4968 RVA: 0x0003FD37 File Offset: 0x0003DF37
		public ShallowItemPropertyVM(TextObject propertyName, int permille, int value)
		{
			this._propertyName = propertyName;
			this.Permille = permille;
			this.Value = value;
			this.RefreshValues();
		}

		// Token: 0x06001369 RID: 4969 RVA: 0x0003FD5A File Offset: 0x0003DF5A
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.NameText = this._propertyName.ToString();
		}

		// Token: 0x17000668 RID: 1640
		// (get) Token: 0x0600136A RID: 4970 RVA: 0x0003FD73 File Offset: 0x0003DF73
		// (set) Token: 0x0600136B RID: 4971 RVA: 0x0003FD7B File Offset: 0x0003DF7B
		[DataSourceProperty]
		public int Value
		{
			get
			{
				return this._value;
			}
			set
			{
				if (value != this._value)
				{
					this._value = value;
					base.OnPropertyChangedWithValue(value, "Value");
				}
			}
		}

		// Token: 0x17000669 RID: 1641
		// (get) Token: 0x0600136C RID: 4972 RVA: 0x0003FD99 File Offset: 0x0003DF99
		// (set) Token: 0x0600136D RID: 4973 RVA: 0x0003FDA1 File Offset: 0x0003DFA1
		[DataSourceProperty]
		public string NameText
		{
			get
			{
				return this._nameText;
			}
			set
			{
				if (value != this._nameText)
				{
					this._nameText = value;
					base.OnPropertyChangedWithValue<string>(value, "NameText");
				}
			}
		}

		// Token: 0x1700066A RID: 1642
		// (get) Token: 0x0600136E RID: 4974 RVA: 0x0003FDC4 File Offset: 0x0003DFC4
		// (set) Token: 0x0600136F RID: 4975 RVA: 0x0003FDCC File Offset: 0x0003DFCC
		[DataSourceProperty]
		public int Permille
		{
			get
			{
				return this._permille;
			}
			set
			{
				if (value != this._permille)
				{
					this._permille = value;
					base.OnPropertyChangedWithValue(value, "Permille");
				}
			}
		}

		// Token: 0x04000954 RID: 2388
		private readonly TextObject _propertyName;

		// Token: 0x04000955 RID: 2389
		private string _nameText;

		// Token: 0x04000956 RID: 2390
		private int _permille;

		// Token: 0x04000957 RID: 2391
		private int _value;
	}
}
