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
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using SaveFileDialog = System.Windows.Forms.SaveFileDialog;

namespace IniFormatter
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "Ra2INI文件(*.ini)|*.ini";
            if (ofd.ShowDialog()== System.Windows.Forms.DialogResult.OK)
            {
                _tbIniPath.Text = ofd.FileName;
                _tbFirst.Text = "0";
                _tbSections.Text = "Animations;InfantryTypes;VehicleTypes;AircraftTypes;BuildingTypes;WeaponTypes;Warheads;SuperWeaponTypes;ParticleSystems;Particles;RadiationTypes;AttachEffectTypes";
                _tbIniOPath.Text = ofd.FileName + ".out.ini";
            }
        }

        private async void FormatButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_tbIniPath.Text))
            {
                return;
            }
            List<string> sections = _tbSections.Text.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            for (int i = 0; i < sections.Count; i++)
            {
                sections[i] = sections[i].Trim();
            }

            _tbReturn.Text = await Sort.SortStart(_tbIniPath.Text, _tbIniOPath.Text, sections.ToArray(),
                string.IsNullOrWhiteSpace(_tbFirst.Text) ? 0 : Convert.ToUInt32(_tbFirst.Text),
                string.IsNullOrWhiteSpace(_tbDigit.Text) ? null : (uint?)Convert.ToUInt32(_tbDigit.Text),
                string.IsNullOrWhiteSpace(_tbPrefix.Text) ? null : (uint?)Convert.ToUInt32(_tbPrefix.Text));
        }
        private void OnlyNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(((TextBox)sender).Text, "^\\d*\\.?\\d*$") && ((TextBox)sender).Text != "") ((TextBox)sender).Text = ((TextBox)sender).Text.Substring(0, ((TextBox)sender).Text.Length - 1);
        }
        private void SaveFileButton_Click(object sender, RoutedEventArgs e)
        {
            var sfd = new SaveFileDialog();
            sfd.Filter = "Ra2INI文件(*.ini)|*.ini";
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _tbIniOPath.Text = sfd.FileName;
            }
        }
    }
}
