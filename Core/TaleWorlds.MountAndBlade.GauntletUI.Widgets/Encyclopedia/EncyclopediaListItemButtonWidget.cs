using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Encyclopedia
{
	// Token: 0x02000139 RID: 313
	public class EncyclopediaListItemButtonWidget : ButtonWidget
	{
		// Token: 0x170005D1 RID: 1489
		// (get) Token: 0x0600107C RID: 4220 RVA: 0x0002E4AA File Offset: 0x0002C6AA
		// (set) Token: 0x0600107D RID: 4221 RVA: 0x0002E4B2 File Offset: 0x0002C6B2
		public TextWidget ListItemNameTextWidget { get; set; }

		// Token: 0x170005D2 RID: 1490
		// (get) Token: 0x0600107E RID: 4222 RVA: 0x0002E4BB File Offset: 0x0002C6BB
		// (set) Token: 0x0600107F RID: 4223 RVA: 0x0002E4C3 File Offset: 0x0002C6C3
		public TextWidget ListComparedValueTextWidget { get; set; }

		// Token: 0x170005D3 RID: 1491
		// (get) Token: 0x06001080 RID: 4224 RVA: 0x0002E4CC File Offset: 0x0002C6CC
		// (set) Token: 0x06001081 RID: 4225 RVA: 0x0002E4D4 File Offset: 0x0002C6D4
		public Brush InfoAvailableItemNameBrush { get; set; }

		// Token: 0x170005D4 RID: 1492
		// (get) Token: 0x06001082 RID: 4226 RVA: 0x0002E4DD File Offset: 0x0002C6DD
		// (set) Token: 0x06001083 RID: 4227 RVA: 0x0002E4E5 File Offset: 0x0002C6E5
		public Brush InfoUnvailableItemNameBrush { get; set; }

		// Token: 0x170005D5 RID: 1493
		// (get) Token: 0x06001084 RID: 4228 RVA: 0x0002E4EE File Offset: 0x0002C6EE
		// (set) Token: 0x06001085 RID: 4229 RVA: 0x0002E4F6 File Offset: 0x0002C6F6
		public bool IsInfoAvailable { get; set; }

		// Token: 0x06001086 RID: 4230 RVA: 0x0002E4FF File Offset: 0x0002C6FF
		public EncyclopediaListItemButtonWidget(UIContext context)
			: base(context)
		{
			base.EventManager.AddLateUpdateAction(this, new Action<float>(this.OnThisLateUpdate), 1);
		}

		// Token: 0x06001087 RID: 4231 RVA: 0x0002E524 File Offset: 0x0002C724
		public void OnThisLateUpdate(float dt)
		{
			this.ListItemNameTextWidget.Brush = (this.IsInfoAvailable ? this.InfoAvailableItemNameBrush : this.InfoUnvailableItemNameBrush);
			this.ListComparedValueTextWidget.Brush = (this.IsInfoAvailable ? this.InfoAvailableItemNameBrush : this.InfoUnvailableItemNameBrush);
		}

		// Token: 0x170005D6 RID: 1494
		// (get) Token: 0x06001088 RID: 4232 RVA: 0x0002E573 File Offset: 0x0002C773
		// (set) Token: 0x06001089 RID: 4233 RVA: 0x0002E57B File Offset: 0x0002C77B
		[Editor(false)]
		public string ListItemId
		{
			get
			{
				return this._listItemId;
			}
			set
			{
				if (this._listItemId != value)
				{
					this._listItemId = value;
					base.OnPropertyChanged<string>(value, "ListItemId");
				}
			}
		}

		// Token: 0x04000799 RID: 1945
		private string _listItemId;
	}
}
