using System;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.Engine.GauntletUI
{
	// Token: 0x02000005 RID: 5
	public class TwoDimensionEnginePlatform : ITwoDimensionPlatform
	{
		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000028 RID: 40 RVA: 0x000028C0 File Offset: 0x00000AC0
		float ITwoDimensionPlatform.Width
		{
			get
			{
				return Screen.RealScreenResolutionWidth * ScreenManager.UsableArea.X;
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000029 RID: 41 RVA: 0x000028E0 File Offset: 0x00000AE0
		float ITwoDimensionPlatform.Height
		{
			get
			{
				return Screen.RealScreenResolutionHeight * ScreenManager.UsableArea.Y;
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600002A RID: 42 RVA: 0x00002900 File Offset: 0x00000B00
		float ITwoDimensionPlatform.ReferenceWidth
		{
			get
			{
				return 1920f;
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600002B RID: 43 RVA: 0x00002907 File Offset: 0x00000B07
		float ITwoDimensionPlatform.ReferenceHeight
		{
			get
			{
				return 1080f;
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600002C RID: 44 RVA: 0x0000290E File Offset: 0x00000B0E
		float ITwoDimensionPlatform.ApplicationTime
		{
			get
			{
				return Time.ApplicationTime;
			}
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00002915 File Offset: 0x00000B15
		public TwoDimensionEnginePlatform(TwoDimensionView view)
		{
			this._view = view;
			this._scissorSet = false;
			this._materials = new Dictionary<TwoDimensionEnginePlatform.MaterialTuple, Material>();
			this._textMaterials = new Dictionary<Texture, Material>();
			this._soundEvents = new Dictionary<string, SoundEvent>();
		}

		// Token: 0x0600002E RID: 46 RVA: 0x0000294C File Offset: 0x00000B4C
		private Material GetOrCreateMaterial(Texture texture, Texture overlayTexture, bool useCustomMesh, bool useOverlayTextureAlphaAsMask)
		{
			TwoDimensionEnginePlatform.MaterialTuple materialTuple = new TwoDimensionEnginePlatform.MaterialTuple(texture, overlayTexture, useCustomMesh);
			Material material;
			if (this._materials.TryGetValue(materialTuple, out material))
			{
				return material;
			}
			Material material2 = Material.GetFromResource("two_dimension_simple_material").CreateCopy();
			material2.SetTexture(Material.MBTextureType.DiffuseMap, texture);
			if (overlayTexture != null)
			{
				material2.AddMaterialShaderFlag("use_overlay_texture", true);
				if (useOverlayTextureAlphaAsMask)
				{
					material2.AddMaterialShaderFlag("use_overlay_texture_alpha_as_mask", true);
				}
				material2.SetTexture(Material.MBTextureType.DiffuseMap2, overlayTexture);
			}
			if (useCustomMesh)
			{
				material2.AddMaterialShaderFlag("use_custom_mesh", true);
			}
			this._materials.Add(materialTuple, material2);
			return material2;
		}

		// Token: 0x0600002F RID: 47 RVA: 0x000029D8 File Offset: 0x00000BD8
		private Material GetOrCreateTextMaterial(Texture texture)
		{
			Material material;
			if (this._textMaterials.TryGetValue(texture, out material))
			{
				return material;
			}
			Material material2 = Material.GetFromResource("two_dimension_text_material").CreateCopy();
			material2.SetTexture(Material.MBTextureType.DiffuseMap, texture);
			this._textMaterials.Add(texture, material2);
			return material2;
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00002A20 File Offset: 0x00000C20
		void ITwoDimensionPlatform.Draw(float x, float y, Material material, DrawObject2D mesh, int layer)
		{
			Vec2 vec = new Vec2(0f, 0f);
			Vec2 vec2 = new Vec2(Screen.RealScreenResolutionWidth, Screen.RealScreenResolutionHeight);
			if (this._scissorSet)
			{
				vec = new Vec2((float)this._currentScissorTestInfo.X, (float)this._currentScissorTestInfo.Y);
				vec2 = new Vec2((float)this._currentScissorTestInfo.Width, (float)this._currentScissorTestInfo.Height);
			}
			if (material is SimpleMaterial)
			{
				SimpleMaterial simpleMaterial = (SimpleMaterial)material;
				Texture texture = simpleMaterial.Texture;
				if (texture != null)
				{
					Texture texture2 = ((EngineTexture)texture.PlatformTexture).Texture;
					if (texture2 != null)
					{
						Material material2 = null;
						Vec2 vec3 = Vec2.Zero;
						Vec2 vec4 = Vec2.Zero;
						float num = 512f;
						float num2 = 512f;
						float num3 = 0f;
						float num4 = 0f;
						if (simpleMaterial.OverlayEnabled)
						{
							Texture texture3 = ((EngineTexture)simpleMaterial.OverlayTexture.PlatformTexture).Texture;
							material2 = this.GetOrCreateMaterial(texture2, texture3, mesh.DrawObjectType == DrawObjectType.Mesh, simpleMaterial.UseOverlayAlphaAsMask);
							vec3 = simpleMaterial.StartCoordinate;
							vec4 = simpleMaterial.Size;
							num = simpleMaterial.OverlayTextureWidth;
							num2 = simpleMaterial.OverlayTextureHeight;
							num3 = simpleMaterial.OverlayXOffset;
							num4 = simpleMaterial.OverlayYOffset;
						}
						if (material2 == null)
						{
							material2 = this.GetOrCreateMaterial(texture2, null, mesh.DrawObjectType == DrawObjectType.Mesh, false);
						}
						uint num5 = simpleMaterial.Color.ToUnsignedInteger();
						float colorFactor = simpleMaterial.ColorFactor;
						float alphaFactor = simpleMaterial.AlphaFactor;
						float hueFactor = simpleMaterial.HueFactor;
						float saturationFactor = simpleMaterial.SaturationFactor;
						float valueFactor = simpleMaterial.ValueFactor;
						Vec2 vec5 = Vec2.Zero;
						float num6 = 0f;
						float num7 = 0f;
						if (simpleMaterial.CircularMaskingEnabled)
						{
							vec5 = simpleMaterial.CircularMaskingCenter;
							num6 = simpleMaterial.CircularMaskingRadius;
							num7 = simpleMaterial.CircularMaskingSmoothingRadius;
						}
						float[] vertices = mesh.Vertices;
						float[] textureCoordinates = mesh.TextureCoordinates;
						uint[] indices = mesh.Indices;
						int vertexCount = mesh.VertexCount;
						TwoDimensionMeshDrawData twoDimensionMeshDrawData = default(TwoDimensionMeshDrawData);
						twoDimensionMeshDrawData.ScreenWidth = Screen.RealScreenResolutionWidth;
						twoDimensionMeshDrawData.ScreenHeight = Screen.RealScreenResolutionHeight;
						twoDimensionMeshDrawData.DrawX = x;
						twoDimensionMeshDrawData.DrawY = y;
						twoDimensionMeshDrawData.ClipRectPosition = vec;
						twoDimensionMeshDrawData.ClipRectSize = vec2;
						twoDimensionMeshDrawData.Layer = layer;
						twoDimensionMeshDrawData.Width = mesh.Width;
						twoDimensionMeshDrawData.Height = mesh.Height;
						twoDimensionMeshDrawData.MinU = mesh.MinU;
						twoDimensionMeshDrawData.MinV = mesh.MinV;
						twoDimensionMeshDrawData.MaxU = mesh.MaxU;
						twoDimensionMeshDrawData.MaxV = mesh.MaxV;
						twoDimensionMeshDrawData.ClipCircleCenter = vec5;
						twoDimensionMeshDrawData.ClipCircleRadius = num6;
						twoDimensionMeshDrawData.ClipCircleSmoothingRadius = num7;
						twoDimensionMeshDrawData.Color = num5;
						twoDimensionMeshDrawData.ColorFactor = colorFactor;
						twoDimensionMeshDrawData.AlphaFactor = alphaFactor;
						twoDimensionMeshDrawData.HueFactor = hueFactor;
						twoDimensionMeshDrawData.SaturationFactor = saturationFactor;
						twoDimensionMeshDrawData.ValueFactor = valueFactor;
						twoDimensionMeshDrawData.OverlayTextureWidth = num;
						twoDimensionMeshDrawData.OverlayTextureHeight = num2;
						twoDimensionMeshDrawData.OverlayXOffset = num3;
						twoDimensionMeshDrawData.OverlayYOffset = num4;
						twoDimensionMeshDrawData.StartCoordinate = vec3;
						twoDimensionMeshDrawData.Size = vec4;
						twoDimensionMeshDrawData.Type = (uint)mesh.DrawObjectType;
						if (!MBDebug.DisableAllUI)
						{
							if (mesh.DrawObjectType == DrawObjectType.Quad)
							{
								this._view.CreateMeshFromDescription(material2, twoDimensionMeshDrawData);
								return;
							}
							this._view.CreateMeshFromDescription(vertices, textureCoordinates, indices, vertexCount, material2, twoDimensionMeshDrawData);
							return;
						}
					}
				}
			}
			else if (material is TextMaterial)
			{
				TextMaterial textMaterial = (TextMaterial)material;
				uint num8 = textMaterial.Color.ToUnsignedInteger();
				Texture texture4 = textMaterial.Texture;
				if (texture4 != null)
				{
					Texture texture5 = ((EngineTexture)texture4.PlatformTexture).Texture;
					if (texture5 != null)
					{
						Material orCreateTextMaterial = this.GetOrCreateTextMaterial(texture5);
						TwoDimensionTextMeshDrawData twoDimensionTextMeshDrawData = default(TwoDimensionTextMeshDrawData);
						twoDimensionTextMeshDrawData.DrawX = x;
						twoDimensionTextMeshDrawData.DrawY = y;
						twoDimensionTextMeshDrawData.ScreenWidth = Screen.RealScreenResolutionWidth;
						twoDimensionTextMeshDrawData.ScreenHeight = Screen.RealScreenResolutionHeight;
						twoDimensionTextMeshDrawData.Color = num8;
						twoDimensionTextMeshDrawData.ScaleFactor = 1.5f / textMaterial.ScaleFactor;
						twoDimensionTextMeshDrawData.SmoothingConstant = textMaterial.SmoothingConstant;
						twoDimensionTextMeshDrawData.GlowColor = textMaterial.GlowColor.ToUnsignedInteger();
						twoDimensionTextMeshDrawData.OutlineColor = textMaterial.OutlineColor.ToVec3();
						twoDimensionTextMeshDrawData.OutlineAmount = textMaterial.OutlineAmount;
						twoDimensionTextMeshDrawData.GlowRadius = textMaterial.GlowRadius;
						twoDimensionTextMeshDrawData.Blur = textMaterial.Blur;
						twoDimensionTextMeshDrawData.ShadowOffset = textMaterial.ShadowOffset;
						twoDimensionTextMeshDrawData.ShadowAngle = textMaterial.ShadowAngle;
						twoDimensionTextMeshDrawData.ColorFactor = textMaterial.ColorFactor;
						twoDimensionTextMeshDrawData.AlphaFactor = textMaterial.AlphaFactor;
						twoDimensionTextMeshDrawData.HueFactor = textMaterial.HueFactor;
						twoDimensionTextMeshDrawData.SaturationFactor = textMaterial.SaturationFactor;
						twoDimensionTextMeshDrawData.ValueFactor = textMaterial.ValueFactor;
						twoDimensionTextMeshDrawData.Layer = layer;
						twoDimensionTextMeshDrawData.ClipRectPosition = vec;
						twoDimensionTextMeshDrawData.ClipRectSize = vec2;
						twoDimensionTextMeshDrawData.HashCode1 = mesh.HashCode1;
						twoDimensionTextMeshDrawData.HashCode2 = mesh.HashCode2;
						if (!MBDebug.DisableAllUI && !this._view.CreateTextMeshFromCache(orCreateTextMaterial, twoDimensionTextMeshDrawData))
						{
							this._view.CreateTextMeshFromDescription(mesh.Vertices, mesh.TextureCoordinates, mesh.Indices, mesh.VertexCount, orCreateTextMaterial, twoDimensionTextMeshDrawData);
						}
					}
				}
			}
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00002F86 File Offset: 0x00001186
		void ITwoDimensionPlatform.SetScissor(ScissorTestInfo scissorTestInfo)
		{
			this._currentScissorTestInfo = scissorTestInfo;
			this._scissorSet = true;
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00002F96 File Offset: 0x00001196
		void ITwoDimensionPlatform.ResetScissor()
		{
			this._scissorSet = false;
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00002F9F File Offset: 0x0000119F
		void ITwoDimensionPlatform.PlaySound(string soundName)
		{
			SoundEvent.PlaySound2D("event:/ui/" + soundName);
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00002FB4 File Offset: 0x000011B4
		void ITwoDimensionPlatform.CreateSoundEvent(string soundName)
		{
			if (!this._soundEvents.ContainsKey(soundName))
			{
				SoundEvent soundEvent = SoundEvent.CreateEventFromString("event:/ui/" + soundName, null);
				this._soundEvents.Add(soundName, soundEvent);
			}
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00002FF0 File Offset: 0x000011F0
		void ITwoDimensionPlatform.PlaySoundEvent(string soundName)
		{
			SoundEvent soundEvent;
			if (this._soundEvents.TryGetValue(soundName, out soundEvent))
			{
				soundEvent.Play();
			}
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00003014 File Offset: 0x00001214
		void ITwoDimensionPlatform.StopAndRemoveSoundEvent(string soundName)
		{
			SoundEvent soundEvent;
			if (this._soundEvents.TryGetValue(soundName, out soundEvent))
			{
				soundEvent.Stop();
				this._soundEvents.Remove(soundName);
			}
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00003044 File Offset: 0x00001244
		void ITwoDimensionPlatform.OpenOnScreenKeyboard(string initialText, string descriptionText, int maxLength, int keyboardTypeEnum)
		{
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00003046 File Offset: 0x00001246
		void ITwoDimensionPlatform.BeginDebugPanel(string panelTitle)
		{
			Imgui.BeginMainThreadScope();
			Imgui.Begin(panelTitle);
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00003053 File Offset: 0x00001253
		void ITwoDimensionPlatform.EndDebugPanel()
		{
			Imgui.End();
			Imgui.EndMainThreadScope();
		}

		// Token: 0x0600003A RID: 58 RVA: 0x0000305F File Offset: 0x0000125F
		void ITwoDimensionPlatform.DrawDebugText(string text)
		{
			Imgui.Text(text);
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00003067 File Offset: 0x00001267
		bool ITwoDimensionPlatform.DrawDebugTreeNode(string text)
		{
			return Imgui.TreeNode(text);
		}

		// Token: 0x0600003C RID: 60 RVA: 0x0000306F File Offset: 0x0000126F
		void ITwoDimensionPlatform.PopDebugTreeNode()
		{
			Imgui.TreePop();
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00003076 File Offset: 0x00001276
		void ITwoDimensionPlatform.DrawCheckbox(string label, ref bool isChecked)
		{
			Imgui.Checkbox(label, ref isChecked);
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00003080 File Offset: 0x00001280
		bool ITwoDimensionPlatform.IsDebugItemHovered()
		{
			return Imgui.IsItemHovered();
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00003087 File Offset: 0x00001287
		bool ITwoDimensionPlatform.IsDebugModeEnabled()
		{
			return UIConfig.DebugModeEnabled;
		}

		// Token: 0x06000040 RID: 64 RVA: 0x0000308E File Offset: 0x0000128E
		public void Reset()
		{
		}

		// Token: 0x0400000C RID: 12
		private TwoDimensionView _view;

		// Token: 0x0400000D RID: 13
		private ScissorTestInfo _currentScissorTestInfo;

		// Token: 0x0400000E RID: 14
		private bool _scissorSet;

		// Token: 0x0400000F RID: 15
		private Dictionary<TwoDimensionEnginePlatform.MaterialTuple, Material> _materials;

		// Token: 0x04000010 RID: 16
		private Dictionary<Texture, Material> _textMaterials;

		// Token: 0x04000011 RID: 17
		private Dictionary<string, SoundEvent> _soundEvents;

		// Token: 0x0200000B RID: 11
		public struct MaterialTuple : IEquatable<TwoDimensionEnginePlatform.MaterialTuple>
		{
			// Token: 0x06000061 RID: 97 RVA: 0x0000336C File Offset: 0x0000156C
			public MaterialTuple(Texture texture, Texture overlayTexture, bool customMesh)
			{
				this.Texture = texture;
				this.OverlayTexture = overlayTexture;
				this.UseCustomMesh = customMesh;
			}

			// Token: 0x06000062 RID: 98 RVA: 0x00003383 File Offset: 0x00001583
			public bool Equals(TwoDimensionEnginePlatform.MaterialTuple other)
			{
				return other.Texture == this.Texture && other.OverlayTexture == this.OverlayTexture && other.UseCustomMesh == this.UseCustomMesh;
			}

			// Token: 0x06000063 RID: 99 RVA: 0x000033BC File Offset: 0x000015BC
			public override int GetHashCode()
			{
				int num = this.Texture.GetHashCode();
				num = ((this.OverlayTexture != null) ? ((num * 397) ^ this.OverlayTexture.GetHashCode()) : num);
				return (num * 397) ^ this.UseCustomMesh.GetHashCode();
			}

			// Token: 0x0400001F RID: 31
			public Texture Texture;

			// Token: 0x04000020 RID: 32
			public Texture OverlayTexture;

			// Token: 0x04000021 RID: 33
			public bool UseCustomMesh;
		}
	}
}
