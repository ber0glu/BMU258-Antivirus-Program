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
using System.Text.Json;
using System.Threading;

using AntivirusProgram.Core;
using MessageBox = System.Windows.Forms.MessageBox;

namespace AntivirusProgram.Frontend
{
    public interface IScanService
    {
        Task<ScanResult> ScanFileAsync(string filePath, IProgress<(int progress, int processedItems, int totalItems)> progress, CancellationToken cancellationToken);
        Task<ScanResult> ScanDirectoryAsync(string directoryPath, IProgress<(int progress, int processedItems, int totalItems)> progress, CancellationToken cancellationToken);
    }

    public class MockScanService : IScanService
    {
        public async Task<ScanResult> ScanFileAsync(string filePath, IProgress<(int progress, int processedItems, int totalItems)> progress, CancellationToken cancellationToken)
        {
            await Task.Delay(1000, cancellationToken);
            progress.Report((0, 0, 1));
            await Task.Delay(500, cancellationToken);
            progress.Report((50, 1, 1));
            await Task.Delay(500, cancellationToken);
            progress.Report((100, 1, 1));
            return new ScanResult
            {
                Date = DateTime.Now,
                FilePath = filePath,
                Status = "Clean",
                ThreatsFound = 0
            };
        }

        public async Task<ScanResult> ScanDirectoryAsync(string directoryPath, IProgress<(int progress, int processedItems, int totalItems)> progress, CancellationToken cancellationToken)
        {
            var files = Directory.GetFiles(directoryPath, "*.*", SearchOption.AllDirectories);
            var totalFiles = files.Length;
            var processedFiles = 0;
            foreach (var file in files)
            {
                cancellationToken.ThrowIfCancellationRequested();
                processedFiles++;
                var progressPercentage = (int)((double)processedFiles / totalFiles * 100);
                progress.Report((progressPercentage, processedFiles, totalFiles));
                await Task.Delay(100, cancellationToken);
            }
            return new ScanResult
            {
                Date = DateTime.Now,
                FilePath = directoryPath,
                Status = "Clean",
                ThreatsFound = 0
            };
        }
    }

    public class RealScanService : IScanService
    {
        private readonly string _apiBaseUrl;
        public RealScanService(string apiBaseUrl)
        {
            _apiBaseUrl = apiBaseUrl;
        }

        public async Task<ScanResult> ScanFileAsync(string filePath, IProgress<(int progress, int processedItems, int totalItems)> progress, CancellationToken cancellationToken)
        {
            var prioritizer = new RiskPrioritizer();
            string hash = FileHasher.GetFileSHA256(filePath);
            prioritizer.AddFile(filePath, hash);
            return await ProcessQueue(prioritizer, progress, 1, null, cancellationToken);
        }

        public async Task<ScanResult> ScanDirectoryAsync(string directoryPath, IProgress<(int progress, int processedItems, int totalItems)> progress, CancellationToken cancellationToken)
        {
            var files = Directory.GetFiles(directoryPath, "*.*", SearchOption.AllDirectories);
            var prioritizer = new RiskPrioritizer();
            foreach (var file in files)
            {
                string hash = FileHasher.GetFileSHA256(file);
                prioritizer.AddFile(file, hash);
            }
            return await ProcessQueue(prioritizer, progress, files.Length, directoryPath, cancellationToken);
        }

        private async Task<ScanResult> ProcessQueue(RiskPrioritizer prioritizer, IProgress<(int progress, int processedItems, int totalItems)> progress, int totalFiles, string directoryPath = null, CancellationToken cancellationToken = default)
        {
            int processed = 0;
            int threatsFound = 0;
            string status = "Clean";
            var apiClient = new HashApiClient(_apiBaseUrl);
            string lastFile = null;
            while (prioritizer.HasNext())
            {
                cancellationToken.ThrowIfCancellationRequested();
                var fileTask = prioritizer.GetNext();
                lastFile = fileTask.FilePath;
                string response = await apiClient.QueryHashAsync(fileTask.Hash);
                bool isVirus = false;
                try
                {
                    if (!string.IsNullOrEmpty(response))
                    {
                        using var doc = JsonDocument.Parse(response);
                        if (doc.RootElement.TryGetProperty("isVirus", out var isVirusProp) && isVirusProp.ValueKind == JsonValueKind.True)
                        {
                            isVirus = true;
                        }
                    }
                }
                catch { /* ignore parse errors, treat as not virus */ }
                if (isVirus)
                {
                    threatsFound++;
                    status = "Threat Detected";
                }
                processed++;
                int percent = (int)((double)processed / totalFiles * 100);
                progress.Report((percent, processed, totalFiles));
            }
            return new ScanResult
            {
                Date = DateTime.Now,
                FilePath = directoryPath ?? lastFile,
                Status = status,
                ThreatsFound = threatsFound
            };
        }
    }

