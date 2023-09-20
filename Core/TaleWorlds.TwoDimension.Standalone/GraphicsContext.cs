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
	public class GraphicsContext
	{
		internal WindowsForm Control { get; set; }

		public static GraphicsContext Active { get; private set; }

		internal Dictionary<string, OpenGLTexture> LoadedTextures { get; private set; }

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

		public GraphicsContext()
		{
			this.LoadedTextures = new Dictionary<string, OpenGLTexture>();
			this._loadedShaders = new Dictionary<string, Shader>();
			this._stopwatch = new Stopwatch();
			this.MaxTimeToRenderOneFrame = 33;
		}

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

		public bool IsActive
		{
			get
			{
				return GraphicsContext.Active == this;
			}
		}

		public void DestroyContext()
		{
			Opengl32.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
			Opengl32.wglDeleteContext(this._handleRenderContext);
			User32.ReleaseDC(this.Control.Handle, this._handleDeviceContext);
		}

		public void SetScissor(ScissorTestInfo scissorTestInfo)
		{
			Opengl32.GetInteger(Target.VIEWPORT, this._scissorParameters);
			Opengl32.Scissor(scissorTestInfo.X, this._scissorParameters[3] - scissorTestInfo.Height - scissorTestInfo.Y, scissorTestInfo.Width, scissorTestInfo.Height);
			Opengl32.Enable(Target.SCISSOR_TEST);
		}

		public void ResetScissor()
		{
			Opengl32.Disable(Target.SCISSOR_TEST);
		}

		public void DrawElements(float x, float y, Material material, DrawObject2D drawObject2D)
		{
			this.ModelMatrix = Matrix4x4.CreateTranslation(x, y, 0f);
			this.DrawElements(material, drawObject2D);
		}

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

		public OpenGLTexture GetTexture(string textureName)
		{
			OpenGLTexture openGLTexture = null;
			if (this.LoadedTextures.ContainsKey(textureName))
			{
				openGLTexture = this.LoadedTextures[textureName];
			}
			return openGLTexture;
		}

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

		public const int MaxFrameRate = 30;

		public readonly int MaxTimeToRenderOneFrame;

		private IntPtr _handleDeviceContext;

		private IntPtr _handleRenderContext;

		private int[] _scissorParameters = new int[4];

		private Matrix4x4 _projectionMatrix = Matrix4x4.Identity;

		private Matrix4x4 _modelMatrix = Matrix4x4.Identity;

		private Matrix4x4 _viewMatrix = Matrix4x4.Identity;

		private Matrix4x4 _modelViewMatrix = Matrix4x4.Identity;

		private Stopwatch _stopwatch;

		private Dictionary<string, Shader> _loadedShaders;

		private VertexArrayObject _simpleVAO;

		private VertexArrayObject _textureVAO;

		private int _screenWidth;

		private int _screenHeight;

		private ResourceDepot _resourceDepot;

		private bool _blendingMode;

		private bool _vertexArrayClientState;

		private bool _textureCoordArrayClientState;
	}
}
