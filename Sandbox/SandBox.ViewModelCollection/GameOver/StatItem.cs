using System;

namespace SandBox.ViewModelCollection.GameOver
{
	public class StatItem
	{
		public StatItem(string id, string value, StatItem.StatType type = StatItem.StatType.None)
		{
			this.ID = id;
			this.Value = value;
			this.Type = type;
		}

		public readonly string ID;

		public readonly string Value;

		public readonly StatItem.StatType Type;

		public enum StatType
		{
			None,
			Influence,
			Issue,
			Tournament,
			Gold,
			Crime,
			Kill
		}
	}
}
