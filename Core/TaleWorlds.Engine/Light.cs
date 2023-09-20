using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x02000050 RID: 80
	[EngineClass("rglLight")]
	public sealed class Light : GameEntityComponent
	{
		// Token: 0x1700002A RID: 42
		// (get) Token: 0x060006D6 RID: 1750 RVA: 0x00005013 File Offset: 0x00003213
		public bool IsValid
		{
			get
			{
				return base.Pointer != UIntPtr.Zero;
			}
		}

		// Token: 0x060006D7 RID: 1751 RVA: 0x00005025 File Offset: 0x00003225
		internal Light(UIntPtr pointer)
			: base(pointer)
		{
		}

		// Token: 0x060006D8 RID: 1752 RVA: 0x0000502E File Offset: 0x0000322E
		public static Light CreatePointLight(float lightRadius)
		{
			return EngineApplicationInterface.ILight.CreatePointLight(lightRadius);
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x060006D9 RID: 1753 RVA: 0x0000503C File Offset: 0x0000323C
		// (set) Token: 0x060006DA RID: 1754 RVA: 0x0000505C File Offset: 0x0000325C
		public MatrixFrame Frame
		{
			get
			{
				MatrixFrame matrixFrame;
				EngineApplicationInterface.ILight.GetFrame(base.Pointer, out matrixFrame);
				return matrixFrame;
			}
			set
			{
				EngineApplicationInterface.ILight.SetFrame(base.Pointer, ref value);
			}
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x060006DB RID: 1755 RVA: 0x00005070 File Offset: 0x00003270
		// (set) Token: 0x060006DC RID: 1756 RVA: 0x00005082 File Offset: 0x00003282
		public Vec3 LightColor
		{
			get
			{
				return EngineApplicationInterface.ILight.GetLightColor(base.Pointer);
			}
			set
			{
				EngineApplicationInterface.ILight.SetLightColor(base.Pointer, value);
			}
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x060006DD RID: 1757 RVA: 0x00005095 File Offset: 0x00003295
		// (set) Token: 0x060006DE RID: 1758 RVA: 0x000050A7 File Offset: 0x000032A7
		public float Intensity
		{
			get
			{
				return EngineApplicationInterface.ILight.GetIntensity(base.Pointer);
			}
			set
			{
				EngineApplicationInterface.ILight.SetIntensity(base.Pointer, value);
			}
		}

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x060006DF RID: 1759 RVA: 0x000050BA File Offset: 0x000032BA
		// (set) Token: 0x060006E0 RID: 1760 RVA: 0x000050CC File Offset: 0x000032CC
		public float Radius
		{
			get
			{
				return EngineApplicationInterface.ILight.GetRadius(base.Pointer);
			}
			set
			{
				EngineApplicationInterface.ILight.SetRadius(base.Pointer, value);
			}
		}

		// Token: 0x060006E1 RID: 1761 RVA: 0x000050DF File Offset: 0x000032DF
		public void SetShadowType(Light.ShadowType type)
		{
			EngineApplicationInterface.ILight.SetShadows(base.Pointer, (int)type);
		}

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x060006E2 RID: 1762 RVA: 0x000050F2 File Offset: 0x000032F2
		// (set) Token: 0x060006E3 RID: 1763 RVA: 0x00005104 File Offset: 0x00003304
		public bool ShadowEnabled
		{
			get
			{
				return EngineApplicationInterface.ILight.IsShadowEnabled(base.Pointer);
			}
			set
			{
				EngineApplicationInterface.ILight.EnableShadow(base.Pointer, value);
			}
		}

		// Token: 0x060006E4 RID: 1764 RVA: 0x00005117 File Offset: 0x00003317
		public void SetLightFlicker(float magnitude, float interval)
		{
			EngineApplicationInterface.ILight.SetLightFlicker(base.Pointer, magnitude, interval);
		}

		// Token: 0x060006E5 RID: 1765 RVA: 0x0000512B File Offset: 0x0000332B
		public void SetVolumetricProperties(bool volumetricLightEnabled, float volumeParameters)
		{
			EngineApplicationInterface.ILight.SetVolumetricProperties(base.Pointer, volumetricLightEnabled, volumeParameters);
		}

		// Token: 0x060006E6 RID: 1766 RVA: 0x0000513F File Offset: 0x0000333F
		public void Dispose()
		{
			if (this.IsValid)
			{
				this.Release();
				GC.SuppressFinalize(this);
			}
		}

		// Token: 0x060006E7 RID: 1767 RVA: 0x00005155 File Offset: 0x00003355
		public void SetVisibility(bool value)
		{
			EngineApplicationInterface.ILight.SetVisibility(base.Pointer, value);
		}

		// Token: 0x060006E8 RID: 1768 RVA: 0x00005168 File Offset: 0x00003368
		private void Release()
		{
			EngineApplicationInterface.ILight.Release(base.Pointer);
		}

		// Token: 0x060006E9 RID: 1769 RVA: 0x0000517C File Offset: 0x0000337C
		~Light()
		{
			this.Dispose();
		}

		// Token: 0x020000B7 RID: 183
		public enum ShadowType
		{
			// Token: 0x040003B4 RID: 948
			NoShadow,
			// Token: 0x040003B5 RID: 949
			StaticShadow,
			// Token: 0x040003B6 RID: 950
			DynamicShadow,
			// Token: 0x040003B7 RID: 951
			Count
		}
	}
}
