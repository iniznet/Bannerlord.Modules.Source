using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby
{
	// Token: 0x02000097 RID: 151
	public class MultiplayerLobbyGameTypeCardButtonWidget : ButtonWidget
	{
		// Token: 0x06000817 RID: 2071 RVA: 0x00017BBE File Offset: 0x00015DBE
		public MultiplayerLobbyGameTypeCardButtonWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000818 RID: 2072 RVA: 0x00017BC8 File Offset: 0x00015DC8
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

		// Token: 0x06000819 RID: 2073 RVA: 0x00017C7C File Offset: 0x00015E7C
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

		// Token: 0x170002D7 RID: 727
		// (get) Token: 0x0600081A RID: 2074 RVA: 0x00017D48 File Offset: 0x00015F48
		// (set) Token: 0x0600081B RID: 2075 RVA: 0x00017D50 File Offset: 0x00015F50
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

		// Token: 0x170002D8 RID: 728
		// (get) Token: 0x0600081C RID: 2076 RVA: 0x00017D79 File Offset: 0x00015F79
		// (set) Token: 0x0600081D RID: 2077 RVA: 0x00017D81 File Offset: 0x00015F81
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

		// Token: 0x170002D9 RID: 729
		// (get) Token: 0x0600081E RID: 2078 RVA: 0x00017DA5 File Offset: 0x00015FA5
		// (set) Token: 0x0600081F RID: 2079 RVA: 0x00017DAD File Offset: 0x00015FAD
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

		// Token: 0x040003B7 RID: 951
		private string _gameTypeId;

		// Token: 0x040003B8 RID: 952
		private BrushWidget _gameTypeImageWidget;

		// Token: 0x040003B9 RID: 953
		private Widget _checkboxWidget;
	}
}
