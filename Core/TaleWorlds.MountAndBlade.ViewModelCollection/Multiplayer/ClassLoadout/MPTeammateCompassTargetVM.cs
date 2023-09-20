using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.ViewModelCollection.HUD.Compass;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.ClassLoadout
{
	// Token: 0x020000CD RID: 205
	public class MPTeammateCompassTargetVM : CompassTargetVM
	{
		// Token: 0x06001329 RID: 4905 RVA: 0x0003EB54 File Offset: 0x0003CD54
		public MPTeammateCompassTargetVM(TargetIconType iconType, uint color, uint color2, BannerCode bannercode, bool isAlly)
			: base(iconType, color, color2, bannercode, false, isAlly)
		{
			base.IconType = iconType.ToString();
			base.IsFlag = false;
			base.Banner = ((bannercode != null) ? new ImageIdentifierVM(bannercode, false) : new ImageIdentifierVM(ImageIdentifierType.Null));
		}

		// Token: 0x0600132A RID: 4906 RVA: 0x0003EBA9 File Offset: 0x0003CDA9
		public void RefreshTargetIconType(TargetIconType targetIconType)
		{
			base.IconType = targetIconType.ToString();
		}

		// Token: 0x0600132B RID: 4907 RVA: 0x0003EBBE File Offset: 0x0003CDBE
		public void RefreshTeam(BannerCode bannerCode, bool isAlly)
		{
			base.Banner = ((bannerCode != null) ? new ImageIdentifierVM(bannerCode, false) : new ImageIdentifierVM(ImageIdentifierType.Null));
			base.IsEnemy = !isAlly;
		}
	}
}
