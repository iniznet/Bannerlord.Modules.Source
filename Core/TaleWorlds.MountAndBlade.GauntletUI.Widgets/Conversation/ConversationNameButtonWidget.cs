using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Conversation
{
	// Token: 0x0200014D RID: 333
	public class ConversationNameButtonWidget : ButtonWidget
	{
		// Token: 0x0600115A RID: 4442 RVA: 0x0002FEF1 File Offset: 0x0002E0F1
		public ConversationNameButtonWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x0600115B RID: 4443 RVA: 0x0002FEFA File Offset: 0x0002E0FA
		protected override void OnHoverBegin()
		{
			base.OnHoverBegin();
			this.RelationBarContainer.IsVisible = this.IsRelationEnabled;
		}

		// Token: 0x0600115C RID: 4444 RVA: 0x0002FF13 File Offset: 0x0002E113
		protected override void OnHoverEnd()
		{
			base.OnHoverEnd();
			this.RelationBarContainer.IsVisible = false;
		}

		// Token: 0x17000624 RID: 1572
		// (get) Token: 0x0600115D RID: 4445 RVA: 0x0002FF27 File Offset: 0x0002E127
		// (set) Token: 0x0600115E RID: 4446 RVA: 0x0002FF2F File Offset: 0x0002E12F
		[Editor(false)]
		public bool IsRelationEnabled
		{
			get
			{
				return this._isRelationEnabled;
			}
			set
			{
				if (value != this._isRelationEnabled)
				{
					this._isRelationEnabled = value;
					base.OnPropertyChanged(value, "IsRelationEnabled");
				}
			}
		}

		// Token: 0x17000625 RID: 1573
		// (get) Token: 0x0600115F RID: 4447 RVA: 0x0002FF4D File Offset: 0x0002E14D
		// (set) Token: 0x06001160 RID: 4448 RVA: 0x0002FF55 File Offset: 0x0002E155
		[Editor(false)]
		public Widget RelationBarContainer
		{
			get
			{
				return this._relationBarContainer;
			}
			set
			{
				if (value != this._relationBarContainer)
				{
					this._relationBarContainer = value;
					base.OnPropertyChanged<Widget>(value, "RelationBarContainer");
				}
			}
		}

		// Token: 0x040007F4 RID: 2036
		private bool _isRelationEnabled;

		// Token: 0x040007F5 RID: 2037
		private Widget _relationBarContainer;
	}
}
