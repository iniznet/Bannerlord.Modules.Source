using System;
using TaleWorlds.Library;

namespace TaleWorlds.Core.ViewModelCollection.Generic
{
	// Token: 0x0200001F RID: 31
	public class BoolItemWithActionVM : ViewModel
	{
		// Token: 0x17000078 RID: 120
		// (get) Token: 0x0600017B RID: 379 RVA: 0x000051AA File Offset: 0x000033AA
		// (set) Token: 0x0600017C RID: 380 RVA: 0x000051B2 File Offset: 0x000033B2
		[DataSourceProperty]
		public bool IsActive
		{
			get
			{
				return this._isActive;
			}
			set
			{
				if (value != this._isActive)
				{
					this._isActive = value;
					base.OnPropertyChangedWithValue(value, "IsActive");
				}
			}
		}

		// Token: 0x0600017D RID: 381 RVA: 0x000051D0 File Offset: 0x000033D0
		public void ExecuteAction()
		{
			this._onExecute(this.Identifier);
		}

		// Token: 0x0600017E RID: 382 RVA: 0x000051E3 File Offset: 0x000033E3
		public BoolItemWithActionVM(Action<object> onExecute, bool isActive, object identifier)
		{
			this._onExecute = onExecute;
			this.Identifier = identifier;
			this.IsActive = isActive;
		}

		// Token: 0x04000095 RID: 149
		public object Identifier;

		// Token: 0x04000096 RID: 150
		protected Action<object> _onExecute;

		// Token: 0x04000097 RID: 151
		private bool _isActive;
	}
}
