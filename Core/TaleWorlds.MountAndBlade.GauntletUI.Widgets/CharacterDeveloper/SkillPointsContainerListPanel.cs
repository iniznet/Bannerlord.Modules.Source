using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.CharacterDeveloper
{
	// Token: 0x02000164 RID: 356
	public class SkillPointsContainerListPanel : ListPanel
	{
		// Token: 0x06001256 RID: 4694 RVA: 0x00032B1B File Offset: 0x00030D1B
		public SkillPointsContainerListPanel(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06001257 RID: 4695 RVA: 0x00032B24 File Offset: 0x00030D24
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			for (int i = 0; i < base.ChildCount; i++)
			{
				if (!this._initialized)
				{
					base.GetChild(i).RegisterBrushStatesOfWidget();
				}
				bool flag = this.CurrentFocusLevel >= i + 1;
				base.GetChild(i).SetState(flag ? "Full" : "Empty");
			}
			this._initialized = true;
		}

		// Token: 0x17000679 RID: 1657
		// (get) Token: 0x06001258 RID: 4696 RVA: 0x00032B8E File Offset: 0x00030D8E
		// (set) Token: 0x06001259 RID: 4697 RVA: 0x00032B96 File Offset: 0x00030D96
		public int CurrentFocusLevel
		{
			get
			{
				return this._currentFocusLevel;
			}
			set
			{
				if (this._currentFocusLevel != value)
				{
					this._currentFocusLevel = value;
					base.OnPropertyChanged(value, "CurrentFocusLevel");
				}
			}
		}

		// Token: 0x04000867 RID: 2151
		private bool _initialized;

		// Token: 0x04000868 RID: 2152
		private int _currentFocusLevel;
	}
}
