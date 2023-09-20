using System;
using TaleWorlds.DotNet;

namespace TaleWorlds.Engine
{
	// Token: 0x02000075 RID: 117
	[EngineStruct("rglPhysics_contact_pair")]
	public struct PhysicsContactPair
	{
		// Token: 0x17000067 RID: 103
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

		// Token: 0x0400015C RID: 348
		public readonly PhysicsContactInfo Contact0;

		// Token: 0x0400015D RID: 349
		public readonly PhysicsContactInfo Contact1;

		// Token: 0x0400015E RID: 350
		public readonly PhysicsContactInfo Contact2;

		// Token: 0x0400015F RID: 351
		public readonly PhysicsContactInfo Contact3;

		// Token: 0x04000160 RID: 352
		public readonly PhysicsContactInfo Contact4;

		// Token: 0x04000161 RID: 353
		public readonly PhysicsContactInfo Contact5;

		// Token: 0x04000162 RID: 354
		public readonly PhysicsContactInfo Contact6;

		// Token: 0x04000163 RID: 355
		public readonly PhysicsContactInfo Contact7;

		// Token: 0x04000164 RID: 356
		public readonly PhysicsEventType ContactEventType;

		// Token: 0x04000165 RID: 357
		public readonly int NumberOfContacts;
	}
}
