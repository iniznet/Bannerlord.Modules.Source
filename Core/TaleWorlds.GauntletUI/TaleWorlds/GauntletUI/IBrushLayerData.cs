using System;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI
{
	// Token: 0x02000023 RID: 35
	public interface IBrushLayerData
	{
		// Token: 0x170000C5 RID: 197
		// (get) Token: 0x0600029D RID: 669
		// (set) Token: 0x0600029E RID: 670
		string Name { get; set; }

		// Token: 0x170000C6 RID: 198
		// (get) Token: 0x0600029F RID: 671
		// (set) Token: 0x060002A0 RID: 672
		Sprite Sprite { get; set; }

		// Token: 0x170000C7 RID: 199
		// (get) Token: 0x060002A1 RID: 673
		// (set) Token: 0x060002A2 RID: 674
		Color Color { get; set; }

		// Token: 0x170000C8 RID: 200
		// (get) Token: 0x060002A3 RID: 675
		// (set) Token: 0x060002A4 RID: 676
		float ColorFactor { get; set; }

		// Token: 0x170000C9 RID: 201
		// (get) Token: 0x060002A5 RID: 677
		// (set) Token: 0x060002A6 RID: 678
		float AlphaFactor { get; set; }

		// Token: 0x170000CA RID: 202
		// (get) Token: 0x060002A7 RID: 679
		// (set) Token: 0x060002A8 RID: 680
		float HueFactor { get; set; }

		// Token: 0x170000CB RID: 203
		// (get) Token: 0x060002A9 RID: 681
		// (set) Token: 0x060002AA RID: 682
		float SaturationFactor { get; set; }

		// Token: 0x170000CC RID: 204
		// (get) Token: 0x060002AB RID: 683
		// (set) Token: 0x060002AC RID: 684
		float ValueFactor { get; set; }

		// Token: 0x170000CD RID: 205
		// (get) Token: 0x060002AD RID: 685
		// (set) Token: 0x060002AE RID: 686
		bool IsHidden { get; set; }

		// Token: 0x170000CE RID: 206
		// (get) Token: 0x060002AF RID: 687
		// (set) Token: 0x060002B0 RID: 688
		float XOffset { get; set; }

		// Token: 0x170000CF RID: 207
		// (get) Token: 0x060002B1 RID: 689
		// (set) Token: 0x060002B2 RID: 690
		float YOffset { get; set; }

		// Token: 0x170000D0 RID: 208
		// (get) Token: 0x060002B3 RID: 691
		// (set) Token: 0x060002B4 RID: 692
		float ExtendLeft { get; set; }

		// Token: 0x170000D1 RID: 209
		// (get) Token: 0x060002B5 RID: 693
		// (set) Token: 0x060002B6 RID: 694
		float ExtendRight { get; set; }

		// Token: 0x170000D2 RID: 210
		// (get) Token: 0x060002B7 RID: 695
		// (set) Token: 0x060002B8 RID: 696
		float ExtendTop { get; set; }

		// Token: 0x170000D3 RID: 211
		// (get) Token: 0x060002B9 RID: 697
		// (set) Token: 0x060002BA RID: 698
		float ExtendBottom { get; set; }

		// Token: 0x170000D4 RID: 212
		// (get) Token: 0x060002BB RID: 699
		// (set) Token: 0x060002BC RID: 700
		float OverridenWidth { get; set; }

		// Token: 0x170000D5 RID: 213
		// (get) Token: 0x060002BD RID: 701
		// (set) Token: 0x060002BE RID: 702
		float OverridenHeight { get; set; }

		// Token: 0x170000D6 RID: 214
		// (get) Token: 0x060002BF RID: 703
		// (set) Token: 0x060002C0 RID: 704
		BrushLayerSizePolicy WidthPolicy { get; set; }

		// Token: 0x170000D7 RID: 215
		// (get) Token: 0x060002C1 RID: 705
		// (set) Token: 0x060002C2 RID: 706
		BrushLayerSizePolicy HeightPolicy { get; set; }

		// Token: 0x170000D8 RID: 216
		// (get) Token: 0x060002C3 RID: 707
		// (set) Token: 0x060002C4 RID: 708
		bool HorizontalFlip { get; set; }

		// Token: 0x170000D9 RID: 217
		// (get) Token: 0x060002C5 RID: 709
		// (set) Token: 0x060002C6 RID: 710
		bool VerticalFlip { get; set; }

		// Token: 0x170000DA RID: 218
		// (get) Token: 0x060002C7 RID: 711
		// (set) Token: 0x060002C8 RID: 712
		bool UseOverlayAlphaAsMask { get; set; }

		// Token: 0x170000DB RID: 219
		// (get) Token: 0x060002C9 RID: 713
		// (set) Token: 0x060002CA RID: 714
		BrushOverlayMethod OverlayMethod { get; set; }

		// Token: 0x170000DC RID: 220
		// (get) Token: 0x060002CB RID: 715
		// (set) Token: 0x060002CC RID: 716
		Sprite OverlaySprite { get; set; }

		// Token: 0x170000DD RID: 221
		// (get) Token: 0x060002CD RID: 717
		// (set) Token: 0x060002CE RID: 718
		float OverlayXOffset { get; set; }

		// Token: 0x170000DE RID: 222
		// (get) Token: 0x060002CF RID: 719
		// (set) Token: 0x060002D0 RID: 720
		float OverlayYOffset { get; set; }

		// Token: 0x170000DF RID: 223
		// (get) Token: 0x060002D1 RID: 721
		// (set) Token: 0x060002D2 RID: 722
		bool UseRandomBaseOverlayXOffset { get; set; }

		// Token: 0x170000E0 RID: 224
		// (get) Token: 0x060002D3 RID: 723
		// (set) Token: 0x060002D4 RID: 724
		bool UseRandomBaseOverlayYOffset { get; set; }

		// Token: 0x060002D5 RID: 725
		float GetValueAsFloat(BrushAnimationProperty.BrushAnimationPropertyType propertyType);

		// Token: 0x060002D6 RID: 726
		Color GetValueAsColor(BrushAnimationProperty.BrushAnimationPropertyType propertyType);

		// Token: 0x060002D7 RID: 727
		Sprite GetValueAsSprite(BrushAnimationProperty.BrushAnimationPropertyType propertyType);
	}
}
