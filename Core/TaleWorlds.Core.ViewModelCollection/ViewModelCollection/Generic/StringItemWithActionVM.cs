using System;
using TaleWorlds.Library;

namespace TaleWorlds.Core.ViewModelCollection.Generic
{
	// Token: 0x02000020 RID: 32
	public class StringItemWithActionVM : ViewModel
	{
		// Token: 0x0600017F RID: 383 RVA: 0x00005200 File Offset: 0x00003400
		public StringItemWithActionVM(Action<object> onExecute, string item, object identifier)
		{
			this._onExecute = onExecute;
			this.Identifier = identifier;
			this.ActionText = item;
		}

		// Token: 0x06000180 RID: 384 RVA: 0x0000521D File Offset: 0x0000341D
		public void ExecuteAction()
		{
			this._onExecute(this.Identifier);
		}

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x06000181 RID: 385 RVA: 0x00005230 File Offset: 0x00003430
		// (set) Token: 0x06000182 RID: 386 RVA: 0x00005238 File Offset: 0x00003438
		[DataSourceProperty]
		public string ActionText
		{
			get
			{
				return this._actionText;
			}
			set
			{
				if (value != this._actionText)
				{
					this._actionText = value;
					base.OnPropertyChangedWithValue<string>(value, "ActionText");
				}
			}
		}

		// Token: 0x04000098 RID: 152
		public object Identifier;

		// Token: 0x04000099 RID: 153
		protected Action<object> _onExecute;

		// Token: 0x0400009A RID: 154
		private string _actionText;
	}
}
