using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI;
using TaleWorlds.MountAndBlade.View.Tableaus;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.TextureProviders
{
	public class ItemTableauTextureProvider : TextureProvider
	{
		public string ItemModifierId
		{
			set
			{
				this._itemTableau.SetItemModifierId(value);
			}
		}

		public string StringId
		{
			set
			{
				this._itemTableau.SetStringId(value);
			}
		}

		public ItemRosterElement Item
		{
			set
			{
				this._itemTableau.SetItem(value);
			}
		}

		public int Ammo
		{
			set
			{
				this._itemTableau.SetAmmo(value);
			}
		}

		public int AverageUnitCost
		{
			set
			{
				this._itemTableau.SetAverageUnitCost(value);
			}
		}

		public string BannerCode
		{
			set
			{
				this._itemTableau.SetBannerCode(value);
			}
		}

		public bool CurrentlyRotating
		{
			set
			{
				this._itemTableau.RotateItem(value);
			}
		}

		public float RotateItemVertical
		{
			set
			{
				this._itemTableau.RotateItemVerticalWithAmount(value);
			}
		}

		public float RotateItemHorizontal
		{
			set
			{
				this._itemTableau.RotateItemHorizontalWithAmount(value);
			}
		}

		public float InitialTiltRotation
		{
			set
			{
				this._itemTableau.SetInitialTiltRotation(value);
			}
		}

		public float InitialPanRotation
		{
			set
			{
				this._itemTableau.SetInitialPanRotation(value);
			}
		}

		public ItemTableauTextureProvider()
		{
			this._itemTableau = new ItemTableau();
		}

		public override void Clear()
		{
			this._itemTableau.OnFinalize();
			base.Clear();
		}

		private void CheckTexture()
		{
			if (this._texture != this._itemTableau.Texture)
			{
				this._texture = this._itemTableau.Texture;
				if (this._texture != null)
				{
					EngineTexture engineTexture = new EngineTexture(this._texture);
					this._providedTexture = new Texture(engineTexture);
					return;
				}
				this._providedTexture = null;
			}
		}

		public override Texture GetTexture(TwoDimensionContext twoDimensionContext, string name)
		{
			this.CheckTexture();
			return this._providedTexture;
		}

		public override void SetTargetSize(int width, int height)
		{
			base.SetTargetSize(width, height);
			this._itemTableau.SetTargetSize(width, height);
		}

		public override void Tick(float dt)
		{
			base.Tick(dt);
			this.CheckTexture();
			this._itemTableau.OnTick(dt);
		}

		private readonly ItemTableau _itemTableau;

		private Texture _texture;

		private Texture _providedTexture;
	}
}
