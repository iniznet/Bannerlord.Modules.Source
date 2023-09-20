using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.View.MissionViews.Singleplayer
{
	// Token: 0x02000077 RID: 119
	public class SiegeDeploymentVisualizationMissionView : MissionView
	{
		// Token: 0x0600046F RID: 1135 RVA: 0x000229D8 File Offset: 0x00020BD8
		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			this._deploymentMissionView = base.Mission.GetMissionBehavior<DeploymentMissionView>();
			DeploymentMissionView deploymentMissionView = this._deploymentMissionView;
			deploymentMissionView.OnDeploymentFinish = (OnPlayerDeploymentFinishDelegate)Delegate.Combine(deploymentMissionView.OnDeploymentFinish, new OnPlayerDeploymentFinishDelegate(this.OnDeploymentFinish));
		}

		// Token: 0x06000470 RID: 1136 RVA: 0x00022A18 File Offset: 0x00020C18
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

		// Token: 0x06000471 RID: 1137 RVA: 0x00022ADC File Offset: 0x00020CDC
		public void OnDeploymentFinish()
		{
			DeploymentMissionView deploymentMissionView = this._deploymentMissionView;
			deploymentMissionView.OnDeploymentFinish = (OnPlayerDeploymentFinishDelegate)Delegate.Remove(deploymentMissionView.OnDeploymentFinish, new OnPlayerDeploymentFinishDelegate(this.OnDeploymentFinish));
			this._deploymentMissionView = null;
			this.TryRemoveDeploymentVisibilities();
			Mission.Current.RemoveMissionBehavior(this);
		}

		// Token: 0x06000472 RID: 1138 RVA: 0x00022B28 File Offset: 0x00020D28
		protected override void OnEndMission()
		{
			base.OnEndMission();
			this.TryRemoveDeploymentVisibilities();
		}

		// Token: 0x06000473 RID: 1139 RVA: 0x00022B36 File Offset: 0x00020D36
		public override void OnRemoveBehavior()
		{
			base.OnRemoveBehavior();
		}

		// Token: 0x06000474 RID: 1140 RVA: 0x00022B40 File Offset: 0x00020D40
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

		// Token: 0x06000475 RID: 1141 RVA: 0x00022BB4 File Offset: 0x00020DB4
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

		// Token: 0x06000476 RID: 1142 RVA: 0x00022C3C File Offset: 0x00020E3C
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

		// Token: 0x06000477 RID: 1143 RVA: 0x00022C85 File Offset: 0x00020E85
		[CommandLineFunctionality.CommandLineArgumentFunction("set_deployment_visualization_selector", "mission")]
		public static string SetDeploymentVisualizationSelector(List<string> strings)
		{
			if (strings.Count == 1 && int.TryParse(strings[0], out SiegeDeploymentVisualizationMissionView.deploymentVisualizerSelector))
			{
				return "Enabled deployment visualization options are:" + SiegeDeploymentVisualizationMissionView.GetSelectorStateDescription();
			}
			return "Format is \"mission.set_deployment_visualization_selector [integer > 0]\".";
		}

		// Token: 0x06000478 RID: 1144 RVA: 0x00022CB8 File Offset: 0x00020EB8
		private void OnDeploymentStateChanged(DeploymentPoint deploymentPoint, SynchedMissionObject targetObject)
		{
			this.OnDeploymentPointStateSet(deploymentPoint);
		}

		// Token: 0x06000479 RID: 1145 RVA: 0x00022CC4 File Offset: 0x00020EC4
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

		// Token: 0x0600047A RID: 1146 RVA: 0x00022EE0 File Offset: 0x000210E0
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

		// Token: 0x0600047B RID: 1147 RVA: 0x00023038 File Offset: 0x00021238
		private void ShowLineFromDeploymentPointToTarget(DeploymentPoint deploymentPoint)
		{
			deploymentPoint.GetDeploymentOrigin();
			Vec3 deploymentTargetPosition = deploymentPoint.DeploymentTargetPosition;
		}

		// Token: 0x0600047C RID: 1148 RVA: 0x00023048 File Offset: 0x00021248
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

		// Token: 0x0600047D RID: 1149 RVA: 0x000230F4 File Offset: 0x000212F4
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

		// Token: 0x0600047E RID: 1150 RVA: 0x00023180 File Offset: 0x00021380
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

		// Token: 0x0600047F RID: 1151 RVA: 0x00023290 File Offset: 0x00021490
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

		// Token: 0x06000480 RID: 1152 RVA: 0x00023300 File Offset: 0x00021500
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

		// Token: 0x06000481 RID: 1153 RVA: 0x00023344 File Offset: 0x00021544
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

		// Token: 0x06000482 RID: 1154 RVA: 0x00023429 File Offset: 0x00021629
		private void ShowPath(DeploymentPoint deploymentPoint)
		{
			(deploymentPoint.GetWeaponsUnder().FirstOrDefault((SynchedMissionObject wu) => wu is IMoveableSiegeWeapon) as IMoveableSiegeWeapon).HighlightPath();
		}

		// Token: 0x06000483 RID: 1155 RVA: 0x0002345F File Offset: 0x0002165F
		private void SetGhostVisibility(DeploymentPoint deploymentPoint, bool isVisible)
		{
		}

		// Token: 0x06000484 RID: 1156 RVA: 0x00023464 File Offset: 0x00021664
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

		// Token: 0x06000485 RID: 1157 RVA: 0x00023528 File Offset: 0x00021728
		private void SetLaddersUpState(DeploymentPoint deploymentPoint, bool isRaised)
		{
			foreach (SiegeLadder siegeLadder in deploymentPoint.GetAssociatedSiegeLadders())
			{
				siegeLadder.SetUpStateVisibility(isRaised);
			}
		}

		// Token: 0x06000486 RID: 1158 RVA: 0x0002357C File Offset: 0x0002177C
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

		// Token: 0x06000487 RID: 1159 RVA: 0x000235B4 File Offset: 0x000217B4
		private void CreateLight(DeploymentPoint deploymentPoint)
		{
			MatrixFrame identity = MatrixFrame.Identity;
			identity.origin = deploymentPoint.DeploymentTargetPosition + new Vec3(0f, 0f, (deploymentPoint.GetDeploymentPointType() == 1) ? 10f : 3f, -1f);
			identity.rotation.RotateAboutSide(1.5707964f);
			identity.Scale(new Vec3(5f, 5f, 5f, -1f));
			GameEntity gameEntity = GameEntity.Instantiate(Mission.Current.Scene, "aserai_keep_interior_a_light_shaft_a", identity);
			this._deploymentLights.Add(deploymentPoint, gameEntity);
		}

		// Token: 0x06000488 RID: 1160 RVA: 0x00023656 File Offset: 0x00021856
		private void ShowTrajectory(DeploymentPoint deploymentPoint)
		{
			(deploymentPoint.DeployedWeapon as RangedSiegeWeapon).ToggleTrajectoryVisibility(true);
		}

		// Token: 0x06000489 RID: 1161 RVA: 0x0002366C File Offset: 0x0002186C
		private void HideTrajectories(DeploymentPoint deploymentPoint)
		{
			foreach (SynchedMissionObject synchedMissionObject in deploymentPoint.GetWeaponsUnder())
			{
				(synchedMissionObject as RangedSiegeWeapon).ToggleTrajectoryVisibility(false);
			}
		}

		// Token: 0x040002BB RID: 699
		private static int deploymentVisualizerSelector;

		// Token: 0x040002BC RID: 700
		private List<DeploymentPoint> _deploymentPoints;

		// Token: 0x040002BD RID: 701
		private bool _deploymentPointsVisible;

		// Token: 0x040002BE RID: 702
		private Dictionary<DeploymentPoint, List<Vec3>> _deploymentArcs = new Dictionary<DeploymentPoint, List<Vec3>>();

		// Token: 0x040002BF RID: 703
		private Dictionary<DeploymentPoint, ValueTuple<GameEntity, GameEntity>> _deploymentBanners = new Dictionary<DeploymentPoint, ValueTuple<GameEntity, GameEntity>>();

		// Token: 0x040002C0 RID: 704
		private Dictionary<DeploymentPoint, GameEntity> _deploymentLights = new Dictionary<DeploymentPoint, GameEntity>();

		// Token: 0x040002C1 RID: 705
		private DeploymentMissionView _deploymentMissionView;

		// Token: 0x040002C2 RID: 706
		private const uint EntityHighlightColor = 4289622555U;

		// Token: 0x020000CB RID: 203
		public enum DeploymentVisualizationPreference
		{
			// Token: 0x0400039E RID: 926
			ShowUndeployed = 1,
			// Token: 0x0400039F RID: 927
			Line,
			// Token: 0x040003A0 RID: 928
			Arc = 4,
			// Token: 0x040003A1 RID: 929
			Banner = 8,
			// Token: 0x040003A2 RID: 930
			Path = 16,
			// Token: 0x040003A3 RID: 931
			Ghost = 32,
			// Token: 0x040003A4 RID: 932
			Contour = 64,
			// Token: 0x040003A5 RID: 933
			LiftLadders = 128,
			// Token: 0x040003A6 RID: 934
			Light = 256,
			// Token: 0x040003A7 RID: 935
			Trajectory = 512,
			// Token: 0x040003A8 RID: 936
			AllEnabled = 1023
		}
	}
}
