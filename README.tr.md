
<div align="center">

# Smart Batch Printing Studio (SBPS)

### Windows için Çoklu Dosya Türü Destekli Toplu Yazdırma Yazılımı

**Farklı dosya türlerini düzenleyin, filtreleyin ve kontrollü bir Windows yazdırma kuyruğu üzerinden toplu olarak yazdırın.**

**C# · .NET 9 · Windows Forms · Microsoft Edge WebView2** teknolojileriyle geliştirilmiştir.

[**English**](README.md) · [**Kurulum**](docs/INSTALLATION.md) · [**Kullanım Kılavuzu**](docs/USER_GUIDE.md) · [**Sorun Giderme**](docs/TROUBLESHOOTING.md)

![Smart Batch Printing Studio ana ekranı](docs/images/main-window.png)

</div>

---

## Genel Bakış

**Smart Batch Printing Studio (SBPS)**, **C#, .NET 9, Windows Forms ve Microsoft Edge WebView2** teknolojileriyle geliştirilmiş, farklı dosya türlerini kontrollü bir yazdırma kuyruğu üzerinden düzenlemek, filtrelemek ve toplu olarak yazdırmak amacıyla tasarlanmış bir Windows masaüstü uygulamasıdır.

Uygulama, desteklenen farklı dosya biçimlerini içeren bir klasörü düzenli ve yönetilebilir bir yazdırma iş akışına dönüştürür.

Kullanıcılar dosyaları tek tek açmak yerine bir klasör seçebilir, mevcut dosya türlerini filtreleyebilir, dosyaları ayrı ayrı yazdırma kuyruğuna dahil edebilir veya kuyruktan çıkarabilir, Windows üzerinde kurulu bir yazıcı seçebilir, erişilebilir sayfa sayısı bilgilerini inceleyebilir ve seçilen dosyaları dosya adı sırasına göre yazdırmaya gönderebilir.

SBPS; PDF belgelerini, görselleri, metin dosyalarını ve web belgelerini çoklu dosya türü destekli yazdırma iş akışı kapsamında destekler.

Her kaynak dosya **ayrı bir yazdırma işi** olarak işlenir. Ardışık dosyaların arasına isteğe bağlı boş ayırıcı sayfalar eklenebilir.

Uygulama, birden fazla dosyayı tekrarlanabilir, düzenli ve yönetilebilir bir şekilde yazdırmak isteyen Windows kullanıcıları ve operatörleri için geliştirilmiştir.

### Başlıca Kullanım Alanları

- Farklı dosya türlerini toplu yazdırma
- Windows masaüstü ortamında yazdırma işlemleri
- Bir klasördeki birden fazla dosyayı yazdırma
- Klasör bazlı belge yazdırma
- Karma dosya türleriyle yazdırma iş akışları
- Yazdırma kuyruğu hazırlama ve yönetimi
- Belge seçimi ve dosya türüne göre filtreleme
- İsteğe bağlı boş ayırıcı sayfalarla yazdırma
- Kontrollü ve iptal edilebilir yazdırma işlemleri

Gerçek yazdırma çıktıları; Windows işletim sistemine, seçilen yazıcıya, yazıcı sürücüsüne ve desteklenen belge işleme yeteneklerine bağlıdır.

---

## Temel Özellikler

### Klasör ve Dosya Yönetimi

- Klasör yolunu manuel olarak girme
- Windows klasör seçme penceresini kullanma
- Sürükle-bırak yöntemiyle klasör seçimi
- İsteğe bağlı olarak alt klasörleri tarama
- Desteklenen dosya uzantılarını otomatik algılama
- Klasör içeriğinden dinamik dosya türü filtreleri oluşturma
- Dosyaları tek tek seçme veya seçimden çıkarma
- Dosyaları dosya adı sırasına göre işleme
- Desteklenen ve desteklenmeyen dosyalara ilişkin istatistikler

### Yazdırma Kuyruğu Yönetimi

- Toplam dosya sayısını anlık görüntüleme
- Seçilen dosya sayısını görüntüleme
- Yazdırılabilir dosya sayısını görüntüleme
- Her dosyanın yazdırma durumunu ayrı takip etme
- Sıralı yazdırma işi gönderimi
- Yazdırma kuyruğunu iptal edebilme
- Tanılama ve işlem kayıtları oluşturma
- Yapılandırılabilir hata yönetimi

