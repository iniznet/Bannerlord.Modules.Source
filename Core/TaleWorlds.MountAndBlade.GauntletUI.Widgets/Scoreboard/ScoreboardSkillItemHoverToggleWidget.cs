using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Scoreboard
{
	public class ScoreboardSkillItemHoverToggleWidget : HoverToggleWidget
	{
		public ScoreboardGainedSkillsListPanel SkillsShowWidget { get; set; }

		public ListPanel GainedSkillsList { get; set; }

		public ScoreboardSkillItemHoverToggleWidget(UIContext context)
			: base(context)
		{
		}

		public List<Widget> GetAllSkillWidgets()
		{
			return this.GainedSkillsList.Children;
		}

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

		private bool _isHoverEndHandled;

		private bool _isHoverBeginHandled;
	}
}
