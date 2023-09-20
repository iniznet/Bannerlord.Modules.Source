using System;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection
{
	// Token: 0x0200001A RID: 26
	public class SelectableFiefItemPropertyVM : SelectableItemPropertyVM
	{
		// Token: 0x06000191 RID: 401 RVA: 0x0000F597 File Offset: 0x0000D797
		public SelectableFiefItemPropertyVM(string name, string value, int changeAmount, SelectableItemPropertyVM.PropertyType type, BasicTooltipViewModel hint = null, bool isWarning = false)
			: base(name, value, hint)
		{
			this.ChangeAmount = changeAmount;
			this.IsWarning = isWarning;
			base.Type = (int)type;
		}

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x06000192 RID: 402 RVA: 0x0000F5BA File Offset: 0x0000D7BA
		// (set) Token: 0x06000193 RID: 403 RVA: 0x0000F5C2 File Offset: 0x0000D7C2
		[DataSourceProperty]
		public bool IsWarning
		{
			get
			{
				return this._isWarning;
			}
			set
			{
				if (value != this._isWarning)
				{
					this._isWarning = value;
					base.OnPropertyChangedWithValue(value, "IsWarning");
				}
			}
		}

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x06000194 RID: 404 RVA: 0x0000F5E0 File Offset: 0x0000D7E0
		// (set) Token: 0x06000195 RID: 405 RVA: 0x0000F5E8 File Offset: 0x0000D7E8
		[DataSourceProperty]
		public int ChangeAmount
		{
			get
			{
				return this._changeAmount;
			}
			set
			{
				if (value != this._changeAmount)
				{
					this._changeAmount = value;
					base.OnPropertyChangedWithValue(value, "ChangeAmount");
				}
			}
		}

		// Token: 0x040000B4 RID: 180
		private bool _isWarning;

		// Token: 0x040000B5 RID: 181
		private int _changeAmount;
	}
}
