using System;

namespace TaleWorlds.DotNet
{
	public class EngineStruct : Attribute
	{
		public string EngineType { get; set; }

		public string AlternateDotNetType { get; set; }

		public EngineStruct(string engineType)
		{
			this.EngineType = engineType;
			this.AlternateDotNetType = null;
		}

		public EngineStruct(string engineType, string alternateDotNetType)
		{
			this.EngineType = engineType;
			this.AlternateDotNetType = alternateDotNetType;
		}
	}
}
