# AntivirusProgram.API

Bu proje, .NET tabanlı çok katmanlı (N-Tier Architecture) bir antivirüs uygulamasıdır. Kullanıcı dosyalarının hash'lerini analiz ederek potansiyel tehditleri tespit eder. VirusTotal API'si ile entegre çalışarak dosya hash'leri hakkında bilgi alır.

## 🔧 Kullanılan Teknolojiler

- **.NET 9 Web API**
- **Entity Framework Core**
- **Repository Pattern**
- **N-Tier Architecture**
- **VirusTotal API entegrasyonu**

## 🧱 Proje Katmanları

N-Tier mimariye sahiptir:

- **AntivirusProgram.API**  
  Web API giriş noktası. Dependency injection, middleware(Exception Handler) ve endpoint yapılandırmaları burada tanımlandı.

- **AntivirusProgram.Entities**  
  Entity modelleri, DTO'lar,Error Modeller ve Custom Exception Modeller bu katmanda tanımlandı.

- **AntivirusProgram.Repositories**  
  EF Core kullanılarak oluşturulmuş repository sınıflarını içerir. Veritabanı işlemleri soyutlanmıştır.

- **AntivirusProgram.Services**  
  İş kuralları ve servis katmanı yer alır. Ayrıca `VirusTotalClient.cs` ile VirusTotal API entegrasyonu buradadır.

- **AntivirusProgram.Presentation**  
  API Controller'ları burada yer alır. HTTP istekleri bu katmandan yönetilir.



---

## ✅ **API Endpoint Dökümantasyonu**

Kullanılabilir endpoint'lerin kısa bir özeti çok işe yarar:

```md
## 📡 API Endpointleri

| Metot   | Endpoint                      | Açıklama                                                                             |
|---------|-------------------------------|--------------------------------------------------------------------------------------|
| GET     | `/api/scan/{hash}`            | Belirli bir hash için veri tabanında arama yapar, yoksa VirusTotal'dan sonuç getirir |
| POST    | `/api/scan/{hash}{fileName?}` | Hash'i kesin olarak virüs olarak işaretler ve veritabanına kaydeder                  |
| DELETE  | `/api/scan/`                  | Veritabanında bulunan tüm kayıtları siler(Development için)                          |

```

## 🔐 VirusTotal API Entegrasyonu
```csharp
  public async Task<FileScanResult> GetOrCreateScanResultByHashAsync(string hash, bool trackChanges)
  {
      // hash'in veritabanında olup olmadığınnın kontrolü
      // eğer hash veritabanında kayıtlıysa kullanıcıya direkt bir dönüş gerçekleşir.
      var existing = await repositoryManager.VirusRepository.GetFileScanResultByHashAsync(hash, trackChanges);
      if (existing != null)
          return existing;
      
      // virus total için oluşturulmuş service yapılanmasına istek atılır
      var vtResult = await virusTotalClient.QueryHashAsync(hash);
  
      try
      {
          ....
              // maliciousCount virus total apiye gönderilen hash'in kaç tane programda virus olarak işaretlendiğine bakılır.
              // eğer bu sayı 0 dan büyükse yani herhangi bir sistem tarafından tehlikeli bir yazılım olarak belirlenmişse isVirus true olarak set edilir
              isVirus = maliciousCount.GetInt32() > 0;
          ....
      } catch { }
      ....
      // isVirus false ise NotAVirusException(status code: 400) döndürülür
      if (!isVirus)
          throw new NotAVirusException(hash);
      
      // manuel mapping yapılıyor. 
      var newRecord = new FileScanResult
      {
         ....
      };
      // Sistemin veritabanına virus kayıt edilir
      repositoryManager.VirusRepository.CreateFileScanResult(newRecord);
      // değişiklikler kaydedilir
      await repositoryManager.SaveAsync();
      
      return newRecord;
  }
```

## 🔐 Clientten gelen hash'in kesin bir şekilde virus olarak işaretlenmesi işlemi
```csharp
  public async Task<FileScanResult> CreateVirusAsync(string hash, string? fileName = null)
  {
      // Gelen hash'in zaten veritabanında kayıtlı olup olmadığının kontrolü gerçekleşiyor
      var existing = await repositoryManager.VirusRepository.GetFileScanResultByHashAsync(hash, false);
      if (existing != null)
          throw new FileHashAlreadyExistsException(hash);
  
      var newRecord = new FileScanResult
      {
          ...
      };
  
      repositoryManager.VirusRepository.CreateFileScanResult(newRecord);
      await repositoryManager.SaveAsync();
      return newRecord;
  }
```


## Proje Kurulumu ve Çalıştırma
- git clone https://github.com/ber0glu/BMU258-Antivirus-Program.git
- cd BMU258-Antivirus-Program/AntivirusProgram.Backend/AntivirusProgram.API
- dotnet restore
- dotnet ef database update
- appsettings.json dosyasını açın ve aşağıdaki bölümü kendi API anahtarınızla güncelleyin:
- ```json
  "VirusTotal": {
    "ApiKey": "YOUR_API_KEY"
  }
  ```
