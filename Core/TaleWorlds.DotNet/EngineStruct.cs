using System;

namespace TaleWorlds.DotNet
{
	public class EngineStruct : Attribute
	{
		public string EngineType { get; set; }

		public string AlternateDotNetType { get; set; }

		public bool IgnoreMemberOffsetTest { get; set; }

		public EngineStruct(string engineType, bool ignoreMemberOffsetTest = false)
		{
			this.EngineType = engineType;
			this.AlternateDotNetType = null;
			this.IgnoreMemberOffsetTest = ignoreMemberOffsetTest;
		}

		public EngineStruct(string engineType, string alternateDotNetType, bool ignoreMemberOffsetTest = false)
		{
			this.EngineType = engineType;
			this.AlternateDotNetType = alternateDotNetType;
			this.IgnoreMemberOffsetTest = ignoreMemberOffsetTest;
		}
	}
}
