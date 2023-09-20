using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TaleWorlds.Library;

namespace TaleWorlds.TwoDimension
{
	public class TwoDimensionContext
	{
		public float Width
		{
			get
			{
				return this.Platform.Width;
			}
		}

		public float Height
		{
			get
			{
				return this.Platform.Height;
			}
		}

		public ITwoDimensionPlatform Platform { get; private set; }

		public ITwoDimensionResourceContext ResourceContext { get; private set; }

		public ResourceDepot ResourceDepot { get; private set; }

		public bool ScissorTestEnabled
		{
			get
			{
				return this._scissorTestEnabled;
			}
		}

		public bool CircularMaskEnabled
		{
			get
			{
				return this._circularMaskEnabled;
			}
		}

		public Vector2 CircularMaskCenter
		{
			get
			{
				return this._circularMaskCenter;
			}
		}

		public float CircularMaskRadius
		{
			get
			{
				return this._circularMaskRadius;
			}
		}

		public float CircularMaskSmoothingRadius
		{
			get
			{
				return this._circularMaskSmoothingRadius;
			}
		}

		public ScissorTestInfo CurrentScissor
		{
			get
			{
				return this._scissorStack[this._scissorStack.Count - 1];
			}
		}

		public bool IsDebugModeEnabled
		{
			get
			{
				return this.Platform.IsDebugModeEnabled();
			}
		}

		public TwoDimensionContext(ITwoDimensionPlatform platform, ITwoDimensionResourceContext resourceContext, ResourceDepot resourceDepot)
		{
			this.ResourceDepot = resourceDepot;
			this._scissorStack = new List<ScissorTestInfo>();
			this._scissorTestEnabled = false;
			this.Platform = platform;
			this.ResourceContext = resourceContext;
		}

		public void PlaySound(string soundName)
		{
			this.Platform.PlaySound(soundName);
		}

		public void CreateSoundEvent(string soundName)
		{
			this.Platform.CreateSoundEvent(soundName);
		}

		public void StopAndRemoveSoundEvent(string soundName)
		{
			this.Platform.StopAndRemoveSoundEvent(soundName);
		}

		public void PlaySoundEvent(string soundName)
		{
			this.Platform.PlaySoundEvent(soundName);
		}

		public void Draw(float x, float y, Material material, DrawObject2D drawObject2D, int layer = 0)
		{
			this.Platform.Draw(x, y, material, drawObject2D, layer);
		}

		public void Draw(Text text, TextMaterial material, float x, float y, float width, float height)
		{
			text.UpdateSize((int)width, (int)height);
			DrawObject2D drawObject2D = text.DrawObject2D;
			if (drawObject2D != null)
			{
				material.Texture = text.Font.FontSprite.Texture;
				material.ScaleFactor = text.FontSize;
				material.SmoothingConstant = text.Font.SmoothingConstant;
				material.Smooth = text.Font.Smooth;
				DrawObject2D drawObject2D2 = new DrawObject2D(drawObject2D.Topology, drawObject2D.Vertices.ToArray<float>(), drawObject2D.TextureCoordinates, drawObject2D.Indices, drawObject2D.VertexCount);
				this.Draw(x, y, material, drawObject2D2, 0);
			}
		}

		public void BeginDebugPanel(string panelTitle)
		{
			this.Platform.BeginDebugPanel(panelTitle);
		}

		public void EndDebugPanel()
		{
			this.Platform.EndDebugPanel();
		}

		public void DrawDebugText(string text)
		{
			this.Platform.DrawDebugText(text);
		}

		public bool DrawDebugTreeNode(string text)
		{
			return this.Platform.DrawDebugTreeNode(text);
		}

		public void PopDebugTreeNode()
		{
			this.Platform.PopDebugTreeNode();
		}

		public void DrawCheckbox(string label, ref bool isChecked)
		{
			this.Platform.DrawCheckbox(label, ref isChecked);
		}

