using System;
using System.Linq.Expressions;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000344 RID: 836
	public class RoadStart : ScriptComponentBehavior
	{
		// Token: 0x06002C93 RID: 11411 RVA: 0x000ACE36 File Offset: 0x000AB036
		protected internal override void OnInit()
		{
			this.pathEntity = GameEntity.CreateEmpty(base.Scene, false);
			this.pathEntity.Name = "Road_Entity";
			this.UpdatePathMesh();
		}

		// Token: 0x06002C94 RID: 11412 RVA: 0x000ACE60 File Offset: 0x000AB060
		protected internal override void OnEditorInit()
		{
			this.OnInit();
		}

		// Token: 0x06002C95 RID: 11413 RVA: 0x000ACE68 File Offset: 0x000AB068
		protected override void OnRemoved(int removeReason)
		{
			base.OnRemoved(removeReason);
			if (this.pathEntity != null)
			{
				this.pathEntity.Remove(removeReason);
			}
		}

		// Token: 0x06002C96 RID: 11414 RVA: 0x000ACE8B File Offset: 0x000AB08B
		protected internal override void OnEditorTick(float dt)
		{
			base.OnEditorTick(dt);
			if (base.Scene.IsEntityFrameChanged(base.GameEntity.Name))
			{
				this.UpdatePathMesh();
			}
		}

		// Token: 0x06002C97 RID: 11415 RVA: 0x000ACEB4 File Offset: 0x000AB0B4
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

		// Token: 0x06002C98 RID: 11416 RVA: 0x000ACF34 File Offset: 0x000AB134
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

		// Token: 0x06002C99 RID: 11417 RVA: 0x000ACFBE File Offset: 0x000AB1BE
		protected internal override bool MovesEntity()
		{
			return false;
		}

		// Token: 0x04001100 RID: 4352
		public float textureSweepX;

		// Token: 0x04001101 RID: 4353
		public float textureSweepY;

		// Token: 0x04001102 RID: 4354
		public string materialName = "blood_decal_terrain_material";

		// Token: 0x04001103 RID: 4355
		private GameEntity pathEntity;

		// Token: 0x04001104 RID: 4356
		private MetaMesh pathMesh;
	}
}