### Sayfa Sayısı Bilgileri

- PDF sayfa sayısını güvenli biçimde belirlenebildiğinde tespit etme
- Desteklenen görsel dosyalarını tek sayfa olarak değerlendirme
- Yazdırma işlemini başlatmadan sayfa sayısı bilgilerini inceleme
- Bilinen içerik sayfası toplamını görüntüleme
- Planlanan boş ayırıcı sayfa toplamını görüntüleme
- Sayfa sayısı belirlenemeyen dosyaları açıkça gösterme

Sayfa sayısı bilgileri, yazıcının kullanacağı fiziksel kâğıt yaprağı sayısını garanti etmez.

### Yazıcı Yapılandırması

- Windows üzerinde kurulu yazıcıları otomatik algılama
- Kullanılacak yazıcıyı manuel seçme
- Yazıcı listesini yenileme
- Desteklenen yazıcılarda varsayılan olarak çift taraflı yazdırma
- İsteğe bağlı Windows yazıcı ayarları penceresini açma
- Boş ayırıcı sayfa kullanımını yapılandırma
- Yazıcı ve sürücü yeteneklerine bağlı yazdırma seçenekleri

### Hata Yönetimi ve İptal

- Dosya bazında işlem durumu takibi
- Tanılama ve hata kayıtları
- Yazdırma kuyruğunu iptal etme
- Yapılandırılabilir hata işleme davranışı
- Varsayılan olarak etkin **Hatalı Dosyaları Atla** seçeneği
- Hata sonrasında kalan dosyaların gönderilmesini durdurabilme

**Hatalı Dosyaları Atla** seçeneği etkin olduğunda bir dosyanın yazdırılamaması, kalan dosyaların işlenmesini otomatik olarak durdurmaz.

Bu seçenek devre dışı bırakıldığında, hata sonrasında kalan dosyalar yazıcıya gönderilmez.

### Kullanıcı Arayüzü

- Windows Forms masaüstü arayüzü
- Yüksek DPI çözünürlüklerine duyarlı tasarım
- Türkçe ve İngilizce dil desteği
- Windows bölgesel ayarlarına göre başlangıç dili seçimi
- Uygulama çalışırken dil değiştirme
- İsteğe bağlı bilgilendirici ipuçları
- Bölümlere ayrılmış uygulama içi yardım ekranı
- Özel Çıkış düğmesi
- Aktif yazdırma sırasında uygulama kapatılırken onay isteme

**İpuçları** seçeneği varsayılan olarak kapalıdır. Kullanıcı tarafından etkinleştirilmediği sürece ipuçları görüntülenmez.

---

## Desteklenen Dosya Biçimleri

Smart Batch Printing Studio, mevcut yazdırma iş akışında birden fazla dosya kategorisini desteklemektedir.

| Kategori | Desteklenen Uzantılar | Sayfa Sayısı |
|---|---|---|
| PDF belgeleri | `.pdf` | Güvenli şekilde belirlenebildiğinde tespit edilir |
| Görseller | `.jpg`, `.jpeg`, `.jfif`, `.png`, `.bmp`, `.gif`, `.webp`, `.svg`, `.avif`, `.ico`, `.jxl` | 1 sayfa |
| Metin dosyaları | `.txt`, `.csv`, `.xml` | Yazdırma sırasında belirlenir |
| Web belgeleri | `.html`, `.htm`, `.xhtml`, `.mht`, `.mhtml` | Yazdırma sırasında belirlenir |

### Önemli Açıklamalar

- Şifrelenmiş, bozuk, alışılmadık biçimde kodlanmış veya çok büyük PDF dosyalarında sayfa sayısı belirlenemeyebilir.
- Desteklenen görseller, uygulamanın mevcut tek sayfalık yazdırma akışına göre işlenir.
- Metin ve web belgelerinin sayfa düzeni, belge işleme motoruna ve yazıcı yapılandırmasına bağlıdır.
- Microsoft Word `.docx` ve Microsoft Excel `.xlsx` dosyaları **yerel olarak desteklenmemektedir**.

**SBPS yalnızca PDF yazdırmak için geliştirilmiş bir araç değildir. Farklı dosya türlerini toplu yazdırmayı amaçlayan bir uygulamadır.**

---

## Ekran Görüntüleri

### Ana Uygulama Ekranı

![SBPS - Smart Batch Printing Studio](docs/images/main-window.png)

