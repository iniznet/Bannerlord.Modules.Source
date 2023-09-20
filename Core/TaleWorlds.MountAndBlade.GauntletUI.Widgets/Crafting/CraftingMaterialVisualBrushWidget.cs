using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Crafting
{
	// Token: 0x02000144 RID: 324
	public class CraftingMaterialVisualBrushWidget : BrushWidget
	{
		// Token: 0x06001103 RID: 4355 RVA: 0x0002F64F File Offset: 0x0002D84F
		public CraftingMaterialVisualBrushWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06001104 RID: 4356 RVA: 0x0002F65F File Offset: 0x0002D85F
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._visualDirty)
			{
				this.UpdateVisual();
				this._visualDirty = false;
			}
		}

		// Token: 0x06001105 RID: 4357 RVA: 0x0002F680 File Offset: 0x0002D880
		private void UpdateVisual()
		{
			this.RegisterBrushStatesOfWidget();
			string text = this.MaterialType;
			if (this.IsBig)
			{
				text += "Big";
			}
			this.SetState(text);
		}

		// Token: 0x17000605 RID: 1541
		// (get) Token: 0x06001106 RID: 4358 RVA: 0x0002F6B5 File Offset: 0x0002D8B5
		// (set) Token: 0x06001107 RID: 4359 RVA: 0x0002F6BD File Offset: 0x0002D8BD
		public string MaterialType
		{
			get
			{
				return this._materialType;
			}
			set
			{
				if (this._materialType != value)
				{
					this._materialType = value;
					this._visualDirty = true;
				}
			}
		}

		// Token: 0x17000606 RID: 1542
		// (get) Token: 0x06001108 RID: 4360 RVA: 0x0002F6DB File Offset: 0x0002D8DB
		// (set) Token: 0x06001109 RID: 4361 RVA: 0x0002F6E3 File Offset: 0x0002D8E3
		public bool IsBig
		{
			get
			{
				return this._isBig;
			}
			set
			{
				if (this._isBig != value)
				{
					this._isBig = value;
					this._visualDirty = true;
				}
			}
		}

		// Token: 0x040007D0 RID: 2000
		private bool _visualDirty = true;

		// Token: 0x040007D1 RID: 2001
		private string _materialType;

		// Token: 0x040007D2 RID: 2002
		private bool _isBig;
	}
}
