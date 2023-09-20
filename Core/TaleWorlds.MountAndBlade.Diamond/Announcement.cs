using System;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class Announcement
	{
		public int Id { get; private set; }

		public Guid BattleId { get; private set; }

		public AnnouncementType Type { get; private set; }

		public TextObject Text { get; private set; }

		public bool IsEnabled { get; private set; }

		public Announcement(int id, Guid battleId, AnnouncementType type, TextObject text, bool isEnabled)
		{
			this.Id = id;
			this.BattleId = battleId;
			this.Type = type;
			this.Text = text;
			this.IsEnabled = isEnabled;
		}
	}
}