---

## Başlangıç ve Kurulum

### Sistem Gereksinimleri

| Bileşen | Gereksinim |
|---|---|
| İşletim Sistemi | Windows 10 veya Windows 11 |
| Geliştirme Platformu | .NET 9 |
| Programlama Dili | C# |
| Masaüstü Arayüzü | Windows Forms |
| Belge İşleme | Microsoft Edge WebView2 Runtime |
| Geliştirme Ortamı | Visual Studio 2022 17.12 veya üzeri |
| Alternatif Geliştirme | .NET CLI |
| Yazdırma | Windows üzerinde kurulu en az bir yazıcı |

Geliştirme için Visual Studio üzerinde **.NET desktop development** iş yükü veya doğrudan .NET SDK kullanılabilir.

### GitHub Deposunu Klonlama

Güncel GitHub deposunu klonlayın:

    git clone https://github.com/AhmetYILDIRIMuyum/smart-batch-printing-studio.git

Proje klasörüne geçin:

    cd smart-batch-printing-studio

### Bağımlılıkların Yüklenmesi

    dotnet restore .\SBPS.sln

### Debug Derlemesi

    dotnet build .\SBPS.sln -c Debug

### Uygulamanın Çalıştırılması

    dotnet run --project .\src\SBPS\SBPS.csproj

### Release Derlemesi

    dotnet build .\SBPS.sln -c Release

Ayrıntılı kurulum bilgileri için:

[Kurulum Kılavuzu](docs/INSTALLATION.md)

---

## Nasıl Kullanılır?

### Adım 1 — Klasör Seçimi

Yazdırılacak dosyaların bulunduğu klasörü seçin.

Kullanılabilecek yöntemler:

- Klasör yolunu manuel girme
- Windows klasör seçme penceresini kullanma
- Klasörü uygulama penceresine sürükleyip bırakma

İsteğe bağlı olarak alt klasör taraması etkinleştirilebilir.

### Adım 2 — Desteklenen Dosyaların Listelenmesi

Dosya listesini yenileyin.

SBPS, desteklenen dosyaları algılar ve desteklenmeyen dosyaların kaç tanesinin liste dışında bırakıldığını gösterir.

### Adım 3 — Dosya Türlerinin Filtrelenmesi

Yazdırma kuyruğuna dahil edilmesini istediğiniz dosya türlerini seçin.

Klasörde PDF dosyaları bulunuyorsa başlangıçta PDF dosya türü seçili gelir.

### Adım 4 — Dosyaların Seçilmesi

Dosyaların yanındaki seçim kutularını kullanarak her dosyayı yazdırma kuyruğuna dahil edin veya kuyruktan çıkarın.

Toplam, seçilen ve yazdırılabilir dosya sayılarını kontrol edin.

### Adım 5 — Sayfa Sayılarının İncelenmesi

Erişilebilir sayfa sayısı bilgilerini inceleyin.

Uygulama, bilinen içerik sayfası toplamını ve planlanan ayırıcı sayfa toplamını görüntüler.

Bazı dosya biçimlerinin sayfa sayısı yalnızca yazdırma sırasında belirlenebilir.

### Adım 6 — Yazdırma Ayarlarının Yapılandırılması

Windows üzerinde kurulu yazıcılardan birini seçin.

Aşağıdaki seçenekleri kontrol edin:

- Çift taraflı yazdırma
- Boş ayırıcı sayfalar
- Gelişmiş yazıcı ayarları
- Hatalı dosyaları atlama

Kullanılabilir yazdırma seçenekleri, seçilen yazıcı ve sürücüsünün yeteneklerine bağlıdır.

### Adım 7 — Yazıcıya Gönderme

**Yazıcıya Gönder (Send to Printer)** düğmesine tıklayın.

Görüntülenen onay penceresinde işlemi onaylayın.

SBPS, seçilen dosyaları dosya adı sırasına göre işler.

### Adım 8 — İşlemin Takibi

Her dosyanın işlem durumunu takip edin.

Gerektiğinde yazdırma işlemini iptal edin.

Sorun giderme amacıyla tanılama kayıtlarını inceleyebilirsiniz.

Ayrıntılı kullanım bilgileri için:

[Kullanım Kılavuzu](docs/USER_GUIDE.md)

---

## Boş Ayırıcı Sayfalar

