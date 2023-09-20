using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000212 RID: 530
	public enum GameKeyDefinition
	{
		// Token: 0x04000A06 RID: 2566
		Up,
		// Token: 0x04000A07 RID: 2567
		Down,
		// Token: 0x04000A08 RID: 2568
		Left,
		// Token: 0x04000A09 RID: 2569
		Right,
		// Token: 0x04000A0A RID: 2570
		Leave,
		// Token: 0x04000A0B RID: 2571
		ShowIndicators,
		// Token: 0x04000A0C RID: 2572
		InitiateAllChat,
		// Token: 0x04000A0D RID: 2573
		InitiateTeamChat,
		// Token: 0x04000A0E RID: 2574
		FinalizeChat,
		// Token: 0x04000A0F RID: 2575
		Attack,
		// Token: 0x04000A10 RID: 2576
		Defend,
		// Token: 0x04000A11 RID: 2577
		EquipPrimaryWeapon,
		// Token: 0x04000A12 RID: 2578
		EquipSecondaryWeapon,
		// Token: 0x04000A13 RID: 2579
		Action,
		// Token: 0x04000A14 RID: 2580
		Jump,
		// Token: 0x04000A15 RID: 2581
		Crouch,
		// Token: 0x04000A16 RID: 2582
		Kick,
		// Token: 0x04000A17 RID: 2583
		ToggleWeaponMode,
		// Token: 0x04000A18 RID: 2584
		EquipWeapon1,
		// Token: 0x04000A19 RID: 2585
		EquipWeapon2,
		// Token: 0x04000A1A RID: 2586
		EquipWeapon3,
		// Token: 0x04000A1B RID: 2587
		EquipWeapon4,
		// Token: 0x04000A1C RID: 2588
		DropWeapon,
		// Token: 0x04000A1D RID: 2589
		SheathWeapon,
		// Token: 0x04000A1E RID: 2590
		Zoom,
		// Token: 0x04000A1F RID: 2591
		ViewCharacter,
		// Token: 0x04000A20 RID: 2592
		LockTarget,
		// Token: 0x04000A21 RID: 2593
		CameraToggle,
		// Token: 0x04000A22 RID: 2594
		MissionScreenHotkeyCameraZoomIn,
		// Token: 0x04000A23 RID: 2595
		MissionScreenHotkeyCameraZoomOut,
		// Token: 0x04000A24 RID: 2596
		ToggleWalkMode,
		// Token: 0x04000A25 RID: 2597
		Cheer,
		// Token: 0x04000A26 RID: 2598
		Taunt,
		// Token: 0x04000A27 RID: 2599
		PushToTalk,
		// Token: 0x04000A28 RID: 2600
		EquipmentSwitch,
		// Token: 0x04000A29 RID: 2601
		ShowMouse,
		// Token: 0x04000A2A RID: 2602
		BannerWindow,
		// Token: 0x04000A2B RID: 2603
		CharacterWindow,
		// Token: 0x04000A2C RID: 2604
		InventoryWindow,
		// Token: 0x04000A2D RID: 2605
		EncyclopediaWindow,
		// Token: 0x04000A2E RID: 2606
		KingdomWindow,
		// Token: 0x04000A2F RID: 2607
		ClanWindow,
		// Token: 0x04000A30 RID: 2608
		QuestsWindow,
		// Token: 0x04000A31 RID: 2609
		PartyWindow,
		// Token: 0x04000A32 RID: 2610
		FacegenWindow,
		// Token: 0x04000A33 RID: 2611
		MapMoveUp,
		// Token: 0x04000A34 RID: 2612
		MapMoveDown,
		// Token: 0x04000A35 RID: 2613
		MapMoveRight,
		// Token: 0x04000A36 RID: 2614
		MapMoveLeft,
		// Token: 0x04000A37 RID: 2615
		PartyMoveUp,
		// Token: 0x04000A38 RID: 2616
		PartyMoveDown,
		// Token: 0x04000A39 RID: 2617
		PartyMoveRight,
		// Token: 0x04000A3A RID: 2618
		PartyMoveLeft,
		// Token: 0x04000A3B RID: 2619
		QuickSave,
		// Token: 0x04000A3C RID: 2620
		MapFastMove,
		// Token: 0x04000A3D RID: 2621
		MapZoomIn,
		// Token: 0x04000A3E RID: 2622
		MapZoomOut,
		// Token: 0x04000A3F RID: 2623
		MapRotateLeft,
		// Token: 0x04000A40 RID: 2624
		MapRotateRight,
		// Token: 0x04000A41 RID: 2625
		MapTimeStop,
		// Token: 0x04000A42 RID: 2626
		MapTimeNormal,
		// Token: 0x04000A43 RID: 2627
		MapTimeFastForward,
		// Token: 0x04000A44 RID: 2628
		MapTimeTogglePause,
		// Token: 0x04000A45 RID: 2629
		MapCameraFollowMode,
		// Token: 0x04000A46 RID: 2630
		MapToggleFastForward,
		// Token: 0x04000A47 RID: 2631
		MapTrackSettlement,
		// Token: 0x04000A48 RID: 2632
		MapGoToEncylopedia,
		// Token: 0x04000A49 RID: 2633
		ViewOrders,
		// Token: 0x04000A4A RID: 2634
		SelectOrder1,
		// Token: 0x04000A4B RID: 2635
		SelectOrder2,
		// Token: 0x04000A4C RID: 2636
		SelectOrder3,
		// Token: 0x04000A4D RID: 2637
		SelectOrder4,
		// Token: 0x04000A4E RID: 2638
		SelectOrder5,
		// Token: 0x04000A4F RID: 2639
		SelectOrder6,
		// Token: 0x04000A50 RID: 2640
		SelectOrder7,
		// Token: 0x04000A51 RID: 2641
		SelectOrder8,
		// Token: 0x04000A52 RID: 2642
		SelectOrderReturn,
		// Token: 0x04000A53 RID: 2643
		EveryoneHear,
		// Token: 0x04000A54 RID: 2644
		Group0Hear,
		// Token: 0x04000A55 RID: 2645
		Group1Hear,
		// Token: 0x04000A56 RID: 2646
		Group2Hear,
		// Token: 0x04000A57 RID: 2647
		Group3Hear,
		// Token: 0x04000A58 RID: 2648
		Group4Hear,
		// Token: 0x04000A59 RID: 2649
		Group5Hear,
		// Token: 0x04000A5A RID: 2650
		Group6Hear,
		// Token: 0x04000A5B RID: 2651
		Group7Hear,
		// Token: 0x04000A5C RID: 2652
		HoldOrder,
		// Token: 0x04000A5D RID: 2653
		SelectNextGroup,
		// Token: 0x04000A5E RID: 2654
		SelectPreviousGroup,
		// Token: 0x04000A5F RID: 2655
		ToggleGroupSelection,
		// Token: 0x04000A60 RID: 2656
		HideUI,
		// Token: 0x04000A61 RID: 2657
		CameraRollLeft,
		// Token: 0x04000A62 RID: 2658
		CameraRollRight,
		// Token: 0x04000A63 RID: 2659
		TakePicture,
		// Token: 0x04000A64 RID: 2660
		TakePictureWithAdditionalPasses,
		// Token: 0x04000A65 RID: 2661
		ToggleCameraFollowMode,
		// Token: 0x04000A66 RID: 2662
		ToggleMouse,
		// Token: 0x04000A67 RID: 2663
		ToggleVignette,
		// Token: 0x04000A68 RID: 2664
		ToggleCharacters,
		// Token: 0x04000A69 RID: 2665
		IncreaseFocus,
		// Token: 0x04000A6A RID: 2666
		DecreaseFocus,
		// Token: 0x04000A6B RID: 2667
		IncreaseFocusStart,
		// Token: 0x04000A6C RID: 2668
		DecreaseFocusStart,
		// Token: 0x04000A6D RID: 2669
		IncreaseFocusEnd,
		// Token: 0x04000A6E RID: 2670
		DecreaseFocusEnd,
		// Token: 0x04000A6F RID: 2671
		Reset,
		// Token: 0x04000A70 RID: 2672
		AcceptPoll,
		// Token: 0x04000A71 RID: 2673
		DeclinePoll,
		// Token: 0x04000A72 RID: 2674
		TotalGameKeyCount
	}
}
