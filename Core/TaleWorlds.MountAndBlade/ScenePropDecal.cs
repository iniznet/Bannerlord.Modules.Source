using System;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000345 RID: 837
	public class ScenePropDecal : ScriptComponentBehavior
	{
		// Token: 0x06002C9B RID: 11419 RVA: 0x000ACFD4 File Offset: 0x000AB1D4
		protected internal override void OnInit()
		{
			base.OnInit();
			this.SetUpMaterial();
		}

		// Token: 0x06002C9C RID: 11420 RVA: 0x000ACFE2 File Offset: 0x000AB1E2
		protected internal override void OnEditorInit()
		{
			base.OnEditorInit();
			this.SetUpMaterial();
		}

		// Token: 0x06002C9D RID: 11421 RVA: 0x000ACFF0 File Offset: 0x000AB1F0
		protected internal override void OnEditorVariableChanged(string variableName)
		{
			base.OnEditorVariableChanged(variableName);
			this.SetUpMaterial();
		}

		// Token: 0x06002C9E RID: 11422 RVA: 0x000AD000 File Offset: 0x000AB200
		private void EnsureUniqueMaterial()
		{
			Material fromResource = Material.GetFromResource(this.MaterialName);
			this.UniqueMaterial = fromResource.CreateCopy();
		}

		// Token: 0x06002C9F RID: 11423 RVA: 0x000AD028 File Offset: 0x000AB228
		private void SetUpMaterial()
		{
			this.EnsureUniqueMaterial();
			Texture texture = Texture.CheckAndGetFromResource(this.DiffuseTexture);
			Texture texture2 = Texture.CheckAndGetFromResource(this.NormalTexture);
			Texture texture3 = Texture.CheckAndGetFromResource(this.SpecularTexture);
			Texture texture4 = Texture.CheckAndGetFromResource(this.MaskTexture);
			if (texture != null)
			{
				this.UniqueMaterial.SetTexture(Material.MBTextureType.DiffuseMap, texture);
			}
			if (texture2 != null)
			{
				this.UniqueMaterial.SetTexture(Material.MBTextureType.BumpMap, texture2);
			}
			if (texture3 != null)
			{
				this.UniqueMaterial.SetTexture(Material.MBTextureType.SpecularMap, texture3);
			}
			if (texture4 != null)
			{
				this.UniqueMaterial.SetTexture(Material.MBTextureType.DiffuseMap2, texture4);
				this.UniqueMaterial.AddMaterialShaderFlag("use_areamap", false);
			}
			this.UniqueMaterial.SetAlphaTestValue(this.AlphaTestValue);
			base.GameEntity.SetMaterialForAllMeshes(this.UniqueMaterial);
			Mesh firstMesh = base.GameEntity.GetFirstMesh();
			if (firstMesh != null)
			{
				if (this.UniqueMaterial != null)
				{
					firstMesh.SetVectorArgument(this.TilingSize, this.TilingSize, this.TilingOffset, this.TilingOffset);
				}
				firstMesh.SetVectorArgument2(this.TextureSweepX, this.TextureSweepY, 0f, this.UseBaseNormals ? 1f : 0f);
			}
		}

		// Token: 0x04001105 RID: 4357
		public string DiffuseTexture;

		// Token: 0x04001106 RID: 4358
		public string NormalTexture;

		// Token: 0x04001107 RID: 4359
		public string SpecularTexture;

		// Token: 0x04001108 RID: 4360
		public string MaskTexture;

		// Token: 0x04001109 RID: 4361
		public bool UseBaseNormals;

		// Token: 0x0400110A RID: 4362
		public float TilingSize = 1f;

		// Token: 0x0400110B RID: 4363
		public float TilingOffset;

		// Token: 0x0400110C RID: 4364
		public float AlphaTestValue;

		// Token: 0x0400110D RID: 4365
		public float TextureSweepX;

		// Token: 0x0400110E RID: 4366
		public float TextureSweepY;

		// Token: 0x0400110F RID: 4367
		public string MaterialName = "deferred_decal_material";

		// Token: 0x04001110 RID: 4368
		protected Material UniqueMaterial;
	}
}
