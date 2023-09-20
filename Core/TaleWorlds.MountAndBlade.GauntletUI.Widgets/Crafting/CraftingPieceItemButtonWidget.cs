using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Crafting
{
	public class CraftingPieceItemButtonWidget : ButtonWidget
	{
		public CraftingPieceItemButtonWidget(UIContext context)
			: base(context)
		{
		}

		private void UpdateSelfBrush()
		{
			if (this.DontHavePieceBrush == null || this.HasPieceBrush == null)
			{
				return;
			}
			base.Brush = (this.PlayerHasPiece ? this.HasPieceBrush : this.DontHavePieceBrush);
		}

		private void UpdateMaterialBrush()
		{
			if (this.DontHavePieceMaterialBrush == null || this.HasPieceMaterialBrush == null || this.ImageIdentifier == null)
			{
				return;
			}
			this.ImageIdentifier.Brush = (this.PlayerHasPiece ? this.HasPieceMaterialBrush : this.DontHavePieceMaterialBrush);
		}

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

		private ImageIdentifierWidget _imageIdentifier;

		private bool _playerHasPiece;

		private Brush _hasPieceBrush;

		private Brush _dontHavePieceBrush;

		private Brush _hasPieceMaterialBrush;

		private Brush _dontHavePieceMaterialBrush;
	}
}
