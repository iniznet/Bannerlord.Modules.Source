using System;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.Engine.GauntletUI
{
	public class TwoDimensionEnginePlatform : ITwoDimensionPlatform
	{
		float ITwoDimensionPlatform.Width
		{
			get
			{
				return Screen.RealScreenResolutionWidth * ScreenManager.UsableArea.X;
			}
		}

		float ITwoDimensionPlatform.Height
		{
			get
			{
				return Screen.RealScreenResolutionHeight * ScreenManager.UsableArea.Y;
			}
		}

		float ITwoDimensionPlatform.ReferenceWidth
		{
			get
			{
				return 1920f;
			}
		}

		float ITwoDimensionPlatform.ReferenceHeight
		{
			get
			{
				return 1080f;
			}
		}

		float ITwoDimensionPlatform.ApplicationTime
		{
			get
			{
				return Time.ApplicationTime;
			}
		}

		public TwoDimensionEnginePlatform(TwoDimensionView view)
		{
			this._view = view;
			this._scissorSet = false;
			this._materials = new Dictionary<TwoDimensionEnginePlatform.MaterialTuple, Material>();
			this._textMaterials = new Dictionary<Texture, Material>();
			this._soundEvents = new Dictionary<string, SoundEvent>();
		}

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

		void ITwoDimensionPlatform.SetScissor(ScissorTestInfo scissorTestInfo)
		{
			this._currentScissorTestInfo = scissorTestInfo;
			this._scissorSet = true;
		}

		void ITwoDimensionPlatform.ResetScissor()
		{
			this._scissorSet = false;
		}

		void ITwoDimensionPlatform.PlaySound(string soundName)
		{
			SoundEvent.PlaySound2D("event:/ui/" + soundName);
		}

		void ITwoDimensionPlatform.CreateSoundEvent(string soundName)
		{
			if (!this._soundEvents.ContainsKey(soundName))
			{
				SoundEvent soundEvent = SoundEvent.CreateEventFromString("event:/ui/" + soundName, null);
				this._soundEvents.Add(soundName, soundEvent);
			}
		}

		void ITwoDimensionPlatform.PlaySoundEvent(string soundName)
		{
			SoundEvent soundEvent;
			if (this._soundEvents.TryGetValue(soundName, out soundEvent))
			{
				soundEvent.Play();
			}
		}

		void ITwoDimensionPlatform.StopAndRemoveSoundEvent(string soundName)
		{
			SoundEvent soundEvent;
			if (this._soundEvents.TryGetValue(soundName, out soundEvent))
			{
				soundEvent.Stop();
				this._soundEvents.Remove(soundName);
			}
		}

		void ITwoDimensionPlatform.OpenOnScreenKeyboard(string initialText, string descriptionText, int maxLength, int keyboardTypeEnum)
		{
		}

		void ITwoDimensionPlatform.BeginDebugPanel(string panelTitle)
		{
			Imgui.BeginMainThreadScope();
			Imgui.Begin(panelTitle);
		}

		void ITwoDimensionPlatform.EndDebugPanel()
		{
			Imgui.End();
			Imgui.EndMainThreadScope();
		}

		void ITwoDimensionPlatform.DrawDebugText(string text)
		{
			Imgui.Text(text);
		}

		bool ITwoDimensionPlatform.DrawDebugTreeNode(string text)
		{
			return Imgui.TreeNode(text);
		}

		void ITwoDimensionPlatform.PopDebugTreeNode()
		{
			Imgui.TreePop();
		}

		void ITwoDimensionPlatform.DrawCheckbox(string label, ref bool isChecked)
		{
			Imgui.Checkbox(label, ref isChecked);
		}

		bool ITwoDimensionPlatform.IsDebugItemHovered()
		{
			return Imgui.IsItemHovered();
		}

		bool ITwoDimensionPlatform.IsDebugModeEnabled()
		{
			return UIConfig.DebugModeEnabled;
		}

		public void Reset()
		{
		}

		private TwoDimensionView _view;

		private ScissorTestInfo _currentScissorTestInfo;

		private bool _scissorSet;

		private Dictionary<TwoDimensionEnginePlatform.MaterialTuple, Material> _materials;

		private Dictionary<Texture, Material> _textMaterials;

		private Dictionary<string, SoundEvent> _soundEvents;

		public struct MaterialTuple : IEquatable<TwoDimensionEnginePlatform.MaterialTuple>
		{
			public MaterialTuple(Texture texture, Texture overlayTexture, bool customMesh)
			{
				this.Texture = texture;
				this.OverlayTexture = overlayTexture;
				this.UseCustomMesh = customMesh;
			}

			public bool Equals(TwoDimensionEnginePlatform.MaterialTuple other)
			{
				return other.Texture == this.Texture && other.OverlayTexture == this.OverlayTexture && other.UseCustomMesh == this.UseCustomMesh;
			}

			public override int GetHashCode()
			{
				int num = this.Texture.GetHashCode();
				num = ((this.OverlayTexture != null) ? ((num * 397) ^ this.OverlayTexture.GetHashCode()) : num);
				return (num * 397) ^ this.UseCustomMesh.GetHashCode();
			}

			public Texture Texture;

			public Texture OverlayTexture;

			public bool UseCustomMesh;
		}
	}
}
