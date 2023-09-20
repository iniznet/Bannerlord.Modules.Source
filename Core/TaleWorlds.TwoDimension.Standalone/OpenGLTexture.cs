using System;
using System.IO;
using StbSharp;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension.Standalone.Native.OpenGL;

namespace TaleWorlds.TwoDimension.Standalone
{
	public class OpenGLTexture : ITexture
	{
		public int Width
		{
			get
			{
				return this._width;
			}
		}

		public int Height
		{
			get
			{
				return this._height;
			}
		}

		internal static OpenGLTexture ActiveTexture { get; private set; }

		internal int Id
		{
			get
			{
				return this._id;
			}
		}

		public string Name { get; private set; }

		public bool ClampToEdge
		{
			get
			{
				return this._clampToEdge;
			}
			set
			{
				this._clampToEdge = value;
				if (OpenGLTexture.ActiveTexture != this)
				{
					this.MakeActive();
					return;
				}
				this.SetTextureParameters();
			}
		}

		public void Initialize(string name, int width, int height)
		{
			this._context = GraphicsContext.Active;
			this.Name = name;
			this._id = 0;
			Opengl32.GenTextures(1, ref this._id);
			this._width = width;
			this._height = height;
		}

		public void CopyFrom(OpenGLTexture texture)
		{
			this._width = texture._width;
			this._height = texture._height;
			this.Name = texture.Name;
			this._id = texture._id;
			this._context = texture._context;
		}

		public void Delete()
		{
			Opengl32.DeleteTextures(1, new int[] { this._id });
			Debug.Print("texture deleted! : " + this.Name, 0, Debug.DebugColor.White, 17592186044416UL);
		}

		internal void MakeActive()
		{
			if (OpenGLTexture.ActiveTexture != this)
			{
				Opengl32.BindTexture(Target.Texture2D, this._id);
				OpenGLTexture.ActiveTexture = this;
				this.SetTextureParameters();
			}
		}

		private void SetTextureParameters()
		{
			Opengl32.TexParameteri(Target.Texture2D, TextureParameterName.TextureMinFilter, 9729);
			Opengl32.TexParameteri(Target.Texture2D, TextureParameterName.TextureMagFilter, 9729);
			if (this.ClampToEdge)
			{
				Opengl32.TexParameteri(Target.Texture2D, TextureParameterName.TextureWrapS, 33071);
				Opengl32.TexParameteri(Target.Texture2D, TextureParameterName.TextureWrapT, 33071);
			}
		}

		public static OpenGLTexture FromFile(ResourceDepot resourceDepot, string name)
		{
			OpenGLTexture openGLTexture = new OpenGLTexture();
			openGLTexture.LoadFromFile(resourceDepot, name);
			return openGLTexture;
		}

		public static OpenGLTexture FromFile(string fullFilePath)
		{
			OpenGLTexture openGLTexture = new OpenGLTexture();
			openGLTexture.LoadFromFile(fullFilePath);
			return openGLTexture;
		}

		public void Release()
		{
			this.Delete();
		}

		public void LoadFromFile(ResourceDepot resourceDepot, string name)
		{
			string filePath = resourceDepot.GetFilePath(name + ".png");
			this.LoadFromFile(filePath);
		}

		public void LoadFromFile(string fullPathName)
		{
			if (!File.Exists(fullPathName))
			{
				Debug.Print("File not found: " + fullPathName, 0, Debug.DebugColor.White, 17592186044416UL);
				return;
			}
			Image image = null;
			using (Stream stream = new MemoryStream(File.ReadAllBytes(fullPathName)))
			{
				image = new ImageReader().Read(stream, 0);
			}
			if (image == null)
			{
				Debug.Print("Error while reading file: " + fullPathName, 0, Debug.DebugColor.White, 17592186044416UL);
				return;
			}
			int width = image.Width;
			int height = image.Height;
			this.Initialize(Path.GetFileName(fullPathName), width, height);
			this.MakeActive();
			PixelFormat pixelFormat = PixelFormat.Red;
			uint num = 0U;
			bool flag = true;
			switch (image.Comp)
			{
			case 1:
				pixelFormat = PixelFormat.Red;
				num = 33321U;
				goto IL_112;
			case 3:
				pixelFormat = PixelFormat.RGB;
				num = 32849U;
				goto IL_112;
			case 4:
				pixelFormat = PixelFormat.RGBA;
				num = 32856U;
				goto IL_112;
			}
			flag = false;
			Debug.Print("Unknown image format at file: " + fullPathName + ". Supported formats are: Single-Channel, RGB and RGBA.", 0, Debug.DebugColor.White, 17592186044416UL);
			IL_112:
			if (flag)
			{
				Opengl32.TexImage2D(Target.Texture2D, 0, num, width, height, 0, pixelFormat, DataType.UnsignedByte, image.Data);
			}
		}

		public bool IsLoaded()
		{
			return true;
		}

		private int _width;

		private int _height;

		private GraphicsContext _context;

		private int _id;

		private bool _clampToEdge;
	}
}
