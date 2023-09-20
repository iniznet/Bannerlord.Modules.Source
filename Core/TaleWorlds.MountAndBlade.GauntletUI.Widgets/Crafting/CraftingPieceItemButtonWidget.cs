using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Crafting
{
	// Token: 0x02000145 RID: 325
	public class CraftingPieceItemButtonWidget : ButtonWidget
	{
		// Token: 0x0600110A RID: 4362 RVA: 0x0002F6FC File Offset: 0x0002D8FC
		public CraftingPieceItemButtonWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x0600110B RID: 4363 RVA: 0x0002F705 File Offset: 0x0002D905
		private void UpdateSelfBrush()
		{
			if (this.DontHavePieceBrush == null || this.HasPieceBrush == null)
			{
				return;
			}
			base.Brush = (this.PlayerHasPiece ? this.HasPieceBrush : this.DontHavePieceBrush);
		}

		// Token: 0x0600110C RID: 4364 RVA: 0x0002F734 File Offset: 0x0002D934
		private void UpdateMaterialBrush()
		{
			if (this.DontHavePieceMaterialBrush == null || this.HasPieceMaterialBrush == null || this.ImageIdentifier == null)
			{
				return;
			}
			this.ImageIdentifier.Brush = (this.PlayerHasPiece ? this.HasPieceMaterialBrush : this.DontHavePieceMaterialBrush);
		}

		// Token: 0x17000607 RID: 1543
		// (get) Token: 0x0600110D RID: 4365 RVA: 0x0002F770 File Offset: 0x0002D970
		// (set) Token: 0x0600110E RID: 4366 RVA: 0x0002F778 File Offset: 0x0002D978
		public ImageIdentifierWidget ImageIdentifier
		{
			get
			{
				return this._imageIdentifier;
			}
			set
			{
				if (this._imageIdentifier != value)
				{
					this._imageIdentifier = value;
					this.UpdateMaterialBrush();
				}
			}
		}

		// Token: 0x17000608 RID: 1544
		// (get) Token: 0x0600110F RID: 4367 RVA: 0x0002F790 File Offset: 0x0002D990
		// (set) Token: 0x06001110 RID: 4368 RVA: 0x0002F798 File Offset: 0x0002D998
		public bool PlayerHasPiece
		{
			get
			{
				return this._playerHasPiece;
			}
			set
			{
				if (this._playerHasPiece != value)
				{
					this._playerHasPiece = value;
					this.UpdateSelfBrush();
					this.UpdateMaterialBrush();
				}
			}
		}

		// Token: 0x17000609 RID: 1545
		// (get) Token: 0x06001111 RID: 4369 RVA: 0x0002F7B6 File Offset: 0x0002D9B6
		// (set) Token: 0x06001112 RID: 4370 RVA: 0x0002F7BE File Offset: 0x0002D9BE
		public Brush HasPieceBrush
		{
			get
			{
				return this._hasPieceBrush;
			}
			set
			{
				if (this._hasPieceBrush != value)
				{
					this._hasPieceBrush = value;
					this.UpdateSelfBrush();
				}
			}
		}

		// Token: 0x1700060A RID: 1546
		// (get) Token: 0x06001113 RID: 4371 RVA: 0x0002F7D6 File Offset: 0x0002D9D6
		// (set) Token: 0x06001114 RID: 4372 RVA: 0x0002F7DE File Offset: 0x0002D9DE
		public Brush DontHavePieceBrush
		{
			get
			{
				return this._dontHavePieceBrush;
			}
			set
			{
				if (this._dontHavePieceBrush != value)
				{
					this._dontHavePieceBrush = value;
					this.UpdateSelfBrush();
				}
			}
		}

		// Token: 0x1700060B RID: 1547
		// (get) Token: 0x06001115 RID: 4373 RVA: 0x0002F7F6 File Offset: 0x0002D9F6
		// (set) Token: 0x06001116 RID: 4374 RVA: 0x0002F7FE File Offset: 0x0002D9FE
		public Brush HasPieceMaterialBrush
		{
			get
			{
				return this._hasPieceMaterialBrush;
			}
			set
			{
				if (this._hasPieceMaterialBrush != value)
				{
					this._hasPieceMaterialBrush = value;
					this.UpdateMaterialBrush();
				}
			}
		}

		// Token: 0x1700060C RID: 1548
		// (get) Token: 0x06001117 RID: 4375 RVA: 0x0002F816 File Offset: 0x0002DA16
		// (set) Token: 0x06001118 RID: 4376 RVA: 0x0002F81E File Offset: 0x0002DA1E
		public Brush DontHavePieceMaterialBrush
		{
			get
			{
				return this._dontHavePieceMaterialBrush;
			}
			set
			{
				if (this._dontHavePieceMaterialBrush != value)
				{
					this._dontHavePieceMaterialBrush = value;
					this.UpdateMaterialBrush();
				}
			}
		}

		// Token: 0x040007D3 RID: 2003
		private ImageIdentifierWidget _imageIdentifier;

		// Token: 0x040007D4 RID: 2004
		private bool _playerHasPiece;

		// Token: 0x040007D5 RID: 2005
		private Brush _hasPieceBrush;

		// Token: 0x040007D6 RID: 2006
		private Brush _dontHavePieceBrush;

		// Token: 0x040007D7 RID: 2007
		private Brush _hasPieceMaterialBrush;

		// Token: 0x040007D8 RID: 2008
		private Brush _dontHavePieceMaterialBrush;
	}
}
