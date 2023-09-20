using System;
using System.Runtime.CompilerServices;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000002 RID: 2
	internal static class AgentHelper
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002048 File Offset: 0x00000248
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static Vec3 GetAgentPosition(UIntPtr agentPositionPointer)
		{
			Vec3* ptr = (Vec3*)agentPositionPointer.ToPointer();
			return *ptr;
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002064 File Offset: 0x00000264
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static void SetAgentPosition(UIntPtr agentPositionPointer, ref Vec3 newPos)
		{
			Debug.FailedAssert("Do not use this!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Helpers\\Helper.cs", "SetAgentPosition", 20);
			Vec3* ptr = (Vec3*)agentPositionPointer.ToPointer();
			*ptr = newPos;
		}

		// Token: 0x06000003 RID: 3 RVA: 0x0000209C File Offset: 0x0000029C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static int GetAgentIndex(UIntPtr indexPtr)
		{
			int* ptr = (int*)indexPtr.ToPointer();
			return *ptr;
		}

		// Token: 0x06000004 RID: 4 RVA: 0x000020B4 File Offset: 0x000002B4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static AgentFlag GetAgentFlags(UIntPtr flagsPtr)
		{
			AgentFlag* ptr = (AgentFlag*)flagsPtr.ToPointer();
			return *ptr;
		}

		// Token: 0x06000005 RID: 5 RVA: 0x000020CC File Offset: 0x000002CC
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static AgentState GetAgentState(UIntPtr statePtr)
		{
			AgentState* ptr = (AgentState*)statePtr.ToPointer();
			return *ptr;
		}
	}
}
