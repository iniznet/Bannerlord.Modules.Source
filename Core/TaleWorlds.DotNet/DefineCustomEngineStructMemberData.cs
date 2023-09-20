using System;

namespace TaleWorlds.DotNet
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public class DefineCustomEngineStructMemberData : Attribute
	{
		public Type Type { get; set; }

		public string MemberName { get; set; }

		public string ManagedMemberName { get; set; }

		public DefineCustomEngineStructMemberData(Type type, string memberName, string managedMemberName)
		{
			this.Type = type;
			this.MemberName = memberName;
			this.ManagedMemberName = managedMemberName;
		}
	}
}
