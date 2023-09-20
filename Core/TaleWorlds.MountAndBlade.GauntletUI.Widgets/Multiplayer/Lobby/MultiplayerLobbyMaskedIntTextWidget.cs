using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby
{
	// Token: 0x0200009B RID: 155
	public class MultiplayerLobbyMaskedIntTextWidget : TextWidget
	{
		// Token: 0x0600082E RID: 2094 RVA: 0x00017F73 File Offset: 0x00016173
		public MultiplayerLobbyMaskedIntTextWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x0600082F RID: 2095 RVA: 0x00017F7C File Offset: 0x0001617C
		private void IntValueUpdated()
		{
			if (this.IntValue == this.MaskedIntValue)
			{
				base.Text = this.MaskText;
				return;
			}
			base.IntText = this.IntValue;
		}

		// Token: 0x170002DE RID: 734
		// (get) Token: 0x06000830 RID: 2096 RVA: 0x00017FA5 File Offset: 0x000161A5
		// (set) Token: 0x06000831 RID: 2097 RVA: 0x00017FAD File Offset: 0x000161AD
		[Editor(false)]
		public int IntValue
		{
			get
			{
				return this._intValue;
			}
			set
			{
				if (this._intValue != value)
				{
					this._intValue = value;
					base.OnPropertyChanged(value, "IntValue");
					this.IntValueUpdated();
				}
			}
		}

		// Token: 0x170002DF RID: 735
		// (get) Token: 0x06000832 RID: 2098 RVA: 0x00017FD1 File Offset: 0x000161D1
		// (set) Token: 0x06000833 RID: 2099 RVA: 0x00017FD9 File Offset: 0x000161D9
		[Editor(false)]
		public int MaskedIntValue
		{
			get
			{
				return this._maskedIntValue;
			}
			set
			{
				if (this._maskedIntValue != value)
				{
					this._maskedIntValue = value;
					base.OnPropertyChanged(value, "MaskedIntValue");
				}
			}
		}

		// Token: 0x170002E0 RID: 736
		// (get) Token: 0x06000834 RID: 2100 RVA: 0x00017FF7 File Offset: 0x000161F7
		// (set) Token: 0x06000835 RID: 2101 RVA: 0x00017FFF File Offset: 0x000161FF
		[Editor(false)]
		public string MaskText
		{
			get
			{
				return this._maskText;
			}
			set
			{
				if (this._maskText != value)
				{
					this._maskText = value;
					base.OnPropertyChanged<string>(value, "MaskText");
				}
			}
		}

		// Token: 0x040003C0 RID: 960
		private int _intValue;

		// Token: 0x040003C1 RID: 961
		private int _maskedIntValue;

		// Token: 0x040003C2 RID: 962
		private string _maskText;
	}
}
