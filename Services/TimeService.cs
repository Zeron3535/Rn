using System;

namespace TinaKuafor.Services
{
    public static class TimeService
    {
        private static readonly TimeZoneInfo TurkeyTimeZone;

        static TimeService()
        {
            try
            {
                // Try to get Turkey time zone by its standard ID
                TurkeyTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");
            }
            catch
            {
                try
                {
                    // Fallback for Linux/Unix systems
                    TurkeyTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Istanbul");
                }
                catch
                {
                    // Final fallback: create a custom time zone (UTC+3)
                    TurkeyTimeZone = TimeZoneInfo.CreateCustomTimeZone(
                        "Turkey Standard Time",
                        new TimeSpan(3, 0, 0),
                        "Turkey Standard Time",
                        "Turkey Standard Time");
                }
            }
        }

        /// <summary>
        /// Gets the current date and time in Turkey's time zone.
        /// </summary>
        public static DateTime Now => TimeZoneInfo.ConvertTime(DateTime.UtcNow, TurkeyTimeZone);

        /// <summary>
        /// Gets the current date in Turkey's time zone.
        /// </summary>
        public static DateTime Today => Now.Date;
    }
}