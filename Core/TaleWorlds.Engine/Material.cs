using System;

namespace TaleWorlds.Engine
{
	public sealed class Material : Resource
	{
		public static Material GetDefaultMaterial()
		{
			return EngineApplicationInterface.IMaterial.GetDefaultMaterial();
		}

		public static Material GetOutlineMaterial(Mesh mesh)
		{
			return EngineApplicationInterface.IMaterial.GetOutlineMaterial(mesh.GetMaterial().Pointer);
		}

		public static Material GetDefaultTableauSampleMaterial(bool transparency)
		{
			if (!transparency)
			{
				return Material.GetFromResource("sample_shield_matte");
			}
			return Material.GetFromResource("tableau_with_transparency");
		}

		public static Material CreateTableauMaterial(RenderTargetComponent.TextureUpdateEventHandler eventHandler, object objectRef, Material sampleMaterial, int tableauSizeX, int tableauSizeY, bool continuousTableau = false)
		{
			if (sampleMaterial == null)
			{
				sampleMaterial = Material.GetDefaultTableauSampleMaterial(true);
			}
			Material material = sampleMaterial.CreateCopy();
			uint num = (uint)material.GetShader().GetMaterialShaderFlagMask("use_tableau_blending", true);
			ulong shaderFlags = material.GetShaderFlags();
			material.SetShaderFlags(shaderFlags | (ulong)num);
			string text = "";
			Type type = objectRef.GetType();
			MaterialCacheIDGetMethodDelegate materialCacheIDGetMethodDelegate;
			if (!continuousTableau && HasTableauCache.TableauCacheTypes.TryGetValue(type, out materialCacheIDGetMethodDelegate))
			{
				text = materialCacheIDGetMethodDelegate(objectRef);
				text = text.ToLower();
				Texture texture = Texture.CheckAndGetFromResource(text);
				if (texture != null)
				{
					material.SetTexture(Material.MBTextureType.DiffuseMap2, texture);
					return material;
				}
			}
			if (text != "")
			{
				Texture.ScaleTextureWithRatio(ref tableauSizeX, ref tableauSizeY);
			}
			Texture texture2 = Texture.CreateTableauTexture(text, eventHandler, objectRef, tableauSizeX, tableauSizeY);
			if (text != "")
			{
				TableauView tableauView = texture2.TableauView;
				tableauView.SetSaveFinalResultToDisk(true);
				tableauView.SetFileNameToSaveResult(text);
				tableauView.SetFileTypeToSave(View.TextureSaveFormat.TextureTypeDds);
			}
			if (text != "")
			{
				texture2.TransformRenderTargetToResource(text);
			}
			material.SetTexture(Material.MBTextureType.DiffuseMap2, texture2);
			return material;
		}

		internal Material(UIntPtr sourceMaterialPointer)
			: base(sourceMaterialPointer)
		{
		}

		public Material CreateCopy()
		{
			return EngineApplicationInterface.IMaterial.CreateCopy(base.Pointer);
		}

		public static Material GetFromResource(string materialName)
		{
			return EngineApplicationInterface.IMaterial.GetFromResource(materialName);
		}

		public void SetShader(Shader shader)
		{
			EngineApplicationInterface.IMaterial.SetShader(base.Pointer, shader.Pointer);
		}

		public Shader GetShader()
		{
			return EngineApplicationInterface.IMaterial.GetShader(base.Pointer);
		}

		public ulong GetShaderFlags()
		{
			return EngineApplicationInterface.IMaterial.GetShaderFlags(base.Pointer);
		}

		public void SetShaderFlags(ulong flagEntry)
		{
			EngineApplicationInterface.IMaterial.SetShaderFlags(base.Pointer, flagEntry);
		}

		public void SetMeshVectorArgument(float x, float y, float z, float w)
		{
			EngineApplicationInterface.IMaterial.SetMeshVectorArgument(base.Pointer, x, y, z, w);
		}

		public void SetTexture(Material.MBTextureType textureType, Texture texture)
		{
			EngineApplicationInterface.IMaterial.SetTexture(base.Pointer, (int)textureType, texture.Pointer);
		}

		public void SetTextureAtSlot(int textureSlot, Texture texture)
		{
			EngineApplicationInterface.IMaterial.SetTextureAtSlot(base.Pointer, textureSlot, texture.Pointer);
		}

