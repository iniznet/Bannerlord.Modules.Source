using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.ClassLoadout
{
	// Token: 0x020000BE RID: 190
	public class MultiplayerClassLoadoutTroopTupleVisualWidget : Widget
	{
		// Token: 0x060009B5 RID: 2485 RVA: 0x0001BC2D File Offset: 0x00019E2D
		public MultiplayerClassLoadoutTroopTupleVisualWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060009B6 RID: 2486 RVA: 0x0001BC38 File Offset: 0x00019E38
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this._initialized)
			{
				base.Sprite = base.Context.SpriteData.GetSprite("MPClassLoadout\\TroopTupleImages\\" + this.TroopTypeCode + "1");
				base.Sprite = base.Sprite;
				base.SuggestedWidth = (float)base.Sprite.Width;
				base.SuggestedHeight = (float)base.Sprite.Height;
				this._initialized = true;
			}
		}

		// Token: 0x17000365 RID: 869
		// (get) Token: 0x060009B7 RID: 2487 RVA: 0x0001BCB6 File Offset: 0x00019EB6
		// (set) Token: 0x060009B8 RID: 2488 RVA: 0x0001BCBE File Offset: 0x00019EBE
		public string FactionCode
		{
			get
			{
				return this._factionCode;
			}
			set
			{
				if (value != this._factionCode)
				{
					this._factionCode = value;
					base.OnPropertyChanged<string>(value, "FactionCode");
				}
			}
		}

		// Token: 0x17000366 RID: 870
		// (get) Token: 0x060009B9 RID: 2489 RVA: 0x0001BCE1 File Offset: 0x00019EE1
		// (set) Token: 0x060009BA RID: 2490 RVA: 0x0001BCE9 File Offset: 0x00019EE9
		public string TroopTypeCode
		{
			get
			{
				return this._troopTypeCode;
			}
			set
			{
				if (value != this._troopTypeCode)
				{
					this._troopTypeCode = value;
					base.OnPropertyChanged<string>(value, "TroopTypeCode");
				}
			}
		}

		// Token: 0x17000367 RID: 871
		// (get) Token: 0x060009BB RID: 2491 RVA: 0x0001BD0C File Offset: 0x00019F0C
		// (set) Token: 0x060009BC RID: 2492 RVA: 0x0001BD14 File Offset: 0x00019F14
		public bool UseSecondary
		{
			get
			{
				return this._useSecondary;
			}
			set
			{
				if (value != this._useSecondary)
				{
					this._useSecondary = value;
					base.OnPropertyChanged(value, "UseSecondary");
				}
			}
		}

		// Token: 0x04000471 RID: 1137
		private bool _initialized;

		// Token: 0x04000472 RID: 1138
		private string _factionCode;

		// Token: 0x04000473 RID: 1139
		private string _troopTypeCode;

		// Token: 0x04000474 RID: 1140
		private bool _useSecondary;
	}
}
