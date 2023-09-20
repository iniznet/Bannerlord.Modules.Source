using System;
using TaleWorlds.DotNet;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.Engine
{
	// Token: 0x02000042 RID: 66
	internal class EngineScreenManager
	{
		// Token: 0x060005A6 RID: 1446 RVA: 0x00003556 File Offset: 0x00001756
		[EngineCallback]
		internal static void PreTick(float dt)
		{
			ScreenManager.EarlyUpdate(EngineApplicationInterface.IScreen.GetUsableAreaPercentages());
		}

		// Token: 0x060005A7 RID: 1447 RVA: 0x00003568 File Offset: 0x00001768
		[EngineCallback]
		public static void Tick(float dt)
		{
			bool mouseVisible = EngineApplicationInterface.IScreen.GetMouseVisible();
			ScreenManager.Tick(dt, mouseVisible);
		}

		// Token: 0x060005A8 RID: 1448 RVA: 0x00003587 File Offset: 0x00001787
		[EngineCallback]
		internal static void LateTick(float dt)
		{
			ScreenManager.LateTick(dt);
		}

		// Token: 0x060005A9 RID: 1449 RVA: 0x0000358F File Offset: 0x0000178F
		[EngineCallback]
		internal static void OnOnscreenKeyboardDone(string inputText)
		{
			ScreenManager.OnOnscreenKeyboardDone(inputText);
		}

		// Token: 0x060005AA RID: 1450 RVA: 0x00003597 File Offset: 0x00001797
		[EngineCallback]
		internal static void OnOnscreenKeyboardCanceled()
		{
			ScreenManager.OnOnscreenKeyboardCanceled();
		}

		// Token: 0x060005AB RID: 1451 RVA: 0x0000359E File Offset: 0x0000179E
		[EngineCallback]
		internal static void OnGameWindowFocusChange(bool focusGained)
		{
			ScreenManager.OnGameWindowFocusChange(focusGained);
		}

		// Token: 0x060005AC RID: 1452 RVA: 0x000035A6 File Offset: 0x000017A6
		[EngineCallback]
		internal static void Update()
		{
			ScreenManager.Update(EngineScreenManager._lastPressedKeys);
		}

		// Token: 0x060005AD RID: 1453 RVA: 0x000035B2 File Offset: 0x000017B2
		[EngineCallback]
		internal static void InitializeLastPressedKeys(NativeArray lastKeysPressed)
		{
			EngineScreenManager._lastPressedKeys = new NativeArrayEnumerator<int>(lastKeysPressed);
		}

		// Token: 0x060005AE RID: 1454 RVA: 0x000035BF File Offset: 0x000017BF
		internal static void Initialize()
		{
			ScreenManager.Initialize(new ScreenManagerEngineConnection());
		}

		// Token: 0x0400004E RID: 78
		private static NativeArrayEnumerator<int> _lastPressedKeys;
	}
}
