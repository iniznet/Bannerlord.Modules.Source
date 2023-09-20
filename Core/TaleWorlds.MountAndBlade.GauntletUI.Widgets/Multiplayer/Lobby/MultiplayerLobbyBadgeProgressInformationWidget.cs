using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby
{
	public class MultiplayerLobbyBadgeProgressInformationWidget : Widget
	{
		public float CenterBadgeSize { get; set; } = 200f;

		public float OuterBadgeBaseSize { get; set; } = 175f;

		public float SizeDecayFromCenterPerElement { get; set; } = 25f;

		public MultiplayerLobbyBadgeProgressInformationWidget(UIContext context)
			: base(context)
		{
		}

		private void OnBadgeAdded(Widget parent, Widget child)
		{
			this.ArrangeChildrenSizes();
		}

		private void ArrangeChildrenSizes()
		{
			this.ActiveBadgesList.IsVisible = this.ShownBadgeCount > 0;
			int num = this.ShownBadgeCount / 2;
			int num2 = 0;
			using (IEnumerator<Widget> enumerator = this.ActiveBadgesList.AllChildren.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					MultiplayerPlayerBadgeVisualWidget multiplayerPlayerBadgeVisualWidget;
					if ((multiplayerPlayerBadgeVisualWidget = enumerator.Current as MultiplayerPlayerBadgeVisualWidget) != null)
					{
						float num3 = (float)MathF.Abs(num2 - num);
						float num4 = this.OuterBadgeBaseSize - this.SizeDecayFromCenterPerElement * num3;
						if (num2 == num)
						{
							num4 = this.CenterBadgeSize;
						}
						multiplayerPlayerBadgeVisualWidget.SetForcedSize(num4, num4);
						num2++;
					}
				}
			}
		}

		[Editor(false)]
		public int ShownBadgeCount
		{
			get
			{
				return this._shownBadgeCount;
			}
			set
			{
				if (value != this._shownBadgeCount)
				{
					this._shownBadgeCount = value;
					base.OnPropertyChanged(value, "ShownBadgeCount");
					this.ArrangeChildrenSizes();
				}
			}
		}

		[Editor(false)]
		public ListPanel ActiveBadgesList
		{
			get
			{
				return this._activeBadgesList;
			}
			set
			{
				if (value != this._activeBadgesList)
				{
					if (this._activeBadgesList != null)
					{
						this._activeBadgesList.ItemAddEventHandlers.Remove(new Action<Widget, Widget>(this.OnBadgeAdded));
					}
					this._activeBadgesList = value;
					base.OnPropertyChanged<ListPanel>(value, "ActiveBadgesList");
					if (value != null)
					{
						this._activeBadgesList.ItemAddEventHandlers.Add(new Action<Widget, Widget>(this.OnBadgeAdded));
					}
				}
			}
		}

		private int _shownBadgeCount;

		private ListPanel _activeBadgesList;
	}
}
