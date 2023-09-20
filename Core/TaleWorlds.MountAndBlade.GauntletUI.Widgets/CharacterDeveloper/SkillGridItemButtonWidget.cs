using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.CharacterDeveloper
{
	public class SkillGridItemButtonWidget : ButtonWidget
	{
		public Brush CannotLearnBrush { get; set; }

		public Brush CanLearnBrush { get; set; }

		public SkillGridItemButtonWidget(UIContext context)
			: base(context)
		{
		}

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

		private bool _isVisualsDirty = true;

		private Widget _focusLevelWidget;

		private int _currentFocusLevel;

		private bool _canLearnSkill;
	}
}
