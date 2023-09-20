using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x0200008B RID: 139
	public sealed class Texture : Resource
	{
		// Token: 0x06000A91 RID: 2705 RVA: 0x0000B91A File Offset: 0x00009B1A
		private Texture()
		{
		}

		// Token: 0x06000A92 RID: 2706 RVA: 0x0000B922 File Offset: 0x00009B22
		internal Texture(UIntPtr ptr)
			: base(ptr)
		{
		}

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x06000A93 RID: 2707 RVA: 0x0000B92B File Offset: 0x00009B2B
		public int Width
		{
			get
			{
				return EngineApplicationInterface.ITexture.GetWidth(base.Pointer);
			}
		}

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x06000A94 RID: 2708 RVA: 0x0000B93D File Offset: 0x00009B3D
		public int Height
		{
			get
			{
				return EngineApplicationInterface.ITexture.GetHeight(base.Pointer);
			}
		}

		// Token: 0x17000083 RID: 131
		// (get) Token: 0x06000A95 RID: 2709 RVA: 0x0000B94F File Offset: 0x00009B4F
		public int MemorySize
		{
			get
			{
				return EngineApplicationInterface.ITexture.GetMemorySize(base.Pointer);
			}
		}

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x06000A96 RID: 2710 RVA: 0x0000B961 File Offset: 0x00009B61
		public bool IsRenderTarget
		{
			get
			{
				return EngineApplicationInterface.ITexture.IsRenderTarget(base.Pointer);
			}
		}

		// Token: 0x06000A97 RID: 2711 RVA: 0x0000B973 File Offset: 0x00009B73
		public static Texture CreateTextureFromPath(PlatformFilePath filePath)
		{
			return EngineApplicationInterface.ITexture.CreateTextureFromPath(filePath);
		}

		// Token: 0x17000085 RID: 133
		// (get) Token: 0x06000A98 RID: 2712 RVA: 0x0000B980 File Offset: 0x00009B80
		// (set) Token: 0x06000A99 RID: 2713 RVA: 0x0000B992 File Offset: 0x00009B92
		public string Name
		{
			get
			{
				return EngineApplicationInterface.ITexture.GetName(base.Pointer);
			}
			set
			{
				EngineApplicationInterface.ITexture.SetName(base.Pointer, value);
			}
		}

		// Token: 0x06000A9A RID: 2714 RVA: 0x0000B9A5 File Offset: 0x00009BA5
		public void TransformRenderTargetToResource(string name)
		{
			EngineApplicationInterface.ITexture.TransformRenderTargetToResourceTexture(base.Pointer, name);
		}

		// Token: 0x06000A9B RID: 2715 RVA: 0x0000B9B8 File Offset: 0x00009BB8
		public static Texture GetFromResource(string resourceName)
		{
			return EngineApplicationInterface.ITexture.GetFromResource(resourceName);
		}

		// Token: 0x06000A9C RID: 2716 RVA: 0x0000B9C5 File Offset: 0x00009BC5
		public bool IsLoaded()
		{
			return EngineApplicationInterface.ITexture.IsLoaded(base.Pointer);
		}

		// Token: 0x06000A9D RID: 2717 RVA: 0x0000B9D7 File Offset: 0x00009BD7
		public static Texture CheckAndGetFromResource(string resourceName)
		{
			return EngineApplicationInterface.ITexture.CheckAndGetFromResource(resourceName);
		}

		// Token: 0x06000A9E RID: 2718 RVA: 0x0000B9E4 File Offset: 0x00009BE4
		public static void ScaleTextureWithRatio(ref int tableauSizeX, ref int tableauSizeY)
		{
			float num = (float)tableauSizeX;
			float num2 = (float)tableauSizeY;
			int num3 = (int)MathF.Log(num, 2f) + 2;
			float num4 = MathF.Pow(2f, (float)num3) / num;
			tableauSizeX = (int)(num * num4);
			tableauSizeY = (int)(num2 * num4);
		}

		// Token: 0x06000A9F RID: 2719 RVA: 0x0000BA23 File Offset: 0x00009C23
		public void PreloadTexture(bool blocking)
		{
			EngineApplicationInterface.ITexture.GetCurObject(base.Pointer, blocking);
		}

		// Token: 0x06000AA0 RID: 2720 RVA: 0x0000BA36 File Offset: 0x00009C36
		public void Release()
		{
			this.RenderTargetComponent.OnTargetReleased();
			EngineApplicationInterface.ITexture.Release(base.Pointer);
		}

		// Token: 0x06000AA1 RID: 2721 RVA: 0x0000BA53 File Offset: 0x00009C53
		public void ReleaseNextFrame()
		{
			this.RenderTargetComponent.OnTargetReleased();
			EngineApplicationInterface.ITexture.ReleaseNextFrame(base.Pointer);
		}

		// Token: 0x06000AA2 RID: 2722 RVA: 0x0000BA70 File Offset: 0x00009C70
		public static Texture LoadTextureFromPath(string fileName, string folder)
		{
			return EngineApplicationInterface.ITexture.LoadTextureFromPath(fileName, folder);
		}

		// Token: 0x06000AA3 RID: 2723 RVA: 0x0000BA7E File Offset: 0x00009C7E
		public static Texture CreateDepthTarget(string name, int width, int height)
		{
			return EngineApplicationInterface.ITexture.CreateDepthTarget(name, width, height);
		}

		// Token: 0x06000AA4 RID: 2724 RVA: 0x0000BA8D File Offset: 0x00009C8D
		public static Texture CreateFromByteArray(byte[] data, int width, int height)
		{
			return EngineApplicationInterface.ITexture.CreateFromByteArray(data, width, height);
		}

		// Token: 0x06000AA5 RID: 2725 RVA: 0x0000BA9C File Offset: 0x00009C9C
		public void SaveToFile(string path)
		{
			EngineApplicationInterface.ITexture.SaveToFile(base.Pointer, path);
		}

		// Token: 0x06000AA6 RID: 2726 RVA: 0x0000BAAF File Offset: 0x00009CAF
		public void SetTextureAsAlwaysValid()
		{
			EngineApplicationInterface.ITexture.SaveTextureAsAlwaysValid(base.Pointer);
		}

		// Token: 0x06000AA7 RID: 2727 RVA: 0x0000BAC1 File Offset: 0x00009CC1
		public static Texture CreateFromMemory(byte[] data)
		{
			return EngineApplicationInterface.ITexture.CreateFromMemory(data);
		}

		// Token: 0x06000AA8 RID: 2728 RVA: 0x0000BACE File Offset: 0x00009CCE
		public static void ReleaseGpuMemories()
		{
			EngineApplicationInterface.ITexture.ReleaseGpuMemories();
		}

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x06000AA9 RID: 2729 RVA: 0x0000BADA File Offset: 0x00009CDA
		public RenderTargetComponent RenderTargetComponent
		{
			get
			{
				return EngineApplicationInterface.ITexture.GetRenderTargetComponent(base.Pointer);
			}
		}

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x06000AAA RID: 2730 RVA: 0x0000BAEC File Offset: 0x00009CEC
		public TableauView TableauView
		{
			get
			{
				return EngineApplicationInterface.ITexture.GetTableauView(base.Pointer);
			}
		}

		// Token: 0x17000088 RID: 136
		// (get) Token: 0x06000AAB RID: 2731 RVA: 0x0000BAFE File Offset: 0x00009CFE
		public object UserData
		{
			get
			{
				return this.RenderTargetComponent.UserData;
			}
		}

		// Token: 0x06000AAC RID: 2732 RVA: 0x0000BB0B File Offset: 0x00009D0B
		private void SetTableauView(TableauView tableauView)
		{
			EngineApplicationInterface.ITexture.SetTableauView(base.Pointer, tableauView.Pointer);
		}

		// Token: 0x06000AAD RID: 2733 RVA: 0x0000BB24 File Offset: 0x00009D24
		public static Texture CreateTableauTexture(string name, RenderTargetComponent.TextureUpdateEventHandler eventHandler, object objectRef, int tableauSizeX, int tableauSizeY)
		{
			Texture texture = Texture.CreateRenderTarget(name, tableauSizeX, tableauSizeY, true, false, false, false);
			RenderTargetComponent renderTargetComponent = texture.RenderTargetComponent;
			renderTargetComponent.PaintNeeded += eventHandler;
			renderTargetComponent.UserData = objectRef;
			TableauView tableauView = TableauView.CreateTableauView();
			tableauView.SetRenderTarget(texture);
			tableauView.SetAutoDepthTargetCreation(true);
			tableauView.SetSceneUsesSkybox(false);
			tableauView.SetClearColor(4294902015U);
			texture.SetTableauView(tableauView);
			return texture;
		}

		// Token: 0x06000AAE RID: 2734 RVA: 0x0000BB80 File Offset: 0x00009D80
		public static Texture CreateRenderTarget(string name, int width, int height, bool autoMipmaps, bool isTableau, bool createUninitialized = false, bool always_valid = false)
		{
			return EngineApplicationInterface.ITexture.CreateRenderTarget(name, width, height, autoMipmaps, isTableau, createUninitialized, always_valid);
		}
	}
}
