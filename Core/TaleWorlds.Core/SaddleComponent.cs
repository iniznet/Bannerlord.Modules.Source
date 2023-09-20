using System;

namespace TaleWorlds.Core
{
	// Token: 0x020000B9 RID: 185
	public class SaddleComponent : ItemComponent
	{
		// Token: 0x0600094F RID: 2383 RVA: 0x0001EE1C File Offset: 0x0001D01C
		public SaddleComponent(SaddleComponent saddleComponent)
		{
		}

		// Token: 0x06000950 RID: 2384 RVA: 0x0001EE24 File Offset: 0x0001D024
		public override ItemComponent GetCopy()
		{
			return new SaddleComponent(this);
		}
	}
}
