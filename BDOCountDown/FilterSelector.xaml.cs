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
    /// FilterSelector.xaml 的交互逻辑
    /// </summary>
    public partial class FilterSelector : Window
    {
        MainWindow mainwin = (MainWindow)Application.Current.MainWindow;

        string[] nameList = { "クザカ", "カランダ", "ヌーベル", "クツム", "オピン", "ギュント", "ムラカ" };
        CheckBox[] checkBoxes;

        public FilterSelector()
        {
            InitializeComponent();

            InitializeSelector();
        }

        private void InitializeSelector()
        {
            checkBoxes = new CheckBox[nameList.Length];

            for (int i = 0; i < nameList.Length; i++)
            {
                checkBoxes[i] = new CheckBox();
                checkBoxes[i].Content = nameList[i];
                checkBoxes[i].Margin = new Thickness(10, 10, 0, 0);

                switch (i)
                {
                    case 0: if ((mainwin.filter & (int)MainWindow.FilterItem.Kzarka) == 0) checkBoxes[i].IsChecked = true; break;
                    case 1: if ((mainwin.filter & (int)MainWindow.FilterItem.Kranda) == 0) checkBoxes[i].IsChecked = true; break;
                    case 2: if ((mainwin.filter & (int)MainWindow.FilterItem.Nouver) == 0) checkBoxes[i].IsChecked = true; break;
                    case 3: if ((mainwin.filter & (int)MainWindow.FilterItem.Kutum) == 0) checkBoxes[i].IsChecked = true; break;
                    case 4: if ((mainwin.filter & (int)MainWindow.FilterItem.Offin) == 0) checkBoxes[i].IsChecked = true; break;
                    case 5: if ((mainwin.filter & (int)MainWindow.FilterItem.Quint) == 0) checkBoxes[i].IsChecked = true; break;
                    case 6: if ((mainwin.filter & (int)MainWindow.FilterItem.Muraka) == 0) checkBoxes[i].IsChecked = true; break;
                    default: checkBoxes[i].IsChecked = true; break;
                }

                stackPanel.Children.Insert(i, checkBoxes[i]);
            }

        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            int new_filter = 0;
            for (int i = 0; i < nameList.Length; i++)
            {
                if (checkBoxes[i].IsChecked == false)
                {
                    switch (i)
                    {
                        case 0: new_filter |= (int)MainWindow.FilterItem.Kzarka; break;
                        case 1: new_filter |= (int)MainWindow.FilterItem.Kranda; break;
                        case 2: new_filter |= (int)MainWindow.FilterItem.Nouver; break;
                        case 3: new_filter |= (int)MainWindow.FilterItem.Kutum; break;
                        case 4: new_filter |= (int)MainWindow.FilterItem.Offin; break;
                        case 5: new_filter |= (int)MainWindow.FilterItem.Quint; break;
                        case 6: new_filter |= (int)MainWindow.FilterItem.Muraka; break;
                        default: break;
                    }
                }
            }

            mainwin.UpdateFilter(new_filter);
            Close();
        }
    }
}
