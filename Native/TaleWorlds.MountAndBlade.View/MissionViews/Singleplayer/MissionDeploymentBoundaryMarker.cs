﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.View.MissionViews.Singleplayer
{
	public class MissionDeploymentBoundaryMarker : MissionView
	{
		public MissionDeploymentBoundaryMarker(IEntityFactory entityFactory, MissionDeploymentBoundaryMarker.MissionDeploymentBoundaryType deploymentBoundaryType, float markerInterval = 2f)
		{
			this.entityFactory = entityFactory;
			this.MarkerInterval = Math.Max(markerInterval, 0.0001f);
			this.DeploymentBoundaryType = deploymentBoundaryType;
		}

		public override void AfterStart()
		{
			base.AfterStart();
			this.AddBoundaryMarkers();
		}

		protected override void OnEndMission()
		{
			base.OnEndMission();
			this.TryRemoveBoundaryMarkers();
		}

		private BattleSideEnum GetBattleSideFromStaticBoundaryName(string boundaryName)
		{
			if (boundaryName.Contains("deployment_castle_boundary"))
			{
				return 0;
			}
			if (boundaryName.Contains("walk_area"))
			{
				return 1;
			}
			Debug.FailedAssert("Unknown static boundary type " + boundaryName + ". Refer to scene artist.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.View\\MissionViews\\Singleplayer\\MissionDeploymentBoundaryMarker.cs", "GetBattleSideFromStaticBoundaryName", 65);
			return 1;
		}

		public override void OnInitialDeploymentPlanMadeForSide(BattleSideEnum side, bool isFirstPlan)
		{
			bool flag = base.Mission.DeploymentPlan.HasDeploymentBoundaries(side, 0);
			if (this.DeploymentBoundaryType == MissionDeploymentBoundaryMarker.MissionDeploymentBoundaryType.DynamicDeploymentBoundaries && isFirstPlan && flag)
			{
				foreach (KeyValuePair<string, List<Vec2>> keyValuePair in base.Mission.DeploymentPlan.GetDeploymentBoundaries(side, 0))
				{
					this.AddBoundaryMarkerForSide(side, new KeyValuePair<string, ICollection<Vec2>>(keyValuePair.Key, keyValuePair.Value));
				}
			}
		}

		public override void OnRemoveBehavior()
		{
			this.TryRemoveBoundaryMarkers();
			base.OnRemoveBehavior();
		}

		private void AddBoundaryMarkers()
		{
			for (int i = 0; i < 2; i++)
			{
				this.boundaryMarkersPerSide[i] = new Dictionary<string, List<GameEntity>>();
			}
			if (this.DeploymentBoundaryType == MissionDeploymentBoundaryMarker.MissionDeploymentBoundaryType.StaticSceneBoundaries)
			{
				foreach (KeyValuePair<string, ICollection<Vec2>> keyValuePair in base.Mission.Boundaries)
				{
					BattleSideEnum battleSideFromStaticBoundaryName = this.GetBattleSideFromStaticBoundaryName(keyValuePair.Key);
					this.AddBoundaryMarkerForSide(battleSideFromStaticBoundaryName, keyValuePair);
				}
				base.Mission.Boundaries.CollectionChanged += this.MissionStaticBoundaries_Changed;
			}
			this._boundaryMarkersRemoved = false;
		}

		private void AddBoundaryMarkerForSide(BattleSideEnum side, KeyValuePair<string, ICollection<Vec2>> boundary)
		{
			string key = boundary.Key;
			if (!this.boundaryMarkersPerSide[side].ContainsKey(key))
			{
				Banner banner = ((side == 1) ? base.Mission.AttackerTeam.Banner : ((side == null) ? base.Mission.DefenderTeam.Banner : null));
				List<GameEntity> list = new List<GameEntity>();
				List<Vec2> list2 = boundary.Value.ToList<Vec2>();
				for (int i = 0; i < list2.Count; i++)
				{
					this.MarkLine(new Vec3(list2[i], 0f, -1f), new Vec3(list2[(i + 1) % list2.Count], 0f, -1f), list, banner);
				}
				this.boundaryMarkersPerSide[side][key] = list;
			}
		}

		private void TryRemoveBoundaryMarkers()
		{
			if (!this._boundaryMarkersRemoved)
			{
				if (this.DeploymentBoundaryType == MissionDeploymentBoundaryMarker.MissionDeploymentBoundaryType.StaticSceneBoundaries)
				{
					base.Mission.Boundaries.CollectionChanged -= this.MissionStaticBoundaries_Changed;
				}
				for (int i = 0; i < 2; i++)
				{
					foreach (string text in this.boundaryMarkersPerSide[i].Keys.ToList<string>())
					{
						this.RemoveBoundaryMarker(text, i);
					}
				}
				this._boundaryMarkersRemoved = true;
			}
		}

		private void RemoveBoundaryMarker(string boundaryName, BattleSideEnum side)
		{
			List<GameEntity> list;
			if (this.boundaryMarkersPerSide[side].TryGetValue(boundaryName, out list))
			{
				foreach (GameEntity gameEntity in list)
				{
					gameEntity.Remove(103);
				}
				this.boundaryMarkersPerSide[side].Remove(boundaryName);
			}
		}

		private void MissionStaticBoundaries_Changed(object sender, NotifyCollectionChangedEventArgs e)
		{
			NotifyCollectionChangedAction action = e.Action;
			if (action != NotifyCollectionChangedAction.Add)
			{
				if (action != NotifyCollectionChangedAction.Remove)
				{
					goto IL_C3;
				}
			}
			else
			{
				using (IEnumerator enumerator = e.NewItems.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						string text = obj.ToString();
						KeyValuePair<string, ICollection<Vec2>> keyValuePair = new KeyValuePair<string, ICollection<Vec2>>(text, base.Mission.Boundaries[text]);
						BattleSideEnum battleSideFromStaticBoundaryName = this.GetBattleSideFromStaticBoundaryName(text);
						this.AddBoundaryMarkerForSide(battleSideFromStaticBoundaryName, keyValuePair);
					}
					return;
				}
			}
			using (IEnumerator enumerator = e.OldItems.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					object obj2 = enumerator.Current;
					string text2 = obj2.ToString();
					BattleSideEnum battleSideFromStaticBoundaryName2 = this.GetBattleSideFromStaticBoundaryName(text2);
					this.RemoveBoundaryMarker(text2, battleSideFromStaticBoundaryName2);
				}
				return;
			}
			IL_C3:
			Debug.FailedAssert("Invalid state", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.View\\MissionViews\\Singleplayer\\MissionDeploymentBoundaryMarker.cs", "MissionStaticBoundaries_Changed", 197);
		}

		protected void MarkLine(Vec3 startPoint, Vec3 endPoint, List<GameEntity> boundary, Banner banner = null)
		{
			Scene scene = base.Mission.Scene;
			Vec3 vec = endPoint - startPoint;
			float length = vec.Length;
			Vec3 vec2 = vec;
			vec2.Normalize();
			vec2 *= this.MarkerInterval;
			for (float num = 0f; num < length; num += this.MarkerInterval)
			{
				MatrixFrame identity = MatrixFrame.Identity;
				identity.rotation.RotateAboutUp(vec.RotationZ + 1.5707964f);
				identity.origin = startPoint;
				if (!scene.GetHeightAtPoint(identity.origin.AsVec2, 2208137, ref identity.origin.z))
				{
					identity.origin.z = 0f;
				}
				identity.origin.z = identity.origin.z - 0.5f;
				identity.Scale(Vec3.One * 0.4f);
				GameEntity gameEntity = this.entityFactory.MakeEntity(new object[] { banner });
				gameEntity.SetFrame(ref identity);
				boundary.Add(gameEntity);
				startPoint += vec2;
			}
		}

		public const string AttackerStaticDeploymentBoundaryName = "walk_area";

		public const string DefenderStaticDeploymentBoundaryName = "deployment_castle_boundary";

		public readonly float MarkerInterval;

		public readonly MissionDeploymentBoundaryMarker.MissionDeploymentBoundaryType DeploymentBoundaryType;

		private readonly Dictionary<string, List<GameEntity>>[] boundaryMarkersPerSide = new Dictionary<string, List<GameEntity>>[2];

		private readonly IEntityFactory entityFactory;

		private bool _boundaryMarkersRemoved = true;

		public enum MissionDeploymentBoundaryType
		{
			StaticSceneBoundaries,
			DynamicDeploymentBoundaries
		}
	}
}
