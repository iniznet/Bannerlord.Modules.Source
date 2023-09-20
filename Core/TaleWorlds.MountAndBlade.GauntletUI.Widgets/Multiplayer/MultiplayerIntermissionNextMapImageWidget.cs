using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer
{
	// Token: 0x0200007D RID: 125
	public class MultiplayerIntermissionNextMapImageWidget : Widget
	{
		// Token: 0x060006E6 RID: 1766 RVA: 0x0001484E File Offset: 0x00012A4E
		public MultiplayerIntermissionNextMapImageWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060006E7 RID: 1767 RVA: 0x00014857 File Offset: 0x00012A57
		private void UpdateMapImage()
		{
			if (string.IsNullOrEmpty(this.MapID))
			{
				return;
			}
			base.Sprite = base.Context.SpriteData.GetSprite(this.MapID);
		}

		// Token: 0x17000273 RID: 627
		// (get) Token: 0x060006E8 RID: 1768 RVA: 0x00014883 File Offset: 0x00012A83
		// (set) Token: 0x060006E9 RID: 1769 RVA: 0x0001488B File Offset: 0x00012A8B
		[DataSourceProperty]
		public string MapID
		{
			get
			{
				return this._mapID;
			}
			set
			{
				if (value != this._mapID)
				{
					this._mapID = value;
					base.OnPropertyChanged<string>(value, "MapID");
					this.UpdateMapImage();
				}
			}
		}

		// Token: 0x0400030E RID: 782
		private string _mapID;
	}
}
