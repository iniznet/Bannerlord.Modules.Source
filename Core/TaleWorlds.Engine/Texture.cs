using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	public sealed class Texture : Resource
	{
		private Texture()
		{
		}

		internal Texture(UIntPtr ptr)
			: base(ptr)
		{
		}

		public int Width
		{
			get
			{
				return EngineApplicationInterface.ITexture.GetWidth(base.Pointer);
			}
		}

		public int Height
		{
			get
			{
				return EngineApplicationInterface.ITexture.GetHeight(base.Pointer);
			}
		}

		public int MemorySize
		{
			get
			{
				return EngineApplicationInterface.ITexture.GetMemorySize(base.Pointer);
			}
		}

		public bool IsRenderTarget
		{
			get
			{
				return EngineApplicationInterface.ITexture.IsRenderTarget(base.Pointer);
			}
		}

		public static Texture CreateTextureFromPath(PlatformFilePath filePath)
		{
			return EngineApplicationInterface.ITexture.CreateTextureFromPath(filePath);
		}

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

		public void TransformRenderTargetToResource(string name)
		{
			EngineApplicationInterface.ITexture.TransformRenderTargetToResourceTexture(base.Pointer, name);
		}

		public static Texture GetFromResource(string resourceName)
		{
			return EngineApplicationInterface.ITexture.GetFromResource(resourceName);
		}

		public bool IsLoaded()
		{
			return EngineApplicationInterface.ITexture.IsLoaded(base.Pointer);
		}

		public static Texture CheckAndGetFromResource(string resourceName)
		{
			return EngineApplicationInterface.ITexture.CheckAndGetFromResource(resourceName);
		}

		public static void ScaleTextureWithRatio(ref int tableauSizeX, ref int tableauSizeY)
		{
			float num = (float)tableauSizeX;
			float num2 = (float)tableauSizeY;
			int num3 = (int)MathF.Log(num, 2f) + 2;
			float num4 = MathF.Pow(2f, (float)num3) / num;
			tableauSizeX = (int)(num * num4);
			tableauSizeY = (int)(num2 * num4);
		}

		public void PreloadTexture(bool blocking)
		{
			EngineApplicationInterface.ITexture.GetCurObject(base.Pointer, blocking);
		}

		public void Release()
		{
			this.RenderTargetComponent.OnTargetReleased();
			EngineApplicationInterface.ITexture.Release(base.Pointer);
		}

		public void ReleaseNextFrame()
		{
			this.RenderTargetComponent.OnTargetReleased();
			EngineApplicationInterface.ITexture.ReleaseNextFrame(base.Pointer);
		}

		public static Texture LoadTextureFromPath(string fileName, string folder)
		{
			return EngineApplicationInterface.ITexture.LoadTextureFromPath(fileName, folder);
		}

		public static Texture CreateDepthTarget(string name, int width, int height)
		{
			return EngineApplicationInterface.ITexture.CreateDepthTarget(name, width, height);
		}

		public static Texture CreateFromByteArray(byte[] data, int width, int height)
		{
			return EngineApplicationInterface.ITexture.CreateFromByteArray(data, width, height);
		}

		public void SaveToFile(string path)
		{
			EngineApplicationInterface.ITexture.SaveToFile(base.Pointer, path);
		}

		public void SetTextureAsAlwaysValid()
		{
			EngineApplicationInterface.ITexture.SaveTextureAsAlwaysValid(base.Pointer);
		}

		public static Texture CreateFromMemory(byte[] data)
		{
			return EngineApplicationInterface.ITexture.CreateFromMemory(data);
		}

		public static void ReleaseGpuMemories()
		{
			EngineApplicationInterface.ITexture.ReleaseGpuMemories();
		}

		public RenderTargetComponent RenderTargetComponent
		{
			get
			{
				return EngineApplicationInterface.ITexture.GetRenderTargetComponent(base.Pointer);
			}
		}

		public TableauView TableauView
		{
			get
			{
				return EngineApplicationInterface.ITexture.GetTableauView(base.Pointer);
			}
		}

		public object UserData
		{
			get
			{
				return this.RenderTargetComponent.UserData;
			}
		}

		private void SetTableauView(TableauView tableauView)
		{
			EngineApplicationInterface.ITexture.SetTableauView(base.Pointer, tableauView.Pointer);
		}

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

		public static Texture CreateRenderTarget(string name, int width, int height, bool autoMipmaps, bool isTableau, bool createUninitialized = false, bool always_valid = false)
		{
			return EngineApplicationInterface.ITexture.CreateRenderTarget(name, width, height, autoMipmaps, isTableau, createUninitialized, always_valid);
		}
	}
}
