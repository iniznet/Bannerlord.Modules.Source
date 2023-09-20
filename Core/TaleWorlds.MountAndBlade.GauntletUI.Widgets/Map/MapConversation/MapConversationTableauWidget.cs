using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Map.MapConversation
{
	public class MapConversationTableauWidget : TextureWidget
	{
		public MapConversationTableauWidget(UIContext context)
			: base(context)
		{
			base.TextureProviderName = "MapConversationTextureProvider";
			this._isRenderRequestedPreviousFrame = false;
			base.UpdateTextureWidget();
			base.EventManager.AddAfterFinalizedCallback(new Action(this.OnEventManagerIsFinalized));
		}

		private void OnEventManagerIsFinalized()
		{
			if (!this._setForClearNextFrame)
			{
				TextureProvider textureProvider = base.TextureProvider;
				if (textureProvider == null)
				{
					return;
				}
				textureProvider.Clear(false);
			}
		}

		protected override void OnDisconnectedFromRoot()
		{
		}

		[Editor(false)]
		public object Data
		{
			get
			{
				return this._data;
			}
			set
			{
				if (value != this._data)
				{
					this._data = value;
					base.OnPropertyChanged<object>(value, "Data");
					base.SetTextureProviderProperty("Data", value);
				}
			}
		}

		[Editor(false)]
		public bool IsTableauEnabled
		{
			get
			{
				return this._isTableauEnabled;
			}
			set
			{
				if (value != this._isTableauEnabled)
				{
					this._isTableauEnabled = value;
					base.OnPropertyChanged(value, "IsTableauEnabled");
					base.SetTextureProviderProperty("IsEnabled", value);
					if (this._isTableauEnabled)
					{
						this._isRenderRequestedPreviousFrame = true;
					}
				}
			}
		}

		private object _data;

		private bool _isTableauEnabled;
	}
}
