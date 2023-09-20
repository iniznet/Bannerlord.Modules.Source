using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Perks
{
	public class MultiplayerPerkPopupWidget : Widget
	{
		public bool ShowAboveContainer { get; set; }

		public MultiplayerPerkPopupWidget(UIContext context)
			: base(context)
		{
		}

		public void SetPopupPerksContainer(MultiplayerPerkContainerPanelWidget container)
		{
			this._latestContainer = container;
			base.ApplyActionOnAllChildren(new Action<Widget>(this.SetContainersOfChildren));
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (base.IsVisible && this._latestContainer != null)
			{
				float num = this._latestContainer.GlobalPosition.X - (base.Size.X / 2f - this._latestContainer.Size.X / 2f);
				base.ScaledPositionXOffset = Mathf.Clamp(num - base.EventManager.LeftUsableAreaStart, 0f, base.Context.EventManager.PageSize.X - base.Size.X);
				if (!this.ShowAboveContainer)
				{
					base.ScaledPositionYOffset = this._latestContainer.GlobalPosition.Y + this._latestContainer.Size.Y - base.EventManager.TopUsableAreaStart;
					return;
				}
				base.ScaledPositionYOffset = this._latestContainer.GlobalPosition.Y - base.Size.Y - base.EventManager.TopUsableAreaStart;
			}
		}

		private void SetContainersOfChildren(Widget obj)
		{
			MultiplayerPerkItemToggleWidget multiplayerPerkItemToggleWidget;
			if ((multiplayerPerkItemToggleWidget = obj as MultiplayerPerkItemToggleWidget) != null)
			{
				multiplayerPerkItemToggleWidget.ContainerPanel = this._latestContainer;
			}
		}

		private MultiplayerPerkContainerPanelWidget _latestContainer;
	}
}
