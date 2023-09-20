using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby.Armory
{
	public class TauntVisualBrushWidget : BrushWidget
	{
		public TauntVisualBrushWidget(UIContext context)
			: base(context)
		{
		}

		private void UpdateTauntVisual()
		{
			Brush tauntIconsBrush = this.TauntIconsBrush;
			Sprite sprite;
			if (tauntIconsBrush == null)
			{
				sprite = null;
			}
			else
			{
				BrushLayer layer = tauntIconsBrush.GetLayer(this.TauntID);
				sprite = ((layer != null) ? layer.Sprite : null);
			}
			Sprite sprite2 = sprite;
			if (base.Brush != null)
			{
				base.Brush.Sprite = sprite2;
				foreach (BrushLayer brushLayer in base.Brush.Layers)
				{
					brushLayer.Sprite = sprite2;
				}
			}
		}

		public Brush TauntIconsBrush
		{
			get
			{
				return this._tauntIconsBrush;
			}
			set
			{
				if (value != this._tauntIconsBrush)
				{
					this._tauntIconsBrush = value;
					base.OnPropertyChanged<Brush>(value, "TauntIconsBrush");
				}
			}
		}

		public string TauntID
		{
			get
			{
				return this._tauntId;
			}
			set
			{
				if (value != this._tauntId)
				{
					this._tauntId = value;
					base.OnPropertyChanged<string>(value, "TauntID");
					this.UpdateTauntVisual();
				}
			}
		}

		public bool IsSelected
		{
			get
			{
				return this._isSelected;
			}
			set
			{
				if (value != this._isSelected)
				{
					this._isSelected = value;
					base.OnPropertyChanged(value, "IsSelected");
					this.SetState(value ? "Selected" : "Default");
				}
			}
		}

		private Brush _tauntIconsBrush;

		private string _tauntId;

		private bool _isSelected;
	}
}
