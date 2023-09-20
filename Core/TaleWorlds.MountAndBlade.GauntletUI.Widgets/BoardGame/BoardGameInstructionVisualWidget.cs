using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.BoardGame
{
	public class BoardGameInstructionVisualWidget : Widget
	{
		public BoardGameInstructionVisualWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			if (base.Sprite == null)
			{
				int siblingIndex = base.ParentWidget.ParentWidget.GetSiblingIndex();
				if (!string.IsNullOrEmpty(this.GameType))
				{
					base.Sprite = base.Context.SpriteData.GetSprite(this.GameType + siblingIndex);
				}
			}
			if (base.Sprite != null)
			{
				base.SuggestedWidth = (float)base.Sprite.Width * 0.5f;
				base.SuggestedHeight = (float)base.Sprite.Height * 0.5f;
			}
		}

		[Editor(false)]
		public string GameType
		{
			get
			{
				return this._gameType;
			}
			set
			{
				if (this._gameType != value)
				{
					this._gameType = value;
				}
			}
		}

		private const float ScaleCoeff = 0.5f;

		private string _gameType;
	}
}
