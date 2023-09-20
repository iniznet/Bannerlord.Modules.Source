using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Crafting
{
	// Token: 0x02000146 RID: 326
	public class CraftingPieceItemImageWidget : ImageWidget
	{
		// Token: 0x06001119 RID: 4377 RVA: 0x0002F836 File Offset: 0x0002DA36
		public CraftingPieceItemImageWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x0600111A RID: 4378 RVA: 0x0002F83F File Offset: 0x0002DA3F
		private void UpdateSelfBrush()
		{
			if (this.DontHavePieceBrush == null || this.HasPieceBrush == null)
			{
				return;
			}
			base.Brush = (this.PlayerHasPiece ? this.HasPieceBrush : this.DontHavePieceBrush);
		}

		// Token: 0x0600111B RID: 4379 RVA: 0x0002F86E File Offset: 0x0002DA6E
		private void UpdateMaterialBrush()
		{
			if (this.DontHavePieceMaterialBrush == null || this.HasPieceMaterialBrush == null || this.ImageIdentifier == null)
			{
				return;
			}
			this.ImageIdentifier.Brush = (this.PlayerHasPiece ? this.HasPieceMaterialBrush : this.DontHavePieceMaterialBrush);
		}

		// Token: 0x1700060D RID: 1549
		// (get) Token: 0x0600111C RID: 4380 RVA: 0x0002F8AA File Offset: 0x0002DAAA
		// (set) Token: 0x0600111D RID: 4381 RVA: 0x0002F8B2 File Offset: 0x0002DAB2
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

		// Token: 0x1700060E RID: 1550
		// (get) Token: 0x0600111E RID: 4382 RVA: 0x0002F8CA File Offset: 0x0002DACA
		// (set) Token: 0x0600111F RID: 4383 RVA: 0x0002F8D2 File Offset: 0x0002DAD2
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

		// Token: 0x1700060F RID: 1551
		// (get) Token: 0x06001120 RID: 4384 RVA: 0x0002F8F0 File Offset: 0x0002DAF0
		// (set) Token: 0x06001121 RID: 4385 RVA: 0x0002F8F8 File Offset: 0x0002DAF8
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

		// Token: 0x17000610 RID: 1552
		// (get) Token: 0x06001122 RID: 4386 RVA: 0x0002F910 File Offset: 0x0002DB10
		// (set) Token: 0x06001123 RID: 4387 RVA: 0x0002F918 File Offset: 0x0002DB18
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

		// Token: 0x17000611 RID: 1553
		// (get) Token: 0x06001124 RID: 4388 RVA: 0x0002F930 File Offset: 0x0002DB30
		// (set) Token: 0x06001125 RID: 4389 RVA: 0x0002F938 File Offset: 0x0002DB38
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

		// Token: 0x17000612 RID: 1554
		// (get) Token: 0x06001126 RID: 4390 RVA: 0x0002F950 File Offset: 0x0002DB50
		// (set) Token: 0x06001127 RID: 4391 RVA: 0x0002F958 File Offset: 0x0002DB58
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

		// Token: 0x040007D9 RID: 2009
		private ImageIdentifierWidget _imageIdentifier;

		// Token: 0x040007DA RID: 2010
		private bool _playerHasPiece;

		// Token: 0x040007DB RID: 2011
		private Brush _hasPieceBrush;

		// Token: 0x040007DC RID: 2012
		private Brush _dontHavePieceBrush;

		// Token: 0x040007DD RID: 2013
		private Brush _hasPieceMaterialBrush;

		// Token: 0x040007DE RID: 2014
		private Brush _dontHavePieceMaterialBrush;
	}
}
