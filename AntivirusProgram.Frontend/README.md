# Antivirus Program Frontend

## Proje Hakkında
Bu proje, Windows tabanlı bir antivirüs tarayıcı uygulamasının kullanıcı arayüzünü içermektedir. WPF (Windows Presentation Foundation) teknolojisi kullanılarak geliştirilmiş modern ve kullanıcı dostu bir arayüz sunmaktadır. Uygulama, Azure tabanlı bir backend servisi ile entegre çalışmaktadır.

## Teknik Detaylar
- **Framework**: .NET 9.0
- **UI Framework**: WPF (Windows Presentation Foundation)
- **Mimari**: MVVM (Model-View-ViewModel) pattern
- **Backend Entegrasyonu**: Azure Web Service
- **Tema Desteği**: Özel Light/Dark tema implementasyonu

## Özellikler
### Temel Özellikler
- 🎯 Dosya ve dizin tarama seçenekleri
- ⚡ Hızlı tarama modu
- �� Gerçek zamanlı tarama ilerleme göstergesi
- 📋 Son 15 tarama geçmişi
- 🌓 Açık/Koyu tema desteği
- 🎨 Modern ve kullanıcı dostu arayüz

### Detaylı Özellikler
- **Dosya Seçimi**
  - Tek dosya seçimi
  - Dizin seçimi
  - Seçilen yol gösterimi
- **Tarama Özellikleri**
  - İlerleme durumu gösterimi
  - İşlenen dosya sayısı takibi
  - Tarama iptal etme özelliği
  - Hızlı tarama modu
- **Sonuç Raporlama**
  - Tarih ve saat bilgisi
  - Dosya yolu
  - Tarama durumu
  - Bulunan tehdit sayısı
- **Kullanıcı Arayüzü**
  - Responsive tasarım
  - Modern UI bileşenleri
  - Tema değiştirme özelliği
  - Durum bildirimleri

## Teknik Gereksinimler
- .NET 9.0 SDK
- Windows işletim sistemi
- Visual Studio 2022 (önerilen)
- Azure hesabı (backend servisi için)

## Kurulum
1. Projeyi klonlayın:
```bash
git clone [(https://github.com/ber0glu/BMU258-Antivirus-Program/edit/main/AntivirusProgram.Frontend)]
```

2. Visual Studio'da `AntivirusProgram.Frontend.sln` dosyasını açın

3. NuGet paketlerinin yüklenmesini bekleyin

4. `App.xaml.cs` dosyasında backend servis URL'sini yapılandırın:
```csharp
_scanService = new TestScanService("your-backend-url");
```

5. Projeyi derleyin ve çalıştırın

## Kullanım
1. Uygulamayı başlatın
2. Sağ üst köşedeki tema değiştirme butonu ile tercih ettiğiniz temayı seçin
3. "Select File" veya "Select Directory" butonları ile taramak istediğiniz dosya veya dizini seçin
4. İsteğe bağlı olarak "Fast Scan" seçeneğini aktifleştirin
5. "Start Scan" butonuna tıklayarak taramayı başlatın
6. Tarama sırasında:
   - İlerleme çubuğunu takip edin
   - İşlenen dosya sayısını görüntüleyin
   - Gerekirse "Cancel Scan" ile taramayı iptal edin
7. Tarama sonuçlarını ve geçmişini alt panelde görüntüleyin



![Ekran görüntüsü 2025-06-11 143122](https://github.com/user-attachments/assets/691ccd57-500e-4c03-9adb-452968a734fc)
![image](https://github.com/user-attachments/assets/35f05a80-7494-4ce9-9df1-ff08a8e522eb)

