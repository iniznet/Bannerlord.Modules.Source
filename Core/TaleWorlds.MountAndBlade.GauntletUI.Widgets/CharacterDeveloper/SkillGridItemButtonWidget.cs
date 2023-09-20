using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.CharacterDeveloper
{
	// Token: 0x02000163 RID: 355
	public class SkillGridItemButtonWidget : ButtonWidget
	{
		// Token: 0x17000674 RID: 1652
		// (get) Token: 0x0600124A RID: 4682 RVA: 0x00032A10 File Offset: 0x00030C10
		// (set) Token: 0x0600124B RID: 4683 RVA: 0x00032A18 File Offset: 0x00030C18
		public Brush CannotLearnBrush { get; set; }

		// Token: 0x17000675 RID: 1653
		// (get) Token: 0x0600124C RID: 4684 RVA: 0x00032A21 File Offset: 0x00030C21
		// (set) Token: 0x0600124D RID: 4685 RVA: 0x00032A29 File Offset: 0x00030C29
		public Brush CanLearnBrush { get; set; }

		// Token: 0x0600124E RID: 4686 RVA: 0x00032A32 File Offset: 0x00030C32
		public SkillGridItemButtonWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x0600124F RID: 4687 RVA: 0x00032A44 File Offset: 0x00030C44
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			Widget focusLevelWidget = this.FocusLevelWidget;
			if (focusLevelWidget != null)
			{
				focusLevelWidget.SetState(this.CurrentFocusLevel.ToString());
			}
			if (this._isVisualsDirty)
			{
				base.Brush = (this.CanLearnSkill ? this.CanLearnBrush : this.CannotLearnBrush);
				this._isVisualsDirty = false;
			}
		}

		// Token: 0x17000676 RID: 1654
		// (get) Token: 0x06001250 RID: 4688 RVA: 0x00032AA2 File Offset: 0x00030CA2
		// (set) Token: 0x06001251 RID: 4689 RVA: 0x00032AAA File Offset: 0x00030CAA
		public Widget FocusLevelWidget
		{
			get
			{
				return this._focusLevelWidget;
			}
			set
			{
				if (this._focusLevelWidget != value)
				{
					this._focusLevelWidget = value;
					base.OnPropertyChanged<Widget>(value, "FocusLevelWidget");
				}
			}
		}

		// Token: 0x17000677 RID: 1655
		// (get) Token: 0x06001252 RID: 4690 RVA: 0x00032AC8 File Offset: 0x00030CC8
		// (set) Token: 0x06001253 RID: 4691 RVA: 0x00032AD0 File Offset: 0x00030CD0
		public bool CanLearnSkill
		{
			get
			{
				return this._canLearnSkill;
			}
			set
			{
				if (this._canLearnSkill != value)
				{
					this._canLearnSkill = value;
					base.OnPropertyChanged(value, "CanLearnSkill");
					this._isVisualsDirty = true;
				}
			}
		}

		// Token: 0x17000678 RID: 1656
		// (get) Token: 0x06001254 RID: 4692 RVA: 0x00032AF5 File Offset: 0x00030CF5
		// (set) Token: 0x06001255 RID: 4693 RVA: 0x00032AFD File Offset: 0x00030CFD
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

		// Token: 0x04000863 RID: 2147
		private bool _isVisualsDirty = true;

		// Token: 0x04000864 RID: 2148
		private Widget _focusLevelWidget;

		// Token: 0x04000865 RID: 2149
		private int _currentFocusLevel;

		// Token: 0x04000866 RID: 2150
		private bool _canLearnSkill;
	}
}
