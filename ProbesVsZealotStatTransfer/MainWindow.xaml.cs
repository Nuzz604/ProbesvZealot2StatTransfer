using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace NuzzProbesvZealot2Restorer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<BankFile> AllBanks { get; set; }
        private string CurrentUser { get; set; }
        private string SourcePath { get; set; }
        private string DestPath { get; set; }
        const string BankName = "ProbesvZealot2";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void window_Initialized(object sender, EventArgs e)
        {
            try
            {
                LoadInitialInfo();
                txtStatus.Text = string.Format("Successfully loaded bankfiles for {0}.  Ready to transfer stats.", CurrentUser);
            }
            catch (Exception ex)
            {
                txtStatus.Text = "Error: " + ex.Message;
            }
        }

        private void LoadInitialInfo(string accountName = null)
        {
            AllBanks = GetBanks();

            //Load source for most recent bank
            var sources = AllBanks.Where(x => x.MapVersion == MapVersion.PvZ2KorvinOfficial);
            if (accountName != null)
                sources = sources.Where(x => x.AccountName == accountName);

            var source = sources.OrderByDescending(x => x.ModDate)
                .FirstOrDefault();

            if (source == null)
                throw new InvalidOperationException("Could not find source bankfile.");
            txtSource.Text = SourcePath = source.Path;
            
            userNamesComboBox.ItemsSource = AllBanks.Select(x => x.AccountName).Distinct().ToList();

            userNamesComboBox.SelectedValue = CurrentUser = source.AccountName;
            txtAccountHandle.Text = source.AccountHandle;
            
            //Get destination bank
            var dest = AllBanks.Where(x => x.MapVersion == MapVersion.PvZ2CoastOfficial)
                .Where(x => x.AccountHandle == source.AccountHandle)
                .SingleOrDefault();
            if (dest == null)
                throw new InvalidOperationException("Could not find destination bankfile.");
            txtDest.Text = DestPath = dest.Path;
        }

        List<BankFile> GetBanks()
        {
            var banks = Directory.GetFiles(BankFile.PathBase, BankName + ".SC2Bank", SearchOption.AllDirectories);
            
            return banks
                .Select(path => new BankFile(path))
                .ToList();
        }

        private void userNamesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                LoadInitialInfo((string)e.AddedItems[0]);
                txtStatus.Text = string.Format("Successfully loaded bankfiles for {0}.  Ready to transfer stats.", CurrentUser);
            }
            catch (Exception ex)
            {
                txtStatus.Text = "Error: " + ex.Message;
            }
        }

        private void btnTransfer_Click(object sender, RoutedEventArgs e)
        {
            txtStatus.Text = "Working...";
            try
            {
                TransferBank(SourcePath, DestPath);
                txtStatus.Text = "Successfully transferred stats!";
            }
            catch (Exception ex)
            {
                txtStatus.Text = "Error: " + ex.Message;
            }
        }

        private void TransferBank(string SourcePath, string DestPath)
        {
            //Get bank info
            var src = new BankFile(SourcePath);
            var dest = new BankFile(DestPath);
            if (src.AccountHandle != dest.AccountHandle || src.AccountName != dest.AccountName)
            {
                throw new InvalidOperationException("Account mismatch on bank files.");
            }
            
            //Backup bank file
            string backupFileTarget = GetBackupFileTarget(DestPath);
            File.Copy(DestPath, backupFileTarget);

            //Sign bank
            string finalBank = BankSigner.Program.Sign(dest.MapHandle, src.AccountHandle, BankName, src.RawFile);
            
            //Write bank file
            File.WriteAllText(DestPath, finalBank);
        }

        private string GetBackupFileTarget(string DestPath)
        {
            int i = 0;
            string currentPath = "";
            do
            {
                i++;
                currentPath = DestPath + " - Nuzz Backup " + i;
            }
            while (File.Exists(currentPath));
            return currentPath;
        }
    }
}
