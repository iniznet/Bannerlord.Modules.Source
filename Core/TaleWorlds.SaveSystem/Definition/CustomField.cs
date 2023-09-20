using System;

namespace TaleWorlds.SaveSystem.Definition
{
	public class CustomField
	{
		public string Name { get; private set; }

		public short SaveId { get; private set; }

		public CustomField(string name, short saveId)
		{
			this.Name = name;
			this.SaveId = saveId;
		}
	}
}
