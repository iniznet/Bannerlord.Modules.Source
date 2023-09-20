using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000243 RID: 579
	public struct CompassItemUpdateParams
	{
		// Token: 0x06001F79 RID: 8057 RVA: 0x0006FA1B File Offset: 0x0006DC1B
		public CompassItemUpdateParams(object item, TargetIconType targetType, Vec3 worldPosition, uint color, uint color2)
		{
			this = default(CompassItemUpdateParams);
			this.Item = item;
			this.TargetType = targetType;
			this.WorldPosition = worldPosition;
			this.Color = color;
			this.Color2 = color2;
			this.IsAttacker = false;
			this.IsAlly = false;
		}

		// Token: 0x06001F7A RID: 8058 RVA: 0x0006FA57 File Offset: 0x0006DC57
		public CompassItemUpdateParams(object item, TargetIconType targetType, Vec3 worldPosition, BannerCode bannerCode, bool isAttacker, bool isAlly)
		{
			this = default(CompassItemUpdateParams);
			this.Item = item;
			this.TargetType = targetType;
			this.WorldPosition = worldPosition;
			this.BannerCode = bannerCode;
			this.IsAttacker = isAttacker;
			this.IsAlly = isAlly;
		}

		// Token: 0x04000B8E RID: 2958
		public readonly object Item;

		// Token: 0x04000B8F RID: 2959
		public readonly TargetIconType TargetType;

		// Token: 0x04000B90 RID: 2960
		public readonly Vec3 WorldPosition;

		// Token: 0x04000B91 RID: 2961
		public readonly uint Color;

		// Token: 0x04000B92 RID: 2962
		public readonly uint Color2;

		// Token: 0x04000B93 RID: 2963
		public readonly BannerCode BannerCode;

		// Token: 0x04000B94 RID: 2964
		public readonly bool IsAttacker;

		// Token: 0x04000B95 RID: 2965
		public readonly bool IsAlly;
	}
}
