using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.BoardGame
{
	// Token: 0x0200016C RID: 364
	public class BoardGameInstructionVisualWidget : Widget
	{
		// Token: 0x060012A8 RID: 4776 RVA: 0x0003381C File Offset: 0x00031A1C
		public BoardGameInstructionVisualWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060012A9 RID: 4777 RVA: 0x00033828 File Offset: 0x00031A28
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

		// Token: 0x17000696 RID: 1686
		// (get) Token: 0x060012AA RID: 4778 RVA: 0x000338C1 File Offset: 0x00031AC1
		// (set) Token: 0x060012AB RID: 4779 RVA: 0x000338C9 File Offset: 0x00031AC9
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

		// Token: 0x0400088B RID: 2187
		private const float ScaleCoeff = 0.5f;

		// Token: 0x0400088C RID: 2188
		private string _gameType;
	}
}
