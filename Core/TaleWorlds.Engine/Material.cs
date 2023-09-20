using System;

namespace TaleWorlds.Engine
{
	// Token: 0x02000059 RID: 89
	public sealed class Material : Resource
	{
		// Token: 0x0600074B RID: 1867 RVA: 0x000069C0 File Offset: 0x00004BC0
		public static Material GetDefaultMaterial()
		{
			return EngineApplicationInterface.IMaterial.GetDefaultMaterial();
		}

		// Token: 0x0600074C RID: 1868 RVA: 0x000069CC File Offset: 0x00004BCC
		public static Material GetOutlineMaterial(Mesh mesh)
		{
			return EngineApplicationInterface.IMaterial.GetOutlineMaterial(mesh.GetMaterial().Pointer);
		}

		// Token: 0x0600074D RID: 1869 RVA: 0x000069E3 File Offset: 0x00004BE3
		public static Material GetDefaultTableauSampleMaterial(bool transparency)
		{
			if (!transparency)
			{
				return Material.GetFromResource("sample_shield_matte");
			}
			return Material.GetFromResource("tableau_with_transparency");
		}

		// Token: 0x0600074E RID: 1870 RVA: 0x00006A00 File Offset: 0x00004C00
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

		// Token: 0x0600074F RID: 1871 RVA: 0x00006B02 File Offset: 0x00004D02
		internal Material(UIntPtr sourceMaterialPointer)
			: base(sourceMaterialPointer)
		{
		}

		// Token: 0x06000750 RID: 1872 RVA: 0x00006B0B File Offset: 0x00004D0B
		public Material CreateCopy()
		{
			return EngineApplicationInterface.IMaterial.CreateCopy(base.Pointer);
		}

		// Token: 0x06000751 RID: 1873 RVA: 0x00006B1D File Offset: 0x00004D1D
		public static Material GetFromResource(string materialName)
		{
			return EngineApplicationInterface.IMaterial.GetFromResource(materialName);
		}

		// Token: 0x06000752 RID: 1874 RVA: 0x00006B2A File Offset: 0x00004D2A
		public void SetShader(Shader shader)
		{
			EngineApplicationInterface.IMaterial.SetShader(base.Pointer, shader.Pointer);
		}

		// Token: 0x06000753 RID: 1875 RVA: 0x00006B42 File Offset: 0x00004D42
		public Shader GetShader()
		{
			return EngineApplicationInterface.IMaterial.GetShader(base.Pointer);
		}

		// Token: 0x06000754 RID: 1876 RVA: 0x00006B54 File Offset: 0x00004D54
		public ulong GetShaderFlags()
		{
			return EngineApplicationInterface.IMaterial.GetShaderFlags(base.Pointer);
		}

		// Token: 0x06000755 RID: 1877 RVA: 0x00006B66 File Offset: 0x00004D66
		public void SetShaderFlags(ulong flagEntry)
		{
			EngineApplicationInterface.IMaterial.SetShaderFlags(base.Pointer, flagEntry);
		}

		// Token: 0x06000756 RID: 1878 RVA: 0x00006B79 File Offset: 0x00004D79
		public void SetMeshVectorArgument(float x, float y, float z, float w)
		{
			EngineApplicationInterface.IMaterial.SetMeshVectorArgument(base.Pointer, x, y, z, w);
		}

		// Token: 0x06000757 RID: 1879 RVA: 0x00006B90 File Offset: 0x00004D90
		public void SetTexture(Material.MBTextureType textureType, Texture texture)
		{
			EngineApplicationInterface.IMaterial.SetTexture(base.Pointer, (int)textureType, texture.Pointer);
		}

		// Token: 0x06000758 RID: 1880 RVA: 0x00006BA9 File Offset: 0x00004DA9
		public void SetTextureAtSlot(int textureSlot, Texture texture)
		{
			EngineApplicationInterface.IMaterial.SetTextureAtSlot(base.Pointer, textureSlot, texture.Pointer);
		}

		// Token: 0x06000759 RID: 1881 RVA: 0x00006BC2 File Offset: 0x00004DC2
		public void SetAreaMapScale(float scale)
		{
			EngineApplicationInterface.IMaterial.SetAreaMapScale(base.Pointer, scale);
		}

		// Token: 0x0600075A RID: 1882 RVA: 0x00006BD5 File Offset: 0x00004DD5
		public void SetEnableSkinning(bool enable)
		{
			EngineApplicationInterface.IMaterial.SetEnableSkinning(base.Pointer, enable);
		}

		// Token: 0x0600075B RID: 1883 RVA: 0x00006BE8 File Offset: 0x00004DE8
		public bool UsingSkinning()
		{
			return EngineApplicationInterface.IMaterial.UsingSkinning(base.Pointer);
		}

