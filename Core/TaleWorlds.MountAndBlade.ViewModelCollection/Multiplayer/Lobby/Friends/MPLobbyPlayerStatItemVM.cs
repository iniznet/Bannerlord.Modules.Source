using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Friends
{
	// Token: 0x02000088 RID: 136
	public class MPLobbyPlayerStatItemVM : ViewModel
	{
		// Token: 0x06000CA6 RID: 3238 RVA: 0x0002C7F4 File Offset: 0x0002A9F4
		public MPLobbyPlayerStatItemVM(string gameMode, TextObject description, string value)
		{
			this.GameMode = gameMode;
			this._descriptionText = description;
			this.Value = value;
			this.RefreshValues();
		}

		// Token: 0x06000CA7 RID: 3239 RVA: 0x0002C817 File Offset: 0x0002AA17
		public MPLobbyPlayerStatItemVM(string gameMode, TextObject description, float value)
			: this(gameMode, description, value.ToString("0.00"))
		{
		}

		// Token: 0x06000CA8 RID: 3240 RVA: 0x0002C82D File Offset: 0x0002AA2D
		public MPLobbyPlayerStatItemVM(string gameMode, TextObject description, int value)
			: this(gameMode, description, value.ToString())
		{
		}

		// Token: 0x06000CA9 RID: 3241 RVA: 0x0002C83E File Offset: 0x0002AA3E
		public override void RefreshValues()
		{
			base.RefreshValues();
			TextObject descriptionText = this._descriptionText;
			this.Description = ((descriptionText != null) ? descriptionText.ToString() : null) ?? "";
		}

		// Token: 0x170003F5 RID: 1013
		// (get) Token: 0x06000CAA RID: 3242 RVA: 0x0002C867 File Offset: 0x0002AA67
		// (set) Token: 0x06000CAB RID: 3243 RVA: 0x0002C86F File Offset: 0x0002AA6F
		[DataSourceProperty]
		public string Description
		{
			get
			{
				return this._description;
			}
			set
			{
				if (value != this._description)
				{
					this._description = value;
					base.OnPropertyChangedWithValue<string>(value, "Description");
				}
			}
		}

		// Token: 0x170003F6 RID: 1014
		// (get) Token: 0x06000CAC RID: 3244 RVA: 0x0002C892 File Offset: 0x0002AA92
		// (set) Token: 0x06000CAD RID: 3245 RVA: 0x0002C89A File Offset: 0x0002AA9A
		[DataSourceProperty]
		public string Value
		{
			get
			{
				return this._value;
			}
			set
			{
				if (value != this._value)
				{
					this._value = value;
					base.OnPropertyChangedWithValue<string>(value, "Value");
				}
			}
		}

		// Token: 0x0400060C RID: 1548
		public readonly string GameMode;

		// Token: 0x0400060D RID: 1549
		private readonly TextObject _descriptionText;

		// Token: 0x0400060E RID: 1550
		private string _description;

		// Token: 0x0400060F RID: 1551
		private string _value;
	}
}
