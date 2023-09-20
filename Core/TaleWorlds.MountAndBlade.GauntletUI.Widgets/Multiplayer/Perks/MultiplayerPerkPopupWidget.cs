using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Perks
{
	// Token: 0x0200008D RID: 141
	public class MultiplayerPerkPopupWidget : Widget
	{
		// Token: 0x1700029F RID: 671
		// (get) Token: 0x0600076F RID: 1903 RVA: 0x00015F49 File Offset: 0x00014149
		// (set) Token: 0x06000770 RID: 1904 RVA: 0x00015F51 File Offset: 0x00014151
		public bool ShowAboveContainer { get; set; }

		// Token: 0x06000771 RID: 1905 RVA: 0x00015F5A File Offset: 0x0001415A
		public MultiplayerPerkPopupWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000772 RID: 1906 RVA: 0x00015F63 File Offset: 0x00014163
		public void SetPopupPerksContainer(MultiplayerPerkContainerPanelWidget container)
		{
			this._latestContainer = container;
			base.ApplyActionOnAllChildren(new Action<Widget>(this.SetContainersOfChildren));
		}

		// Token: 0x06000773 RID: 1907 RVA: 0x00015F80 File Offset: 0x00014180
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

		// Token: 0x06000774 RID: 1908 RVA: 0x0001608C File Offset: 0x0001428C
		private void SetContainersOfChildren(Widget obj)
		{
			MultiplayerPerkItemToggleWidget multiplayerPerkItemToggleWidget;
			if ((multiplayerPerkItemToggleWidget = obj as MultiplayerPerkItemToggleWidget) != null)
			{
				multiplayerPerkItemToggleWidget.ContainerPanel = this._latestContainer;
			}
		}

		// Token: 0x04000359 RID: 857
		private MultiplayerPerkContainerPanelWidget _latestContainer;
	}
}
