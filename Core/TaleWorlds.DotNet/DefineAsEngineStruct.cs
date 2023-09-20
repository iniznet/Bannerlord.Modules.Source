using System;

namespace TaleWorlds.DotNet
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public class DefineAsEngineStruct : Attribute
	{
		public Type Type { get; set; }

		public string EngineType { get; set; }

		public DefineAsEngineStruct(Type type, string engineType)
		{
			this.Type = type;
			this.EngineType = engineType;
		}
	}
}
