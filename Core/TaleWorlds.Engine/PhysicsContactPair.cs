using System;
using TaleWorlds.DotNet;

namespace TaleWorlds.Engine
{
	[EngineStruct("rglPhysics_contact_pair")]
	public struct PhysicsContactPair
	{
		public PhysicsContactInfo this[int index]
		{
			get
			{
				switch (index)
				{
				case 0:
					return this.Contact0;
				case 1:
					return this.Contact1;
				case 2:
					return this.Contact2;
				case 3:
					return this.Contact3;
				case 4:
					return this.Contact4;
				case 5:
					return this.Contact5;
				case 6:
					return this.Contact6;
				case 7:
					return this.Contact7;
				default:
					return default(PhysicsContactInfo);
				}
			}
		}

		public readonly PhysicsContactInfo Contact0;

		public readonly PhysicsContactInfo Contact1;

		public readonly PhysicsContactInfo Contact2;

		public readonly PhysicsContactInfo Contact3;

		public readonly PhysicsContactInfo Contact4;

		public readonly PhysicsContactInfo Contact5;

		public readonly PhysicsContactInfo Contact6;

		public readonly PhysicsContactInfo Contact7;

		public readonly PhysicsEventType ContactEventType;

		public readonly int NumberOfContacts;
	}
}
