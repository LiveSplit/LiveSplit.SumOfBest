using System;

namespace LiveSplit.TimeFormatters
{
    class RegularSumOfBestTimeFormatter : ITimeFormatter
    {
        public TimeAccuracy Accuracy { get; set; }
        
        public string Format(TimeSpan? time)
        {
            var formatter = new RegularTimeFormatter(Accuracy);
            if (time == null)
                return "-";
            else
                return formatter.Format(time);
        }
    }
}
