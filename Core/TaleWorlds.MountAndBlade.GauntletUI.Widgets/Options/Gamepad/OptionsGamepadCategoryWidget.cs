using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Options.Gamepad
{
	public class OptionsGamepadCategoryWidget : Widget
	{
		public Widget Playstation4LayoutParentWidget { get; set; }

		public Widget Playstation5LayoutParentWidget { get; set; }

		public Widget XboxLayoutParentWidget { get; set; }

		public OptionsGamepadCategoryWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this._initalized)
			{
				this.SetGamepadLayoutVisibility(this.CurrentGamepadType);
				this._initalized = true;
			}
		}

		private void SetGamepadLayoutVisibility(int gamepadType)
		{
			this.XboxLayoutParentWidget.IsVisible = false;
			this.Playstation4LayoutParentWidget.IsVisible = false;
			this.Playstation5LayoutParentWidget.IsVisible = false;
			if (gamepadType == 0)
			{
				this.XboxLayoutParentWidget.IsVisible = true;
				return;
			}
			if (gamepadType == 1)
			{
				this.Playstation4LayoutParentWidget.IsVisible = true;
				return;
			}
			if (gamepadType == 2)
			{
				this.Playstation5LayoutParentWidget.IsVisible = true;
				return;
			}
			this.XboxLayoutParentWidget.IsVisible = true;
			Debug.FailedAssert("This kind of gamepad is not visually supported", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.GauntletUI.Widgets\\Options\\Gamepad\\OptionsGamepadCategoryWidget.cs", "SetGamepadLayoutVisibility", 47);
		}

		public int CurrentGamepadType
		{
			get
			{
				return this._currentGamepadType;
			}
			set
			{
				if (this._currentGamepadType != value)
				{
					this._currentGamepadType = value;
				}
			}
		}

		private bool _initalized;

		private int _currentGamepadType = -1;
	}
}
