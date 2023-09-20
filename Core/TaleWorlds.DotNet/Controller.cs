using System;
using System.Runtime.InteropServices;
using TaleWorlds.Library;

namespace TaleWorlds.DotNet
{
	public static class Controller
	{
		private static Runtime RuntimeLibrary { get; set; } = Runtime.DotNet;

		[MonoPInvokeCallback(typeof(Controller.OverrideManagedDllFolderDelegate))]
		public static void OverrideManagedDllFolder(IntPtr overridenFolderAsPointer)
		{
			ManagedDllFolder.OverrideManagedDllFolder(Marshal.PtrToStringAnsi(overridenFolderAsPointer));
		}

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

		public static void SetEngineMethodsAsMono(IntPtr passControllerMethods, IntPtr passManagedInitializeMethod, IntPtr passManagedCallbackMethod)
		{
			Debug.Print("Setting engine methods at Controller::SetEngineMethodsAsMono", 0, Debug.DebugColor.White, 17592186044416UL);
			Controller.RuntimeLibrary = Runtime.Mono;
			Controller.SetEngineMethodsAsHostedByNative(passControllerMethods, passManagedInitializeMethod, passManagedCallbackMethod);
			Debug.Print("Setting engine methods at Controller::SetEngineMethodsAsMono - Done", 0, Debug.DebugColor.White, 17592186044416UL);
		}

		public static void SetEngineMethodsAsHostedDotNetCore(IntPtr passControllerMethods, IntPtr passManagedInitializeMethod, IntPtr passManagedCallbackMethod)
		{
			Debug.Print("Setting engine methods at Controller::SetEngineMethodsAsHostedDotNetCore", 0, Debug.DebugColor.White, 17592186044416UL);
			Controller.RuntimeLibrary = Runtime.DotNetCore;
			Controller.SetEngineMethodsAsHostedByNative(passControllerMethods, passManagedInitializeMethod, passManagedCallbackMethod);
			Debug.Print("Setting engine methods at Controller::SetEngineMethodsAsHostedDotNetCore - Done", 0, Debug.DebugColor.White, 17592186044416UL);
		}

		public static void SetEngineMethodsAsDotNet(Delegate passControllerMethods, Delegate passManagedInitializeMethod, Delegate passManagedCallbackMethod)
		{
			Debug.Print("Setting engine methods at Controller::SetEngineMethodsAsDotNet", 0, Debug.DebugColor.White, 17592186044416UL);
			if (RuntimeInformation.FrameworkDescription.StartsWith(".NET Framework"))
			{
				Controller.RuntimeLibrary = Runtime.DotNet;
			}
			else if (RuntimeInformation.FrameworkDescription.StartsWith(".NET Core") || RuntimeInformation.FrameworkDescription.StartsWith(".NET 6"))
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

		private static void Start()
		{
			Controller._loadOnCurrentApplicationDomainMethod = new Controller.CreateApplicationDomainMethodDelegate(Controller.LoadOnCurrentApplicationDomain);
			Controller.PassControllerMethods(Controller._loadOnCurrentApplicationDomainMethod);
		}

		private static void PassControllerMethods(Delegate loadOnCurrentApplicationDomainMethod)
		{
			if (Controller._passControllerMethods != null)
			{
				Controller._passControllerMethods.DynamicInvoke(new object[] { loadOnCurrentApplicationDomainMethod });
				return;
			}
			Debug.Print("Could not find _passControllerMethods", 0, Debug.DebugColor.White, 17592186044416UL);
		}

		private static bool _hostedByNative;

		private static Delegate _passControllerMethods;

		private static Delegate _passManagedInitializeMethod;

		private static Delegate _passManagedCallbackMethod;

		private static IntPtr _passManagedInitializeMethodPointer;

		private static IntPtr _passManagedCallbackMethodPointer;

		private static Controller.CreateApplicationDomainMethodDelegate _loadOnCurrentApplicationDomainMethod;

		[MonoNativeFunctionWrapper]
		private delegate void ControllerMethodDelegate();

		[MonoNativeFunctionWrapper]
		private delegate void CreateApplicationDomainMethodDelegate(IntPtr gameDllNameAsPointer, IntPtr gameTypeNameAsPointer, int currentEngineAsInteger, int currentPlatformAsInteger);

		[MonoNativeFunctionWrapper]
		private delegate void OverrideManagedDllFolderDelegate(IntPtr overridenFolderAsPointer);
	}
}
