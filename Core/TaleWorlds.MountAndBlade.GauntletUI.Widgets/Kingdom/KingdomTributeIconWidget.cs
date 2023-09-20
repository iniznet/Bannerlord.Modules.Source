using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Kingdom
{
	// Token: 0x02000118 RID: 280
	public class KingdomTributeIconWidget : Widget
	{
		// Token: 0x06000E42 RID: 3650 RVA: 0x00027A42 File Offset: 0x00025C42
		public KingdomTributeIconWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000E43 RID: 3651 RVA: 0x00027A4B File Offset: 0x00025C4B
		public void UpdateIcons(int tribute)
		{
			if (this.PayIcon != null && this.ReceiveIcon != null)
			{
				this.PayIcon.IsVisible = tribute > 0;
				this.ReceiveIcon.IsVisible = tribute < 0;
			}
		}

		// Token: 0x17000512 RID: 1298
		// (set) Token: 0x06000E44 RID: 3652 RVA: 0x00027A7B File Offset: 0x00025C7B
		public int Tribute
		{
			set
			{
				this.UpdateIcons(value);
			}
		}

		// Token: 0x17000513 RID: 1299
		// (get) Token: 0x06000E45 RID: 3653 RVA: 0x00027A84 File Offset: 0x00025C84
		// (set) Token: 0x06000E46 RID: 3654 RVA: 0x00027A8C File Offset: 0x00025C8C
		public Widget PayIcon
		{
			get
			{
				return this._payIcon;
			}
			set
			{
				if (value != this._payIcon)
				{
					this._payIcon = value;
				}
			}
		}

		// Token: 0x17000514 RID: 1300
		// (get) Token: 0x06000E47 RID: 3655 RVA: 0x00027A9E File Offset: 0x00025C9E
		// (set) Token: 0x06000E48 RID: 3656 RVA: 0x00027AA6 File Offset: 0x00025CA6
		public Widget ReceiveIcon
		{
			get
			{
				return this._receiveIcon;
			}
			set
			{
				if (value != this._receiveIcon)
				{
					this._receiveIcon = value;
				}
			}
		}

		// Token: 0x0400068F RID: 1679
		private Widget _payIcon;

		// Token: 0x04000690 RID: 1680
		private Widget _receiveIcon;
	}
}
