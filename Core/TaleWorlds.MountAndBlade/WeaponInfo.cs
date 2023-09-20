using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000393 RID: 915
	public struct WeaponInfo
	{
		// Token: 0x17000902 RID: 2306
		// (get) Token: 0x0600324A RID: 12874 RVA: 0x000D0ECB File Offset: 0x000CF0CB
		// (set) Token: 0x0600324B RID: 12875 RVA: 0x000D0ED3 File Offset: 0x000CF0D3
		public bool IsValid { get; private set; }

		// Token: 0x17000903 RID: 2307
		// (get) Token: 0x0600324C RID: 12876 RVA: 0x000D0EDC File Offset: 0x000CF0DC
		// (set) Token: 0x0600324D RID: 12877 RVA: 0x000D0EE4 File Offset: 0x000CF0E4
		public bool IsMeleeWeapon { get; private set; }

		// Token: 0x17000904 RID: 2308
		// (get) Token: 0x0600324E RID: 12878 RVA: 0x000D0EED File Offset: 0x000CF0ED
		// (set) Token: 0x0600324F RID: 12879 RVA: 0x000D0EF5 File Offset: 0x000CF0F5
		public bool IsRangedWeapon { get; private set; }

		// Token: 0x06003250 RID: 12880 RVA: 0x000D0EFE File Offset: 0x000CF0FE
		public WeaponInfo(bool isValid, bool isMeleeWeapon, bool isRangedWeapon)
		{
			this.IsValid = isValid;
			this.IsMeleeWeapon = isMeleeWeapon;
			this.IsRangedWeapon = isRangedWeapon;
		}
	}
}
