using System;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade.View.MissionViews.SiegeWeapon
{
	// Token: 0x0200007E RID: 126
	[DefaultView]
	public class RangedSiegeWeaponViewController : MissionView
	{
		// Token: 0x060004A9 RID: 1193 RVA: 0x00023D5C File Offset: 0x00021F5C
		public override void OnObjectUsed(Agent userAgent, UsableMissionObject usedObject)
		{
			base.OnObjectUsed(userAgent, usedObject);
			if (userAgent.IsMainAgent && usedObject is StandingPoint)
			{
				UsableMachine usableMachineFromPoint = this.GetUsableMachineFromPoint(usedObject as StandingPoint);
				if (usableMachineFromPoint is RangedSiegeWeapon)
				{
					RangedSiegeWeapon rangedSiegeWeapon = usableMachineFromPoint as RangedSiegeWeapon;
					if (rangedSiegeWeapon.GetComponent<RangedSiegeWeaponView>() == null)
					{
						this.AddRangedSiegeWeaponView(rangedSiegeWeapon);
					}
				}
			}
		}

		// Token: 0x060004AA RID: 1194 RVA: 0x00023DAC File Offset: 0x00021FAC
		private UsableMachine GetUsableMachineFromPoint(StandingPoint standingPoint)
		{
			GameEntity gameEntity = standingPoint.GameEntity;
			while (gameEntity != null && !gameEntity.HasScriptOfType<UsableMachine>())
			{
				gameEntity = gameEntity.Parent;
			}
			if (gameEntity != null)
			{
				UsableMachine firstScriptOfType = gameEntity.GetFirstScriptOfType<UsableMachine>();
				if (firstScriptOfType != null)
				{
					return firstScriptOfType;
				}
			}
			return null;
		}

		// Token: 0x060004AB RID: 1195 RVA: 0x00023DF0 File Offset: 0x00021FF0
		private void AddRangedSiegeWeaponView(RangedSiegeWeapon rangedSiegeWeapon)
		{
			RangedSiegeWeaponView rangedSiegeWeaponView;
			if (rangedSiegeWeapon is Trebuchet)
			{
				rangedSiegeWeaponView = new TrebuchetView();
			}
			else if (rangedSiegeWeapon is Mangonel)
			{
				rangedSiegeWeaponView = new MangonelView();
			}
			else if (rangedSiegeWeapon is Ballista)
			{
				rangedSiegeWeaponView = new BallistaView();
			}
			else
			{
				rangedSiegeWeaponView = new RangedSiegeWeaponView();
			}
			rangedSiegeWeaponView.Initialize(rangedSiegeWeapon, base.MissionScreen);
			rangedSiegeWeapon.AddComponent(rangedSiegeWeaponView);
		}
	}
}
