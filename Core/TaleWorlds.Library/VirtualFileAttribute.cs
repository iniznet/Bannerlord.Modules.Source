using System;

namespace TaleWorlds.Library
{
	public class VirtualFileAttribute : Attribute
	{
		public string Name { get; private set; }

		public string Content { get; private set; }

		public VirtualFileAttribute(string name, string content)
		{
			this.Name = name;
			this.Content = content;
		}
	}
}
