using System;
using TaleWorlds.Library;

namespace TaleWorlds.ScreenSystem
{
	// Token: 0x02000004 RID: 4
	public class InputRestrictions
	{
		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600000D RID: 13 RVA: 0x000020DF File Offset: 0x000002DF
		// (set) Token: 0x0600000E RID: 14 RVA: 0x000020E7 File Offset: 0x000002E7
		public int Order { get; private set; }

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600000F RID: 15 RVA: 0x000020F0 File Offset: 0x000002F0
		// (set) Token: 0x06000010 RID: 16 RVA: 0x000020F8 File Offset: 0x000002F8
		public Guid Id { get; private set; }

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000011 RID: 17 RVA: 0x00002101 File Offset: 0x00000301
		// (set) Token: 0x06000012 RID: 18 RVA: 0x00002109 File Offset: 0x00000309
		public bool MouseVisibility { get; private set; }

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000013 RID: 19 RVA: 0x00002112 File Offset: 0x00000312
		// (set) Token: 0x06000014 RID: 20 RVA: 0x0000211A File Offset: 0x0000031A
		public bool CanOverrideFocusOnHit { get; private set; }

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000015 RID: 21 RVA: 0x00002123 File Offset: 0x00000323
		// (set) Token: 0x06000016 RID: 22 RVA: 0x0000212B File Offset: 0x0000032B
		public InputUsageMask InputUsageMask { get; private set; }

		// Token: 0x06000017 RID: 23 RVA: 0x00002134 File Offset: 0x00000334
		public InputRestrictions(int order)
		{
			this.Id = default(Guid);
			this.InputUsageMask = InputUsageMask.Invalid;
			this.Order = order;
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00002164 File Offset: 0x00000364
		public void SetMouseVisibility(bool isVisible)
		{
			this.MouseVisibility = isVisible;
		}

		// Token: 0x06000019 RID: 25 RVA: 0x0000216D File Offset: 0x0000036D
		public void SetInputRestrictions(bool isMouseVisible = true, InputUsageMask mask = InputUsageMask.All)
		{
			this.InputUsageMask = mask;
			this.SetMouseVisibility(isMouseVisible);
		}

		// Token: 0x0600001A RID: 26 RVA: 0x0000217D File Offset: 0x0000037D
		public void ResetInputRestrictions()
		{
			this.InputUsageMask = InputUsageMask.Invalid;
			this.SetMouseVisibility(false);
		}

		// Token: 0x0600001B RID: 27 RVA: 0x0000218D File Offset: 0x0000038D
		public void SetCanOverrideFocusOnHit(bool canOverrideFocusOnHit)
		{
			this.CanOverrideFocusOnHit = canOverrideFocusOnHit;
		}
	}
}
