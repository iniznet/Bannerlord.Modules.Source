using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.ViewModelCollection.HUD.Compass;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.ClassLoadout
{
	public class MPTeammateCompassTargetVM : CompassTargetVM
	{
		public MPTeammateCompassTargetVM(TargetIconType iconType, uint color, uint color2, BannerCode bannercode, bool isAlly)
			: base(iconType, color, color2, bannercode, false, isAlly)
		{
			base.IconType = iconType.ToString();
			base.IsFlag = false;
			base.Banner = ((bannercode != null) ? new ImageIdentifierVM(bannercode, false) : new ImageIdentifierVM(ImageIdentifierType.Null));
		}

		public void RefreshTargetIconType(TargetIconType targetIconType)
		{
			base.IconType = targetIconType.ToString();
		}

		public void RefreshTeam(BannerCode bannerCode, bool isAlly)
		{
			base.Banner = ((bannerCode != null) ? new ImageIdentifierVM(bannerCode, false) : new ImageIdentifierVM(ImageIdentifierType.Null));
			base.IsEnemy = !isAlly;
		}
	}
}
