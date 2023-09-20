using System;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade.View.MissionViews.SiegeWeapon
{
	[DefaultView]
	public class RangedSiegeWeaponViewController : MissionView
	{
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
