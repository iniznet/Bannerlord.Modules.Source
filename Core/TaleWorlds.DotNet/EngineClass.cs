using System;

namespace TaleWorlds.DotNet
{
	public class EngineClass : Attribute
	{
		public string EngineType { get; set; }

		public EngineClass(string engineType)
		{
			this.EngineType = engineType;
		}
	}
}