		public void SetAreaMapScale(float scale)
		{
			EngineApplicationInterface.IMaterial.SetAreaMapScale(base.Pointer, scale);
		}

		public void SetEnableSkinning(bool enable)
		{
			EngineApplicationInterface.IMaterial.SetEnableSkinning(base.Pointer, enable);
		}

		public bool UsingSkinning()
		{
			return EngineApplicationInterface.IMaterial.UsingSkinning(base.Pointer);
		}

		public Texture GetTexture(Material.MBTextureType textureType)
		{
			return EngineApplicationInterface.IMaterial.GetTexture(base.Pointer, (int)textureType);
		}

		public Texture GetTextureWithSlot(int textureSlot)
		{
			return EngineApplicationInterface.IMaterial.GetTexture(base.Pointer, textureSlot);
		}

		public string Name
		{
			get
			{
				return EngineApplicationInterface.IMaterial.GetName(base.Pointer);
			}
			set
			{
				EngineApplicationInterface.IMaterial.SetName(base.Pointer, value);
			}
		}

		public static Material GetAlphaMaskTableauMaterial()
		{
			return EngineApplicationInterface.IMaterial.GetFromResource("tableau_with_alpha_mask");
		}

		public Material.MBAlphaBlendMode GetAlphaBlendMode()
		{
			return (Material.MBAlphaBlendMode)EngineApplicationInterface.IMaterial.GetAlphaBlendMode(base.Pointer);
		}

		public void SetAlphaBlendMode(Material.MBAlphaBlendMode alphaBlendMode)
		{
			EngineApplicationInterface.IMaterial.SetAlphaBlendMode(base.Pointer, (int)alphaBlendMode);
		}

		public void SetAlphaTestValue(float alphaTestValue)
		{
			EngineApplicationInterface.IMaterial.SetAlphaTestValue(base.Pointer, alphaTestValue);
		}

		public float GetAlphaTestValue()
		{
			return EngineApplicationInterface.IMaterial.GetAlphaTestValue(base.Pointer);
		}

		private bool CheckMaterialShaderFlag(Material.MBMaterialShaderFlags flagEntry)
		{
			return (EngineApplicationInterface.IMaterial.GetShaderFlags(base.Pointer) & (ulong)((long)flagEntry)) > 0UL;
		}

		private void SetMaterialShaderFlag(Material.MBMaterialShaderFlags flagEntry, bool value)
		{
			ulong num = (EngineApplicationInterface.IMaterial.GetShaderFlags(base.Pointer) & (ulong)(~(ulong)((long)flagEntry))) | (ulong)((long)flagEntry & (value ? 255L : 0L));
			EngineApplicationInterface.IMaterial.SetShaderFlags(base.Pointer, num);
		}

		public void AddMaterialShaderFlag(string flagName, bool showErrors)
		{
			EngineApplicationInterface.IMaterial.AddMaterialShaderFlag(base.Pointer, flagName, showErrors);
		}

		public bool UsingSpecular
		{
			get
			{
				return this.CheckMaterialShaderFlag(Material.MBMaterialShaderFlags.UseSpecular);
			}
			set
			{
				this.SetMaterialShaderFlag(Material.MBMaterialShaderFlags.UseSpecular, value);
			}
		}

		public bool UsingSpecularMap
		{
			get
			{
				return this.CheckMaterialShaderFlag(Material.MBMaterialShaderFlags.UseSpecularMap);
			}
			set
			{
				this.SetMaterialShaderFlag(Material.MBMaterialShaderFlags.UseSpecularMap, value);
			}
		}

		public bool UsingEnvironmentMap
		{
			get
			{
				return this.CheckMaterialShaderFlag(Material.MBMaterialShaderFlags.UseEnvironmentMap);
			}
			set
			{
				this.SetMaterialShaderFlag(Material.MBMaterialShaderFlags.UseEnvironmentMap, value);
			}
		}

		public bool UsingSpecularAlpha
		{
			get
			{
				return this.CheckMaterialShaderFlag(Material.MBMaterialShaderFlags.UseSpecularAlpha);
			}
			set
			{
				this.SetMaterialShaderFlag(Material.MBMaterialShaderFlags.UseSpecularAlpha, value);
			}
		}

