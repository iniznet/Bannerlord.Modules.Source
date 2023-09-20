using System;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	public class ScenePropDecal : ScriptComponentBehavior
	{
		protected internal override void OnInit()
		{
			base.OnInit();
			this.SetUpMaterial();
		}

		protected internal override void OnEditorInit()
		{
			base.OnEditorInit();
			this.SetUpMaterial();
		}

		protected internal override void OnEditorVariableChanged(string variableName)
		{
			base.OnEditorVariableChanged(variableName);
			this.SetUpMaterial();
		}

		private void EnsureUniqueMaterial()
		{
			Material fromResource = Material.GetFromResource(this.MaterialName);
			this.UniqueMaterial = fromResource.CreateCopy();
		}

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

		public string DiffuseTexture;

		public string NormalTexture;

		public string SpecularTexture;

		public string MaskTexture;

		public bool UseBaseNormals;

		public float TilingSize = 1f;

		public float TilingOffset;

		public float AlphaTestValue;

		public float TextureSweepX;

		public float TextureSweepY;

		public string MaterialName = "deferred_decal_material";

		protected Material UniqueMaterial;
	}
}
