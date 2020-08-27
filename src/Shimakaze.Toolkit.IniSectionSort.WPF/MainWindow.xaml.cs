using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Win32;
using Shimakaze.Struct.Ini;

namespace Shimakaze.Toolkit.IniSectionSort.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void FormatButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbInput.Text) || string.IsNullOrWhiteSpace(tbOutput.Text)) return;
            var btn = sender as Button;
            btn.IsEnabled = false;
            btn.Content = "正在处理";
            try
            {
                using var input = new FileStream(tbInput.Text, FileMode.Open, FileAccess.Read, FileShare.Read);
                var ini = await IniDocument.ParseAsync(input);
                using var output = new FileStream(tbOutput.Text, FileMode.Create, FileAccess.Write, FileShare.Read);
                using var sw = new StreamWriter(output);
                var options = new SortOptions
                {
                    Digit = string.IsNullOrWhiteSpace(tbDigit.Text) ? (int?)null : int.Parse(tbDigit.Text),
                    First = string.IsNullOrWhiteSpace(tbDigit.Text) ? 0 : int.Parse(tbFirst.Text),
                    Prefix = tbPrefix.Text,
                    Sort = cbSort.IsChecked,
                    SortTargetKey = tbSortTargetKey.Text,
                    SummaryKey = tbSummaryKey.Text,
                    KeyConstraint = tbKeyConstraint.Text,
                    ValueConstraint = tbValueConstraint.Text
                };

                var result = SortHelper.Sort(ini, options);

                if (!string.IsNullOrEmpty(tbTargetSectionName.Text)) await sw.WriteLineAsync($"[{tbTargetSectionName.Text}]");
                result.ToList().ForEach(i => sw.WriteLine(i));
                await sw.FlushAsync();
            }
            catch (Exception ex)
            {
                using var logfs = new FileStream("program.err.log", FileMode.Create, FileAccess.Write, FileShare.Read);
                using var sw = new StreamWriter(logfs);
                await sw.WriteLineAsync(ex.ToString());
                await sw.WriteLineAsync("全部堆栈跟踪");
                await sw.WriteLineAsync(ex.StackTrace);
                await sw.FlushAsync();
                MessageBox.Show(ex.ToString(), "发生异常:");
            }
            finally
            {
                btn.IsEnabled = true;
                btn.Content = "开始格式化";
            }
        }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "Ra2INI文件(*.ini)|*.ini";
            if (ofd.ShowDialog() ?? false)
            {
                tbInput.Text = ofd.FileName;
                tbFirst.Text = "0";
                //_tbSections.Text = "Animations;InfantryTypes;VehicleTypes;AircraftTypes;BuildingTypes;WeaponTypes;Warheads;SuperWeaponTypes;ParticleSystems;Particles;RadiationTypes;AttachEffectTypes";
                tbOutput.Text = ofd.FileName + ".out.ini";
            }
        }

        private void OnlyNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox tb)
            {
                if (!int.TryParse(tb.Text, out _))
                {
                    tb.Text = tb.Text[0..^1];
                    tb.CaretIndex = tb.Text.Length;
                }
            }
        }
        private void SaveFileButton_Click(object sender, RoutedEventArgs e)
        {
            var sfd = new SaveFileDialog
            {
                Filter = "Ra2INI文件(*.ini)|*.ini"
            };
            if (sfd.ShowDialog() ?? false)
            {
                tbOutput.Text = sfd.FileName;
            }
        }
    }
}
