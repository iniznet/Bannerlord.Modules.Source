using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby
{
	// Token: 0x0200009E RID: 158
	public class MultiplayerLobbyRankItemButtonWidget : ButtonWidget
	{
		// Token: 0x0600084A RID: 2122 RVA: 0x00018205 File Offset: 0x00016405
		public MultiplayerLobbyRankItemButtonWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x0600084B RID: 2123 RVA: 0x00018210 File Offset: 0x00016410
		private void UpdateSprite()
		{
			string text = "unranked";
			if (this.RankID != string.Empty)
			{
				text = this.RankID;
			}
			base.Brush.DefaultLayer.Sprite = base.Context.SpriteData.GetSprite("MPGeneral\\MPRanks\\" + text);
		}

		// Token: 0x170002E8 RID: 744
		// (get) Token: 0x0600084C RID: 2124 RVA: 0x00018267 File Offset: 0x00016467
		// (set) Token: 0x0600084D RID: 2125 RVA: 0x0001826F File Offset: 0x0001646F
		[Editor(false)]
		public string RankID
		{
			get
			{
				return this._rankID;
			}
			set
			{
				if (value != this._rankID)
				{
					this._rankID = value;
					base.OnPropertyChanged<string>(value, "RankID");
					this.UpdateSprite();
				}
			}
		}

		// Token: 0x040003CA RID: 970
		private const string _defaultRankID = "unranked";

		// Token: 0x040003CB RID: 971
		private string _rankID;
	}
}
