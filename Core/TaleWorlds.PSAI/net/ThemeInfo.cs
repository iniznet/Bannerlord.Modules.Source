using System;

namespace psai.net
{
	public class ThemeInfo
	{
		public override string ToString()
		{
			return string.Concat(new object[] { this.id, ": ", this.name, " [", this.type, "]" });
		}

		public int id;

		public ThemeType type;

		public int[] segmentIds;

		public string name;
	}
}