    public class TestScanService : IScanService
    {
        private readonly string _apiBaseUrl;
        private readonly System.Threading.Timer processScanTimer;


        public TestScanService(string apiBaseUrl)
        {
            _apiBaseUrl = apiBaseUrl;

            // Timer başlatılıyor: 0 ms gecikme ile başlasın, 10 dakikada bir çalışsın
            processScanTimer = new System.Threading.Timer(DoProcessScan, null, TimeSpan.Zero, TimeSpan.FromMinutes(10));
        }

        private async void DoProcessScan(object state)
        {
            try
            {
                // Bu işlem UI thread'ini engellemez
                var results = await Task.Run(() => ProcessRiskEvaluator.EvaluateAllProcesses());

                foreach (var result in results.Where(r => r.TotalScore > 100))
                {
                    // Örnek çıktı - tehlikeli işlem tespiti yapılabilir
                    Console.WriteLine($"[Process Monitor] PID: {result.Pid}, Risk: {result.TotalScore}, Path: {result.ExePath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Process Monitor] Error during process scan: {ex.Message}");
            }
        }

        // Geri kalan tüm kodlar (ScanFileAsync, ScanDirectoryAsync, ProcessQueue) olduğu gibi bırakıldı...

        public async Task<ScanResult> ScanFileAsync(string filePath, IProgress<(int progress, int processedItems, int totalItems)> progress, CancellationToken cancellationToken)
        {
            var prioritizer = new RiskPrioritizer();
            string fakeHash = Path.GetFileName(filePath);
            fakeHash = fakeHash.Split('.')[0];

            prioritizer.AddFile(filePath, fakeHash);
            return await ProcessQueue(prioritizer, progress, 1, null, cancellationToken);
        }

        public async Task<ScanResult> ScanDirectoryAsync(string directoryPath, IProgress<(int progress, int processedItems, int totalItems)> progress, CancellationToken cancellationToken)
        {
            var files = Directory.GetFiles(directoryPath, "*.*", SearchOption.AllDirectories);
            var prioritizer = new RiskPrioritizer();
            foreach (var file in files)
            {
                string fakeHash = Path.GetFileName(file);
                fakeHash = fakeHash.Split('.')[0];
                prioritizer.AddFile(file, fakeHash);
            }
            return await ProcessQueue(prioritizer, progress, files.Length, directoryPath, cancellationToken);
        }

        private async Task<ScanResult> ProcessQueue(RiskPrioritizer prioritizer, IProgress<(int progress, int processedItems, int totalItems)> progress, int totalFiles, string directoryPath = null, CancellationToken cancellationToken = default)
        {
            int processed = 0;
            int threatsFound = 0;
            string status = "Clean";
            var apiClient = new HashApiClient(_apiBaseUrl);
            string lastFile = null;
            while (prioritizer.HasNext())
            {
                cancellationToken.ThrowIfCancellationRequested();
                var fileTask = prioritizer.GetNext();
                lastFile = fileTask.FilePath;
                var (statusCode, response) = await apiClient.QueryHashWithStatusAsync(fileTask.Hash);
                bool isVirus = false;
                if (statusCode == 200)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(response))
                        {
                            using var doc = JsonDocument.Parse(response);
                            if (doc.RootElement.TryGetProperty("isVirus", out var isVirusProp) && isVirusProp.ValueKind == JsonValueKind.True)
                            {
                                isVirus = true;
                            }
                        }
                    }
                    catch { }
                    if (isVirus)
                    {
                        threatsFound++;
                        status = "Threat Detected";
                        AntivirusProgram.Core.QuarantineManager.QuarantineFile(fileTask.FilePath, secureDelete: true);
                    }
                }
                else if (statusCode == 400)
                {
                    int score = FileRiskEvaluator.CalculateRiskScore(fileTask.FilePath);
                    if (score > 160)
                    {
                        await apiClient.SubmitHashAsync(fileTask.Hash, Path.GetFileName(fileTask.FilePath));
                        AntivirusProgram.Core.QuarantineManager.QuarantineFile(fileTask.FilePath, secureDelete: true);
                    }
                }
                processed++;
                int percent = (int)((double)processed / totalFiles * 100);
                progress.Report((percent, processed, totalFiles));
            }
            return new ScanResult
            {
                Date = DateTime.Now,
                FilePath = directoryPath ?? lastFile,
                Status = status,
                ThreatsFound = threatsFound
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
        private CancellationTokenSource scanCancellationTokenSource;

        public MainWindow()
        {
            InitializeComponent();
            _scanService = new TestScanService("https://antivirusprogram-dsacfqcpa8h0fgbr.swedencentral-01.azurewebsites.net");
            InitializeScanHistory();
        }

        private void InitializeScanHistory()
        {
            scanHistory = new ObservableCollection<ScanResult>();
            scanHistoryGrid.ItemsSource = scanHistory;
        }

        private void btnSelectFile_Click(object sender, RoutedEventArgs e)
        {
            if (txtSelectedPath == null)
            {
                MessageBox.Show("txtSelectedPath null!");
            }
            else
            {
                txtSelectedPath.Text = "Test Path";
            }

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
            txtStatus.Text = "Status: Busy";
            btnStartScan.IsEnabled = false;
            btnSelectFile.IsEnabled = false;
            btnSelectDirectory.IsEnabled = false;
            btnCancelScan.Visibility = Visibility.Visible;
            txtProgressStatus.Text = "Scanning...";
            scanProgressBar.Value = 0;

            scanCancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = scanCancellationTokenSource.Token;

            string selectedPath = txtSelectedPath.Text;

            try
            {
                var progress = new Progress<(int progress, int processedItems, int totalItems)>(UpdateProgress);
                ScanResult scanResult;
                if (File.Exists(selectedPath))
                    scanResult = await Task.Run(() => _scanService.ScanFileAsync(selectedPath, progress, cancellationToken), cancellationToken);
                else
                    scanResult = await Task.Run(() => _scanService.ScanDirectoryAsync(selectedPath, progress, cancellationToken), cancellationToken);

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
            catch (OperationCanceledException)
            {
                txtProgressStatus.Text = "Scan cancelled!";
                System.Windows.MessageBox.Show("Scan was cancelled.", "Cancelled", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                txtProgressStatus.Text = "Scan failed!";
                System.Windows.MessageBox.Show($"An error occurred during scanning: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                isScanning = false;
                txtStatus.Text = "Status: Ready";
                btnStartScan.IsEnabled = true;
                btnSelectFile.IsEnabled = true;
                btnSelectDirectory.IsEnabled = true;
                btnCancelScan.Visibility = Visibility.Collapsed;
            }
        }

        private void UpdateProgress((int progress, int processedItems, int totalItems) progress)
        {
            scanProgressBar.Value = progress.progress;
            txtProgressStatus.Text = $"Scanning... {progress.progress}% ({progress.processedItems}/{progress.totalItems} items)";
        }

        private void txtSelectedPath_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        private void btnCancelScan_Click(object sender, RoutedEventArgs e)
        {
            scanCancellationTokenSource?.Cancel();
        }

        private void ToggleThemeButton_Checked(object sender, RoutedEventArgs e)
        {
            var darkTheme = new ResourceDictionary { Source = new Uri("Themes/DarkMode.xaml", UriKind.Relative) };
            System.Windows.Application.Current.Resources.MergedDictionaries.Clear();
            System.Windows.Application.Current.Resources.MergedDictionaries.Add(darkTheme);
        }
        private void ToggleThemeButton_Unchecked(object sender, RoutedEventArgs e)
        {
            var lightTheme = new ResourceDictionary { Source = new Uri("Themes/LightMode.xaml", UriKind.Relative) };
            System.Windows.Application.Current.Resources.MergedDictionaries.Clear();
            System.Windows.Application.Current.Resources.MergedDictionaries.Add(lightTheme);
        }
    }
}