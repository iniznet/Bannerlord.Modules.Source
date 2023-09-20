using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.CharacterDeveloper
{
	public class SkillPointsContainerListPanel : ListPanel
	{
		public SkillPointsContainerListPanel(UIContext context)
			: base(context)
		{
		}

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

		private bool _initialized;

		private int _currentFocusLevel;
	}
}
