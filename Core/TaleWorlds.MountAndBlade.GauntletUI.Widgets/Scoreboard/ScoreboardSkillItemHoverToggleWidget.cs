using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Scoreboard
{
	// Token: 0x0200004D RID: 77
	public class ScoreboardSkillItemHoverToggleWidget : HoverToggleWidget
	{
		// Token: 0x17000176 RID: 374
		// (get) Token: 0x06000427 RID: 1063 RVA: 0x0000D5F2 File Offset: 0x0000B7F2
		// (set) Token: 0x06000428 RID: 1064 RVA: 0x0000D5FA File Offset: 0x0000B7FA
		public ScoreboardGainedSkillsListPanel SkillsShowWidget { get; set; }

		// Token: 0x17000177 RID: 375
		// (get) Token: 0x06000429 RID: 1065 RVA: 0x0000D603 File Offset: 0x0000B803
		// (set) Token: 0x0600042A RID: 1066 RVA: 0x0000D60B File Offset: 0x0000B80B
		public ListPanel GainedSkillsList { get; set; }

		// Token: 0x0600042B RID: 1067 RVA: 0x0000D614 File Offset: 0x0000B814
		public ScoreboardSkillItemHoverToggleWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x0600042C RID: 1068 RVA: 0x0000D61D File Offset: 0x0000B81D
		public List<Widget> GetAllSkillWidgets()
		{
			return this.GainedSkillsList.Children;
		}

		// Token: 0x0600042D RID: 1069 RVA: 0x0000D62C File Offset: 0x0000B82C
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (base.IsOverWidget && !this._isHoverBeginHandled)
			{
				this.SkillsShowWidget.SetCurrentUnit(this);
				this._isHoverBeginHandled = true;
				this._isHoverEndHandled = true;
				return;
			}
			if (!base.IsOverWidget && this._isHoverEndHandled)
			{
				this.SkillsShowWidget.SetCurrentUnit(null);
				this._isHoverEndHandled = false;
				this._isHoverBeginHandled = false;
			}
		}

		// Token: 0x040001D1 RID: 465
		private bool _isHoverEndHandled;

		// Token: 0x040001D2 RID: 466
		private bool _isHoverBeginHandled;
	}
}
