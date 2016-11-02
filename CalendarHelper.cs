namespace Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class CalendarDate
    {
        public DateTime Date { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public int WeekNum { get; set; }
    }

    public class CalendarHelper
    {
        public static DateTime GetFirstDayOfWeek(DateTime dayInWeek, CultureInfo cultureInfo)
        {
            DayOfWeek firstDay = cultureInfo.DateTimeFormat.FirstDayOfWeek;
            DateTime firstDayInWeek = dayInWeek.Date;
            while (firstDayInWeek.DayOfWeek != firstDay)
            { 
                firstDayInWeek = firstDayInWeek.AddDays(-1);
            }

            return firstDayInWeek;
        }

        public static DateTime GetFirstDayOfWeek(DateTime dayInWeek)
        {
            CultureInfo defaultCultureInfo = CultureInfo.CurrentCulture;
            return GetFirstDayOfWeek(dayInWeek, defaultCultureInfo);
        }

        public static int GetIso8601WeekOfYear(DateTime date)
        {
            GregorianCalendar gcal = new GregorianCalendar();

            // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll
            // be the same week# as whatever Thursday, Friday or Saturday are,
            // and we always get those right
            DayOfWeek day = gcal.GetDayOfWeek(date);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                date = date.AddDays(3);
            }

            // Return the week of our adjusted day
            return gcal.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        public static int GetIso8601WeekOfYear(GregorianCalendar gcal, DateTime date)
        {
            // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll
            // be the same week# as whatever Thursday, Friday or Saturday are,
            // and we always get those right
            DayOfWeek day = gcal.GetDayOfWeek(date);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                date = date.AddDays(3);
            }

            // Return the week of our adjusted day
            return gcal.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        public static List<CalendarDate> BuildCalendar(int year, int month)
        {
            var result = new List<CalendarDate>(6 * 7);
            var gcal = new GregorianCalendar();
            var firstDay = new DateTime(year, month, 1);
            var week = gcal.GetWeekOfYear(firstDay, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            var day = firstDay.DayOfWeek;
            var currentDate = GetFirstDayOfWeek(firstDay);
            if (day == DayOfWeek.Monday)
            {
                currentDate = GetFirstDayOfWeek(firstDay.AddDays(-1));
            }

            // Calendar has 6 rows and 7 columns.
            for (int row = 0; row < 6; row++)
            {
                week = GetIso8601WeekOfYear(gcal, currentDate);

                for (int column = 0; column < 7; column++)
                {
                    result.Add(new CalendarDate()
                    {
                        Date = currentDate,
                        Row = row,
                        Column = column,
                        WeekNum = week

                    });

                    currentDate = currentDate.AddDays(1);
                }
            }

            return result;
        }
    }

}
