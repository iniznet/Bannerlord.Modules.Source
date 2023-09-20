using System;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.GameMenus
{
	public class MenuCallbackArgs
	{
		public MenuContext MenuContext { get; private set; }

		public MapState MapState { get; private set; }

		public MenuCallbackArgs(MenuContext menuContext, TextObject text)
		{
			this.MenuContext = menuContext;
			this.Text = text;
		}

		public MenuCallbackArgs(MapState mapState, TextObject text)
		{
			this.MapState = mapState;
			this.Text = text;
		}

		public MenuCallbackArgs(MapState mapState, TextObject text, float dt)
		{
			this.MapState = mapState;
			this.Text = text;
			this.DeltaTime = dt;
		}

		public float DeltaTime;

		public bool IsEnabled = true;

		public TextObject Text;

		public TextObject Tooltip = TextObject.Empty;

		public GameMenuOption.IssueQuestFlags OptionQuestData;

		public GameMenuOption.LeaveType optionLeaveType;

		public TextObject MenuTitle = TextObject.Empty;
	}
}
