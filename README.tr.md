<div align="center">

# SBPS — Akıllı Toplu Yazdırma Stüdyosu

**Desteklenen dosyaları düzenlemek ve toplu yazdırmak için Windows masaüstü uygulaması.**

[English](README.md) · [Kurulum](docs/INSTALLATION.md) · [Kullanım](docs/USER_GUIDE.md) · [Sorun Giderme](docs/TROUBLESHOOTING.md)

</div>

## Genel Bakış

SBPS, farklı dosya türleri içeren bir klasörü kontrollü bir baskı kuyruğuna dönüştürür. Kullanıcı klasörde bulunan dosya tiplerine göre filtreleme yapabilir, dosyaları tek tek seçebilir, Windows'ta kurulu bir yazıcı belirleyebilir, güvenli biçimde hesaplanabilen sayfa bilgilerini görebilir ve oluşan kuyruğu dosya adına göre sırayla yazıcıya gönderebilir.

Her kaynak dosya ayrı bir yazdırma işi olarak gönderilir. Kullanıcı isterse ardışık dosyaların arasına boş bir ayırıcı sayfa ekleyebilir.

## Temel Özellikler

- Klasör yolunu elle yazma veya Windows klasör seçicisini kullanma
- İsteğe bağlı alt klasör tarama
- Klasörde bulunan uzantılardan dinamik dosya tipi filtreleri
- Baskı listesinden dosya bazında seçim
- Anlık toplam, seçilen ve yazdırılacak dosya sayaçları
- Dosya bazında güvenli sayfa sayısı bilgisi
- Bilinen içerik sayfası ve planlanan ayırıcı sayfa özeti
- Kurulu Windows yazıcılarını seçme ve listeyi yenileme
- Yazıcı destekliyorsa varsayılan açık çift taraflı yazdırma
- Yazdırmadan önce isteğe bağlı Windows yazıcı ayarları
- Varsayılan açık, dosyalar arasında isteğe bağlı boş ayırıcı sayfa
- Sıralı ve durdurulabilir işlem; dosya durumları ve tanılama kaydı
- Windows bölge/dil ayarından otomatik belirlenen ve ekrandan değiştirilebilen Türkçe/İngilizce arayüz
- Yüksek DPI uyumlu Windows Forms arayüzü ve bölümlü uygulama içi yardım penceresi
- Ekranda Çıkış düğmesi; yazdırma sürerken kapanış onay ister

## Desteklenen Dosya Türleri

| Kategori | Uzantılar | Sayfa bilgisi |
|---|---|---|
| PDF | `.pdf` | Güvenli biçimde belirlenebiliyorsa belgeden okunur; aksi halde hesaplanamadı |
| Görsel | `.jpg`, `.jpeg`, `.jfif`, `.png`, `.bmp`, `.gif`, `.webp`, `.svg`, `.avif`, `.ico`, `.jxl` | Mevcut baskı akışına uygun olarak 1 |
| Metin | `.txt`, `.csv`, `.xml` | Yazdırmada belirlenir |
| Web | `.html`, `.htm`, `.xhtml`, `.mht`, `.mhtml` | Yazdırmada belirlenir |

DOCX ve XLSX için yerleşik destek iddiası yoktur.

## Ekran Görüntüsü

![SBPS ana penceresi](docs/images/main-window.png)

## Başlangıç

### Gereksinimler

- Windows 10 veya Windows 11
- Geliştirme için [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- .NET masaüstü geliştirme iş yüküyle Visual Studio 2022 17.12 veya üzeri ya da .NET CLI
- Microsoft Edge WebView2 Runtime
- Yazdırma için Windows'ta kurulu en az bir yazıcı

### Derleme ve çalıştırma

```powershell
git clone https://github.com/AhmetYILDIRIMuyum/SBPS.git
cd SBPS
dotnet restore .\SBPS.sln
dotnet build .\SBPS.sln -c Debug
dotnet run --project .\src\SBPS\SBPS.csproj
```

Release derlemesi:

```powershell
dotnet build .\SBPS.sln -c Release
```

## Kullanım

1. Klasör yolunu yazın veya klasör seçin.
2. Gerekirse alt klasörleri etkinleştirip listeyi yenileyin.
3. Dahil edilecek dosya tiplerini seçin. PDF bulunursa başlangıçta seçili gelir.
4. Dosya satırlarındaki seçim kutularıyla kuyruğu düzenleyin.
5. Sayfa ve kuyruk istatistiklerini kontrol edin.
6. Yazıcıyı; çift taraf, ayırıcı sayfa ve gelişmiş ayar seçeneklerini belirleyin.
7. **Yazıcıya Gönder** düğmesine basın ve onay penceresini kabul edin.
8. Bölümlü rehber için **Yardım**, uygulamayı kapatmak için **Çıkış** düğmesini kullanın.

İlk dosyadan önce ve son dosyadan sonra ayırıcı eklenmez. *N* dosya için en fazla *N − 1* ayırıcı sayfa oluşturulur.

## Teknoloji

- C# ve .NET 9
- Windows Forms
- Belge görüntüleme ve yazdırma için Microsoft Edge WebView2
- `System.Drawing.Printing` üzerinden Windows yazdırma API'leri
- PDF sayfa sayısı için en iyi çaba yaklaşımıyla Windows Property System yedeği

Tek NuGet bağımlılığı `Microsoft.Web.WebView2` paketidir.

## Yapılandırma ve Veriler

Yazıcılar Windows'tan okunur. Nihai çift taraf davranışı ve desteklenen seçenekler yazıcı sürücüsüne bağlıdır.

Mevcut sürüm, ekran tercihlerini oturumlar arasında saklamaz. Ayırıcı sayfa ve çift taraf seçeneği açık; **Önce ayarları göster** seçeneği kapalı başlar. Önceki kurulumlarla geriye dönük uyumluluk için günlükler `%LocalAppData%\FilePrintStudio\Logs` altında tutulmaya devam eder.

## Sınırlamalar

- Sayfa sayısı fiziksel yaprak sayısı garantisi değildir.
- Metin ve HTML sayfalaması yazdırma motoruna, kâğıda ve yazıcı ayarlarına bağlıdır.
- Şifreli, bozuk, alışılmadık biçimde sıkıştırılmış veya çok büyük PDF'lerde sayfa sayısı hesaplanamayabilir.
- Ayrı baskı işleri nedeniyle sürücülerin çift taraf ve boş sayfa sınırlarını nasıl ele aldığı garanti edilemez.
- Windows ve WebView2 Runtime gereklidir.

## Lisans

Kaynak kodun herkese açık görüntülenmesi, projeyi açık kaynak yapmaz. Önceden yazılı izin olmadan kullanım, kopyalama, değiştirme, dağıtma, alt lisanslama veya satış hakkı verilmez. Ayrıntılar için [LICENSE](LICENSE) dosyasına bakın.

## Yazar

**Ahmet YILDIRIM [dede]**  
[GitHub](https://github.com/AhmetYILDIRIMuyum) · [ahmetyildirimuyum@gmail.com](mailto:ahmetyildirimuyum@gmail.com)

## Katkı ve Durum

Herkese açık bir katkı politikası belirlenmemiştir; katkı hazırlamadan önce yazarla iletişim kurun. SBPS aktif geliştirme aşamasındadır ve bu ifade üretim ortamı veya destek garantisi oluşturmaz.
