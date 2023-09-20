using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace TaleWorlds.Library
{
	// Token: 0x0200006A RID: 106
	public class NavigationPath : ISerializable
	{
		// Token: 0x17000059 RID: 89
		// (get) Token: 0x0600039C RID: 924 RVA: 0x0000B55E File Offset: 0x0000975E
		// (set) Token: 0x0600039D RID: 925 RVA: 0x0000B566 File Offset: 0x00009766
		public Vec2[] PathPoints { get; private set; }

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x0600039E RID: 926 RVA: 0x0000B56F File Offset: 0x0000976F
		// (set) Token: 0x0600039F RID: 927 RVA: 0x0000B577 File Offset: 0x00009777
		[CachedData]
		public int Size { get; set; }

		// Token: 0x060003A0 RID: 928 RVA: 0x0000B580 File Offset: 0x00009780
		public NavigationPath()
		{
			this.PathPoints = new Vec2[128];
		}

		// Token: 0x060003A1 RID: 929 RVA: 0x0000B598 File Offset: 0x00009798
		protected NavigationPath(SerializationInfo info, StreamingContext context)
		{
			this.PathPoints = new Vec2[128];
			this.Size = info.GetInt32("s");
			for (int i = 0; i < this.Size; i++)
			{
				float single = info.GetSingle("x" + i);
				float single2 = info.GetSingle("y" + i);
				this.PathPoints[i] = new Vec2(single, single2);
			}
		}

		// Token: 0x060003A2 RID: 930 RVA: 0x0000B620 File Offset: 0x00009820
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("s", this.Size);
			for (int i = 0; i < this.Size; i++)
			{
				info.AddValue("x" + i, this.PathPoints[i].x);
				info.AddValue("y" + i, this.PathPoints[i].y);
			}
		}

		// Token: 0x1700005B RID: 91
		public Vec2 this[int i]
		{
			get
			{
				return this.PathPoints[i];
			}
		}

		// Token: 0x04000115 RID: 277
		private const int PathSize = 128;
	}
}
