using System;
using System.IO;
using StbSharp;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension.Standalone.Native.OpenGL;

namespace TaleWorlds.TwoDimension.Standalone
{
	// Token: 0x02000008 RID: 8
	public class OpenGLTexture : ITexture
	{
		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000059 RID: 89 RVA: 0x000039F8 File Offset: 0x00001BF8
		public int Width
		{
			get
			{
				return this._width;
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600005A RID: 90 RVA: 0x00003A00 File Offset: 0x00001C00
		public int Height
		{
			get
			{
				return this._height;
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600005B RID: 91 RVA: 0x00003A08 File Offset: 0x00001C08
		// (set) Token: 0x0600005C RID: 92 RVA: 0x00003A0F File Offset: 0x00001C0F
		internal static OpenGLTexture ActiveTexture { get; private set; }

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600005D RID: 93 RVA: 0x00003A17 File Offset: 0x00001C17
		internal int Id
		{
			get
			{
				return this._id;
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x0600005E RID: 94 RVA: 0x00003A1F File Offset: 0x00001C1F
		// (set) Token: 0x0600005F RID: 95 RVA: 0x00003A27 File Offset: 0x00001C27
		public string Name { get; private set; }

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000060 RID: 96 RVA: 0x00003A30 File Offset: 0x00001C30
		// (set) Token: 0x06000061 RID: 97 RVA: 0x00003A38 File Offset: 0x00001C38
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

		// Token: 0x06000063 RID: 99 RVA: 0x00003A5E File Offset: 0x00001C5E
		public void Initialize(string name, int width, int height)
		{
			this._context = GraphicsContext.Active;
			this.Name = name;
			this._id = 0;
			Opengl32.GenTextures(1, ref this._id);
			this._width = width;
			this._height = height;
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00003A93 File Offset: 0x00001C93
		public void CopyFrom(OpenGLTexture texture)
		{
			this._width = texture._width;
			this._height = texture._height;
			this.Name = texture.Name;
			this._id = texture._id;
			this._context = texture._context;
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00003AD1 File Offset: 0x00001CD1
		public void Delete()
		{
			Opengl32.DeleteTextures(1, new int[] { this._id });
			Debug.Print("texture deleted! : " + this.Name, 0, Debug.DebugColor.White, 17592186044416UL);
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00003B09 File Offset: 0x00001D09
		internal void MakeActive()
		{
			if (OpenGLTexture.ActiveTexture != this)
			{
				Opengl32.BindTexture(Target.Texture2D, this._id);
				OpenGLTexture.ActiveTexture = this;
				this.SetTextureParameters();
			}
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00003B30 File Offset: 0x00001D30
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

		// Token: 0x06000068 RID: 104 RVA: 0x00003B95 File Offset: 0x00001D95
		public static OpenGLTexture FromFile(ResourceDepot resourceDepot, string name)
		{
			OpenGLTexture openGLTexture = new OpenGLTexture();
			openGLTexture.LoadFromFile(resourceDepot, name);
			return openGLTexture;
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00003BA4 File Offset: 0x00001DA4
		public static OpenGLTexture FromFile(string fullFilePath)
		{
			OpenGLTexture openGLTexture = new OpenGLTexture();
			openGLTexture.LoadFromFile(fullFilePath);
			return openGLTexture;
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00003BB2 File Offset: 0x00001DB2
		public void Release()
		{
			this.Delete();
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00003BBC File Offset: 0x00001DBC
		public void LoadFromFile(ResourceDepot resourceDepot, string name)
		{
			string filePath = resourceDepot.GetFilePath(name + ".png");
			this.LoadFromFile(filePath);
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00003BE4 File Offset: 0x00001DE4
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

		// Token: 0x0600006D RID: 109 RVA: 0x00003D34 File Offset: 0x00001F34
		public bool IsLoaded()
		{
			return true;
		}

		// Token: 0x04000033 RID: 51
		private int _width;

		// Token: 0x04000034 RID: 52
		private int _height;

		// Token: 0x04000036 RID: 54
		private GraphicsContext _context;

		// Token: 0x04000037 RID: 55
		private int _id;

		// Token: 0x04000039 RID: 57
		private bool _clampToEdge;
	}
}