		// Token: 0x0600075C RID: 1884 RVA: 0x00006BFA File Offset: 0x00004DFA
		public Texture GetTexture(Material.MBTextureType textureType)
		{
			return EngineApplicationInterface.IMaterial.GetTexture(base.Pointer, (int)textureType);
		}

		// Token: 0x0600075D RID: 1885 RVA: 0x00006C0D File Offset: 0x00004E0D
		public Texture GetTextureWithSlot(int textureSlot)
		{
			return EngineApplicationInterface.IMaterial.GetTexture(base.Pointer, textureSlot);
		}

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x0600075E RID: 1886 RVA: 0x00006C20 File Offset: 0x00004E20
		// (set) Token: 0x0600075F RID: 1887 RVA: 0x00006C32 File Offset: 0x00004E32
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

		// Token: 0x06000760 RID: 1888 RVA: 0x00006C45 File Offset: 0x00004E45
		public static Material GetAlphaMaskTableauMaterial()
		{
			return EngineApplicationInterface.IMaterial.GetFromResource("tableau_with_alpha_mask");
		}

		// Token: 0x06000761 RID: 1889 RVA: 0x00006C56 File Offset: 0x00004E56
		public Material.MBAlphaBlendMode GetAlphaBlendMode()
		{
			return (Material.MBAlphaBlendMode)EngineApplicationInterface.IMaterial.GetAlphaBlendMode(base.Pointer);
		}

		// Token: 0x06000762 RID: 1890 RVA: 0x00006C69 File Offset: 0x00004E69
		public void SetAlphaBlendMode(Material.MBAlphaBlendMode alphaBlendMode)
		{
			EngineApplicationInterface.IMaterial.SetAlphaBlendMode(base.Pointer, (int)alphaBlendMode);
		}

		// Token: 0x06000763 RID: 1891 RVA: 0x00006C7C File Offset: 0x00004E7C
		public void SetAlphaTestValue(float alphaTestValue)
		{
			EngineApplicationInterface.IMaterial.SetAlphaTestValue(base.Pointer, alphaTestValue);
		}

		// Token: 0x06000764 RID: 1892 RVA: 0x00006C8F File Offset: 0x00004E8F
		public float GetAlphaTestValue()
		{
			return EngineApplicationInterface.IMaterial.GetAlphaTestValue(base.Pointer);
		}

		// Token: 0x06000765 RID: 1893 RVA: 0x00006CA1 File Offset: 0x00004EA1
		private bool CheckMaterialShaderFlag(Material.MBMaterialShaderFlags flagEntry)
		{
			return (EngineApplicationInterface.IMaterial.GetShaderFlags(base.Pointer) & (ulong)((long)flagEntry)) > 0UL;
		}

		// Token: 0x06000766 RID: 1894 RVA: 0x00006CBC File Offset: 0x00004EBC
		private void SetMaterialShaderFlag(Material.MBMaterialShaderFlags flagEntry, bool value)
		{
			ulong num = (EngineApplicationInterface.IMaterial.GetShaderFlags(base.Pointer) & (ulong)(~(ulong)((long)flagEntry))) | (ulong)((long)flagEntry & (value ? 255L : 0L));
			EngineApplicationInterface.IMaterial.SetShaderFlags(base.Pointer, num);
		}

		// Token: 0x06000767 RID: 1895 RVA: 0x00006D00 File Offset: 0x00004F00
		public void AddMaterialShaderFlag(string flagName, bool showErrors)
		{
			EngineApplicationInterface.IMaterial.AddMaterialShaderFlag(base.Pointer, flagName, showErrors);
		}

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x06000768 RID: 1896 RVA: 0x00006D14 File Offset: 0x00004F14
		// (set) Token: 0x06000769 RID: 1897 RVA: 0x00006D1D File Offset: 0x00004F1D
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

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x0600076A RID: 1898 RVA: 0x00006D27 File Offset: 0x00004F27
		// (set) Token: 0x0600076B RID: 1899 RVA: 0x00006D30 File Offset: 0x00004F30
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

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x0600076C RID: 1900 RVA: 0x00006D3A File Offset: 0x00004F3A
		// (set) Token: 0x0600076D RID: 1901 RVA: 0x00006D43 File Offset: 0x00004F43
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

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x0600076E RID: 1902 RVA: 0x00006D4D File Offset: 0x00004F4D
		// (set) Token: 0x0600076F RID: 1903 RVA: 0x00006D5A File Offset: 0x00004F5A
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

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x06000770 RID: 1904 RVA: 0x00006D68 File Offset: 0x00004F68
		// (set) Token: 0x06000771 RID: 1905 RVA: 0x00006D72 File Offset: 0x00004F72
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

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x06000772 RID: 1906 RVA: 0x00006D7D File Offset: 0x00004F7D
		// (set) Token: 0x06000773 RID: 1907 RVA: 0x00006D87 File Offset: 0x00004F87
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

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x06000774 RID: 1908 RVA: 0x00006D92 File Offset: 0x00004F92
		// (set) Token: 0x06000775 RID: 1909 RVA: 0x00006D9F File Offset: 0x00004F9F
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

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x06000776 RID: 1910 RVA: 0x00006DAD File Offset: 0x00004FAD
		// (set) Token: 0x06000777 RID: 1911 RVA: 0x00006DBA File Offset: 0x00004FBA
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

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x06000778 RID: 1912 RVA: 0x00006DC8 File Offset: 0x00004FC8
		// (set) Token: 0x06000779 RID: 1913 RVA: 0x00006DD5 File Offset: 0x00004FD5
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

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x0600077A RID: 1914 RVA: 0x00006DE3 File Offset: 0x00004FE3
		// (set) Token: 0x0600077B RID: 1915 RVA: 0x00006DF0 File Offset: 0x00004FF0
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

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x0600077C RID: 1916 RVA: 0x00006DFE File Offset: 0x00004FFE
		// (set) Token: 0x0600077D RID: 1917 RVA: 0x00006E0B File Offset: 0x0000500B
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

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x0600077E RID: 1918 RVA: 0x00006E19 File Offset: 0x00005019
		// (set) Token: 0x0600077F RID: 1919 RVA: 0x00006E26 File Offset: 0x00005026
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

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x06000780 RID: 1920 RVA: 0x00006E34 File Offset: 0x00005034
		// (set) Token: 0x06000781 RID: 1921 RVA: 0x00006E46 File Offset: 0x00005046
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

