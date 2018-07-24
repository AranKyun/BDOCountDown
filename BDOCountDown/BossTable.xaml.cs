using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BDOCountDown
{
    /// <summary>
    /// BossTable.xaml 的交互逻辑
    /// </summary>
    public partial class BossTable : Window
    {
        MainWindow mainwin = (MainWindow)Application.Current.MainWindow;

        public BossTable()
        {
            InitializeComponent();
            AddListItem();
        }

        public void AddListItem()
        {
            foreach (Boss boss in mainwin.currentBoss)
            {
                listView.Items.Add(boss);
            }

            foreach (Boss boss in mainwin.bossList)
            {
                listView.Items.Add(boss);
            }
        }
    }
}
