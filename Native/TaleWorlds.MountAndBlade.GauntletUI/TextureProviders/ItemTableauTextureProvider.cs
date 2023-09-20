using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI;
using TaleWorlds.MountAndBlade.View.Tableaus;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.TextureProviders
{
	// Token: 0x02000018 RID: 24
	public class ItemTableauTextureProvider : TextureProvider
	{
		// Token: 0x1700002A RID: 42
		// (set) Token: 0x060000D3 RID: 211 RVA: 0x00005E40 File Offset: 0x00004040
		public string ItemModifierId
		{
			set
			{
				this._itemTableau.SetItemModifierId(value);
			}
		}

		// Token: 0x1700002B RID: 43
		// (set) Token: 0x060000D4 RID: 212 RVA: 0x00005E4E File Offset: 0x0000404E
		public string StringId
		{
			set
			{
				this._itemTableau.SetStringId(value);
			}
		}

		// Token: 0x1700002C RID: 44
		// (set) Token: 0x060000D5 RID: 213 RVA: 0x00005E5C File Offset: 0x0000405C
		public ItemRosterElement Item
		{
			set
			{
				this._itemTableau.SetItem(value);
			}
		}

		// Token: 0x1700002D RID: 45
		// (set) Token: 0x060000D6 RID: 214 RVA: 0x00005E6A File Offset: 0x0000406A
		public int Ammo
		{
			set
			{
				this._itemTableau.SetAmmo(value);
			}
		}

		// Token: 0x1700002E RID: 46
		// (set) Token: 0x060000D7 RID: 215 RVA: 0x00005E78 File Offset: 0x00004078
		public int AverageUnitCost
		{
			set
			{
				this._itemTableau.SetAverageUnitCost(value);
			}
		}

		// Token: 0x1700002F RID: 47
		// (set) Token: 0x060000D8 RID: 216 RVA: 0x00005E86 File Offset: 0x00004086
		public string BannerCode
		{
			set
			{
				this._itemTableau.SetBannerCode(value);
			}
		}

		// Token: 0x17000030 RID: 48
		// (set) Token: 0x060000D9 RID: 217 RVA: 0x00005E94 File Offset: 0x00004094
		public bool CurrentlyRotating
		{
			set
			{
				this._itemTableau.RotateItem(value);
			}
		}

		// Token: 0x17000031 RID: 49
		// (set) Token: 0x060000DA RID: 218 RVA: 0x00005EA2 File Offset: 0x000040A2
		public float RotateItemVertical
		{
			set
			{
				this._itemTableau.RotateItemVerticalWithAmount(value);
			}
		}

		// Token: 0x17000032 RID: 50
		// (set) Token: 0x060000DB RID: 219 RVA: 0x00005EB0 File Offset: 0x000040B0
		public float RotateItemHorizontal
		{
			set
			{
				this._itemTableau.RotateItemHorizontalWithAmount(value);
			}
		}

		// Token: 0x17000033 RID: 51
		// (set) Token: 0x060000DC RID: 220 RVA: 0x00005EBE File Offset: 0x000040BE
		public float InitialTiltRotation
		{
			set
			{
				this._itemTableau.SetInitialTiltRotation(value);
			}
		}

		// Token: 0x17000034 RID: 52
		// (set) Token: 0x060000DD RID: 221 RVA: 0x00005ECC File Offset: 0x000040CC
		public float InitialPanRotation
		{
			set
			{
				this._itemTableau.SetInitialPanRotation(value);
			}
		}

		// Token: 0x060000DE RID: 222 RVA: 0x00005EDA File Offset: 0x000040DA
		public ItemTableauTextureProvider()
		{
			this._itemTableau = new ItemTableau();
		}

		// Token: 0x060000DF RID: 223 RVA: 0x00005EED File Offset: 0x000040ED
		public override void Clear()
		{
			this._itemTableau.OnFinalize();
			base.Clear();
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x00005F00 File Offset: 0x00004100
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

		// Token: 0x060000E1 RID: 225 RVA: 0x00005F64 File Offset: 0x00004164
		public override Texture GetTexture(TwoDimensionContext twoDimensionContext, string name)
		{
			this.CheckTexture();
			return this._providedTexture;
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x00005F72 File Offset: 0x00004172
		public override void SetTargetSize(int width, int height)
		{
			base.SetTargetSize(width, height);
			this._itemTableau.SetTargetSize(width, height);
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x00005F89 File Offset: 0x00004189
		public override void Tick(float dt)
		{
			base.Tick(dt);
			this.CheckTexture();
			this._itemTableau.OnTick(dt);
		}

		// Token: 0x04000080 RID: 128
		private readonly ItemTableau _itemTableau;

		// Token: 0x04000081 RID: 129
		private Texture _texture;

		// Token: 0x04000082 RID: 130
		private Texture _providedTexture;
	}
}
