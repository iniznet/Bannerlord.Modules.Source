using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension.Standalone.Native;
using TaleWorlds.TwoDimension.Standalone.Native.OpenGL;
using TaleWorlds.TwoDimension.Standalone.Native.Windows;

namespace TaleWorlds.TwoDimension.Standalone
{
	// Token: 0x02000003 RID: 3
	public class GraphicsContext
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000004 RID: 4 RVA: 0x00002050 File Offset: 0x00000250
		// (set) Token: 0x06000005 RID: 5 RVA: 0x00002058 File Offset: 0x00000258
		internal WindowsForm Control { get; set; }

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000006 RID: 6 RVA: 0x00002061 File Offset: 0x00000261
		// (set) Token: 0x06000007 RID: 7 RVA: 0x00002068 File Offset: 0x00000268
		public static GraphicsContext Active { get; private set; }

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000008 RID: 8 RVA: 0x00002070 File Offset: 0x00000270
		// (set) Token: 0x06000009 RID: 9 RVA: 0x00002078 File Offset: 0x00000278
		internal Dictionary<string, OpenGLTexture> LoadedTextures { get; private set; }

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600000A RID: 10 RVA: 0x00002081 File Offset: 0x00000281
		// (set) Token: 0x0600000B RID: 11 RVA: 0x00002089 File Offset: 0x00000289
		public Matrix4x4 ProjectionMatrix
		{
			get
			{
				return this._projectionMatrix;
			}
			set
			{
				this._projectionMatrix = value;
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600000C RID: 12 RVA: 0x00002092 File Offset: 0x00000292
		// (set) Token: 0x0600000D RID: 13 RVA: 0x0000209A File Offset: 0x0000029A
		public Matrix4x4 ViewMatrix
		{
			get
			{
				return this._viewMatrix;
			}
			set
			{
				this._viewMatrix = value;
				this._modelViewMatrix = this._viewMatrix * this._modelMatrix;
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600000E RID: 14 RVA: 0x000020BA File Offset: 0x000002BA
		// (set) Token: 0x0600000F RID: 15 RVA: 0x000020C2 File Offset: 0x000002C2
		public Matrix4x4 ModelMatrix
		{
			get
			{
				return this._modelMatrix;
			}
			set
			{
				this._modelMatrix = value;
				this._modelViewMatrix = this._viewMatrix * this._modelMatrix;
			}
		}

		// Token: 0x06000010 RID: 16 RVA: 0x000020E4 File Offset: 0x000002E4
		public GraphicsContext()
		{
			this.LoadedTextures = new Dictionary<string, OpenGLTexture>();
			this._loadedShaders = new Dictionary<string, Shader>();
			this._stopwatch = new Stopwatch();
			this.MaxTimeToRenderOneFrame = 33;
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00002158 File Offset: 0x00000358
		public void CreateContext(ResourceDepot resourceDepot)
		{
			this._resourceDepot = resourceDepot;
			this._handleDeviceContext = User32.GetDC(this.Control.Handle);
			if (this._handleDeviceContext == IntPtr.Zero)
			{
				Debug.Print("Can't get device context", 0, Debug.DebugColor.White, 17592186044416UL);
			}
			if (!Opengl32.wglMakeCurrent(this._handleDeviceContext, IntPtr.Zero))
			{
				Debug.Print("Can't reset context", 0, Debug.DebugColor.White, 17592186044416UL);
			}
			PixelFormatDescriptor pixelFormatDescriptor = default(PixelFormatDescriptor);
			Marshal.SizeOf(typeof(PixelFormatDescriptor));
			pixelFormatDescriptor.nSize = (ushort)Marshal.SizeOf(typeof(PixelFormatDescriptor));
			pixelFormatDescriptor.nVersion = 1;
			pixelFormatDescriptor.dwFlags = 32805U;
			pixelFormatDescriptor.iPixelType = 0;
			pixelFormatDescriptor.cColorBits = 32;
			pixelFormatDescriptor.cRedBits = 0;
			pixelFormatDescriptor.cRedShift = 0;
			pixelFormatDescriptor.cGreenBits = 0;
			pixelFormatDescriptor.cGreenShift = 0;
			pixelFormatDescriptor.cBlueBits = 0;
			pixelFormatDescriptor.cBlueShift = 0;
			pixelFormatDescriptor.cAlphaBits = 8;
			pixelFormatDescriptor.cAlphaShift = 0;
			pixelFormatDescriptor.cAccumBits = 0;
			pixelFormatDescriptor.cAccumRedBits = 0;
			pixelFormatDescriptor.cAccumGreenBits = 0;
			pixelFormatDescriptor.cAccumBlueBits = 0;
			pixelFormatDescriptor.cAccumAlphaBits = 0;
			pixelFormatDescriptor.cDepthBits = 24;
			pixelFormatDescriptor.cStencilBits = 0;
			pixelFormatDescriptor.cAuxBuffers = 0;
			pixelFormatDescriptor.iLayerType = 0;
			pixelFormatDescriptor.bReserved = 0;
			pixelFormatDescriptor.dwLayerMask = 0U;
			pixelFormatDescriptor.dwVisibleMask = 0U;
			pixelFormatDescriptor.dwDamageMask = 0U;
			int num = Gdi32.ChoosePixelFormat(this._handleDeviceContext, ref pixelFormatDescriptor);
			if (!Gdi32.SetPixelFormat(this._handleDeviceContext, num, ref pixelFormatDescriptor))
			{
				Debug.Print("can't set pixel format", 0, Debug.DebugColor.White, 17592186044416UL);
			}
			this._handleRenderContext = Opengl32.wglCreateContext(this._handleDeviceContext);
			this.SetActive();
			Opengl32ARB.LoadExtensions();
			IntPtr handleRenderContext = this._handleRenderContext;
			this._handleRenderContext = IntPtr.Zero;
			GraphicsContext.Active = null;
			int[] array = new int[10];
			int num2 = 0;
			array[num2++] = 8337;
			array[num2++] = 3;
			array[num2++] = 8338;
			array[num2++] = 3;
			array[num2++] = 37158;
			array[num2++] = 1;
			array[num2++] = 0;
			this._handleRenderContext = Opengl32ARB.wglCreateContextAttribs(this._handleDeviceContext, IntPtr.Zero, array);
			this.SetActive();
			Opengl32.wglDeleteContext(handleRenderContext);
			Opengl32.ShadeModel(ShadingModel.Smooth);
			Opengl32.ClearColor(0f, 0f, 0f, 0f);
			Opengl32.ClearDepth(1.0);
			Opengl32.Disable(Target.DepthTest);
			Opengl32.Hint(3152U, 4354U);
			this.ProjectionMatrix = Matrix4x4.Identity;
			this.ModelMatrix = Matrix4x4.Identity;
			this.ViewMatrix = Matrix4x4.Identity;
			this._simpleVAO = VertexArrayObject.Create();
			this._textureVAO = VertexArrayObject.CreateWithUVBuffer();
		}

		// Token: 0x06000012 RID: 18 RVA: 0x0000242D File Offset: 0x0000062D
		public void SetActive()
		{
			if (GraphicsContext.Active != this)
			{
				if (Opengl32.wglMakeCurrent(this._handleDeviceContext, this._handleRenderContext))
				{
					GraphicsContext.Active = this;
					return;
				}
				Debug.Print("Can't activate context", 0, Debug.DebugColor.White, 17592186044416UL);
			}
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002468 File Offset: 0x00000668
		public void BeginFrame(int width, int height)
		{
			this._stopwatch.Start();
			this.Resize(width, height);
			Opengl32.Clear(AttribueMask.ColorBufferBit);
			Opengl32.ClearDepth(1.0);
			Opengl32.Disable(Target.DepthTest);
			Opengl32.Disable(Target.SCISSOR_TEST);
			Opengl32.Disable(Target.STENCIL_TEST);
			Opengl32.Disable(Target.Blend);
		}

		// Token: 0x06000014 RID: 20 RVA: 0x000024C8 File Offset: 0x000006C8
		public void SwapBuffers()
		{
			int num = (int)this._stopwatch.ElapsedMilliseconds;
			int num2 = 0;
			if (this.MaxTimeToRenderOneFrame > num)
			{
				num2 = this.MaxTimeToRenderOneFrame - num;
			}
			if (num2 > 0)
			{
				Thread.Sleep(num2);
			}
			Gdi32.SwapBuffers(this._handleDeviceContext);
			this._stopwatch.Restart();
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000015 RID: 21 RVA: 0x00002517 File Offset: 0x00000717
		public bool IsActive
		{
			get
			{
				return GraphicsContext.Active == this;
			}
		}

		// Token: 0x06000016 RID: 22 RVA: 0x00002521 File Offset: 0x00000721
		public void DestroyContext()
		{
			Opengl32.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
			Opengl32.wglDeleteContext(this._handleRenderContext);
			User32.ReleaseDC(this.Control.Handle, this._handleDeviceContext);
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00002558 File Offset: 0x00000758
		public void SetScissor(ScissorTestInfo scissorTestInfo)
		{
			Opengl32.GetInteger(Target.VIEWPORT, this._scissorParameters);
			Opengl32.Scissor(scissorTestInfo.X, this._scissorParameters[3] - scissorTestInfo.Height - scissorTestInfo.Y, scissorTestInfo.Width, scissorTestInfo.Height);
			Opengl32.Enable(Target.SCISSOR_TEST);
		}

		// Token: 0x06000018 RID: 24 RVA: 0x000025AC File Offset: 0x000007AC
		public void ResetScissor()
		{
			Opengl32.Disable(Target.SCISSOR_TEST);
		}

		// Token: 0x06000019 RID: 25 RVA: 0x000025B8 File Offset: 0x000007B8
		public void DrawElements(float x, float y, Material material, DrawObject2D drawObject2D)
		{
			this.ModelMatrix = Matrix4x4.CreateTranslation(x, y, 0f);
			this.DrawElements(material, drawObject2D);
		}

		// Token: 0x0600001A RID: 26 RVA: 0x000025D8 File Offset: 0x000007D8
		public Shader GetOrLoadShader(string shaderName)
		{
			if (!this._loadedShaders.ContainsKey(shaderName))
			{
				string filePath = this._resourceDepot.GetFilePath(shaderName + ".vert");
				string filePath2 = this._resourceDepot.GetFilePath(shaderName + ".frag");
				string text = File.ReadAllText(filePath);
				string text2 = File.ReadAllText(filePath2);
				Shader shader = Shader.CreateShader(this, text, text2);
				this._loadedShaders.Add(shaderName, shader);
				return shader;
			}
			return this._loadedShaders[shaderName];
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00002654 File Offset: 0x00000854
		public void DrawElements(Material material, DrawObject2D drawObject2D)
		{
			bool blending = material.Blending;
			Shader orLoadShader = this.GetOrLoadShader(material.GetType().Name);
			orLoadShader.Use();
			Matrix4x4 matrix4x = this._modelMatrix * this._viewMatrix * this._projectionMatrix;
			orLoadShader.SetMatrix("MVP", matrix4x);
			MeshTopology topology = drawObject2D.Topology;
			if (material is SimpleMaterial)
			{
				SimpleMaterial simpleMaterial = (SimpleMaterial)material;
				if (simpleMaterial.Texture != null)
				{
					OpenGLTexture openGLTexture = simpleMaterial.Texture.PlatformTexture as OpenGLTexture;
					orLoadShader.SetTexture("Texture", openGLTexture);
				}
				orLoadShader.SetBoolean("OverlayEnabled", simpleMaterial.OverlayEnabled);
				if (simpleMaterial.OverlayEnabled)
				{
					OpenGLTexture openGLTexture2 = simpleMaterial.OverlayTexture.PlatformTexture as OpenGLTexture;
					orLoadShader.SetVector2("StartCoord", simpleMaterial.StartCoordinate);
					orLoadShader.SetVector2("Size", simpleMaterial.Size);
					orLoadShader.SetTexture("OverlayTexture", openGLTexture2);
					orLoadShader.SetVector2("OverlayOffset", new Vector2(simpleMaterial.OverlayXOffset, simpleMaterial.OverlayYOffset));
				}
				float num = MathF.Clamp(simpleMaterial.HueFactor / 360f, -0.5f, 0.5f);
				float num2 = MathF.Clamp(simpleMaterial.SaturationFactor / 360f, -0.5f, 0.5f);
				float num3 = MathF.Clamp(simpleMaterial.ValueFactor / 360f, -0.5f, 0.5f);
				orLoadShader.SetColor("InputColor", simpleMaterial.Color);
				orLoadShader.SetFloat("ColorFactor", simpleMaterial.ColorFactor);
				orLoadShader.SetFloat("AlphaFactor", simpleMaterial.AlphaFactor);
				orLoadShader.SetFloat("HueFactor", num);
				orLoadShader.SetFloat("SaturationFactor", num2);
				orLoadShader.SetFloat("ValueFactor", num3);
				this._textureVAO.Bind();
				if (simpleMaterial.CircularMaskingEnabled)
				{
					orLoadShader.SetBoolean("CircularMaskingEnabled", true);
					orLoadShader.SetVector2("MaskingCenter", simpleMaterial.CircularMaskingCenter);
					orLoadShader.SetFloat("MaskingRadius", simpleMaterial.CircularMaskingRadius);
					orLoadShader.SetFloat("MaskingSmoothingRadius", simpleMaterial.CircularMaskingSmoothingRadius);
				}
				else
				{
					orLoadShader.SetBoolean("CircularMaskingEnabled", false);
				}
				this._textureVAO.LoadVertexData(drawObject2D.Vertices);
				this._textureVAO.LoadUVData(drawObject2D.TextureCoordinates);
				this._textureVAO.LoadIndexData(drawObject2D.Indices);
			}
			else if (material is TextMaterial)
			{
				TextMaterial textMaterial = (TextMaterial)material;
				if (textMaterial.Texture != null)
				{
					OpenGLTexture openGLTexture3 = textMaterial.Texture.PlatformTexture as OpenGLTexture;
					orLoadShader.SetTexture("Texture", openGLTexture3);
				}
				orLoadShader.SetColor("InputColor", textMaterial.Color);
				orLoadShader.SetColor("GlowColor", textMaterial.GlowColor);
				orLoadShader.SetColor("OutlineColor", textMaterial.OutlineColor);
				orLoadShader.SetFloat("OutlineAmount", textMaterial.OutlineAmount);
				orLoadShader.SetFloat("ScaleFactor", 1.5f / textMaterial.ScaleFactor);
				orLoadShader.SetFloat("SmoothingConstant", textMaterial.SmoothingConstant);
				orLoadShader.SetFloat("GlowRadius", textMaterial.GlowRadius);
				orLoadShader.SetFloat("Blur", textMaterial.Blur);
				orLoadShader.SetFloat("ShadowOffset", textMaterial.ShadowOffset);
				orLoadShader.SetFloat("ShadowAngle", textMaterial.ShadowAngle);
				orLoadShader.SetFloat("ColorFactor", textMaterial.ColorFactor);
				orLoadShader.SetFloat("AlphaFactor", textMaterial.AlphaFactor);
				this._textureVAO.Bind();
				this._textureVAO.LoadVertexData(drawObject2D.Vertices);
				this._textureVAO.LoadUVData(drawObject2D.TextureCoordinates);
				this._textureVAO.LoadIndexData(drawObject2D.Indices);
			}
			else if (material is PrimitivePolygonMaterial)
			{
				Color color = ((PrimitivePolygonMaterial)material).Color;
				orLoadShader.SetColor("Color", color);
				this._simpleVAO.Bind();
				this._simpleVAO.LoadVertexData(drawObject2D.Vertices);
			}
			this.DrawElements(drawObject2D.Indices, topology, blending);
			VertexArrayObject.UnBind();
			orLoadShader.StopUsing();
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00002A70 File Offset: 0x00000C70
		private void DrawElements(uint[] indices, MeshTopology meshTopology, bool blending)
		{
			this.SetBlending(blending);
			using (new AutoPinner(indices))
			{
				BeginMode beginMode = BeginMode.Quads;
				if (meshTopology == MeshTopology.Lines)
				{
					beginMode = BeginMode.Lines;
				}
				else if (meshTopology == MeshTopology.Triangles)
				{
					beginMode = BeginMode.Triangles;
				}
				Opengl32.DrawElements(beginMode, indices.Length, DataType.UnsignedInt, null);
			}
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00002AC8 File Offset: 0x00000CC8
		internal void Resize(int width, int height)
		{
			if (!this.IsActive)
			{
				this.SetActive();
			}
			this._screenWidth = width;
			this._screenHeight = height;
			Opengl32.Viewport(0, 0, width, height);
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00002AEF File Offset: 0x00000CEF
		public void LoadTextureUsing(OpenGLTexture texture, ResourceDepot resourceDepot, string name)
		{
			if (!this.LoadedTextures.ContainsKey(name))
			{
				texture.LoadFromFile(resourceDepot, name);
				this.LoadedTextures.Add(name, texture);
				return;
			}
			texture.CopyFrom(this.LoadedTextures[name]);
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00002B28 File Offset: 0x00000D28
		public OpenGLTexture LoadTexture(ResourceDepot resourceDepot, string name)
		{
			OpenGLTexture openGLTexture;
			if (this.LoadedTextures.ContainsKey(name))
			{
				openGLTexture = this.LoadedTextures[name];
			}
			else
			{
				openGLTexture = OpenGLTexture.FromFile(resourceDepot, name);
				this.LoadedTextures.Add(name, openGLTexture);
			}
			return openGLTexture;
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00002B6C File Offset: 0x00000D6C
		public OpenGLTexture GetTexture(string textureName)
		{
			OpenGLTexture openGLTexture = null;
			if (this.LoadedTextures.ContainsKey(textureName))
			{
				openGLTexture = this.LoadedTextures[textureName];
			}
			return openGLTexture;
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00002B97 File Offset: 0x00000D97
		public void SetBlending(bool enable)
		{
			this._blendingMode = enable;
			if (this._blendingMode)
			{
				Opengl32.Enable(Target.Blend);
				Opengl32ARB.BlendFuncSeparate(BlendingSourceFactor.SourceAlpha, BlendingDestinationFactor.OneMinusSourceAlpha, BlendingSourceFactor.One, BlendingDestinationFactor.One);
				return;
			}
			Opengl32.Disable(Target.Blend);
		}

		// Token: 0x06000022 RID: 34 RVA: 0x00002BD3 File Offset: 0x00000DD3
		public void SetVertexArrayClientState(bool enable)
		{
			if (this._vertexArrayClientState != enable)
			{
				this._vertexArrayClientState = enable;
				if (this._vertexArrayClientState)
				{
					Opengl32.EnableClientState(32884U);
					return;
				}
				Opengl32.DisableClientState(32884U);
			}
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00002C02 File Offset: 0x00000E02
		public void SetTextureCoordArrayClientState(bool enable)
		{
			if (this._textureCoordArrayClientState != enable)
			{
				this._textureCoordArrayClientState = enable;
				if (this._textureCoordArrayClientState)
				{
					Opengl32.EnableClientState(32888U);
					return;
				}
				Opengl32.DisableClientState(32888U);
			}
		}

		// Token: 0x04000001 RID: 1
		public const int MaxFrameRate = 30;

		// Token: 0x04000002 RID: 2
		public readonly int MaxTimeToRenderOneFrame;

		// Token: 0x04000004 RID: 4
		private IntPtr _handleDeviceContext;

		// Token: 0x04000005 RID: 5
		private IntPtr _handleRenderContext;

		// Token: 0x04000008 RID: 8
		private int[] _scissorParameters = new int[4];

		// Token: 0x04000009 RID: 9
		private Matrix4x4 _projectionMatrix = Matrix4x4.Identity;

		// Token: 0x0400000A RID: 10
		private Matrix4x4 _modelMatrix = Matrix4x4.Identity;

		// Token: 0x0400000B RID: 11
		private Matrix4x4 _viewMatrix = Matrix4x4.Identity;

		// Token: 0x0400000C RID: 12
		private Matrix4x4 _modelViewMatrix = Matrix4x4.Identity;

		// Token: 0x0400000D RID: 13
		private Stopwatch _stopwatch;

		// Token: 0x0400000E RID: 14
		private Dictionary<string, Shader> _loadedShaders;

		// Token: 0x0400000F RID: 15
		private VertexArrayObject _simpleVAO;

		// Token: 0x04000010 RID: 16
		private VertexArrayObject _textureVAO;

		// Token: 0x04000011 RID: 17
		private int _screenWidth;

		// Token: 0x04000012 RID: 18
		private int _screenHeight;

		// Token: 0x04000013 RID: 19
		private ResourceDepot _resourceDepot;

		// Token: 0x04000014 RID: 20
		private bool _blendingMode;

		// Token: 0x04000015 RID: 21
		private bool _vertexArrayClientState;

		// Token: 0x04000016 RID: 22
		private bool _textureCoordArrayClientState;
	}
}