		// Token: 0x020000B8 RID: 184
		public enum MBTextureType
		{
			// Token: 0x040003B9 RID: 953
			DiffuseMap,
			// Token: 0x040003BA RID: 954
			DiffuseMap2,
			// Token: 0x040003BB RID: 955
			BumpMap,
			// Token: 0x040003BC RID: 956
			EnvironmentMap,
			// Token: 0x040003BD RID: 957
			SpecularMap
		}

		// Token: 0x020000B9 RID: 185
		public enum MBAlphaBlendMode : byte
		{
			// Token: 0x040003BF RID: 959
			None,
			// Token: 0x040003C0 RID: 960
			Modulate,
			// Token: 0x040003C1 RID: 961
			AddAlpha,
			// Token: 0x040003C2 RID: 962
			Multiply,
			// Token: 0x040003C3 RID: 963
			Add,
			// Token: 0x040003C4 RID: 964
			Max,
			// Token: 0x040003C5 RID: 965
			Factor,
			// Token: 0x040003C6 RID: 966
			AddModulateCombined,
			// Token: 0x040003C7 RID: 967
			NoAlphaBlendNoWrite,
			// Token: 0x040003C8 RID: 968
			ModulateNoWrite,
			// Token: 0x040003C9 RID: 969
			GbufferAlphaBlend,
			// Token: 0x040003CA RID: 970
			GbufferAlphaBlendwithVtResolve
		}

		// Token: 0x020000BA RID: 186
		[Flags]
		private enum MBMaterialShaderFlags
		{
			// Token: 0x040003CC RID: 972
			UseSpecular = 1,
			// Token: 0x040003CD RID: 973
			UseSpecularMap = 2,
			// Token: 0x040003CE RID: 974
			UseHemisphericalAmbient = 4,
			// Token: 0x040003CF RID: 975
			UseEnvironmentMap = 8,
			// Token: 0x040003D0 RID: 976
			UseDXT5Normal = 16,
			// Token: 0x040003D1 RID: 977
			UseDynamicLight = 32,
			// Token: 0x040003D2 RID: 978
			UseSunLight = 64,
			// Token: 0x040003D3 RID: 979
			UseSpecularAlpha = 128,
			// Token: 0x040003D4 RID: 980
			UseFresnel = 256,
			// Token: 0x040003D5 RID: 981
			SunShadowReceiver = 512,
			// Token: 0x040003D6 RID: 982
			DynamicShadowReceiver = 1024,
			// Token: 0x040003D7 RID: 983
			UseDiffuseAlphaMap = 2048,
			// Token: 0x040003D8 RID: 984
			UseParallaxMapping = 4096,
			// Token: 0x040003D9 RID: 985
			UseParallaxOcclusion = 8192,
			// Token: 0x040003DA RID: 986
			UseAlphaTestingBit0 = 16384,
			// Token: 0x040003DB RID: 987
			UseAlphaTestingBit1 = 32768,
			// Token: 0x040003DC RID: 988
			UseAreaMap = 65536,
			// Token: 0x040003DD RID: 989
			UseDetailNormalMap = 131072,
			// Token: 0x040003DE RID: 990
			UseGroundSlopeAlpha = 262144,
			// Token: 0x040003DF RID: 991
			UseSelfIllumination = 524288,
			// Token: 0x040003E0 RID: 992
			UseColorMapping = 1048576,
			// Token: 0x040003E1 RID: 993
			UseCubicAmbient = 2097152
		}
	}
}
