using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	public class BrightnessDemoWidget : TextureWidget
	{
		public BrightnessDemoWidget(UIContext context)
			: base(context)
		{
			base.TextureProviderName = "BrightnessDemoTextureProvider";
		}

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

		private BrightnessDemoWidget.DemoTypes _demoType = BrightnessDemoWidget.DemoTypes.None;

		public enum DemoTypes
		{
			None = -1,
			BrightnessWide,
			ExposureTexture1,
			ExposureTexture2,
			ExposureTexture3
		}
	}
}
