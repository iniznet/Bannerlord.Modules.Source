using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.ClassLoadout
{
	public class MultiplayerClassLoadoutItemTabListPanel : ListPanel
	{
		public event Action OnInitialized;

		public MultiplayerClassLoadoutItemTabListPanel(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this._isInitialized)
			{
				this._isInitialized = true;
				Action onInitialized = this.OnInitialized;
				if (onInitialized == null)
				{
					return;
				}
				onInitialized();
			}
		}

		private bool _isInitialized;
	}
}
