using System;
using System.Collections.Generic;
using System.Diagnostics;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.View.MissionViews
{
	public class MissionFormationTargetSelectionHandler : MissionView
	{
		public event Action<MBReadOnlyList<Formation>> OnFormationFocused;

		private Camera ActiveCamera
		{
			get
			{
				return base.MissionScreen.CustomCamera ?? base.MissionScreen.CombatCamera;
			}
		}

		public MissionFormationTargetSelectionHandler()
		{
			this._distanceCache = new List<ValueTuple<Formation, float>>();
			this._focusedFormationCache = new MBList<Formation>();
		}

		public override void OnPreDisplayMissionTick(float dt)
		{
			base.OnPreDisplayMissionTick(dt);
			this._distanceCache.Clear();
			this._focusedFormationCache.Clear();
			Mission mission = base.Mission;
			if (((mission != null) ? mission.Teams : null) != null)
			{
				Vec3 position = this.ActiveCamera.Position;
				this._centerOfScreen.x = Screen.RealScreenResolutionWidth / 2f;
				this._centerOfScreen.y = Screen.RealScreenResolutionHeight / 2f;
				for (int i = 0; i < base.Mission.Teams.Count; i++)
				{
					Team team = base.Mission.Teams[i];
					if (!team.IsPlayerAlly)
					{
						for (int j = 0; j < team.FormationsIncludingEmpty.Count; j++)
						{
							Formation formation = team.FormationsIncludingEmpty[j];
							if (formation.CountOfUnits > 0)
							{
								float formationDistanceToCenter = this.GetFormationDistanceToCenter(formation, position);
								this._distanceCache.Add(new ValueTuple<Formation, float>(formation, formationDistanceToCenter));
							}
						}
					}
				}
				if (this._distanceCache.Count == 0)
				{
					Action<MBReadOnlyList<Formation>> onFormationFocused = this.OnFormationFocused;
					if (onFormationFocused == null)
					{
						return;
					}
					onFormationFocused(null);
					return;
				}
				else
				{
					Formation formation2 = null;
					float num = this.MaxDistanceToCenterForFocus;
					for (int k = 0; k < this._distanceCache.Count; k++)
					{
						ValueTuple<Formation, float> valueTuple = this._distanceCache[k];
						if (valueTuple.Item2 == 0f)
						{
							this._focusedFormationCache.Add(valueTuple.Item1);
						}
						else if (valueTuple.Item2 < num)
						{
							num = valueTuple.Item2;
							formation2 = valueTuple.Item1;
						}
					}
					if (formation2 != null)
					{
						this._focusedFormationCache.Add(formation2);
					}
					Action<MBReadOnlyList<Formation>> onFormationFocused2 = this.OnFormationFocused;
					if (onFormationFocused2 == null)
					{
						return;
					}
					onFormationFocused2(this._focusedFormationCache);
				}
			}
		}

		private float GetFormationDistanceToCenter(Formation formation, Vec3 cameraPosition)
		{
			WorldPosition medianPosition = formation.QuerySystem.MedianPosition;
			medianPosition.SetVec2(formation.QuerySystem.AveragePosition);
			float num = formation.QuerySystem.AveragePosition.Distance(cameraPosition.AsVec2);
			if (num >= 1000f)
			{
				return 2.1474836E+09f;
			}
			if (num <= 10f)
			{
				return 0f;
			}
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			MBWindowManager.WorldToScreenInsideUsableArea(this.ActiveCamera, medianPosition.GetGroundVec3() + new Vec3(0f, 0f, 3f, -1f), ref num2, ref num3, ref num4);
			if (num4 <= 0f)
			{
				return 2.1474836E+09f;
			}
			return new Vec2(num2, num3).Distance(this._centerOfScreen);
		}

		public override void OnRemoveBehavior()
		{
			this._distanceCache.Clear();
			this._focusedFormationCache.Clear();
			this.OnFormationFocused = null;
			base.OnRemoveBehavior();
		}

		[Conditional("DEBUG")]
		public void TickDebug()
		{
		}

		public const float MaxDistanceForFocusCheck = 1000f;

		public const float MinDistanceForFocusCheck = 10f;

		public readonly float MaxDistanceToCenterForFocus = 70f * (Screen.RealScreenResolutionHeight / 1080f);

		private readonly List<ValueTuple<Formation, float>> _distanceCache;

		private readonly MBList<Formation> _focusedFormationCache;

		private Vec2 _centerOfScreen = new Vec2(Screen.RealScreenResolutionWidth / 2f, Screen.RealScreenResolutionHeight / 2f);
	}
}
