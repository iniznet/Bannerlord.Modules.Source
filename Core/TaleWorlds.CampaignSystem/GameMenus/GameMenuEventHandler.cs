using System;

namespace TaleWorlds.CampaignSystem.GameMenus
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class GameMenuEventHandler : Attribute
	{
		public string MenuId { get; private set; }

		public string MenuOptionId { get; private set; }

		public GameMenuEventHandler.EventType Type { get; private set; }

		public GameMenuEventHandler(string menuId, string menuOptionId, GameMenuEventHandler.EventType type)
		{
			this.MenuId = menuId;
			this.MenuOptionId = menuOptionId;
			this.Type = type;
		}

		public enum EventType
		{
			OnCondition,
			OnConsequence
		}
	}
}
