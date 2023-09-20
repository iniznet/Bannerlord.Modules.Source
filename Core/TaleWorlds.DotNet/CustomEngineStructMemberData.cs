using System;

namespace TaleWorlds.DotNet
{
	public class CustomEngineStructMemberData : Attribute
	{
		public string CustomMemberName { get; set; }

		public bool IgnoreMemberOffsetTest { get; set; }

		public bool PublicPrivateModifierFlippedInNative { get; set; }

		public CustomEngineStructMemberData(string customMemberName)
		{
			this.CustomMemberName = customMemberName;
			this.IgnoreMemberOffsetTest = false;
			this.PublicPrivateModifierFlippedInNative = false;
		}

		public CustomEngineStructMemberData(string customMemberName, bool ignoreMemberOffsetTest)
		{
			this.CustomMemberName = customMemberName;
			this.IgnoreMemberOffsetTest = ignoreMemberOffsetTest;
			this.PublicPrivateModifierFlippedInNative = false;
		}

		public CustomEngineStructMemberData(bool publicPrivateModifierFlippedInNative)
		{
			this.CustomMemberName = null;
			this.IgnoreMemberOffsetTest = false;
			this.PublicPrivateModifierFlippedInNative = publicPrivateModifierFlippedInNative;
		}
	}
}
