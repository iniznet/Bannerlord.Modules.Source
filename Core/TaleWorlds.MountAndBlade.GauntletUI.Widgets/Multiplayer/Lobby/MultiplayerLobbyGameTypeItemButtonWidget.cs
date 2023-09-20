using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby
{
	// Token: 0x02000099 RID: 153
	public class MultiplayerLobbyGameTypeItemButtonWidget : ButtonWidget
	{
		// Token: 0x06000821 RID: 2081 RVA: 0x00017DDF File Offset: 0x00015FDF
		public MultiplayerLobbyGameTypeItemButtonWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000822 RID: 2082 RVA: 0x00017DE8 File Offset: 0x00015FE8
		private void UpdateSprite()
		{
			base.Brush.DefaultLayer.Sprite = base.Context.SpriteData.GetSprite("MPLobby\\GameTypes\\" + this.GameTypeID);
		}

		// Token: 0x170002DA RID: 730
		// (get) Token: 0x06000823 RID: 2083 RVA: 0x00017E1A File Offset: 0x0001601A
		// (set) Token: 0x06000824 RID: 2084 RVA: 0x00017E22 File Offset: 0x00016022
		[Editor(false)]
		public string GameTypeID
		{
			get
			{
				return this._gameTypeID;
			}
			set
			{
				if (value != this._gameTypeID)
				{
					this._gameTypeID = value;
					base.OnPropertyChanged<string>(value, "GameTypeID");
					this.UpdateSprite();
				}
			}
		}

		// Token: 0x040003BB RID: 955
		private string _gameTypeID;
	}
}
