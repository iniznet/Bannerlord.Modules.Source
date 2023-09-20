using System;
using TaleWorlds.DotNet;

namespace TaleWorlds.Engine
{
	// Token: 0x02000076 RID: 118
	[EngineStruct("rglPhysics_contact")]
	public struct PhysicsContact
	{
		// Token: 0x17000068 RID: 104
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

		// Token: 0x04000166 RID: 358
		public readonly PhysicsContactPair ContactPair0;

		// Token: 0x04000167 RID: 359
		public readonly PhysicsContactPair ContactPair1;

		// Token: 0x04000168 RID: 360
		public readonly PhysicsContactPair ContactPair2;

		// Token: 0x04000169 RID: 361
		public readonly PhysicsContactPair ContactPair3;

		// Token: 0x0400016A RID: 362
		public readonly PhysicsContactPair ContactPair4;

		// Token: 0x0400016B RID: 363
		public readonly PhysicsContactPair ContactPair5;

		// Token: 0x0400016C RID: 364
		public readonly PhysicsContactPair ContactPair6;

		// Token: 0x0400016D RID: 365
		public readonly PhysicsContactPair ContactPair7;

		// Token: 0x0400016E RID: 366
		public readonly PhysicsContactPair ContactPair8;

		// Token: 0x0400016F RID: 367
		public readonly PhysicsContactPair ContactPair9;

		// Token: 0x04000170 RID: 368
		public readonly PhysicsContactPair ContactPair10;

		// Token: 0x04000171 RID: 369
		public readonly PhysicsContactPair ContactPair11;

		// Token: 0x04000172 RID: 370
		public readonly PhysicsContactPair ContactPair12;

		// Token: 0x04000173 RID: 371
		public readonly PhysicsContactPair ContactPair13;

		// Token: 0x04000174 RID: 372
		public readonly PhysicsContactPair ContactPair14;

		// Token: 0x04000175 RID: 373
		public readonly PhysicsContactPair ContactPair15;

		// Token: 0x04000176 RID: 374
		public readonly IntPtr body0;

		// Token: 0x04000177 RID: 375
		public readonly IntPtr body1;

		// Token: 0x04000178 RID: 376
		public readonly int NumberOfContactPairs;
	}
}
