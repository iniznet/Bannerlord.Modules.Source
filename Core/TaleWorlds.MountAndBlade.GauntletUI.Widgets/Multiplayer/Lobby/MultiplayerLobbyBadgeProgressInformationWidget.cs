using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby
{
	// Token: 0x02000092 RID: 146
	public class MultiplayerLobbyBadgeProgressInformationWidget : Widget
	{
		// Token: 0x170002B2 RID: 690
		// (get) Token: 0x060007A8 RID: 1960 RVA: 0x000168B5 File Offset: 0x00014AB5
		// (set) Token: 0x060007A9 RID: 1961 RVA: 0x000168BD File Offset: 0x00014ABD
		public float CenterBadgeSize { get; set; } = 200f;

		// Token: 0x170002B3 RID: 691
		// (get) Token: 0x060007AA RID: 1962 RVA: 0x000168C6 File Offset: 0x00014AC6
		// (set) Token: 0x060007AB RID: 1963 RVA: 0x000168CE File Offset: 0x00014ACE
		public float OuterBadgeBaseSize { get; set; } = 175f;

		// Token: 0x170002B4 RID: 692
		// (get) Token: 0x060007AC RID: 1964 RVA: 0x000168D7 File Offset: 0x00014AD7
		// (set) Token: 0x060007AD RID: 1965 RVA: 0x000168DF File Offset: 0x00014ADF
		public float SizeDecayFromCenterPerElement { get; set; } = 25f;

		// Token: 0x060007AE RID: 1966 RVA: 0x000168E8 File Offset: 0x00014AE8
		public MultiplayerLobbyBadgeProgressInformationWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060007AF RID: 1967 RVA: 0x00016912 File Offset: 0x00014B12
		private void OnBadgeAdded(Widget parent, Widget child)
		{
			this.ArrangeChildrenSizes();
		}

		// Token: 0x060007B0 RID: 1968 RVA: 0x0001691C File Offset: 0x00014B1C
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

		// Token: 0x170002B5 RID: 693
		// (get) Token: 0x060007B1 RID: 1969 RVA: 0x000169C8 File Offset: 0x00014BC8
		// (set) Token: 0x060007B2 RID: 1970 RVA: 0x000169D0 File Offset: 0x00014BD0
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

		// Token: 0x170002B6 RID: 694
		// (get) Token: 0x060007B3 RID: 1971 RVA: 0x000169F4 File Offset: 0x00014BF4
		// (set) Token: 0x060007B4 RID: 1972 RVA: 0x000169FC File Offset: 0x00014BFC
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

		// Token: 0x04000378 RID: 888
		private int _shownBadgeCount;

		// Token: 0x04000379 RID: 889
		private ListPanel _activeBadgesList;
	}
}
