using System;
using System.Threading.Tasks;

namespace TaleWorlds.ActivitySystem
{
	public class ActivityManager
	{
		public static IActivityService ActivityService { get; set; } = new TestActivityService();

		public static bool StartActivity(string activityId)
		{
			return ActivityManager.ActivityService.StartActivity(activityId);
		}

		public static bool EndActivity(string activityId, ActivityOutcome outcome)
		{
			return ActivityManager.ActivityService.EndActivity(activityId, outcome);
		}

		public static bool SetActivityAvailability(string activityId, bool isAvailable)
		{
			return ActivityManager.ActivityService.SetAvailability(activityId, isAvailable);
		}

		public static Task<Activity> GetActivity(string activityId)
		{
			return ActivityManager.ActivityService.GetActivity(activityId);
		}

		public static ActivityTransition GetActivityTransition(string activityId)
		{
			return ActivityManager.ActivityService.GetActivityTransition(activityId);
		}
	}
}
