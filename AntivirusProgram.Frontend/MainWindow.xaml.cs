using System;
using System.Windows;
using Microsoft.Win32;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Linq;

namespace AntivirusProgram.Frontend
{
    public class ScanResult
    {
        public DateTime Date { get; set; }
        public string FilePath { get; set; }
        public string Status { get; set; }
        public int ThreatsFound { get; set; }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<ScanResult> scanHistory;

        public MainWindow()
        {
            InitializeComponent();
            InitializeScanHistory();
        }

        private void InitializeScanHistory()
        {
            scanHistory = new ObservableCollection<ScanResult>();
            
            // Add sample data for the last 15 days
            var random = new Random();
            for (int i = 14; i >= 0; i--)
            {
                scanHistory.Add(new ScanResult
                {
                    Date = DateTime.Now.AddDays(-i),
                    FilePath = $"C:\\Sample\\File{i}.exe",
                    Status = random.Next(2) == 0 ? "Clean" : "Threats Found",
                    ThreatsFound = random.Next(5)
                });
            }

            scanHistoryGrid.ItemsSource = scanHistory;
        }

        private void btnSelectFile_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Select a file to scan",
                Filter = "All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                txtSelectedPath.Text = openFileDialog.FileName;
            }
        }

        private void btnSelectDirectory_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.Description = "Select a directory to scan";

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtSelectedPath.Text = dialog.SelectedPath;
            }
        }

        private async void btnStartScan_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtSelectedPath.Text))
            {
                System.Windows.MessageBox.Show("Please select a file or directory to scan first.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            btnStartScan.IsEnabled = false;
            btnSelectFile.IsEnabled = false;
            btnSelectDirectory.IsEnabled = false;
            txtProgressStatus.Text = "Scanning...";
            scanProgressBar.Value = 0;

            try
            {
                // Simulate scanning process
                await Task.Run(async () =>
                {
                    for (int i = 0; i <= 100; i += 10)
                    {
                        await Dispatcher.InvokeAsync(() =>
                        {
                            scanProgressBar.Value = i;
                            txtProgressStatus.Text = $"Scanning... {i}%";
                        });
                        await Task.Delay(500); // Simulate work
                    }
                });

                // Add new scan result
                var random = new Random();
                var newResult = new ScanResult
                {
                    Date = DateTime.Now,
                    FilePath = txtSelectedPath.Text,
                    Status = random.Next(2) == 0 ? "Clean" : "Threats Found",
                    ThreatsFound = random.Next(5)
                };

                await Dispatcher.InvokeAsync(() =>
                {
                    scanHistory.Insert(0, newResult);
                    if (scanHistory.Count > 15)
                    {
                        scanHistory.RemoveAt(scanHistory.Count - 1);
                    }
                });

                txtProgressStatus.Text = "Scan completed!";
                System.Windows.MessageBox.Show("Scan completed successfully!", "Scan Complete", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                txtProgressStatus.Text = "Scan failed!";
                System.Windows.MessageBox.Show($"An error occurred during scanning: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                btnStartScan.IsEnabled = true;
                btnSelectFile.IsEnabled = true;
                btnSelectDirectory.IsEnabled = true;
            }
        }
    }
}