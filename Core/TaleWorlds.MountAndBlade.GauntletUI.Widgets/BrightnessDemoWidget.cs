using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x02000009 RID: 9
	public class BrightnessDemoWidget : TextureWidget
	{
		// Token: 0x0600002A RID: 42 RVA: 0x000024EE File Offset: 0x000006EE
		public BrightnessDemoWidget(UIContext context)
			: base(context)
		{
			base.TextureProviderName = "BrightnessDemoTextureProvider";
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600002B RID: 43 RVA: 0x00002509 File Offset: 0x00000709
		// (set) Token: 0x0600002C RID: 44 RVA: 0x00002514 File Offset: 0x00000714
		[Editor(false)]
		public BrightnessDemoWidget.DemoTypes DemoType
		{
			get
			{
				return this._demoType;
			}
			set
			{
				if (this._demoType != value)
				{
					this._demoType = value;
					base.OnPropertyChanged<string>(Enum.GetName(typeof(BrightnessDemoWidget.DemoTypes), value), "DemoType");
					base.SetTextureProviderProperty("DemoType", (int)value);
				}
			}
		}

		// Token: 0x0400000F RID: 15
		private BrightnessDemoWidget.DemoTypes _demoType = BrightnessDemoWidget.DemoTypes.None;

		// Token: 0x02000172 RID: 370
		public enum DemoTypes
		{
			// Token: 0x0400089F RID: 2207
			None = -1,
			// Token: 0x040008A0 RID: 2208
			BrightnessWide,
			// Token: 0x040008A1 RID: 2209
			ExposureTexture1,
			// Token: 0x040008A2 RID: 2210
			ExposureTexture2,
			// Token: 0x040008A3 RID: 2211
			ExposureTexture3
		}
	}
}
