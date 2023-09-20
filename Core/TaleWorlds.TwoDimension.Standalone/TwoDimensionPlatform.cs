using System;
using TaleWorlds.Library;

namespace TaleWorlds.TwoDimension.Standalone
{
	public class TwoDimensionPlatform : ITwoDimensionPlatform, ITwoDimensionResourceContext
	{
		float ITwoDimensionPlatform.Width
		{
			get
			{
				return (float)this._form.Width;
			}
		}

		float ITwoDimensionPlatform.Height
		{
			get
			{
				return (float)this._form.Height;
			}
		}

		float ITwoDimensionPlatform.ReferenceWidth
		{
			get
			{
				return (float)this._form.Width;
			}
		}

		float ITwoDimensionPlatform.ReferenceHeight
		{
			get
			{
				return (float)this._form.Height;
			}
		}

		float ITwoDimensionPlatform.ApplicationTime
		{
			get
			{
				return (float)Environment.TickCount;
			}
		}

		public TwoDimensionPlatform(GraphicsForm form, bool isAssetsUnderDefaultFolders)
		{
			this._form = form;
			this._isAssetsUnderDefaultFolders = isAssetsUnderDefaultFolders;
			this._graphicsContext = this._form.GraphicsContext;
		}

		void ITwoDimensionPlatform.Draw(float x, float y, Material material, DrawObject2D drawObject2D, int layer)
		{
			this._graphicsContext.DrawElements(x, y, material, drawObject2D);
		}

		Texture ITwoDimensionResourceContext.LoadTexture(ResourceDepot resourceDepot, string name)
		{
			OpenGLTexture openGLTexture = new OpenGLTexture();
			string text = name;
			if (!this._isAssetsUnderDefaultFolders)
			{
				string[] array = name.Split(new char[] { '\\' });
				text = array[array.Length - 1];
			}
			openGLTexture.LoadFromFile(resourceDepot, text);
			return new Texture(openGLTexture);
		}

		void ITwoDimensionPlatform.PlaySound(string soundName)
		{
			Debug.Print("Playing sound: " + soundName, 0, Debug.DebugColor.White, 17592186044416UL);
		}

		void ITwoDimensionPlatform.SetScissor(ScissorTestInfo scissorTestInfo)
		{
			this._graphicsContext.SetScissor(scissorTestInfo);
		}

		void ITwoDimensionPlatform.ResetScissor()
		{
			this._graphicsContext.ResetScissor();
		}

		void ITwoDimensionPlatform.CreateSoundEvent(string soundName)
		{
			Debug.Print("Created sound event: " + soundName, 0, Debug.DebugColor.White, 17592186044416UL);
		}

		void ITwoDimensionPlatform.StopAndRemoveSoundEvent(string soundName)
		{
			Debug.Print("Stopped sound event: " + soundName, 0, Debug.DebugColor.White, 17592186044416UL);
		}

		void ITwoDimensionPlatform.PlaySoundEvent(string soundName)
		{
			Debug.Print("Played sound event: " + soundName, 0, Debug.DebugColor.White, 17592186044416UL);
		}

		void ITwoDimensionPlatform.OpenOnScreenKeyboard(string initialText, string descriptionText, int maxLength, int keyboardTypeEnum)
		{
			Debug.Print("Opened on-screen keyboard", 0, Debug.DebugColor.White, 17592186044416UL);
		}

		void ITwoDimensionPlatform.BeginDebugPanel(string panelTitle)
		{
		}

		void ITwoDimensionPlatform.EndDebugPanel()
		{
		}

		void ITwoDimensionPlatform.DrawDebugText(string text)
		{
			Debug.Print(text, 0, Debug.DebugColor.White, 17592186044416UL);
		}

		bool ITwoDimensionPlatform.IsDebugModeEnabled()
		{
			return false;
		}

		bool ITwoDimensionPlatform.DrawDebugTreeNode(string text)
		{
			return false;
		}

		void ITwoDimensionPlatform.DrawCheckbox(string label, ref bool isChecked)
		{
		}

		bool ITwoDimensionPlatform.IsDebugItemHovered()
		{
			return false;
		}

		void ITwoDimensionPlatform.PopDebugTreeNode()
		{
		}

		private GraphicsContext _graphicsContext;

		private GraphicsForm _form;

		private bool _isAssetsUnderDefaultFolders;
	}
}
