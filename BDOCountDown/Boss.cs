using System;
using System.Windows.Media;

namespace BDOCountDown
{
    class Boss
    {
        public enum BossType { Kzarka, Nouver, Kranda, Kutum, Offin, Quint, Muraka };
        public static readonly string[] BossName = { "Kzarka", "Nouver", "Kranda", "Kutum", "Offin", "Quint", "Muraka", "Gamos" };
        public static readonly string[] BossNameJapanese = { "クザカ", "ヌーベル", "カランダ", "クツム", "オピン", "ギュント", "ムラカ", "ガーモス" };
        public static readonly Brush[] BossColor = { Brushes.Red, Brushes.Orange, Brushes.SkyBlue, Brushes.MediumPurple, Brushes.Yellow, Brushes.Brown, Brushes.Brown, Brushes.OrangeRed };

        public BossType Type { get; private set; }

        public string Name { get; private set; }
        public Brush Color { get; private set; }
        public DateTime TimeAppear { get; private set; }

        public Boss(BossType bossType, DayOfWeek dayOfWeek, TimeSpan time)
        {
            Type = bossType;

            Name = BossNameJapanese[(int)bossType];
            Color = BossColor[(int)bossType];
            DateTime now = JapanTimeNow();

            DateTime startOfWeek = now - new TimeSpan((int)now.DayOfWeek, now.Hour, now.Minute, now.Second, now.Millisecond);
            TimeAppear = startOfWeek + new TimeSpan((int)dayOfWeek, 0, 0, 0) + time;

            if (TimeAppear < now)
            {
                TimeAppear = TimeAppear + new TimeSpan(7, 0, 0, 0);
            }
        }

        public static DateTime JapanTimeNow()
        {
            DateTime dt = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now, TimeZoneInfo.Local);
            return TimeZoneInfo.ConvertTimeFromUtc(dt, TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time"));
        }
    }
}
