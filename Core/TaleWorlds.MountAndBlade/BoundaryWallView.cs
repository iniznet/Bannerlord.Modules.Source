using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000332 RID: 818
	public class BoundaryWallView : ScriptComponentBehavior
	{
		// Token: 0x06002C22 RID: 11298 RVA: 0x000AB120 File Offset: 0x000A9320
		protected internal override void OnInit()
		{
			throw new Exception("This should only be used in editor.");
		}

		// Token: 0x06002C23 RID: 11299 RVA: 0x000AB12C File Offset: 0x000A932C
		protected internal override void OnEditorTick(float dt)
		{
			base.OnEditorTick(dt);
			this.timer += dt;
			if (!MBEditor.BorderHelpersEnabled())
			{
				return;
			}
			if (this.timer < 0.2f)
			{
				return;
			}
			this.timer = 0f;
			if (base.Scene == null)
			{
				return;
			}
			bool flag = this.CalculateBoundaries("walk_area_vertex", ref this._lastPoints);
			bool flag2 = this.CalculateBoundaries("defender_area_vertex", ref this._lastDefenderPoints);
			bool flag3 = this.CalculateBoundaries("attacker_area_vertex", ref this._lastAttackerPoints);
			if (this._lastPoints.Count >= 3 || this._lastDefenderPoints.Count >= 3 || this._lastAttackerPoints.Count >= 3)
			{
				if (flag || flag2 || flag3)
				{
					base.GameEntity.ClearEntityComponents(true, false, true);
					base.GameEntity.Name = "editor_map_border";
					Mesh mesh = BoundaryWallView.CreateBoundaryMesh(base.Scene, this._lastPoints, 536918784U);
					if (mesh != null)
					{
						base.GameEntity.AddMesh(mesh, true);
					}
					Color color = new Color(0f, 0f, 0.8f, 1f);
					Mesh mesh2 = BoundaryWallView.CreateBoundaryMesh(base.Scene, this._lastDefenderPoints, color.ToUnsignedInteger());
					if (mesh2 != null)
					{
						base.GameEntity.AddMesh(mesh2, true);
					}
					Color color2 = new Color(0f, 0.8f, 0.8f, 1f);
					Mesh mesh3 = BoundaryWallView.CreateBoundaryMesh(base.Scene, this._lastAttackerPoints, color2.ToUnsignedInteger());
					if (mesh3 != null)
					{
						base.GameEntity.AddMesh(mesh3, true);
						return;
					}
				}
			}
			else
			{
				base.GameEntity.ClearEntityComponents(true, false, true);
			}
		}

		// Token: 0x06002C24 RID: 11300 RVA: 0x000AB2E0 File Offset: 0x000A94E0
		private bool CalculateBoundaries(string vertexTag, ref List<Vec2> lastPoints)
		{
			IEnumerable<GameEntity> enumerable = base.Scene.FindEntitiesWithTag(vertexTag);
			enumerable = enumerable.Where((GameEntity e) => !e.EntityFlags.HasAnyFlag(EntityFlags.DontSaveToScene));
			int num = enumerable.Count<GameEntity>();
			bool flag = false;
			if (num >= 3)
			{
				List<Vec2> list = enumerable.Select((GameEntity e) => e.GlobalPosition.AsVec2).ToList<Vec2>();
				Vec2 mid = new Vec2(list.Average((Vec2 p) => p.x), list.Average((Vec2 p) => p.y));
				list = list.OrderBy((Vec2 p) => MathF.Atan2(p.x - mid.x, p.y - mid.y)).ToList<Vec2>();
				if (lastPoints != null && lastPoints.Count == list.Count)
				{
					flag = true;
					for (int i = 0; i < list.Count; i++)
					{
						if (list[i] != lastPoints[i])
						{
							flag = false;
							break;
						}
					}
				}
				lastPoints = list;
				return !flag;
			}
			return false;
		}

		// Token: 0x06002C25 RID: 11301 RVA: 0x000AB41C File Offset: 0x000A961C
		public static Mesh CreateBoundaryMesh(Scene scene, ICollection<Vec2> boundaryPoints, uint meshColor = 536918784U)
		{
			if (boundaryPoints == null || boundaryPoints.Count < 3)
			{
				return null;
			}
			Mesh mesh = Mesh.CreateMesh(true);
			UIntPtr uintPtr = mesh.LockEditDataWrite();
			Vec3 vec;
			Vec3 vec2;
			scene.GetBoundingBox(out vec, out vec2);
			vec2.z += 50f;
			vec.z -= 50f;
			for (int i = 0; i < boundaryPoints.Count; i++)
			{
				Vec2 vec3 = boundaryPoints.ElementAt(i);
				Vec2 vec4 = boundaryPoints.ElementAt((i + 1) % boundaryPoints.Count);
				float num = 0f;
				float num2 = 0f;
				if (!scene.IsAtmosphereIndoor)
				{
					if (!scene.GetHeightAtPoint(vec3, BodyFlags.CommonCollisionExcludeFlagsForCombat, ref num))
					{
						MBDebug.ShowWarning("GetHeightAtPoint failed at CreateBoundaryEntity!");
						return null;
					}
					if (!scene.GetHeightAtPoint(vec4, BodyFlags.CommonCollisionExcludeFlagsForCombat, ref num2))
					{
						MBDebug.ShowWarning("GetHeightAtPoint failed at CreateBoundaryEntity!");
						return null;
					}
				}
				else
				{
					num = vec.z;
					num2 = vec.z;
				}
				Vec3 vec5 = vec3.ToVec3(num);
				Vec3 vec6 = vec4.ToVec3(num2);
				Vec3 vec7 = Vec3.Up * 2f;
				Vec3 vec8 = vec5;
				Vec3 vec9 = vec6;
				Vec3 vec10 = vec5;
				Vec3 vec11 = vec6;
				vec8.z = MathF.Min(vec8.z, vec.z);
				vec9.z = MathF.Min(vec9.z, vec.z);
				vec10.z = MathF.Max(vec10.z, vec2.z);
				vec11.z = MathF.Max(vec11.z, vec2.z);
				vec8 -= vec7;
				vec9 -= vec7;
				vec10 += vec7;
				vec11 += vec7;
				mesh.AddTriangle(vec8, vec9, vec10, Vec2.Zero, Vec2.Side, Vec2.Forward, meshColor, uintPtr);
				mesh.AddTriangle(vec10, vec9, vec11, Vec2.Forward, Vec2.Side, Vec2.One, meshColor, uintPtr);
			}
			mesh.SetMaterial("editor_map_border");
			mesh.VisibilityMask = VisibilityMaskFlags.Final | VisibilityMaskFlags.EditModeBorders;
			mesh.SetColorAlpha(150U);
			mesh.SetMeshRenderOrder(250);
			mesh.CullingMode = MBMeshCullingMode.None;
			float num3 = 25f;
			if (MBEditor.IsEditModeOn && scene.IsEditorScene())
			{
				num3 = 100000f;
			}
			IEnumerable<GameEntity> enumerable = scene.FindEntitiesWithTag("walk_area_vertex");
			float num4;
			if (!enumerable.Any<GameEntity>())
			{
				num4 = 0f;
			}
			else
			{
				num4 = enumerable.Average((GameEntity ent) => ent.GlobalPosition.z);
			}
			float num5 = num4;
			mesh.SetVectorArgument(num3, num5, 0f, 0f);
			mesh.ComputeNormals();
			mesh.ComputeTangents();
			mesh.RecomputeBoundingBox();
			mesh.UnlockEditDataWrite(uintPtr);
			return mesh;
		}

		// Token: 0x040010B8 RID: 4280
		private List<Vec2> _lastPoints = new List<Vec2>();

		// Token: 0x040010B9 RID: 4281
		private List<Vec2> _lastAttackerPoints = new List<Vec2>();

		// Token: 0x040010BA RID: 4282
		private List<Vec2> _lastDefenderPoints = new List<Vec2>();

		// Token: 0x040010BB RID: 4283
		private float timer;
	}
}
