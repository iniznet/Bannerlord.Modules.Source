using System;

namespace TaleWorlds.TwoDimension
{
	// Token: 0x02000022 RID: 34
	public interface ITwoDimensionPlatform
	{
		// Token: 0x17000069 RID: 105
		// (get) Token: 0x06000139 RID: 313
		float Width { get; }

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x0600013A RID: 314
		float Height { get; }

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x0600013B RID: 315
		float ReferenceWidth { get; }

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x0600013C RID: 316
		float ReferenceHeight { get; }

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x0600013D RID: 317
		float ApplicationTime { get; }

		// Token: 0x0600013E RID: 318
		void Draw(float x, float y, Material material, DrawObject2D drawObject2D, int layer);

		// Token: 0x0600013F RID: 319
		void SetScissor(ScissorTestInfo scissorTestInfo);

		// Token: 0x06000140 RID: 320
		void ResetScissor();

		// Token: 0x06000141 RID: 321
		void PlaySound(string soundName);

		// Token: 0x06000142 RID: 322
		void CreateSoundEvent(string soundName);

		// Token: 0x06000143 RID: 323
		void PlaySoundEvent(string soundName);

		// Token: 0x06000144 RID: 324
		void StopAndRemoveSoundEvent(string soundName);

		// Token: 0x06000145 RID: 325
		void OpenOnScreenKeyboard(string initialText, string descriptionText, int maxLength, int keyboardTypeEnum);

		// Token: 0x06000146 RID: 326
		void BeginDebugPanel(string panelTitle);

		// Token: 0x06000147 RID: 327
		void EndDebugPanel();

		// Token: 0x06000148 RID: 328
		void DrawDebugText(string text);

		// Token: 0x06000149 RID: 329
		bool DrawDebugTreeNode(string text);

		// Token: 0x0600014A RID: 330
		void PopDebugTreeNode();

		// Token: 0x0600014B RID: 331
		void DrawCheckbox(string label, ref bool isChecked);

		// Token: 0x0600014C RID: 332
		bool IsDebugItemHovered();

		// Token: 0x0600014D RID: 333
		bool IsDebugModeEnabled();
	}
}
