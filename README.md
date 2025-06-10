# 🛡️ BMU-258 - Antivirüs Uygulaması

BMU-258, **C#** ve **.NET Core** teknolojileriyle geliştirilmiş bir antivirüs uygulamasıdır.  
Masaüstü tarafı **WPF**, web servisi ise **ASP.NET Core Web API** kullanılarak tasarlanmıştır.

Bu uygulama, dosya ve işlem taramalarıyla potansiyel tehditleri tespit etmeyi ve şüpheli dosyaları izole etmeyi amaçlar.

---

## 📁 Proje Yapısı

### 🔧 Core (İş Mantığı Katmanı)

Antivirüs motorunun temel bileşenleri:

- **FileHasher:** Dosyaların SHA256 hash’ini hesaplar.
- **FileRiskEvaluator:** Dosya türü, imzası, konumu ve içeriğini analiz ederek şüpheli davranışları değerlendirir (örn. Base64 kodu, null byte).
- **ProcessRiskEvaluator:** Çalışan işlemleri değerlendirir:
  - Dosyanın risk puanı
  - Diğer işlemlere thread enjekte etme
  - Bellekte hem yazılabilir hem çalıştırılabilir alan kullanımı
  - Şüpheli konumlardan dosya yükleme
- **ProcessMemoryScanner**, **ModuleAnalyzer**, **ThreadAnalyzer:** Bellek ve işlem analizinde kullanılır.
- **RiskPrioritizer:** Dosyaları risk seviyelerine göre önceliklendirir.
- **QuarantineManager:** Zararlı dosyaları karantinaya alır veya siler.

---

### 🖥️ Frontend (WPF Arayüzü)

Sade ve kullanıcı dostu arayüz:

- Dosya veya klasör seçimi
- Tarama başlatma
- Tarama ilerlemesini izleme
- Geçmiş taramaları görüntüleme

> Not: Taramalar sadece arayüzde görüntülenir, veritabanına yazılmaz.

---

### 🌐 Backend (ASP.NET Core Web API)

- **Azure** üzerinde host edilmiştir.
- Dosya hash değerine göre virüs kontrolü yapar.
- Tehlikeli dosyaları kayıt altına alır.
- Gerekirse tüm kayıtları temizler.
- Katmanlı mimari (Controller, Business, Repository) kullanılmıştır.
- Hatalar özel bir yapı ile yönetilir.

---

### 🗄️ Veritabanı ve Repository

- **Entity Framework Core** kullanılmıştır.
- Veritabanında her kayıt şu bilgileri içerir:
  - Dosya adı
  - Hash değeri (benzersiz olarak indekslenmiş)
  - Tarama tarihi
  - Virüs durumu

---

## ⚙️ Uygulama Akışı

1. Kullanıcı dosya veya klasör seçer.
2. Dosyaların hash değerleri hesaplanır.
3. Her dosya analiz edilerek risk puanı atanır.
4. Hash değeri VirusTotal servisine gönderilir.
5. Tehlikeli dosyalar karantinaya alınır ve veritabanına kaydedilir.
6. Sonuçlar kullanıcıya arayüz üzerinden gösterilir.

> Uygulama ayrıca her **10 dakikada bir çalışan işlemleri tarayan** bir arka plan görevine sahiptir.

---

## ✅ Sonuç

BMU-258 projesi, antivirüs algoritmalarını gerçek bir uygulamaya dönüştürmeyi başarmıştır.  
Yapısı sade, işlevleri faydalı ve yazılım geliştirme ilkelerine uygundur.  

Geliştirme sürecinde loglama, test kapsamı ve kullanıcı geri bildirimine yönelik özellikler eklendiğinde daha güçlü bir güvenlik aracı haline gelecektir.

---

## 🛠️ Kullanılan Teknolojiler

- .NET Core
- C#
- WPF
- ASP.NET Core Web API
- Entity Framework Core
- Azure
- VirusTotal API
