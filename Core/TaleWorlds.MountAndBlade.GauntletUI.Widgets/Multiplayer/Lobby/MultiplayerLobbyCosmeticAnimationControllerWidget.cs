using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby
{
	// Token: 0x02000094 RID: 148
	public class MultiplayerLobbyCosmeticAnimationControllerWidget : Widget
	{
		// Token: 0x060007C0 RID: 1984 RVA: 0x00016F4B File Offset: 0x0001514B
		private double GetRandomDoubleBetween(double min, double max)
		{
			return base.Context.UIRandom.NextDouble() * (max - min) + max;
		}

		// Token: 0x060007C1 RID: 1985 RVA: 0x00016F64 File Offset: 0x00015164
		public MultiplayerLobbyCosmeticAnimationControllerWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060007C2 RID: 1986 RVA: 0x00016FBA File Offset: 0x000151BA
		private void RestartAllAnimations()
		{
			this.SetAllAnimationPartColors();
			this.StopAllAnimations();
			this.StartAllAnimations();
		}

		// Token: 0x060007C3 RID: 1987 RVA: 0x00016FCE File Offset: 0x000151CE
		private void SetAllAnimationPartColors()
		{
			this.ApplyActionOnAllAnimations(new Action<MultiplayerLobbyCosmeticAnimationPartWidget>(this.SetColorOfPart));
		}

		// Token: 0x060007C4 RID: 1988 RVA: 0x00016FE2 File Offset: 0x000151E2
		private void StartAllAnimations()
		{
			this.ApplyActionOnAllAnimations(new Action<MultiplayerLobbyCosmeticAnimationPartWidget>(this.StartAnimationOfPart));
		}

		// Token: 0x060007C5 RID: 1989 RVA: 0x00016FF6 File Offset: 0x000151F6
		private void StopAllAnimations()
		{
			this.ApplyActionOnAllAnimations(new Action<MultiplayerLobbyCosmeticAnimationPartWidget>(this.StopAnimationOfPart));
		}

		// Token: 0x060007C6 RID: 1990 RVA: 0x0001700C File Offset: 0x0001520C
		private void StartAnimationOfPart(MultiplayerLobbyCosmeticAnimationPartWidget part)
		{
			double randomDoubleBetween = this.GetRandomDoubleBetween((double)this.MinAlphaChangeDuration, (double)this.MaxAlphaChangeDuration);
			double randomDoubleBetween2 = this.GetRandomDoubleBetween((double)this.MinAlphaLowerBound, (double)this.MinAlphaUpperBound);
			double randomDoubleBetween3 = this.GetRandomDoubleBetween((double)this.MaxAlphaLowerBound, (double)this.MaxAlphaUpperBound);
			part.StartAnimation((float)randomDoubleBetween, (float)randomDoubleBetween2, (float)randomDoubleBetween3);
		}

		// Token: 0x060007C7 RID: 1991 RVA: 0x00017064 File Offset: 0x00015264
		private void StopAnimationOfPart(MultiplayerLobbyCosmeticAnimationPartWidget part)
		{
			part.StopAnimation();
		}

		// Token: 0x060007C8 RID: 1992 RVA: 0x0001706C File Offset: 0x0001526C
		private void SetColorOfPart(MultiplayerLobbyCosmeticAnimationPartWidget part)
		{
			switch (this.CosmeticRarity)
			{
			case 0:
			case 1:
				part.Color = this.RarityCommonColor;
				return;
			case 2:
				part.Color = this.RarityRareColor;
				return;
			case 3:
				part.Color = this.RarityUniqueColor;
				return;
			default:
				part.Color = MultiplayerLobbyCosmeticAnimationControllerWidget.DefaultColor;
				return;
			}
		}

		// Token: 0x060007C9 RID: 1993 RVA: 0x000170CC File Offset: 0x000152CC
		private void ApplyActionOnAllAnimations(Action<MultiplayerLobbyCosmeticAnimationPartWidget> action)
		{
			BasicContainer animationPartContainer = this.AnimationPartContainer;
			if (animationPartContainer == null)
			{
				return;
			}
			animationPartContainer.Children.ForEach(delegate(Widget c)
			{
				action(c as MultiplayerLobbyCosmeticAnimationPartWidget);
			});
		}

		// Token: 0x060007CA RID: 1994 RVA: 0x00017108 File Offset: 0x00015308
		private void OnAnimationPartAdded(Widget parent, Widget child)
		{
			MultiplayerLobbyCosmeticAnimationPartWidget multiplayerLobbyCosmeticAnimationPartWidget = child as MultiplayerLobbyCosmeticAnimationPartWidget;
			this.SetColorOfPart(multiplayerLobbyCosmeticAnimationPartWidget);
			this.StartAnimationOfPart(multiplayerLobbyCosmeticAnimationPartWidget);
		}

		// Token: 0x170002BA RID: 698
		// (get) Token: 0x060007CB RID: 1995 RVA: 0x0001712A File Offset: 0x0001532A
		// (set) Token: 0x060007CC RID: 1996 RVA: 0x00017132 File Offset: 0x00015332
		[Editor(false)]
		public int CosmeticRarity
		{
			get
			{
				return this._cosmeticRarity;
			}
			set
			{
				if (value != this._cosmeticRarity)
				{
					this._cosmeticRarity = value;
					base.OnPropertyChanged(value, "CosmeticRarity");
					this.RestartAllAnimations();
				}
			}
		}

		// Token: 0x170002BB RID: 699
		// (get) Token: 0x060007CD RID: 1997 RVA: 0x00017156 File Offset: 0x00015356
		// (set) Token: 0x060007CE RID: 1998 RVA: 0x0001715E File Offset: 0x0001535E
		[Editor(false)]
		public float MinAlphaChangeDuration
		{
			get
			{
				return this._minAlphaChangeDuration;
			}
			set
			{
				if (this._minAlphaChangeDuration != value)
				{
					this._minAlphaChangeDuration = value;
					base.OnPropertyChanged(value, "MinAlphaChangeDuration");
					this.RestartAllAnimations();
				}
			}
		}

		// Token: 0x170002BC RID: 700
		// (get) Token: 0x060007CF RID: 1999 RVA: 0x00017182 File Offset: 0x00015382
		// (set) Token: 0x060007D0 RID: 2000 RVA: 0x0001718A File Offset: 0x0001538A
		[Editor(false)]
		public float MaxAlphaChangeDuration
		{
			get
			{
				return this._maxAlphaChangeDuration;
			}
			set
			{
				if (this._maxAlphaChangeDuration != value)
				{
					this._maxAlphaChangeDuration = value;
					base.OnPropertyChanged(value, "MaxAlphaChangeDuration");
					this.RestartAllAnimations();
				}
			}
		}

		// Token: 0x170002BD RID: 701
		// (get) Token: 0x060007D1 RID: 2001 RVA: 0x000171AE File Offset: 0x000153AE
		// (set) Token: 0x060007D2 RID: 2002 RVA: 0x000171B6 File Offset: 0x000153B6
		[Editor(false)]
		public float MinAlphaLowerBound
		{
			get
			{
				return this._minAlphaLowerBound;
			}
			set
			{
				if (this._minAlphaLowerBound != value)
				{
					this._minAlphaLowerBound = value;
					base.OnPropertyChanged(value, "MinAlphaLowerBound");
					this.RestartAllAnimations();
				}
			}
		}

		// Token: 0x170002BE RID: 702
		// (get) Token: 0x060007D3 RID: 2003 RVA: 0x000171DA File Offset: 0x000153DA
		// (set) Token: 0x060007D4 RID: 2004 RVA: 0x000171E2 File Offset: 0x000153E2
		[Editor(false)]
		public float MinAlphaUpperBound
		{
			get
			{
				return this._minAlphaUpperBound;
			}
			set
			{
				if (this._minAlphaUpperBound != value)
				{
					this._minAlphaUpperBound = value;
					base.OnPropertyChanged(value, "MinAlphaUpperBound");
					this.RestartAllAnimations();
				}
			}
		}

		// Token: 0x170002BF RID: 703
		// (get) Token: 0x060007D5 RID: 2005 RVA: 0x00017206 File Offset: 0x00015406
		// (set) Token: 0x060007D6 RID: 2006 RVA: 0x0001720E File Offset: 0x0001540E
		[Editor(false)]
		public float MaxAlphaLowerBound
		{
			get
			{
				return this._maxAlphaLowerBound;
			}
			set
			{
				if (this._maxAlphaLowerBound != value)
				{
					this._maxAlphaLowerBound = value;
					base.OnPropertyChanged(value, "MaxAlphaLowerBound");
					this.RestartAllAnimations();
				}
			}
		}

		// Token: 0x170002C0 RID: 704
		// (get) Token: 0x060007D7 RID: 2007 RVA: 0x00017232 File Offset: 0x00015432
		// (set) Token: 0x060007D8 RID: 2008 RVA: 0x0001723A File Offset: 0x0001543A
		[Editor(false)]
		public float MaxAlphaUpperBound
		{
			get
			{
				return this._maxAlphaUpperBound;
			}
			set
			{
				if (this._maxAlphaUpperBound != value)
				{
					this._maxAlphaUpperBound = value;
					base.OnPropertyChanged(value, "MaxAlphaUpperBound");
					this.RestartAllAnimations();
				}
			}
		}

		// Token: 0x170002C1 RID: 705
		// (get) Token: 0x060007D9 RID: 2009 RVA: 0x0001725E File Offset: 0x0001545E
		// (set) Token: 0x060007DA RID: 2010 RVA: 0x00017266 File Offset: 0x00015466
		[Editor(false)]
		public Color RarityCommonColor
		{
			get
			{
				return this._rarityCommonColor;
			}
			set
			{
				if (this._rarityCommonColor != value)
				{
					this._rarityCommonColor = value;
					base.OnPropertyChanged(value, "RarityCommonColor");
					this.RestartAllAnimations();
				}
			}
		}

		// Token: 0x170002C2 RID: 706
		// (get) Token: 0x060007DB RID: 2011 RVA: 0x0001728F File Offset: 0x0001548F
		// (set) Token: 0x060007DC RID: 2012 RVA: 0x00017297 File Offset: 0x00015497
		[Editor(false)]
		public Color RarityRareColor
		{
			get
			{
				return this._rarityRareColor;
			}
			set
			{
				if (this._rarityRareColor != value)
				{
					this._rarityRareColor = value;
					base.OnPropertyChanged(value, "RarityRareColor");
					this.RestartAllAnimations();
				}
			}
		}

		// Token: 0x170002C3 RID: 707
		// (get) Token: 0x060007DD RID: 2013 RVA: 0x000172C0 File Offset: 0x000154C0
		// (set) Token: 0x060007DE RID: 2014 RVA: 0x000172C8 File Offset: 0x000154C8
		[Editor(false)]
		public Color RarityUniqueColor
		{
			get
			{
				return this._rarityUniqueColor;
			}
			set
			{
				if (this._rarityUniqueColor != value)
				{
					this._rarityUniqueColor = value;
					base.OnPropertyChanged(value, "RarityUniqueColor");
					this.RestartAllAnimations();
				}
			}
		}

		// Token: 0x170002C4 RID: 708
		// (get) Token: 0x060007DF RID: 2015 RVA: 0x000172F1 File Offset: 0x000154F1
		// (set) Token: 0x060007E0 RID: 2016 RVA: 0x000172FC File Offset: 0x000154FC
		[Editor(false)]
		public BasicContainer AnimationPartContainer
		{
			get
			{
				return this._animationPartContainer;
			}
			set
			{
				if (value != this._animationPartContainer)
				{
					if (this._animationPartContainer != null)
					{
						this._animationPartContainer.ItemAddEventHandlers.Remove(new Action<Widget, Widget>(this.OnAnimationPartAdded));
					}
					this._animationPartContainer = value;
					if (this._animationPartContainer != null)
					{
						this._animationPartContainer.ItemAddEventHandlers.Add(new Action<Widget, Widget>(this.OnAnimationPartAdded));
					}
					base.OnPropertyChanged<BasicContainer>(value, "AnimationPartContainer");
					this.RestartAllAnimations();
				}
			}
		}

		// Token: 0x0400038F RID: 911
		private static readonly Color DefaultColor = Color.FromUint(0U);

		// Token: 0x04000390 RID: 912
		private int _cosmeticRarity;

		// Token: 0x04000391 RID: 913
		private float _minAlphaChangeDuration = 1.5f;

		// Token: 0x04000392 RID: 914
		private float _maxAlphaChangeDuration = 2.5f;

		// Token: 0x04000393 RID: 915
		private float _minAlphaLowerBound = 0.4f;

		// Token: 0x04000394 RID: 916
		private float _minAlphaUpperBound = 0.6f;

		// Token: 0x04000395 RID: 917
		private float _maxAlphaLowerBound = 0.6f;

		// Token: 0x04000396 RID: 918
		private float _maxAlphaUpperBound = 0.8f;

		// Token: 0x04000397 RID: 919
		private Color _rarityCommonColor;

		// Token: 0x04000398 RID: 920
		private Color _rarityRareColor;

		// Token: 0x04000399 RID: 921
		private Color _rarityUniqueColor;

		// Token: 0x0400039A RID: 922
		private BasicContainer _animationPartContainer;
	}
}
