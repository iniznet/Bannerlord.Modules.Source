using System;
using System.Runtime.InteropServices;
using TaleWorlds.Library;

namespace TaleWorlds.DotNet
{
	// Token: 0x02000007 RID: 7
	public static class Controller
	{
		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600000F RID: 15 RVA: 0x00002248 File Offset: 0x00000448
		// (set) Token: 0x06000010 RID: 16 RVA: 0x0000224F File Offset: 0x0000044F
		private static Runtime RuntimeLibrary { get; set; } = Runtime.DotNet;

		// Token: 0x06000011 RID: 17 RVA: 0x00002257 File Offset: 0x00000457
		[MonoPInvokeCallback(typeof(Controller.OverrideManagedDllFolderDelegate))]
		public static void OverrideManagedDllFolder(IntPtr overridenFolderAsPointer)
		{
			ManagedDllFolder.OverrideManagedDllFolder(Marshal.PtrToStringAnsi(overridenFolderAsPointer));
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00002264 File Offset: 0x00000464
		[MonoPInvokeCallback(typeof(Controller.CreateApplicationDomainMethodDelegate))]
		public static void LoadOnCurrentApplicationDomain(IntPtr gameDllNameAsPointer, IntPtr gameTypeNameAsPointer, int currentEngineAsInteger, int currentPlatformAsInteger)
		{
			ApplicationPlatform.Initialize((EngineType)currentEngineAsInteger, (Platform)currentPlatformAsInteger, Controller.RuntimeLibrary);
			string text = Marshal.PtrToStringAnsi(gameDllNameAsPointer);
			string text2 = Marshal.PtrToStringAnsi(gameTypeNameAsPointer);
			Debug.Print("Appending private path to current application domain.", 0, Debug.DebugColor.White, 17592186044416UL);
			AppDomain.CurrentDomain.AppendPrivatePath(ManagedDllFolder.Name);
			Debug.Print("Creating GameApplicationDomainController on current application domain.", 0, Debug.DebugColor.White, 17592186044416UL);
			GameApplicationDomainController gameApplicationDomainController = new GameApplicationDomainController(false);
			if (gameApplicationDomainController == null)
			{
				Console.WriteLine("GameApplicationDomainController is NULL!");
				Console.WriteLine("Press a key to continue...");
				Console.ReadKey();
			}
			if (Controller._hostedByNative)
			{
				Debug.Print("Initializing GameApplicationDomainController as Hosted by Native(Mono or hosted .NET Core).", 0, Debug.DebugColor.White, 17592186044416UL);
				gameApplicationDomainController.LoadAsHostedByNative(Controller._passManagedInitializeMethodPointer, Controller._passManagedCallbackMethodPointer, text, text2, (Platform)currentPlatformAsInteger);
				return;
			}
			Debug.Print("Initializing GameApplicationDomainController as Dot Net.", 0, Debug.DebugColor.White, 17592186044416UL);
			gameApplicationDomainController.Load(Controller._passManagedInitializeMethod, Controller._passManagedCallbackMethod, text, text2, (Platform)currentPlatformAsInteger);
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002348 File Offset: 0x00000548
		private static void SetEngineMethodsAsHostedByNative(IntPtr passControllerMethods, IntPtr passManagedInitializeMethod, IntPtr passManagedCallbackMethod)
		{
			Debug.Print("Setting engine methods at Controller::SetEngineMethodsAsHostedByNative", 0, Debug.DebugColor.White, 17592186044416UL);
			Debug.Print("Beginning...", 0, Debug.DebugColor.White, 17592186044416UL);
			Controller._hostedByNative = true;
			Controller._passControllerMethods = (OneMethodPasserDelegate)Marshal.GetDelegateForFunctionPointer(passControllerMethods, typeof(OneMethodPasserDelegate));
			Controller._passManagedInitializeMethodPointer = passManagedInitializeMethod;
			Controller._passManagedCallbackMethodPointer = passManagedCallbackMethod;
			Debug.Print("Starting controller...", 0, Debug.DebugColor.White, 17592186044416UL);
			Controller.Start();
			Debug.Print("Setting engine methods at Controller::SetEngineMethodsAsHostedByNative - Done", 0, Debug.DebugColor.White, 17592186044416UL);
		}

		// Token: 0x06000014 RID: 20 RVA: 0x000023DE File Offset: 0x000005DE
		public static void SetEngineMethodsAsMono(IntPtr passControllerMethods, IntPtr passManagedInitializeMethod, IntPtr passManagedCallbackMethod)
		{
			Debug.Print("Setting engine methods at Controller::SetEngineMethodsAsMono", 0, Debug.DebugColor.White, 17592186044416UL);
			Controller.RuntimeLibrary = Runtime.Mono;
			Controller.SetEngineMethodsAsHostedByNative(passControllerMethods, passManagedInitializeMethod, passManagedCallbackMethod);
			Debug.Print("Setting engine methods at Controller::SetEngineMethodsAsMono - Done", 0, Debug.DebugColor.White, 17592186044416UL);
		}

		// Token: 0x06000015 RID: 21 RVA: 0x0000241A File Offset: 0x0000061A
		public static void SetEngineMethodsAsHostedDotNetCore(IntPtr passControllerMethods, IntPtr passManagedInitializeMethod, IntPtr passManagedCallbackMethod)
		{
			Debug.Print("Setting engine methods at Controller::SetEngineMethodsAsHostedDotNetCore", 0, Debug.DebugColor.White, 17592186044416UL);
			Controller.RuntimeLibrary = Runtime.DotNetCore;
			Controller.SetEngineMethodsAsHostedByNative(passControllerMethods, passManagedInitializeMethod, passManagedCallbackMethod);
			Debug.Print("Setting engine methods at Controller::SetEngineMethodsAsHostedDotNetCore - Done", 0, Debug.DebugColor.White, 17592186044416UL);
		}

		// Token: 0x06000016 RID: 22 RVA: 0x00002458 File Offset: 0x00000658
		public static void SetEngineMethodsAsDotNet(Delegate passControllerMethods, Delegate passManagedInitializeMethod, Delegate passManagedCallbackMethod)
		{
			Debug.Print("Setting engine methods at Controller::SetEngineMethodsAsDotNet", 0, Debug.DebugColor.White, 17592186044416UL);
			if (RuntimeInformation.FrameworkDescription.StartsWith(".NET Framework"))
			{
				Controller.RuntimeLibrary = Runtime.DotNet;
			}
			else if (RuntimeInformation.FrameworkDescription.StartsWith(".NET Core"))
			{
				Controller.RuntimeLibrary = Runtime.DotNetCore;
			}
			Controller._passControllerMethods = passControllerMethods;
			Controller._passManagedInitializeMethod = passManagedInitializeMethod;
			Controller._passManagedCallbackMethod = passManagedCallbackMethod;
			if (Controller._passControllerMethods == null)
			{
				Debug.Print("_passControllerMethods is null", 0, Debug.DebugColor.White, 17592186044416UL);
			}
			if (Controller._passManagedInitializeMethod == null)
			{
				Debug.Print("_passManagedInitializeMethod is null", 0, Debug.DebugColor.White, 17592186044416UL);
			}
			if (Controller._passManagedCallbackMethod == null)
			{
				Debug.Print("_passManagedCallbackMethod is null", 0, Debug.DebugColor.White, 17592186044416UL);
			}
			Controller.Start();
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00002519 File Offset: 0x00000719
		private static void Start()
		{
			Controller._loadOnCurrentApplicationDomainMethod = new Controller.CreateApplicationDomainMethodDelegate(Controller.LoadOnCurrentApplicationDomain);
			Controller.PassControllerMethods(Controller._loadOnCurrentApplicationDomainMethod);
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00002536 File Offset: 0x00000736
		private static void PassControllerMethods(Delegate loadOnCurrentApplicationDomainMethod)
		{
			if (Controller._passControllerMethods != null)
			{
				Controller._passControllerMethods.DynamicInvoke(new object[] { loadOnCurrentApplicationDomainMethod });
				return;
			}
			Debug.Print("Could not find _passControllerMethods", 0, Debug.DebugColor.White, 17592186044416UL);
		}

		// Token: 0x04000009 RID: 9
		private static bool _hostedByNative;

		// Token: 0x0400000A RID: 10
		private static Delegate _passControllerMethods;

		// Token: 0x0400000B RID: 11
		private static Delegate _passManagedInitializeMethod;

		// Token: 0x0400000C RID: 12
		private static Delegate _passManagedCallbackMethod;

		// Token: 0x0400000D RID: 13
		private static IntPtr _passManagedInitializeMethodPointer;

		// Token: 0x0400000E RID: 14
		private static IntPtr _passManagedCallbackMethodPointer;

		// Token: 0x0400000F RID: 15
		private static Controller.CreateApplicationDomainMethodDelegate _loadOnCurrentApplicationDomainMethod;

		// Token: 0x02000030 RID: 48
		// (Invoke) Token: 0x06000120 RID: 288
		[MonoNativeFunctionWrapper]
		private delegate void ControllerMethodDelegate();

		// Token: 0x02000031 RID: 49
		// (Invoke) Token: 0x06000124 RID: 292
		[MonoNativeFunctionWrapper]
		private delegate void CreateApplicationDomainMethodDelegate(IntPtr gameDllNameAsPointer, IntPtr gameTypeNameAsPointer, int currentEngineAsInteger, int currentPlatformAsInteger);

		// Token: 0x02000032 RID: 50
		// (Invoke) Token: 0x06000128 RID: 296
		[MonoNativeFunctionWrapper]
		private delegate void OverrideManagedDllFolderDelegate(IntPtr overridenFolderAsPointer);
	}
}