Smart Batch Printing Studio, ardışık dosyaların arasına isteğe bağlı boş ayırıcı sayfa ekleyebilir.

Bu seçenek varsayılan olarak etkindir.

Örnek:

    Dosya_001.pdf
           |
           v
    Boş Ayırıcı Sayfa
           |
           v
    Dosya_002.png
           |
           v
    Boş Ayırıcı Sayfa
           |
           v
    Dosya_003.txt

**N adet dosya** içeren bir yazdırma kuyruğunda en fazla **N − 1 ayırıcı sayfa** oluşturulur.

İlk dosyadan önce veya son dosyadan sonra boş ayırıcı sayfa eklenmez.

### Önemli Sınırlama

Her kaynak dosya ayrı bir yazdırma işi olarak gönderilir.

Yazıcı sürücüleri çift taraflı yazdırma sınırlarını ve ayırıcı sayfa işlerini farklı şekillerde yönetebilir.

Uygulama, bütün yazıcı modellerinde fiziksel ayırıcı sayfaların aynı şekilde oluşturulacağını garanti etmez.

---

## Kullanılan Teknolojiler

| Teknoloji | Kullanım Amacı |
|---|---|
| C# | Ana uygulama geliştirme dili |
| .NET 9 | Uygulama platformu |
| Windows Forms | Masaüstü kullanıcı arayüzü |
| Microsoft Edge WebView2 | Belge işleme ve yazdırma |
| System.Drawing.Printing | Windows yazdırma entegrasyonu |
| Windows Property System | PDF sayfa sayısının alternatif yöntemle tespiti |

### Bağımlılıklar

Projedeki tek NuGet bağımlılığı:

`Microsoft.Web.WebView2`

---

## Proje Mimarisi

Çözüm; kullanıcı etkileşimi, dosya bilgileri, yazdırma iş akışları, dil desteği ve uygulama içi yardım gibi sorumluluklara ayrılmış bileşenlerden oluşmaktadır.

### Proje Klasör Yapısı

    SBPS.sln
    |
    +-- src/
    |   |
    |   +-- SBPS/
    |       |
    |       +-- SBPS.csproj
    |       +-- MainForm.cs
    |       +-- FilePageCountService.cs
    |       +-- PdfFileItem.cs
    |       +-- Localization.cs
    |       +-- HelpDialog.cs
    |       +-- PremiumControls.cs
    |       +-- Program.cs
    |       +-- app.manifest
    |
    +-- docs/
        |
        +-- ARCHITECTURE.md
        +-- INSTALLATION.md
        +-- USER_GUIDE.md
        +-- TROUBLESHOOTING.md

### Temel Bileşenler

**MainForm.cs**

Ana masaüstü kullanıcı arayüzünü ve kullanıcı etkileşimlerini yönetir.

**FilePageCountService.cs**

Sayfa sayısı inceleme ve ilgili dosya bilgisi işlemlerini gerçekleştirir.

**PdfFileItem.cs**

Uygulamanın belge işleme akışında kullanılan dosya bilgilerini temsil eder.

**Localization.cs**

Türkçe ve İngilizce dil desteğini sağlar.

**HelpDialog.cs**

Uygulama içi yardım ekranını oluşturur.

**PremiumControls.cs**

Özelleştirilmiş kullanıcı arayüzü bileşenlerini içerir.

**Program.cs**

Uygulamanın başlangıç noktasını oluşturur.

Teknik mimari hakkında ayrıntılı bilgi için:

[Mimari Dokümantasyonu](docs/ARCHITECTURE.md)

---

## Yapılandırma ve Veri Saklama

SBPS, Windows üzerinde kurulu yazıcıları işletim sisteminden okur.

Yazıcı yetenekleri, kullanılabilir seçenekler ve gerçek yazdırma davranışı seçilen yazıcıya ve sürücüsüne bağlıdır.

### Varsayılan Ayarlar

| Ayar | Varsayılan Durum |
|---|---|
| Çift taraflı yazdırma | Etkin |
| Boş ayırıcı sayfalar | Etkin |
| Hatalı dosyaları atla | Etkin |
| Önce yazıcı ayarlarını göster | Kapalı |
| İpuçları | Kapalı |

Yazdırma seçenekleri uygulama oturumları arasında kalıcı olarak saklanmaz.

### Yerel Uygulama Verileri

SBPS, son seçilen klasör bilgisini aşağıdaki dosyada saklar:

