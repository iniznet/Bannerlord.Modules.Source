using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby
{
	public class MultiplayerLobbyGameTypeCardButtonWidget : ButtonWidget
	{
		public MultiplayerLobbyGameTypeCardButtonWidget(UIContext context)
			: base(context)
		{
		}

		protected override void RefreshState()
		{
			base.RefreshState();
			if (!base.OverrideDefaultStateSwitchingEnabled)
			{
				if (base.IsDisabled)
				{
					this.SetState(base.IsSelected ? "SelectedDisabled" : "Disabled");
				}
				else if (base.IsSelected)
				{
					this.SetState("Selected");
				}
				else if (base.IsPressed)
				{
					this.SetState("Pressed");
				}
				else if (base.IsHovered)
				{
					this.SetState("Hovered");
				}
				else
				{
					this.SetState("Default");
				}
			}
			if (base.UpdateChildrenStates)
			{
				for (int i = 0; i < base.ChildCount; i++)
				{
					base.GetChild(i).SetState(base.CurrentState);
				}
			}
		}

		private void UpdateGameTypeImage()
		{
			if (this.GameTypeImageWidget == null || string.IsNullOrEmpty(this.GameTypeId))
			{
				return;
			}
			Sprite sprite = base.Context.SpriteData.GetSprite("MPLobby\\Matchmaking\\GameTypeCards\\" + this.GameTypeId);
			foreach (Style style in this.GameTypeImageWidget.Brush.Styles)
			{
				foreach (StyleLayer styleLayer in style.Layers)
				{
					styleLayer.Sprite = sprite;
				}
			}
		}

		[Editor(false)]
		public string GameTypeId
		{
			get
			{
				return this._gameTypeId;
			}
			set
			{
				if (this._gameTypeId != value)
				{
					this._gameTypeId = value;
					base.OnPropertyChanged<string>(value, "GameTypeId");
					this.UpdateGameTypeImage();
				}
			}
		}

		[Editor(false)]
		public BrushWidget GameTypeImageWidget
		{
			get
			{
				return this._gameTypeImageWidget;
			}
			set
			{
				if (this._gameTypeImageWidget != value)
				{
					this._gameTypeImageWidget = value;
					base.OnPropertyChanged<BrushWidget>(value, "GameTypeImageWidget");
					this.UpdateGameTypeImage();
				}
			}
		}

		[Editor(false)]
		public Widget CheckboxWidget
		{
			get
			{
				return this._checkboxWidget;
			}
			set
			{
				if (this._checkboxWidget != value)
				{
					this._checkboxWidget = value;
					base.OnPropertyChanged<Widget>(value, "CheckboxWidget");
				}
			}
		}

		private string _gameTypeId;

		private BrushWidget _gameTypeImageWidget;

		private Widget _checkboxWidget;
	}
}