		public bool UsingDynamicLight
		{
			get
			{
				return this.CheckMaterialShaderFlag(Material.MBMaterialShaderFlags.UseDynamicLight);
			}
			set
			{
				this.SetMaterialShaderFlag(Material.MBMaterialShaderFlags.UseDynamicLight, value);
			}
		}

		public bool UsingSunLight
		{
			get
			{
				return this.CheckMaterialShaderFlag(Material.MBMaterialShaderFlags.UseSunLight);
			}
			set
			{
				this.SetMaterialShaderFlag(Material.MBMaterialShaderFlags.UseSunLight, value);
			}
		}

		public bool UsingFresnel
		{
			get
			{
				return this.CheckMaterialShaderFlag(Material.MBMaterialShaderFlags.UseFresnel);
			}
			set
			{
				this.SetMaterialShaderFlag(Material.MBMaterialShaderFlags.UseFresnel, value);
			}
		}

		public bool IsSunShadowReceiver
		{
			get
			{
				return this.CheckMaterialShaderFlag(Material.MBMaterialShaderFlags.SunShadowReceiver);
			}
			set
			{
				this.SetMaterialShaderFlag(Material.MBMaterialShaderFlags.SunShadowReceiver, value);
			}
		}

		public bool IsDynamicShadowReceiver
		{
			get
			{
				return this.CheckMaterialShaderFlag(Material.MBMaterialShaderFlags.DynamicShadowReceiver);
			}
			set
			{
				this.SetMaterialShaderFlag(Material.MBMaterialShaderFlags.DynamicShadowReceiver, value);
			}
		}

		public bool UsingDiffuseAlphaMap
		{
			get
			{
				return this.CheckMaterialShaderFlag(Material.MBMaterialShaderFlags.UseDiffuseAlphaMap);
			}
			set
			{
				this.SetMaterialShaderFlag(Material.MBMaterialShaderFlags.UseDiffuseAlphaMap, value);
			}
		}

		public bool UsingParallaxMapping
		{
			get
			{
				return this.CheckMaterialShaderFlag(Material.MBMaterialShaderFlags.UseParallaxMapping);
			}
			set
			{
				this.SetMaterialShaderFlag(Material.MBMaterialShaderFlags.UseParallaxMapping, value);
			}
		}

		public bool UsingParallaxOcclusion
		{
			get
			{
				return this.CheckMaterialShaderFlag(Material.MBMaterialShaderFlags.UseParallaxOcclusion);
			}
			set
			{
				this.SetMaterialShaderFlag(Material.MBMaterialShaderFlags.UseParallaxOcclusion, value);
			}
		}

		public MaterialFlags Flags
		{
			get
			{
				return (MaterialFlags)EngineApplicationInterface.IMaterial.GetFlags(base.Pointer);
			}
			set
			{
				EngineApplicationInterface.IMaterial.SetFlags(base.Pointer, (uint)value);
			}
		}

		public enum MBTextureType
		{
			DiffuseMap,
			DiffuseMap2,
			BumpMap,
			EnvironmentMap,
			SpecularMap
		}

		public enum MBAlphaBlendMode : byte
		{
			None,
			Modulate,
			AddAlpha,
			Multiply,
			Add,
			Max,
			Factor,
			AddModulateCombined,
			NoAlphaBlendNoWrite,
			ModulateNoWrite,
			GbufferAlphaBlend,
			GbufferAlphaBlendwithVtResolve
		}

		[Flags]
		private enum MBMaterialShaderFlags
		{
			UseSpecular = 1,
			UseSpecularMap = 2,
			UseHemisphericalAmbient = 4,
			UseEnvironmentMap = 8,
			UseDXT5Normal = 16,
			UseDynamicLight = 32,
			UseSunLight = 64,
			UseSpecularAlpha = 128,
			UseFresnel = 256,
			SunShadowReceiver = 512,
			DynamicShadowReceiver = 1024,
			UseDiffuseAlphaMap = 2048,
			UseParallaxMapping = 4096,
			UseParallaxOcclusion = 8192,
			UseAlphaTestingBit0 = 16384,
			UseAlphaTestingBit1 = 32768,
			UseAreaMap = 65536,
			UseDetailNormalMap = 131072,
			UseGroundSlopeAlpha = 262144,
			UseSelfIllumination = 524288,
			UseColorMapping = 1048576,
			UseCubicAmbient = 2097152
		}
	}
}
