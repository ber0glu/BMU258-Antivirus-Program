using System;
using System.Windows;
using Microsoft.Win32;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AntivirusProgram.Frontend
{
    public interface IScanService
    {
        Task<ScanResult> ScanFileAsync(string filePath, IProgress<(int progress, int processedItems, int totalItems)> progress);
        Task<ScanResult> ScanDirectoryAsync(string directoryPath, IProgress<(int progress, int processedItems, int totalItems)> progress);
    }

    public class MockScanService : IScanService
    {
        public async Task<ScanResult> ScanFileAsync(string filePath, IProgress<(int progress, int processedItems, int totalItems)> progress)
        {
            // Simulate API call delay
            await Task.Delay(1000);

            // Report progress
            progress.Report((0, 0, 1));
            await Task.Delay(500);
            progress.Report((50, 1, 1));
            await Task.Delay(500);
            progress.Report((100, 1, 1));

            // Return mock result
            return new ScanResult
            {
                Date = DateTime.Now,
                FilePath = filePath,
                Status = "Clean",
                ThreatsFound = 0
            };
        }

        public async Task<ScanResult> ScanDirectoryAsync(string directoryPath, IProgress<(int progress, int processedItems, int totalItems)> progress)
        {
            var files = Directory.GetFiles(directoryPath, "*.*", SearchOption.AllDirectories);
            var totalFiles = files.Length;
            var processedFiles = 0;

            // Simulate scanning each file
            foreach (var file in files)
            {
                processedFiles++;
                var progressPercentage = (int)((double)processedFiles / totalFiles * 100);
                progress.Report((progressPercentage, processedFiles, totalFiles));
                await Task.Delay(100); // Simulate API call delay
            }

            // Return mock result
            return new ScanResult
            {
                Date = DateTime.Now,
                FilePath = directoryPath,
                Status = "Clean",
                ThreatsFound = 0
            };
        }
    }

    public class ScanResult : INotifyPropertyChanged
    {
        private DateTime date;
        private string filePath;
        private string status;
        private int threatsFound;

        public DateTime Date
        {
            get => date;
            set
            {
                date = value;
                OnPropertyChanged();
            }
        }

        public string FilePath
        {
            get => filePath;
            set
            {
                filePath = value;
                OnPropertyChanged();
            }
        }

        public string Status
        {
            get => status;
            set
            {
                status = value;
                OnPropertyChanged();
            }
        }

        public int ThreatsFound
        {
            get => threatsFound;
            set
            {
                threatsFound = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IScanService _scanService;
        private ObservableCollection<ScanResult> scanHistory;
        private bool isScanning;

        public MainWindow()
        {
            InitializeComponent();
            _scanService = new MockScanService(); // In the future, this will be injected
            InitializeScanHistory();
        }

        private void InitializeScanHistory()
        {
            scanHistory = new ObservableCollection<ScanResult>();
            scanHistoryGrid.ItemsSource = scanHistory;
        }

        private void btnSelectFile_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Select a file to scan",
                Filter = "All files (*.*)|*.*",
                CheckFileExists = true,
                CheckPathExists = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                txtSelectedPath.Text = openFileDialog.FileName;
            }
        }

        private void btnSelectDirectory_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = "Select a directory to scan",
                ShowNewFolderButton = true
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtSelectedPath.Text = dialog.SelectedPath;
            }
        }

        private async void btnStartScan_Click(object sender, RoutedEventArgs e)
        {
            if (isScanning)
            {
                System.Windows.MessageBox.Show("A scan is already in progress.", "Scan in Progress", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(txtSelectedPath.Text))
            {
                System.Windows.MessageBox.Show("Please select a file or directory to scan first.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!File.Exists(txtSelectedPath.Text) && !Directory.Exists(txtSelectedPath.Text))
            {
                System.Windows.MessageBox.Show("The selected path does not exist.", "Invalid Path", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            isScanning = true;
            btnStartScan.IsEnabled = false;
            btnSelectFile.IsEnabled = false;
            btnSelectDirectory.IsEnabled = false;
            txtProgressStatus.Text = "Scanning...";
            scanProgressBar.Value = 0;

            try
            {
                var progress = new Progress<(int progress, int processedItems, int totalItems)>(UpdateProgress);
                var scanResult = File.Exists(txtSelectedPath.Text)
                    ? await _scanService.ScanFileAsync(txtSelectedPath.Text, progress)
                    : await _scanService.ScanDirectoryAsync(txtSelectedPath.Text, progress);

                await Dispatcher.InvokeAsync(() =>
                {
                    scanHistory.Insert(0, scanResult);
                    if (scanHistory.Count > 15)
                    {
                        scanHistory.RemoveAt(scanHistory.Count - 1);
                    }
                });

                txtProgressStatus.Text = "Scan completed!";
                System.Windows.MessageBox.Show($"Scan completed successfully!\nThreats found: {scanResult.ThreatsFound}", 
                    "Scan Complete", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                txtProgressStatus.Text = "Scan failed!";
                System.Windows.MessageBox.Show($"An error occurred during scanning: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                isScanning = false;
                btnStartScan.IsEnabled = true;
                btnSelectFile.IsEnabled = true;
                btnSelectDirectory.IsEnabled = true;
            }
        }

        private void UpdateProgress((int progress, int processedItems, int totalItems) progress)
        {
            scanProgressBar.Value = progress.progress;
            txtProgressStatus.Text = $"Scanning... {progress.progress}% ({progress.processedItems}/{progress.totalItems} items)";
        }
    }
}