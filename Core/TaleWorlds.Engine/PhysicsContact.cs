using System;
using TaleWorlds.DotNet;

namespace TaleWorlds.Engine
{
	[EngineStruct("rglPhysics_contact", false)]
	public struct PhysicsContact
	{
		public PhysicsContactPair this[int index]
		{
			get
			{
				switch (index)
				{
				case 0:
					return this.ContactPair0;
				case 1:
					return this.ContactPair1;
				case 2:
					return this.ContactPair2;
				case 3:
					return this.ContactPair3;
				case 4:
					return this.ContactPair4;
				case 5:
					return this.ContactPair5;
				case 6:
					return this.ContactPair6;
				case 7:
					return this.ContactPair7;
				case 8:
					return this.ContactPair8;
				case 9:
					return this.ContactPair9;
				case 10:
					return this.ContactPair10;
				case 11:
					return this.ContactPair11;
				case 12:
					return this.ContactPair12;
				case 13:
					return this.ContactPair13;
				case 14:
					return this.ContactPair14;
				case 15:
					return this.ContactPair15;
				default:
					return default(PhysicsContactPair);
				}
			}
		}

		[CustomEngineStructMemberData("ignoredMember", true)]
		public readonly PhysicsContactPair ContactPair0;

		[CustomEngineStructMemberData("ignoredMember", true)]
		public readonly PhysicsContactPair ContactPair1;

		[CustomEngineStructMemberData("ignoredMember", true)]
		public readonly PhysicsContactPair ContactPair2;

		[CustomEngineStructMemberData("ignoredMember", true)]
		public readonly PhysicsContactPair ContactPair3;

		[CustomEngineStructMemberData("ignoredMember", true)]
		public readonly PhysicsContactPair ContactPair4;

		[CustomEngineStructMemberData("ignoredMember", true)]
		public readonly PhysicsContactPair ContactPair5;

		[CustomEngineStructMemberData("ignoredMember", true)]
		public readonly PhysicsContactPair ContactPair6;

		[CustomEngineStructMemberData("ignoredMember", true)]
		public readonly PhysicsContactPair ContactPair7;

		[CustomEngineStructMemberData("ignoredMember", true)]
		public readonly PhysicsContactPair ContactPair8;

		[CustomEngineStructMemberData("ignoredMember", true)]
		public readonly PhysicsContactPair ContactPair9;

		[CustomEngineStructMemberData("ignoredMember", true)]
		public readonly PhysicsContactPair ContactPair10;

		[CustomEngineStructMemberData("ignoredMember", true)]
		public readonly PhysicsContactPair ContactPair11;

		[CustomEngineStructMemberData("ignoredMember", true)]
		public readonly PhysicsContactPair ContactPair12;

		[CustomEngineStructMemberData("ignoredMember", true)]
		public readonly PhysicsContactPair ContactPair13;

		[CustomEngineStructMemberData("ignoredMember", true)]
		public readonly PhysicsContactPair ContactPair14;

		[CustomEngineStructMemberData("ignoredMember", true)]
		public readonly PhysicsContactPair ContactPair15;

		public readonly IntPtr body0;

		public readonly IntPtr body1;

		public readonly int NumberOfContactPairs;
	}
}