`%LocalAppData%\FilePrintStudio\last-folder.txt`

Tanılama kayıtları ve WebView2 önbelleği de aşağıdaki dizini kullanır:

`%LocalAppData%\FilePrintStudio`

Bu dizin, önceki kurulumlarla uyumluluğu korumak amacıyla kullanılmaya devam edilmektedir.

---

## Derleme ve Geliştirme

Uygulamanın hedef platformu:

`net9.0-windows`

Aşağıdaki derleme ve geliştirme çıktıları kaynak kod yönetiminden hariç tutulmuştur:

- `bin/`
- `obj/`
- `.vs/`
- `publish/`

### Geliştirme Doğrulamaları

Projede şu anda otomatik test projesi bulunmamaktadır.

Değişiklikler aşağıdaki kontrollerle doğrulanmalıdır:

- Debug derlemesi
- Release derlemesi
- Uygulama başlangıç kontrolleri
- Kullanıcı arayüzü kontrolleri
- Fiziksel yazdırma içermeyen iş akışı testleri

Operatörün açık onayı olmadan fiziksel test sayfası gönderilmemelidir.

---

## Sorun Giderme

### WebView2 Başlatma Hatası

Microsoft Edge WebView2 başlatılamıyorsa:

1. Microsoft Edge WebView2 Runtime bileşeninin kurulu olduğunu kontrol edin.
2. Microsoft Edge WebView2 Evergreen Runtime bileşenini yükleyin veya onarın.
3. Smart Batch Printing Studio uygulamasını yeniden başlatın.
4. Sorun devam ederse tanılama kayıtlarını inceleyin.

### Yazıcı Listede Görünmüyor

Kurulu bir yazıcı listede görüntülenmiyorsa:

1. Windows Ayarlar ekranını açın.
2. **Bluetooth ve cihazlar > Yazıcılar ve tarayıcılar** bölümüne gidin.
3. Yazıcının kurulu olduğunu doğrulayın.
4. SBPS uygulamasına dönün.
5. **Yazıcıları Yenile (Refresh Printers)** seçeneğini kullanın.

### PDF Sayfa Sayısı Belirlenemiyor

Aşağıdaki durumlarda PDF sayfa sayısı belirlenemeyebilir:

- Dosyanın şifrelenmiş olması
- Dosyanın bozuk olması
- Alışılmadık bir kodlama biçimi kullanılması
- Dosyanın güvenli inceleme sınırını aşacak kadar büyük olması

Sayfa sayısının belirlenememesi, belgenin mutlaka yazdırılamayacağı anlamına gelmez.

### Yazdırma Hatası

Yazdırma sırasında bir dosyada hata oluşursa:

- Seçilen yazıcıyı kontrol edin.
- Yazıcının kullanılabilir olduğunu doğrulayın.
- Tanılama kayıtlarını inceleyin.
- **Hatalı Dosyaları Atla** seçeneğinin durumunu kontrol edin.

**Hatalı Dosyaları Atla** etkin olduğunda, bir dosyada hata oluşsa bile kalan kuyruk işlenmeye devam eder.

Seçenek devre dışı bırakıldığında, hata sonrasında kalan dosyaların yazıcıya gönderimi durdurulur.

Ayrıntılı bilgi için:

[Sorun Giderme Kılavuzu](docs/TROUBLESHOOTING.md)

---

## Bilinen Sınırlamalar

Smart Batch Printing Studio aşağıdaki sınırlamalara sahiptir:

- Uygulama Windows 10 veya Windows 11 gerektirir.
- Microsoft Edge WebView2 Runtime gereklidir.
- Microsoft Word DOCX dosyalarının yerel yazdırma desteği bulunmamaktadır.
- Microsoft Excel XLSX dosyalarının yerel yazdırma desteği bulunmamaktadır.
- Desteklenen belgelerin işlenmesi kullanılan yazdırma altyapısına bağlıdır.
- PDF sayfa sayısı tespiti mümkün olan en iyi yöntemle gerçekleştirilir; garanti edilmez.
- Bazı şifrelenmiş, bozuk, sıkıştırılmış veya çok büyük PDF dosyalarında sayfa sayısı görüntülenmeyebilir.
- Metin ve HTML belgelerinin sayfa düzeni; kâğıt boyutu, kenar boşlukları, yazdırma motoru ve yazıcı ayarlarına bağlıdır.
- Görüntülenen sayfa sayıları fiziksel kâğıt yaprağı sayısını garanti etmez.
- Çift taraflı yazdırma davranışı yazıcı ve sürücü özelliklerine bağlıdır.
- Boş ayırıcı sayfalar farklı yazıcı modellerinde farklı davranabilir.
- Ayrı yazdırma işleri, çift taraflı yazdırma sınırlarının bütün cihazlarda aynı şekilde yönetileceğini garanti etmez.
- Yazdırma seçenekleri oturumlar arasında saklanmaz.
- Projede şu anda otomatik test projesi bulunmamaktadır.

