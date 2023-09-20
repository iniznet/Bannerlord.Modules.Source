using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public struct CompassItemUpdateParams
	{
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

		public readonly object Item;

		public readonly TargetIconType TargetType;

		public readonly Vec3 WorldPosition;

		public readonly uint Color;

		public readonly uint Color2;

		public readonly BannerCode BannerCode;

		public readonly bool IsAttacker;

		public readonly bool IsAlly;
	}
}
