using System;
using TaleWorlds.Library;

namespace TaleWorlds.TwoDimension.Standalone
{
	// Token: 0x0200000B RID: 11
	public class TwoDimensionPlatform : ITwoDimensionPlatform, ITwoDimensionResourceContext
	{
		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000098 RID: 152 RVA: 0x00004210 File Offset: 0x00002410
		float ITwoDimensionPlatform.Width
		{
			get
			{
				return (float)this._form.Width;
			}
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000099 RID: 153 RVA: 0x0000421E File Offset: 0x0000241E
		float ITwoDimensionPlatform.Height
		{
			get
			{
				return (float)this._form.Height;
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x0600009A RID: 154 RVA: 0x0000422C File Offset: 0x0000242C
		float ITwoDimensionPlatform.ReferenceWidth
		{
			get
			{
				return (float)this._form.Width;
			}
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x0600009B RID: 155 RVA: 0x0000423A File Offset: 0x0000243A
		float ITwoDimensionPlatform.ReferenceHeight
		{
			get
			{
				return (float)this._form.Height;
			}
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x0600009C RID: 156 RVA: 0x00004248 File Offset: 0x00002448
		float ITwoDimensionPlatform.ApplicationTime
		{
			get
			{
				return (float)Environment.TickCount;
			}
		}

		// Token: 0x0600009D RID: 157 RVA: 0x00004250 File Offset: 0x00002450
		public TwoDimensionPlatform(GraphicsForm form)
		{
			this._form = form;
			this._graphicsContext = this._form.GraphicsContext;
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00004270 File Offset: 0x00002470
		void ITwoDimensionPlatform.Draw(float x, float y, Material material, DrawObject2D drawObject2D, int layer)
		{
			this._graphicsContext.DrawElements(x, y, material, drawObject2D);
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00004282 File Offset: 0x00002482
		Texture ITwoDimensionResourceContext.LoadTexture(ResourceDepot resourceDepot, string name)
		{
			OpenGLTexture openGLTexture = new OpenGLTexture();
			openGLTexture.LoadFromFile(resourceDepot, name);
			return new Texture(openGLTexture);
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00004296 File Offset: 0x00002496
		void ITwoDimensionPlatform.PlaySound(string soundName)
		{
			Debug.Print("Playing sound: " + soundName, 0, Debug.DebugColor.White, 17592186044416UL);
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x000042B4 File Offset: 0x000024B4
		void ITwoDimensionPlatform.SetScissor(ScissorTestInfo scissorTestInfo)
		{
			this._graphicsContext.SetScissor(scissorTestInfo);
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x000042C2 File Offset: 0x000024C2
		void ITwoDimensionPlatform.ResetScissor()
		{
			this._graphicsContext.ResetScissor();
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x000042CF File Offset: 0x000024CF
		void ITwoDimensionPlatform.CreateSoundEvent(string soundName)
		{
			Debug.Print("Created sound event: " + soundName, 0, Debug.DebugColor.White, 17592186044416UL);
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x000042ED File Offset: 0x000024ED
		void ITwoDimensionPlatform.StopAndRemoveSoundEvent(string soundName)
		{
			Debug.Print("Stopped sound event: " + soundName, 0, Debug.DebugColor.White, 17592186044416UL);
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x0000430B File Offset: 0x0000250B
		void ITwoDimensionPlatform.PlaySoundEvent(string soundName)
		{
			Debug.Print("Played sound event: " + soundName, 0, Debug.DebugColor.White, 17592186044416UL);
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x00004329 File Offset: 0x00002529
		void ITwoDimensionPlatform.OpenOnScreenKeyboard(string initialText, string descriptionText, int maxLength, int keyboardTypeEnum)
		{
			Debug.Print("Opened on-screen keyboard", 0, Debug.DebugColor.White, 17592186044416UL);
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x00004341 File Offset: 0x00002541
		void ITwoDimensionPlatform.BeginDebugPanel(string panelTitle)
		{
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x00004343 File Offset: 0x00002543
		void ITwoDimensionPlatform.EndDebugPanel()
		{
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x00004345 File Offset: 0x00002545
		void ITwoDimensionPlatform.DrawDebugText(string text)
		{
			Debug.Print(text, 0, Debug.DebugColor.White, 17592186044416UL);
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00004359 File Offset: 0x00002559
		bool ITwoDimensionPlatform.IsDebugModeEnabled()
		{
			return false;
		}

		// Token: 0x060000AB RID: 171 RVA: 0x0000435C File Offset: 0x0000255C
		bool ITwoDimensionPlatform.DrawDebugTreeNode(string text)
		{
			return false;
		}

		// Token: 0x060000AC RID: 172 RVA: 0x0000435F File Offset: 0x0000255F
		void ITwoDimensionPlatform.DrawCheckbox(string label, ref bool isChecked)
		{
		}

		// Token: 0x060000AD RID: 173 RVA: 0x00004361 File Offset: 0x00002561
		bool ITwoDimensionPlatform.IsDebugItemHovered()
		{
			return false;
		}

		// Token: 0x060000AE RID: 174 RVA: 0x00004364 File Offset: 0x00002564
		void ITwoDimensionPlatform.PopDebugTreeNode()
		{
		}

		// Token: 0x0400003E RID: 62
		private GraphicsContext _graphicsContext;

		// Token: 0x0400003F RID: 63
		private GraphicsForm _form;
	}
}
