using System;
using System.Collections.Generic;

namespace SandBox.ViewModelCollection.GameOver
{
	public class StatCategory
	{
		public StatCategory(string id, IEnumerable<StatItem> items)
		{
			this.ID = id;
			this.Items = items;
		}

		public readonly IEnumerable<StatItem> Items;

		public readonly string ID;
	}
}
