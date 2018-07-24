using System;
using System.Collections;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.IO;

namespace BDOCountDown
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        double left_scale = 0.68;
        double top_scale = 0.1;

        public ArrayList bossList = new ArrayList();
        public ArrayList currentBoss = new ArrayList();
        DateTime NextTime;

        private static System.Windows.Threading.DispatcherTimer Timer = new System.Windows.Threading.DispatcherTimer();

        System.Windows.Forms.NotifyIcon notifyIcon = null;

        BossTable bossTable = null;
        FilterSelector selector = null;

        public enum FilterItem { Kzarka = 0b1, Kranda = 0b10, Nouver = 0b100, Kutum = 0b1000, Offin = 0b10000, Muraka = 0b100000, Quint = 0b1000000 };
        public int filter = 0;

        private const uint WS_EX_LAYERED = 0x80000;
        private const int WS_EX_TRANSPARENT = 0x20;
        private const int GWL_STYLE = (-16);
        private const int GWL_EXSTYLE = (-20);
        private const int LWA_ALPHA = 0;
        [DllImport("user32", EntryPoint = "SetWindowLong")]
        private static extern uint SetWindowLong(IntPtr hwnd, int nIndex, uint dwNewLong);
        [DllImport("user32", EntryPoint = "GetWindowLong")]
        private static extern uint GetWindowLong(IntPtr hwnd, int nIndex);
        [DllImport("user32", EntryPoint = "SetLayeredWindowAttributes")]
        private static extern int SetLayeredWindowAttributes(IntPtr hwnd, int crKey, int bAlpha, int dwFlags);


        public MainWindow()
        {
            InitializeComponent();

            Left = SystemParameters.PrimaryScreenWidth * left_scale;
            Top = SystemParameters.PrimaryScreenHeight * top_scale;

            InitializeIcon();

            ReadSchedule();

            UpdateTime(null, null);

            Timer.Tick += new EventHandler(UpdateTime);
            Timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            Timer.Start();
        }

        bool isLocked = false;
        private void SetPenetrate()
        {
            if (!isLocked)
            {
                IntPtr hwnd = new WindowInteropHelper(this).Handle;
                GetWindowLong(hwnd, GWL_EXSTYLE);
                SetWindowLong(hwnd, GWL_EXSTYLE, WS_EX_TRANSPARENT | WS_EX_LAYERED);
            }
            else
            {
                IntPtr hwnd = new WindowInteropHelper(this).Handle;
                GetWindowLong(hwnd, GWL_EXSTYLE);
                SetWindowLong(hwnd, GWL_EXSTYLE, 0);
            }
            isLocked = !isLocked;
        }

        private void InitializeIcon()
        {
            notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.Text = "黑色沙漠Boss计时";

            System.IO.Stream stream = Application.GetResourceStream(new Uri("/icon.ico", UriKind.Relative)).Stream;
            this.notifyIcon.Icon = new System.Drawing.Icon(stream);
            notifyIcon.Visible = true;

            notifyIcon.MouseDoubleClick += OnNotifyIconDoubleClick;

            System.Windows.Forms.MenuItem schedule = new System.Windows.Forms.MenuItem("时间表");
            schedule.Click += Schedule_Click;

            System.Windows.Forms.MenuItem selector = new System.Windows.Forms.MenuItem("筛选器");
            selector.Click += Selector_Click;

            System.Windows.Forms.MenuItem lockWindow = new System.Windows.Forms.MenuItem("锁定");
            lockWindow.Click += LockWindow_Click;

            System.Windows.Forms.MenuItem close = new System.Windows.Forms.MenuItem("关闭");
            close.Click += Close_Click;

            System.Windows.Forms.MenuItem[] childen = new System.Windows.Forms.MenuItem[] { schedule, selector, lockWindow, close };
            notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu(childen);
        }

        private void LockWindow_Click(object sender, EventArgs e)
        {
            SetPenetrate();
        }

        private void Close_Click(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Selector_Click(object sender, EventArgs e)
        {
            selector = new FilterSelector();
            selector.Show();
        }

        private void Schedule_Click(object sender, EventArgs e)
        {
            bossTable = new BossTable();
            bossTable.Show();
        }

        private void OnNotifyIconDoubleClick(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Normal)
                WindowState = WindowState.Minimized;
            else
                WindowState = WindowState.Normal;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        public void UpdateFilter(int new_filter)
        {
            filter = new_filter;

            FileStream configStream = new FileStream("BDOBT.config", FileMode.Create, FileAccess.ReadWrite);
            StreamWriter configWriter = new StreamWriter(configStream);
            configWriter.WriteLine(new_filter.ToString());
            configWriter.Close();
            configStream.Close();

            Timer.Stop();

            bossList.Clear();
            currentBoss.Clear();

            ReadSchedule();
            UpdateTime(null, null);

            Timer.Start();
        }


        private void UpdateTime(object sender, EventArgs e)
        {
            DateTime now = Boss.JapanTimeNow();

            if (sender == null || NextTime < now)
            {
                bosslist.Inlines.Clear();
                currentBoss.Clear();

                Boss boss = (Boss)bossList[0];
                currentBoss.Add(boss);
                bossList.RemoveAt(0);
                NextTime = boss.TimeAppear;

                Run bossrun = new Run(boss.Name + ' ');
                bossrun.Foreground = boss.Color;
                bosslist.Inlines.Add(bossrun);

                while (bossList[0] != null && ((Boss)bossList[0]).TimeAppear.ToString("yyyy-MM-dd HH:mm:ss") == NextTime.ToString("yyyy-MM-dd HH:mm:ss"))
                {
                    Boss nextboss = (Boss)bossList[0];
                    currentBoss.Add(boss);
                    bossList.RemoveAt(0);

                    Run nextbossrun = new Run(nextboss.Name + ' ');
                    nextbossrun.Foreground = nextboss.Color;
                    bosslist.Inlines.Add(nextbossrun);
                }
            }

            TimeSpan span = NextTime - now;
            if (span >= new TimeSpan(1, 0, 0, 0))
                Time.Text = span.ToString(@"d\天\ hh\:mm\:ss");
            else
                Time.Text = span.ToString(@"hh\:mm\:ss");

        }


        private void ReadSchedule()
        {
            FileStream configStream = new FileStream("BDOBT.config", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            StreamReader configReader = new StreamReader(configStream);

            string filterString = configReader.ReadLine();
            if (filterString == null)
            {
                filter = 0;
                StreamWriter configWriter = new StreamWriter(configStream);
                configWriter.WriteLine(filter);
                configWriter.Close();
            }
            else
            {
                filter = int.Parse(filterString);
            }

            System.IO.Stream stream = Application.GetResourceStream(new Uri("/BossSchedule-180725.txt", UriKind.Relative)).Stream;
            System.IO.StreamReader reader = new System.IO.StreamReader(stream);

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line == "" || line == "--")
                    continue;

                string[] values = line.Split(' ');

                for (int i = 0; i < Boss.BossName.Length; i++)
                {
                    if (Boss.BossName[i] == values[0])
                    {
                        if ((filter & (1 << i)) == 0)
                        {
                            Boss boss = new Boss((Boss.BossType)i, StringToDayOfWeek(values[1]), TimeSpan.Parse(values[2]));

                            if (bossList.Count == 0)
                            {
                                bossList.Insert(0, boss);
                            }
                            else
                            {
                                for (int j = 0; j < bossList.Count; j++)
                                {
                                    Boss iter = (Boss)bossList[j];
                                    if (iter.TimeAppear > boss.TimeAppear)
                                    {
                                        bossList.Insert(j, boss);
                                        break;
                                    }
                                    else if (j == bossList.Count - 1)
                                    {
                                        bossList.Insert(j + 1, boss);
                                        break;
                                    }
                                }
                            }
                        }
                        break;

                    }
                }
             
            }
        }

        private static DayOfWeek StringToDayOfWeek(string str)
        {
            string[] day = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };

            for (int i = 0; i < day.Length; i++)
            {
                if (day[i] == str)
                    return (DayOfWeek)i;
            }

            return DayOfWeek.Sunday;
        }

    }
}
