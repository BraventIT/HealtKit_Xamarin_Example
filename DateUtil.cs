using System;
using Foundation;

namespace HealthKitPoc
{
    public class DateUtil
    {
		public static DateTime NSDateToDateTime(NSDate date)
		{
			return ((DateTime)date).ToLocalTime();
		}

		public static NSDate DateTimeToNSDate(DateTime date)
		{
			if (date.Kind == DateTimeKind.Unspecified)
				date = DateTime.SpecifyKind(date, DateTimeKind.Local);
			return (NSDate)date;
		}
    }
}
