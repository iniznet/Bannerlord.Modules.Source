using System;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.CharacterCreation
{
	// Token: 0x0200012A RID: 298
	public class CharacterCreationOptionVM : StringItemWithActionVM
	{
		// Token: 0x06001C7D RID: 7293 RVA: 0x000665A9 File Offset: 0x000647A9
		public CharacterCreationOptionVM(Action<object> onExecute, string item, object identifier)
			: base(onExecute, item, identifier)
		{
		}

		// Token: 0x170009C1 RID: 2497
		// (get) Token: 0x06001C7E RID: 7294 RVA: 0x000665B4 File Offset: 0x000647B4
		// (set) Token: 0x06001C7F RID: 7295 RVA: 0x000665BC File Offset: 0x000647BC
		[DataSourceProperty]
		public bool IsSelected
		{
			get
			{
				return this._isSelected;
			}
			set
			{
				if (value != this._isSelected)
				{
					this._isSelected = value;
					base.OnPropertyChangedWithValue(value, "IsSelected");
					base.ExecuteAction();
				}
			}
		}

		// Token: 0x04000D6F RID: 3439
		private bool _isSelected;
	}
}
