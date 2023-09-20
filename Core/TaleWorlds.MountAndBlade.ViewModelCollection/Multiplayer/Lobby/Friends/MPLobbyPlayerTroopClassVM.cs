using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Friends
{
	// Token: 0x02000089 RID: 137
	public class MPLobbyPlayerTroopClassVM : ViewModel
	{
		// Token: 0x06000CAE RID: 3246 RVA: 0x0002C8BD File Offset: 0x0002AABD
		public MPLobbyPlayerTroopClassVM()
		{
			this.Name = "Varangian Guard";
			this.Preview = new ImageIdentifierVM(ImageIdentifierType.Null);
		}

		// Token: 0x170003F7 RID: 1015
		// (get) Token: 0x06000CAF RID: 3247 RVA: 0x0002C8DC File Offset: 0x0002AADC
		// (set) Token: 0x06000CB0 RID: 3248 RVA: 0x0002C8E4 File Offset: 0x0002AAE4
		[DataSourceProperty]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (value != this._name)
				{
					this._name = value;
					base.OnPropertyChangedWithValue<string>(value, "Name");
				}
			}
		}

		// Token: 0x170003F8 RID: 1016
		// (get) Token: 0x06000CB1 RID: 3249 RVA: 0x0002C907 File Offset: 0x0002AB07
		// (set) Token: 0x06000CB2 RID: 3250 RVA: 0x0002C90F File Offset: 0x0002AB0F
		[DataSourceProperty]
		public ImageIdentifierVM Preview
		{
			get
			{
				return this._preview;
			}
			set
			{
				if (value != this._preview)
				{
					this._preview = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "Preview");
				}
			}
		}

		// Token: 0x04000610 RID: 1552
		private string _name;

		// Token: 0x04000611 RID: 1553
		private ImageIdentifierVM _preview;
	}
}
