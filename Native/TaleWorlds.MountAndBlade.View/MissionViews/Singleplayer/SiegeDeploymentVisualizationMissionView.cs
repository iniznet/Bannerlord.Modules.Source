using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.View.MissionViews.Singleplayer
{
	public class SiegeDeploymentVisualizationMissionView : MissionView
	{
		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			this._deploymentMissionView = base.Mission.GetMissionBehavior<DeploymentMissionView>();
			DeploymentMissionView deploymentMissionView = this._deploymentMissionView;
			deploymentMissionView.OnDeploymentFinish = (OnPlayerDeploymentFinishDelegate)Delegate.Combine(deploymentMissionView.OnDeploymentFinish, new OnPlayerDeploymentFinishDelegate(this.OnDeploymentFinish));
		}

		public override void AfterStart()
		{
			base.AfterStart();
			this._deploymentPoints = (from dp in MBExtensions.FindAllWithType<DeploymentPoint>(Mission.Current.ActiveMissionObjects)
				where !dp.IsDisabled
				select dp).ToList<DeploymentPoint>();
			foreach (DeploymentPoint deploymentPoint in this._deploymentPoints)
			{
				deploymentPoint.OnDeploymentPointTypeDetermined += this.OnDeploymentPointStateSet;
				deploymentPoint.OnDeploymentStateChanged += this.OnDeploymentStateChanged;
			}
			this._deploymentPointsVisible = true;
			Mission.Current.GetMissionBehavior<SiegeDeploymentMissionController>();
		}

		public void OnDeploymentFinish()
		{
			DeploymentMissionView deploymentMissionView = this._deploymentMissionView;
			deploymentMissionView.OnDeploymentFinish = (OnPlayerDeploymentFinishDelegate)Delegate.Remove(deploymentMissionView.OnDeploymentFinish, new OnPlayerDeploymentFinishDelegate(this.OnDeploymentFinish));
			this._deploymentMissionView = null;
			this.TryRemoveDeploymentVisibilities();
			Mission.Current.RemoveMissionBehavior(this);
		}

		protected override void OnEndMission()
		{
			base.OnEndMission();
			this.TryRemoveDeploymentVisibilities();
		}

		public override void OnRemoveBehavior()
		{
			base.OnRemoveBehavior();
		}

		private void TryRemoveDeploymentVisibilities()
		{
			if (this._deploymentPointsVisible)
			{
				foreach (DeploymentPoint deploymentPoint in this._deploymentPoints)
				{
					this.RemoveDeploymentVisibility(deploymentPoint);
					deploymentPoint.OnDeploymentStateChanged -= this.OnDeploymentStateChanged;
				}
				this._deploymentPointsVisible = false;
			}
		}

		private void RemoveDeploymentVisibility(DeploymentPoint deploymentPoint)
		{
			switch (deploymentPoint.GetDeploymentPointType())
			{
			case 0:
				this.HideDeploymentBanners(deploymentPoint, true);
				this.SetGhostVisibility(deploymentPoint, false);
				return;
			case 1:
				this.HideDeploymentBanners(deploymentPoint, true);
				this.SetGhostVisibility(deploymentPoint, false);
				this.SetDeploymentTargetContourState(deploymentPoint, false);
				this.SetLaddersUpState(deploymentPoint, false);
				this.SetLightState(deploymentPoint, false);
				return;
			case 2:
				this.HideDeploymentBanners(deploymentPoint, true);
				this.SetDeploymentTargetContourState(deploymentPoint, false);
				this.SetLightState(deploymentPoint, false);
				return;
			case 3:
				this.HideTrajectories(deploymentPoint);
				return;
			default:
				return;
			}
		}

		private static string GetSelectorStateDescription()
		{
			string text = "";
			for (int i = 1; i < 1023; i *= 2)
			{
				if ((SiegeDeploymentVisualizationMissionView.deploymentVisualizerSelector & i) > 0)
				{
					string text2 = text;
					string text3 = " ";
					SiegeDeploymentVisualizationMissionView.DeploymentVisualizationPreference deploymentVisualizationPreference = (SiegeDeploymentVisualizationMissionView.DeploymentVisualizationPreference)i;
					text = text2 + text3 + deploymentVisualizationPreference.ToString();
				}
			}
			return text;
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("set_deployment_visualization_selector", "mission")]
		public static string SetDeploymentVisualizationSelector(List<string> strings)
		{
			if (strings.Count == 1 && int.TryParse(strings[0], out SiegeDeploymentVisualizationMissionView.deploymentVisualizerSelector))
			{
				return "Enabled deployment visualization options are:" + SiegeDeploymentVisualizationMissionView.GetSelectorStateDescription();
			}
			return "Format is \"mission.set_deployment_visualization_selector [integer > 0]\".";
		}

		private void OnDeploymentStateChanged(DeploymentPoint deploymentPoint, SynchedMissionObject targetObject)
		{
			this.OnDeploymentPointStateSet(deploymentPoint);
		}

		private void OnDeploymentPointStateSet(DeploymentPoint deploymentPoint)
		{
			switch (deploymentPoint.GetDeploymentPointState())
			{
			case 0:
				if ((SiegeDeploymentVisualizationMissionView.deploymentVisualizerSelector & 1) > 0)
				{
					if (deploymentPoint.GetDeploymentPointType() == null)
					{
						if ((SiegeDeploymentVisualizationMissionView.deploymentVisualizerSelector & 2) > 0)
						{
							this.ShowLineFromDeploymentPointToTarget(deploymentPoint);
						}
						if ((SiegeDeploymentVisualizationMissionView.deploymentVisualizerSelector & 4) > 0)
						{
							this.CreateArcPoints(deploymentPoint);
						}
						if ((SiegeDeploymentVisualizationMissionView.deploymentVisualizerSelector & 8) > 0)
						{
							this.ShowDeploymentBanners(deploymentPoint);
						}
						if ((SiegeDeploymentVisualizationMissionView.deploymentVisualizerSelector & 16) > 0)
						{
							this.ShowPath(deploymentPoint);
						}
						if ((SiegeDeploymentVisualizationMissionView.deploymentVisualizerSelector & 32) > 0)
						{
							this.SetGhostVisibility(deploymentPoint, true);
							return;
						}
					}
				}
				else
				{
					if (deploymentPoint.GetDeploymentPointType() == null)
					{
						this.HideDeploymentBanners(deploymentPoint, false);
						return;
					}
					if (deploymentPoint.GetDeploymentPointType() == 3)
					{
						this.HideTrajectories(deploymentPoint);
					}
				}
				break;
			case 1:
			case 3:
				if ((SiegeDeploymentVisualizationMissionView.deploymentVisualizerSelector & 2) > 0)
				{
					this.ShowLineFromDeploymentPointToTarget(deploymentPoint);
				}
				if ((SiegeDeploymentVisualizationMissionView.deploymentVisualizerSelector & 4) > 0)
				{
					this.CreateArcPoints(deploymentPoint);
				}
				if ((SiegeDeploymentVisualizationMissionView.deploymentVisualizerSelector & 8) > 0)
				{
					this.ShowDeploymentBanners(deploymentPoint);
				}
				if ((SiegeDeploymentVisualizationMissionView.deploymentVisualizerSelector & 16) > 0)
				{
					this.ShowPath(deploymentPoint);
				}
				if ((SiegeDeploymentVisualizationMissionView.deploymentVisualizerSelector & 32) > 0)
				{
					this.SetGhostVisibility(deploymentPoint, true);
				}
				this.SetLaddersUpState(deploymentPoint, false);
				this.SetLightState(deploymentPoint, false);
				return;
			case 2:
				if ((SiegeDeploymentVisualizationMissionView.deploymentVisualizerSelector & 2) > 0)
				{
					this.ShowLineFromDeploymentPointToTarget(deploymentPoint);
				}
				if ((SiegeDeploymentVisualizationMissionView.deploymentVisualizerSelector & 4) > 0)
				{
					this.CreateArcPoints(deploymentPoint);
				}
				if ((SiegeDeploymentVisualizationMissionView.deploymentVisualizerSelector & 8) > 0)
				{
					this.ShowDeploymentBanners(deploymentPoint);
				}
				if ((SiegeDeploymentVisualizationMissionView.deploymentVisualizerSelector & 64) > 0)
				{
					this.SetDeploymentTargetContourState(deploymentPoint, true);
				}
				if ((SiegeDeploymentVisualizationMissionView.deploymentVisualizerSelector & 128) > 0)
				{
					this.SetLaddersUpState(deploymentPoint, true);
				}
				if ((SiegeDeploymentVisualizationMissionView.deploymentVisualizerSelector & 256) > 0)
				{
					this.SetLightState(deploymentPoint, true);
					return;
				}
				break;
			case 4:
				if ((SiegeDeploymentVisualizationMissionView.deploymentVisualizerSelector & 2) > 0)
				{
					this.ShowLineFromDeploymentPointToTarget(deploymentPoint);
				}
				if ((SiegeDeploymentVisualizationMissionView.deploymentVisualizerSelector & 4) > 0)
				{
					this.CreateArcPoints(deploymentPoint);
				}
				if ((SiegeDeploymentVisualizationMissionView.deploymentVisualizerSelector & 8) > 0)
				{
					this.ShowDeploymentBanners(deploymentPoint);
				}
				if ((SiegeDeploymentVisualizationMissionView.deploymentVisualizerSelector & 64) > 0)
				{
					this.SetDeploymentTargetContourState(deploymentPoint, true);
				}
				if ((SiegeDeploymentVisualizationMissionView.deploymentVisualizerSelector & 256) > 0)
				{
					this.SetLightState(deploymentPoint, true);
					return;
				}
				break;
			case 5:
				if ((SiegeDeploymentVisualizationMissionView.deploymentVisualizerSelector & 512) > 0)
				{
					this.ShowTrajectory(deploymentPoint);
					return;
				}
				break;
			default:
				return;
			}
		}

		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			foreach (DeploymentPoint deploymentPoint in this._deploymentPoints)
			{
				switch (deploymentPoint.GetDeploymentPointState())
				{
				case 0:
					if ((SiegeDeploymentVisualizationMissionView.deploymentVisualizerSelector & 1) > 0 && deploymentPoint.GetDeploymentPointType() == null)
					{
						if ((SiegeDeploymentVisualizationMissionView.deploymentVisualizerSelector & 2) > 0)
						{
							this.ShowLineFromDeploymentPointToTarget(deploymentPoint);
						}
						if ((SiegeDeploymentVisualizationMissionView.deploymentVisualizerSelector & 4) > 0)
						{
							this.ShowArcFromDeploymentPointToTarget(deploymentPoint);
						}
						if ((SiegeDeploymentVisualizationMissionView.deploymentVisualizerSelector & 16) > 0)
						{
							this.ShowPath(deploymentPoint);
						}
					}
					break;
				case 1:
				case 3:
					if ((SiegeDeploymentVisualizationMissionView.deploymentVisualizerSelector & 2) > 0)
					{
						this.ShowLineFromDeploymentPointToTarget(deploymentPoint);
					}
					if ((SiegeDeploymentVisualizationMissionView.deploymentVisualizerSelector & 4) > 0)
					{
						this.ShowArcFromDeploymentPointToTarget(deploymentPoint);
					}
					if ((SiegeDeploymentVisualizationMissionView.deploymentVisualizerSelector & 16) > 0)
					{
						this.ShowPath(deploymentPoint);
					}
					break;
				case 2:
					if ((SiegeDeploymentVisualizationMissionView.deploymentVisualizerSelector & 2) > 0)
					{
						this.ShowLineFromDeploymentPointToTarget(deploymentPoint);
					}
					if ((SiegeDeploymentVisualizationMissionView.deploymentVisualizerSelector & 4) > 0)
					{
						this.ShowArcFromDeploymentPointToTarget(deploymentPoint);
					}
					break;
				case 4:
					if ((SiegeDeploymentVisualizationMissionView.deploymentVisualizerSelector & 2) > 0)
					{
						this.ShowLineFromDeploymentPointToTarget(deploymentPoint);
					}
					if ((SiegeDeploymentVisualizationMissionView.deploymentVisualizerSelector & 4) > 0)
					{
						this.ShowArcFromDeploymentPointToTarget(deploymentPoint);
					}
					break;
				}
			}
		}

		private void ShowLineFromDeploymentPointToTarget(DeploymentPoint deploymentPoint)
		{
			deploymentPoint.GetDeploymentOrigin();
			Vec3 deploymentTargetPosition = deploymentPoint.DeploymentTargetPosition;
		}

		private List<Vec3> CreateArcPoints(DeploymentPoint deploymentPoint)
		{
			Vec3 deploymentOrigin = deploymentPoint.GetDeploymentOrigin();
			Vec3 deploymentTargetPosition = deploymentPoint.DeploymentTargetPosition;
			float num = (deploymentTargetPosition - deploymentOrigin).Length / 3f;
			List<Vec3> list = new List<Vec3>();
			int num2 = 0;
			while ((float)num2 < num)
			{
				Vec3 vec = MBMath.Lerp(deploymentOrigin, deploymentTargetPosition, (float)num2 / num, 0f);
				float num3 = 8f - MathF.Pow(MathF.Abs((float)num2 - num * 0.5f) / (num * 0.5f), 1.2f) * 8f;
				vec.z += num3;
				list.Add(vec);
				num2++;
			}
			list.Add(deploymentTargetPosition);
			return list;
		}

		private void ShowArcFromDeploymentPointToTarget(DeploymentPoint deploymentPoint)
		{
			Vec3 deploymentTargetPosition = deploymentPoint.DeploymentTargetPosition;
			List<Vec3> list;
			this._deploymentArcs.TryGetValue(deploymentPoint, out list);
			if (list == null || list[list.Count - 1] != deploymentTargetPosition)
			{
				list = this.CreateArcPoints(deploymentPoint);
			}
			Vec3 vec = Vec3.Invalid;
			foreach (Vec3 vec2 in list)
			{
				bool isValid = vec.IsValid;
				vec = vec2;
			}
		}

		private void ShowDeploymentBanners(DeploymentPoint deploymentPoint)
		{
			Vec3 deploymentOrigin = deploymentPoint.GetDeploymentOrigin();
			Vec3 deploymentTargetPosition = deploymentPoint.DeploymentTargetPosition;
			ValueTuple<GameEntity, GameEntity> valueTuple;
			this._deploymentBanners.TryGetValue(deploymentPoint, out valueTuple);
			if (valueTuple.Item1 == null || valueTuple.Item2 == null)
			{
				valueTuple = this.CreateBanners(deploymentPoint);
			}
			GameEntity item = this._deploymentBanners[deploymentPoint].Item1;
			MatrixFrame matrixFrame = MatrixFrame.Identity;
			matrixFrame.origin = deploymentOrigin;
			matrixFrame.origin.z = matrixFrame.origin.z + 7.5f;
			matrixFrame.rotation.ApplyScaleLocal(10f);
			MatrixFrame matrixFrame2 = matrixFrame;
			item.SetFrame(ref matrixFrame2);
			item.SetVisibilityExcludeParents(true);
			item.SetAlpha(1f);
			GameEntity item2 = this._deploymentBanners[deploymentPoint].Item2;
			matrixFrame = MatrixFrame.Identity;
			matrixFrame.origin = deploymentTargetPosition;
			matrixFrame.origin.z = matrixFrame.origin.z + 7.5f;
			matrixFrame.rotation.ApplyScaleLocal(10f);
			MatrixFrame matrixFrame3 = matrixFrame;
			item2.SetFrame(ref matrixFrame3);
			item2.SetVisibilityExcludeParents(true);
			item2.SetAlpha(1f);
		}

		private void HideDeploymentBanners(DeploymentPoint deploymentPoint, bool isRemoving = false)
		{
			ValueTuple<GameEntity, GameEntity> valueTuple;
			this._deploymentBanners.TryGetValue(deploymentPoint, out valueTuple);
			if (valueTuple.Item1 != null && valueTuple.Item2 != null)
			{
				if (isRemoving)
				{
					valueTuple.Item1.Remove(104);
					valueTuple.Item2.Remove(105);
					return;
				}
				valueTuple.Item1.SetVisibilityExcludeParents(false);
				valueTuple.Item2.SetVisibilityExcludeParents(false);
			}
		}

		private ValueTuple<GameEntity, GameEntity> CreateBanners(DeploymentPoint deploymentPoint)
		{
			GameEntity gameEntity = this.CreateBannerEntity(false);
			gameEntity.SetVisibilityExcludeParents(false);
			GameEntity gameEntity2 = this.CreateBannerEntity(true);
			gameEntity2.SetVisibilityExcludeParents(false);
			ValueTuple<GameEntity, GameEntity> valueTuple = new ValueTuple<GameEntity, GameEntity>(gameEntity, gameEntity2);
			this._deploymentBanners.Add(deploymentPoint, valueTuple);
			return valueTuple;
		}

		private GameEntity CreateBannerEntity(bool isTargetEntity)
		{
			GameEntity gameEntity = GameEntity.CreateEmpty(Mission.Current.Scene, true);
			gameEntity.EntityFlags |= 512;
			uint num = 4278190080U;
			uint num2;
			if (!isTargetEntity)
			{
				num2 = 2141323264U;
			}
			else
			{
				num2 = 2131100887U;
			}
			gameEntity.AddMultiMesh(MetaMesh.GetCopy("billboard_unit_mesh", true, false), true);
			gameEntity.GetFirstMesh().Color = uint.MaxValue;
			Material material = Material.GetFromResource("formation_icon").CreateCopy();
			if (isTargetEntity)
			{
				Texture fromResource = Texture.GetFromResource("plain_yellow");
				material.SetTexture(1, fromResource);
			}
			else
			{
				Texture fromResource2 = Texture.GetFromResource("plain_blue");
				material.SetTexture(1, fromResource2);
			}
			gameEntity.GetFirstMesh().SetMaterial(material);
			gameEntity.GetFirstMesh().Color = num2;
			gameEntity.GetFirstMesh().Color2 = num;
			gameEntity.GetFirstMesh().SetVectorArgument(0f, 1f, 0f, 1f);
			return gameEntity;
		}

		private void ShowPath(DeploymentPoint deploymentPoint)
		{
			(deploymentPoint.GetWeaponsUnder().FirstOrDefault((SynchedMissionObject wu) => wu is IMoveableSiegeWeapon) as IMoveableSiegeWeapon).HighlightPath();
		}

		private void SetGhostVisibility(DeploymentPoint deploymentPoint, bool isVisible)
		{
		}

		private void SetDeploymentTargetContourState(DeploymentPoint deploymentPoint, bool isHighlighted)
		{
			DeploymentPoint.DeploymentPointState deploymentPointState = deploymentPoint.GetDeploymentPointState();
			if (deploymentPointState == 2)
			{
				using (List<SiegeLadder>.Enumerator enumerator = deploymentPoint.GetAssociatedSiegeLadders().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						SiegeLadder siegeLadder = enumerator.Current;
						if (isHighlighted)
						{
							siegeLadder.GameEntity.SetContourColor(new uint?(4289622555U), true);
						}
						else
						{
							siegeLadder.GameEntity.SetContourColor(null, true);
						}
					}
					return;
				}
			}
			if (deploymentPointState == 4)
			{
				if (isHighlighted)
				{
					deploymentPoint.AssociatedWallSegment.GameEntity.SetContourColor(new uint?(4289622555U), true);
					return;
				}
				deploymentPoint.AssociatedWallSegment.GameEntity.SetContourColor(null, true);
			}
		}

		private void SetLaddersUpState(DeploymentPoint deploymentPoint, bool isRaised)
		{
			foreach (SiegeLadder siegeLadder in deploymentPoint.GetAssociatedSiegeLadders())
			{
				siegeLadder.SetUpStateVisibility(isRaised);
			}
		}

		private void SetLightState(DeploymentPoint deploymentPoint, bool isVisible)
		{
			GameEntity gameEntity;
			this._deploymentLights.TryGetValue(deploymentPoint, out gameEntity);
			if (gameEntity != null)
			{
				gameEntity.SetVisibilityExcludeParents(isVisible);
				return;
			}
			if (isVisible)
			{
				this.CreateLight(deploymentPoint);
			}
		}

		private void CreateLight(DeploymentPoint deploymentPoint)
		{
			MatrixFrame identity = MatrixFrame.Identity;
			identity.origin = deploymentPoint.DeploymentTargetPosition + new Vec3(0f, 0f, (deploymentPoint.GetDeploymentPointType() == 1) ? 10f : 3f, -1f);
			identity.rotation.RotateAboutSide(1.5707964f);
			identity.Scale(new Vec3(5f, 5f, 5f, -1f));
			GameEntity gameEntity = GameEntity.Instantiate(Mission.Current.Scene, "aserai_keep_interior_a_light_shaft_a", identity);
			this._deploymentLights.Add(deploymentPoint, gameEntity);
		}

		private void ShowTrajectory(DeploymentPoint deploymentPoint)
		{
			(deploymentPoint.DeployedWeapon as RangedSiegeWeapon).ToggleTrajectoryVisibility(true);
		}

		private void HideTrajectories(DeploymentPoint deploymentPoint)
		{
			foreach (SynchedMissionObject synchedMissionObject in deploymentPoint.GetWeaponsUnder())
			{
				(synchedMissionObject as RangedSiegeWeapon).ToggleTrajectoryVisibility(false);
			}
		}

		private static int deploymentVisualizerSelector;

		private List<DeploymentPoint> _deploymentPoints;

		private bool _deploymentPointsVisible;

		private Dictionary<DeploymentPoint, List<Vec3>> _deploymentArcs = new Dictionary<DeploymentPoint, List<Vec3>>();

		private Dictionary<DeploymentPoint, ValueTuple<GameEntity, GameEntity>> _deploymentBanners = new Dictionary<DeploymentPoint, ValueTuple<GameEntity, GameEntity>>();

		private Dictionary<DeploymentPoint, GameEntity> _deploymentLights = new Dictionary<DeploymentPoint, GameEntity>();

		private DeploymentMissionView _deploymentMissionView;

		private const uint EntityHighlightColor = 4289622555U;

		public enum DeploymentVisualizationPreference
		{
			ShowUndeployed = 1,
			Line,
			Arc = 4,
			Banner = 8,
			Path = 16,
			Ghost = 32,
			Contour = 64,
			LiftLadders = 128,
			Light = 256,
			Trajectory = 512,
			AllEnabled = 1023
		}
	}
}
