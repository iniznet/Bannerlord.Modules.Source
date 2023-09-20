using System;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.Overlay
{
	// Token: 0x020000A2 RID: 162
	public class GameMenuOverlayActionVM : StringItemWithEnabledAndHintVM
	{
		// Token: 0x06001020 RID: 4128 RVA: 0x0003FB32 File Offset: 0x0003DD32
		public GameMenuOverlayActionVM(Action<object> onExecute, string item, bool isEnabled, object identifier, TextObject hint = null)
			: base(onExecute, item, isEnabled, identifier, hint)
		{
		}

		// Token: 0x1700054F RID: 1359
		// (get) Token: 0x06001021 RID: 4129 RVA: 0x0003FB41 File Offset: 0x0003DD41
		// (set) Token: 0x06001022 RID: 4130 RVA: 0x0003FB49 File Offset: 0x0003DD49
		[DataSourceProperty]
		public bool IsHiglightEnabled
		{
			get
			{
				return this._isHiglightEnabled;
			}
			set
			{
				if (value != this._isHiglightEnabled)
				{
					this._isHiglightEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsHiglightEnabled");
				}
			}
		}

		// Token: 0x0400077C RID: 1916
		private bool _isHiglightEnabled;
	}
}