		public bool IsDebugItemHovered()
		{
			return this.Platform.IsDebugItemHovered();
		}

		public Texture LoadTexture(string name)
		{
			return this.ResourceContext.LoadTexture(this.ResourceDepot, name);
		}

		public void SetCircualMask(Vector2 position, float radius, float smoothingRadius)
		{
			this._circularMaskEnabled = true;
			this._circularMaskCenter = position;
			this._circularMaskRadius = radius;
			this._circularMaskSmoothingRadius = smoothingRadius;
		}

		public void ClearCircualMask()
		{
			this._circularMaskEnabled = false;
		}

		public void DrawSprite(Sprite sprite, SimpleMaterial material, float x, float y, float scale, float width, float height, bool horizontalFlip, bool verticalFlip)
		{
			DrawObject2D arrays = sprite.GetArrays(new SpriteDrawData(0f, 0f, scale, width, height, horizontalFlip, verticalFlip));
			material.Texture = sprite.Texture;
			if (this._circularMaskEnabled)
			{
				material.CircularMaskingEnabled = true;
				material.CircularMaskingCenter = this._circularMaskCenter;
				material.CircularMaskingRadius = this._circularMaskRadius;
				material.CircularMaskingSmoothingRadius = this._circularMaskSmoothingRadius;
			}
			this.Draw(x, y, material, arrays, 0);
		}

		public void SetScissor(int x, int y, int width, int height)
		{
			ScissorTestInfo scissorTestInfo = new ScissorTestInfo(x, y, width, height);
			this.SetScissor(scissorTestInfo);
		}

		public void SetScissor(ScissorTestInfo scissor)
		{
			this.Platform.SetScissor(scissor);
		}

		public void ResetScissor()
		{
			this.Platform.ResetScissor();
		}

		public void PushScissor(int x, int y, int width, int height)
		{
			ScissorTestInfo scissorTestInfo = new ScissorTestInfo(x, y, width, height);
			if (this._scissorStack.Count > 0)
			{
				ScissorTestInfo scissorTestInfo2 = this._scissorStack[this._scissorStack.Count - 1];
				int num = scissorTestInfo2.X + scissorTestInfo2.Width;
				int num2 = scissorTestInfo2.Y + scissorTestInfo2.Height;
				int num3 = x + width;
				int num4 = y + height;
				scissorTestInfo.X = ((scissorTestInfo.X > scissorTestInfo2.X) ? scissorTestInfo.X : scissorTestInfo2.X);
				scissorTestInfo.Y = ((scissorTestInfo.Y > scissorTestInfo2.Y) ? scissorTestInfo.Y : scissorTestInfo2.Y);
				int num5 = ((num > num3) ? num3 : num);
				int num6 = ((num2 > num4) ? num4 : num2);
				scissorTestInfo.Width = num5 - scissorTestInfo.X;
				scissorTestInfo.Height = num6 - scissorTestInfo.Y;
			}
			this._scissorStack.Add(scissorTestInfo);
			this._scissorTestEnabled = true;
			this.Platform.SetScissor(scissorTestInfo);
		}

		public void PopScissor()
		{
			this._scissorStack.RemoveAt(this._scissorStack.Count - 1);
			if (this._scissorTestEnabled)
			{
				if (this._scissorStack.Count > 0)
				{
					ScissorTestInfo scissorTestInfo = this._scissorStack[this._scissorStack.Count - 1];
					this.Platform.SetScissor(scissorTestInfo);
					return;
				}
				this.Platform.ResetScissor();
				this._scissorTestEnabled = false;
			}
		}

		private List<ScissorTestInfo> _scissorStack;

		private bool _scissorTestEnabled;

		private bool _circularMaskEnabled;

		private float _circularMaskRadius;

		private float _circularMaskSmoothingRadius;

		private Vector2 _circularMaskCenter;
	}
}
