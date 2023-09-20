using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x02000005 RID: 5
	public class BannerIconTextureWidget : TextureWidget
	{
		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600000B RID: 11 RVA: 0x00002164 File Offset: 0x00000364
		// (set) Token: 0x0600000C RID: 12 RVA: 0x0000216C File Offset: 0x0000036C
		[Editor(false)]
		public string Code
		{
			get
			{
				return this._code;
			}
			set
			{
				if (this._code != value)
				{
					this._code = value;
					base.OnPropertyChanged<string>(value, "Code");
					base.SetTextureProviderProperty("Code", value);
				}
			}
		}

		// Token: 0x0600000D RID: 13 RVA: 0x0000219B File Offset: 0x0000039B
		public BannerIconTextureWidget(UIContext context)
			: base(context)
		{
			base.TextureProviderName = "GauntletBannerTableauProvider";
		}

		// Token: 0x04000003 RID: 3
		private string _code;
	}
}
