using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class TrajectoryVisualizer
	{
		public bool IsVisible { get; private set; }

		public TrajectoryVisualizer(Scene scene)
		{
			this.trajectoryPointList = new List<GameEntity>();
			this._scene = scene;
			this.IsVisible = true;
		}

		public void Init(Vec3 startingPosition, Vec3 startingVelocity, float simulationTime, float pointCount)
		{
			this.trajectoryPointList.Clear();
			this.collisionEntity = new GameEntity[0];
			for (int i = 0; i < this.collisionEntity.Length; i++)
			{
				this.collisionEntity[i] = GameEntity.Instantiate(this._scene, "trajectory_entity", true);
				this.collisionEntity[i].SetMobility(GameEntity.Mobility.dynamic);
				MatrixFrame frame = this.collisionEntity[i].GetFrame();
				frame.Scale(new Vec3(2f, 2f, 2f, -1f));
				this.collisionEntity[i].SetFrame(ref frame);
				this.collisionEntity[i].EntityFlags |= EntityFlags.NonModifiableFromEditor;
				this.collisionEntity[i].EntityFlags |= EntityFlags.DontSaveToScene;
				this.collisionEntity[i].EntityFlags |= EntityFlags.DoesNotAffectParentsLocalBb;
				this.collisionEntity[i].EntityFlags |= EntityFlags.NotAffectedBySeason;
			}
			float num = simulationTime / pointCount;
			float num2 = 0f;
			while (num2 < simulationTime && (float)this.trajectoryPointList.Count < pointCount)
			{
				Vec3 vec = startingVelocity + MBGlobals.GravityVec3 * num2;
				Vec3 vec2 = startingPosition + vec * num2;
				GameEntity gameEntity = GameEntity.Instantiate(this._scene, "trajectory_entity", true);
				gameEntity.SetMobility(GameEntity.Mobility.dynamic);
				gameEntity.EntityFlags |= EntityFlags.NonModifiableFromEditor;
				gameEntity.EntityFlags |= EntityFlags.DontSaveToScene;
				gameEntity.EntityFlags |= EntityFlags.DoesNotAffectParentsLocalBb;
				gameEntity.EntityFlags |= EntityFlags.NotAffectedBySeason;
				MatrixFrame frame2 = gameEntity.GetFrame();
				frame2.origin = vec2;
				gameEntity.SetFrame(ref frame2);
				this.trajectoryPointList.Add(gameEntity);
				num2 += num;
			}
		}

		public void Clear()
		{
			foreach (GameEntity gameEntity in this.trajectoryPointList)
			{
				gameEntity.Remove(86);
			}
			this.trajectoryPointList.Clear();
			for (int i = 0; i < this.collisionEntity.Length; i++)
			{
				this.collisionEntity[i].Remove(87);
			}
		}

		public void Update(Vec3 position, Vec3 velocity, float simulationTime, float pointCount, ItemObject missileItem)
		{
			if (missileItem != null)
			{
				this.UpdateCollisionSpheres(position, velocity, missileItem);
			}
			float num = simulationTime / pointCount;
			int num2 = 0;
			int num3 = 0;
			float num4 = num / 10f;
			float num5 = 0f;
			while (num5 < simulationTime - 1E-05f && (float)num2 < pointCount)
			{
				velocity += MBGlobals.GravityVec3 * num4;
				position += velocity * num4;
				if (num3 % 10 == 0)
				{
					GameEntity gameEntity = this.trajectoryPointList[num2++];
					MatrixFrame frame = gameEntity.GetFrame();
					frame.origin = position;
					gameEntity.SetFrame(ref frame);
				}
				num3++;
				num5 += num4;
			}
		}

		private void UpdateCollisionSpheres(Vec3 position, Vec3 velocity, ItemObject missileItem)
		{
			MissionWeapon missionWeapon = new MissionWeapon(missileItem, null, null);
			Vec3 vec = velocity;
			vec.Normalize();
			Vec3 vec2 = vec;
			vec2.Normalize();
			float x = vec2.x;
			float y = vec2.y;
			float num = vec2.z;
			float num2 = x * position.x + y * position.y + num * position.z;
			Vec3 vec3 = new Vec3(1f, 1f, 0f, -1f);
			num = ((MathF.Abs(num - 0f) < float.Epsilon) ? 1f : num);
			vec3.z = (-num2 - x * (1f - position.x) - y * (1f - position.y) + num * position.z) / num;
			vec3.Normalize();
			Vec3 vec4 = Vec3.CrossProduct(vec2, vec3);
			vec4.Normalize();
			Vec3[] array = new Vec3[0];
			for (int i = 0; i < 0; i++)
			{
				float num3 = (float)i * 360f / 0f * 3.1415927f / 180f;
				array[i] = vec2 + vec3 * MathF.Sin(num3) * 0.1f + vec4 * MathF.Cos(num3) * 0.1f;
			}
			for (int j = 0; j < array.Length; j++)
			{
				Vec3 vec5 = array[j];
				WeaponData weaponData = missionWeapon.GetWeaponData(false);
				Vec3 missileCollisionPoint = Mission.Current.GetMissileCollisionPoint(position, vec5, velocity.Length, weaponData);
				MatrixFrame frame = this.collisionEntity[j].GetFrame();
				frame.origin = missileCollisionPoint;
				this.collisionEntity[j].SetFrame(ref frame);
			}
		}

		public void SetVisible(bool isVisible)
		{
			if (this.IsVisible != isVisible)
			{
				this.IsVisible = isVisible;
				foreach (GameEntity gameEntity in this.trajectoryPointList)
				{
					gameEntity.SetVisibilityExcludeParents(isVisible);
				}
				GameEntity[] array = this.collisionEntity;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].SetVisibilityExcludeParents(isVisible);
				}
			}
		}

		private List<GameEntity> trajectoryPointList;

		protected MatrixFrame initialFrame;

		private GameEntity[] collisionEntity;

		private const int vectorCount = 0;

		private const string trajectoryPointPrefabName = "trajectory_entity";

		private Scene _scene;
	}
}
