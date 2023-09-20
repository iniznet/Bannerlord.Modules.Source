using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby.Armory
{
	public class TauntEquipmentUsageIconBrushWidget : BrushWidget
	{
		public TauntEquipmentUsageIconBrushWidget(UIContext context)
			: base(context)
		{
		}

		private void OnUsageNameUpdated(string usageName)
		{
			Sprite sprite;
			if (usageName == null)
			{
				sprite = null;
			}
			else
			{
				Brush iconsBrush = this.IconsBrush;
				if (iconsBrush == null)
				{
					sprite = null;
				}
				else
				{
					BrushLayer layer = iconsBrush.GetLayer(usageName);
					sprite = ((layer != null) ? layer.Sprite : null);
				}
			}
			Sprite sprite2 = sprite;
			base.IsVisible = sprite2 != null;
			if (base.IsVisible)
			{
				base.Brush.Sprite = sprite2;
				base.Brush.DefaultLayer.Sprite = sprite2;
			}
		}

		public Brush IconsBrush
		{
			get
			{
				return this._iconsBrush;
			}
			set
			{
				if (value != this._iconsBrush)
				{
					this._iconsBrush = value;
					base.OnPropertyChanged<Brush>(value, "IconsBrush");
					this.OnUsageNameUpdated(this.UsageName);
				}
			}
		}

		public string UsageName
		{
			get
			{
				return this._usageName;
			}
			set
			{
				if (value != this._usageName)
				{
					this._usageName = value;
					base.OnPropertyChanged<string>(value, "UsageName");
					this.OnUsageNameUpdated(value);
				}
			}
		}

		private Brush _iconsBrush;

		private string _usageName;
	}
}
