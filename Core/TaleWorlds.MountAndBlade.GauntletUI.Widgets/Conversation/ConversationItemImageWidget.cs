using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Conversation
{
	// Token: 0x0200014C RID: 332
	public class ConversationItemImageWidget : ImageWidget
	{
		// Token: 0x17000621 RID: 1569
		// (get) Token: 0x06001152 RID: 4434 RVA: 0x0002FE81 File Offset: 0x0002E081
		// (set) Token: 0x06001153 RID: 4435 RVA: 0x0002FE89 File Offset: 0x0002E089
		public Brush NormalBrush { get; set; }

		// Token: 0x17000622 RID: 1570
		// (get) Token: 0x06001154 RID: 4436 RVA: 0x0002FE92 File Offset: 0x0002E092
		// (set) Token: 0x06001155 RID: 4437 RVA: 0x0002FE9A File Offset: 0x0002E09A
		public Brush SpecialBrush { get; set; }

		// Token: 0x17000623 RID: 1571
		// (get) Token: 0x06001156 RID: 4438 RVA: 0x0002FEA3 File Offset: 0x0002E0A3
		// (set) Token: 0x06001157 RID: 4439 RVA: 0x0002FEAB File Offset: 0x0002E0AB
		public bool IsSpecial { get; set; }

		// Token: 0x06001158 RID: 4440 RVA: 0x0002FEB4 File Offset: 0x0002E0B4
		public ConversationItemImageWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06001159 RID: 4441 RVA: 0x0002FEBD File Offset: 0x0002E0BD
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this._isInitialized)
			{
				base.Brush = (this.IsSpecial ? this.SpecialBrush : this.NormalBrush);
				this._isInitialized = true;
			}
		}

		// Token: 0x040007F3 RID: 2035
		private bool _isInitialized;
	}
}
