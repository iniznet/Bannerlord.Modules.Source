using System;
using System.Linq.Expressions;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	public class RoadStart : ScriptComponentBehavior
	{
		protected internal override void OnInit()
		{
			this.pathEntity = GameEntity.CreateEmpty(base.Scene, false);
			this.pathEntity.Name = "Road_Entity";
			this.UpdatePathMesh();
		}

		protected internal override void OnEditorInit()
		{
			this.OnInit();
		}

		protected override void OnRemoved(int removeReason)
		{
			base.OnRemoved(removeReason);
			if (this.pathEntity != null)
			{
				this.pathEntity.Remove(removeReason);
			}
		}

		protected internal override void OnEditorTick(float dt)
		{
			base.OnEditorTick(dt);
			if (base.Scene.IsEntityFrameChanged(base.GameEntity.Name))
			{
				this.UpdatePathMesh();
			}
		}

		protected internal override void OnEditorVariableChanged(string variableName)
		{
			base.OnEditorVariableChanged(variableName);
			if (variableName == MBGlobals.GetMemberName<string>(Expression.Lambda<Func<string>>(Expression.Field(Expression.Constant(this, typeof(RoadStart)), fieldof(RoadStart.materialName)), Array.Empty<ParameterExpression>())))
			{
				this.UpdatePathMesh();
			}
			if (this.pathMesh != null)
			{
				this.pathMesh.SetVectorArgument2(this.textureSweepX, this.textureSweepY, 0f, 0f);
			}
		}

		private void UpdatePathMesh()
		{
			this.pathEntity.ClearComponents();
			this.pathMesh = MetaMesh.CreateMetaMesh(null);
			Material fromResource = Material.GetFromResource(this.materialName);
			if (fromResource != null)
			{
				this.pathMesh.SetMaterial(fromResource);
			}
			else
			{
				this.pathMesh.SetMaterial(Material.GetDefaultMaterial());
			}
			this.pathEntity.AddMultiMesh(this.pathMesh, true);
			this.pathMesh.SetVectorArgument2(this.textureSweepX, this.textureSweepY, 0f, 0f);
		}

		protected internal override bool MovesEntity()
		{
			return false;
		}

		public float textureSweepX;

		public float textureSweepY;

		public string materialName = "blood_decal_terrain_material";

		private GameEntity pathEntity;

		private MetaMesh pathMesh;
	}
}
