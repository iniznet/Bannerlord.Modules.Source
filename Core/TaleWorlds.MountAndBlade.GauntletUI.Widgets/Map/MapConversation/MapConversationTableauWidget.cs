using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Map.MapConversation
{
	// Token: 0x02000107 RID: 263
	public class MapConversationTableauWidget : TextureWidget
	{
		// Token: 0x06000D8F RID: 3471 RVA: 0x0002603D File Offset: 0x0002423D
		public MapConversationTableauWidget(UIContext context)
			: base(context)
		{
			base.TextureProviderName = "MapConversationTextureProvider";
			this._isRenderRequestedPreviousFrame = true;
			base.UpdateTextureWidget();
		}

		// Token: 0x170004D4 RID: 1236
		// (get) Token: 0x06000D90 RID: 3472 RVA: 0x0002605E File Offset: 0x0002425E
		// (set) Token: 0x06000D91 RID: 3473 RVA: 0x00026066 File Offset: 0x00024266
		[Editor(false)]
		public object Data
		{
			get
			{
				return this._data;
			}
			set
			{
				if (value != this._data)
				{
					this._data = value;
					base.OnPropertyChanged<object>(value, "Data");
					base.SetTextureProviderProperty("Data", value);
				}
			}
		}

		// Token: 0x170004D5 RID: 1237
		// (get) Token: 0x06000D92 RID: 3474 RVA: 0x00026090 File Offset: 0x00024290
		// (set) Token: 0x06000D93 RID: 3475 RVA: 0x00026098 File Offset: 0x00024298
		[Editor(false)]
		public bool IsTableauEnabled
		{
			get
			{
				return this._isTableauEnabled;
			}
			set
			{
				if (value != this._isTableauEnabled)
				{
					this._isTableauEnabled = value;
					base.OnPropertyChanged(value, "IsTableauEnabled");
					base.SetTextureProviderProperty("IsEnabled", value);
				}
			}
		}

		// Token: 0x04000641 RID: 1601
		private object _data;

		// Token: 0x04000642 RID: 1602
		private bool _isTableauEnabled;
	}
}
