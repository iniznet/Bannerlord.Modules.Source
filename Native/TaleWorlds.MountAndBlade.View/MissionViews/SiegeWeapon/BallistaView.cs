using System;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade.View.MissionViews.SiegeWeapon
{
	// Token: 0x0200007A RID: 122
	public class BallistaView : RangedSiegeWeaponView
	{
		// Token: 0x0600048E RID: 1166 RVA: 0x000236FF File Offset: 0x000218FF
		protected override void OnAdded(Scene scene)
		{
			base.OnAdded(scene);
			this.UsesMouseForAiming = true;
		}

		// Token: 0x0600048F RID: 1167 RVA: 0x0002370F File Offset: 0x0002190F
		protected override void StartUsingWeaponCamera()
		{
			base.StartUsingWeaponCamera();
			base.MissionScreen.SetExtraCameraParameters(true, 1.5f);
		}

		// Token: 0x06000490 RID: 1168 RVA: 0x00023728 File Offset: 0x00021928
		protected override void HandleUserCameraRotation(float dt)
		{
		}
	}
}
