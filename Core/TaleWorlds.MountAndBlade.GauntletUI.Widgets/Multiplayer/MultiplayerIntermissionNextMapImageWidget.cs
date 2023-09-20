using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer
{
	public class MultiplayerIntermissionNextMapImageWidget : Widget
	{
		public MultiplayerIntermissionNextMapImageWidget(UIContext context)
			: base(context)
		{
		}

		private void UpdateMapImage()
		{
			if (string.IsNullOrEmpty(this.MapID))
			{
				return;
			}
			base.Sprite = base.Context.SpriteData.GetSprite(this.MapID);
		}

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

		private string _mapID;
	}
}
