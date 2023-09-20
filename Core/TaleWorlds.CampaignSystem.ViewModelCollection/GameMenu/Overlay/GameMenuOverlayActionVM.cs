using System;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.Overlay
{
	public class GameMenuOverlayActionVM : StringItemWithEnabledAndHintVM
	{
		public GameMenuOverlayActionVM(Action<object> onExecute, string item, bool isEnabled, object identifier, TextObject hint = null)
			: base(onExecute, item, isEnabled, identifier, hint)
		{
		}

		[DataSourceProperty]
		public bool IsHiglightEnabled
		{
			get
			{
				return this._isHiglightEnabled;
			}
			set
			{
				if (value != this._isHiglightEnabled)
				{
					this._isHiglightEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsHiglightEnabled");
				}
			}
		}

		private bool _isHiglightEnabled;
	}
}