---

## Lisans

**Smart Batch Printing Studio (SBPS) kaynak kodu herkese açık olarak görüntülenebilir; ancak açık kaynak kodlu bir yazılım değildir.**

Kaynak kod GitHub üzerinden incelenebilir.

Bununla birlikte, kaynak kodun herkese açık olması aşağıdaki işlemler için izin verildiği anlamına gelmez:

- Yazılımı izinsiz kullanmak
- Kaynak kodu kopyalamak
- Yazılımı değiştirmek
- Uygulamayı yeniden dağıtmak
- Yazılım için alt lisans vermek
- Yazılımı veya kaynak kodunu satmak

Bu işlemler için hak sahibinin önceden yazılı izni gereklidir.

Lisans koşullarının tamamı için:

[LICENSE](LICENSE)

---

## Geliştirici

**Ahmet YILDIRIM [dede]**

Kurumsal yazılım uygulamaları, masaüstü yazılım geliştirme ve otomasyon çözümleri üzerine yazılım geliştirme çalışmaları.

**GitHub Profili:**

https://github.com/AhmetYILDIRIMuyum

**Proje Deposu:**

https://github.com/AhmetYILDIRIMuyum/smart-batch-printing-studio

**E-posta:**

ahmetyildirimuyum@gmail.com

---

## Katkıda Bulunma

Proje için henüz herkese açık bir katkı politikası belirlenmemiştir.

Hata bildirmek, geliştirme önerisinde bulunmak veya olası bir katkıyı görüşmek istiyorsanız GitHub üzerinden bir issue oluşturabilir ya da geliştiriciyle iletişime geçebilirsiniz.

Deponun herkese açık olması, kaynak kodu değiştirme, yeniden dağıtma veya başka projelerde kullanma izni verildiği anlamına gelmez.

---

## Proje Durumu

**Smart Batch Printing Studio aktif geliştirme aşamasındadır.**

Proje aşağıdaki konulara odaklanmaktadır:

- Windows masaüstü ortamında toplu yazdırma
- Farklı dosya biçimlerinin işlenmesi
- Yazdırma kuyruğu yönetimi
- Yazıcı yapılandırması
- Dosya seçimi ve filtreleme
- Sayfa sayısı inceleme
- Türkçe ve İngilizce dil desteği
- Yazdırma tanılama kayıtları ve iptal işlemleri

Değişiklikler yayımlandığında Debug ve Release derleme sonuçları belgelendirilir.

Üretim ortamına hazır olma, ticari destek veya sürüm desteği garantisi verilmemektedir.

---

## Proje Bağlantıları

| Kaynak | Bağlantı |
|---|---|
| GitHub Deposu | [Smart Batch Printing Studio](https://github.com/AhmetYILDIRIMuyum/smart-batch-printing-studio) |
| İngilizce Dokümantasyon | [README.md](README.md) |
| Kurulum | [Kurulum Kılavuzu](docs/INSTALLATION.md) |
| Kullanım | [Kullanım Kılavuzu](docs/USER_GUIDE.md) |
| Teknik Mimari | [Mimari Dokümantasyonu](docs/ARCHITECTURE.md) |
| Sorun Giderme | [Sorun Giderme Kılavuzu](docs/TROUBLESHOOTING.md) |
| Lisans | [LICENSE](LICENSE) |

---

<div align="center">

### Smart Batch Printing Studio (SBPS)

**Çoklu Dosya Türü Destekli Toplu Yazdırma | Windows Masaüstü Yazdırma | Yazdırma Kuyruğu Yönetimi**

**C# · .NET 9 · Windows Forms · Microsoft Edge WebView2**

**© 2026 Ahmet YILDIRIM. Tüm hakları saklıdır.**

</div>
