# BePop Müzik Uygulaması — .NET 9.0 | ML.NET | JWT

M&Y Yazılım Akademi kapsamında geliştirdiğim bu proje; modern web teknolojilerini, güvenli kimlik doğrulamayı ve makine öğrenmesini bir arada kullanan, katmanlı mimariye sahip bir müzik platformudur. Eğitim sürecinde değerli katkılarını esirgemeyen **Murat Yücedağ**'a teşekkür ederim.

---

## Proje Hakkında

Uygulama; kullanıcıların paket seviyelerine göre şarkı ve sanatçılara erişebildiği, dinleme geçmişinin takip edildiği ve **ML.NET** ile kişiselleştirilmiş öneriler sunulan bir yapıya sahiptir. Tüm bu işlemler **JWT tabanlı güvenli oturum yönetimi** altında gerçekleşmektedir.

---

## Katman Yapısı

```
📁 WebAPI          → API controller'ları ve uygulama giriş noktası
📁 Business        → İş mantığı, servisler ve ML.NET model entegrasyonu  
📁 DataAccess      → EF Core repository'leri ve DbContext
📁 Core            → Entity'ler, DTO'lar ve arayüzler
```

---

## Öne Çıkan Özellikler

**Kimlik Doğrulama**
JSON Web Token kullanılarak kullanıcı kayıt, giriş ve çıkış süreçleri güvence altına alınmıştır. Token tabanlı yapı sayesinde her istek yetki kontrolünden geçer.

**Paket & Erişim Yönetimi**
Her kullanıcı bir pakete sahiptir ve yalnızca o pakete tanımlı içeriklere ulaşabilir. Bu yapı, abonelik bazlı bir iş modeli için sağlam bir zemin oluşturmaktadır.

**Şarkı & Sanatçı Yönetimi**
Şarkılar tür, sanatçı ve pakete göre filtrelenebilir. Her şarkı detay sayfasında ilgili meta veriler eksiksiz şekilde sunulur.

**Dinleme Geçmişi**
Kullanıcının her dinleme etkileşimi kayıt altına alınır. Bu veriler hem istatistik hem de ML modeli için kaynak oluşturur.

**ML.NET Öneri Motoru**
Kullanıcının biriktirdiği dinleme verileri ML.NET ile analiz edilerek, standart popülerlik sıralamalarından bağımsız, tamamen kişiye özel öneriler üretilir.

**Admin Paneli**
Yönetici ekranında paket, şarkı, kullanıcı ve dinleme geçmişi üzerinde tam CRUD yetkisi bulunmaktadır. Dashboard'da anlık istatistikler görüntülenir.

---

## Teknoloji Yığını

| Alan | Teknoloji |
|---|---|
| Backend | ASP.NET Core 9.0 Web API |
| ORM | Entity Framework Core |
| Veritabanı | Microsoft SQL Server |
| Kimlik Doğrulama | JWT (JSON Web Token) |
| Makine Öğrenmesi | ML.NET |
| Mimari | N-Katmanlı (N-Tier Architecture) |
| Frontend | HTML5, CSS3, JavaScript, Bootstrap |

---

## Ekran Görüntüleri

##UI

<img width="1920" height="1080" alt="Home1" src="https://github.com/user-attachments/assets/50dd86bf-999c-41cf-bb51-021bb9d2dfb4" />
<img width="1920" height="1080" alt="Home2" src="https://github.com/user-attachments/assets/9b0797c0-c119-4868-8978-200ab284f20a" />
<img width="1920" height="1080" alt="login" src="https://github.com/user-attachments/assets/ef16b55d-e9c3-4e2e-b4c6-be1874e2a433" />
<img width="1920" height="1080" alt="signUp" src="https://github.com/user-attachments/assets/02c466dd-3160-4b69-a222-c2329a50aca0" />
<img width="1920" height="1080" alt="Discover1" src="https://github.com/user-attachments/assets/ed5626cb-faaf-4c23-8f37-a604ccaac42b" />
<img width="1920" height="1080" alt="Discover2" src="https://github.com/user-attachments/assets/1a5bd4bc-16b9-416f-a072-c5ea93c0d708" />
<img width="1920" height="1080" alt="Artist1" src="https://github.com/user-attachments/assets/ca9a8edb-e90c-4150-b411-da05efe48f0a" />
<img width="1920" height="1080" alt="Artist2" src="https://github.com/user-attachments/assets/a4940207-d53a-4782-9cbb-83f10d52217b" />
<img width="1920" height="1080" alt="ArtistDetail1" src="https://github.com/user-attachments/assets/2308c606-6122-4d24-8bc0-2a82afaaff07" />
<img width="1920" height="1080" alt="ArtistDetail2" src="https://github.com/user-attachments/assets/658e7155-9e1f-499b-93e2-5b887a3dc481" />
<img width="1920" height="1080" alt="SongDetail" src="https://github.com/user-attachments/assets/58a73a7a-8872-47eb-8e35-e01c5bcf743d" />
<img width="1920" height="1080" alt="GenresAll" src="https://github.com/user-attachments/assets/71efdd66-a19b-41b2-93d1-bdf122903407" />
<img width="1920" height="1080" alt="MyPlaylist" src="https://github.com/user-attachments/assets/8c986a5a-9621-4598-8efc-917c32f2e187" />
<img width="1920" height="1080" alt="addPlaylist" src="https://github.com/user-attachments/assets/5c7b53d3-5ed2-4872-b110-5b9ae995cc99" />
<img width="1920" height="1080" alt="playListDetail" src="https://github.com/user-attachments/assets/38c2f553-c8c4-4736-96c3-55fb95dea784" />
<img width="1920" height="1080" alt="Profile" src="https://github.com/user-attachments/assets/0a18aecf-d00f-4ef9-bcc8-1a92d0366eca" />
<img width="1920" height="1080" alt="followers" src="https://github.com/user-attachments/assets/b53bf5c9-9c5e-426d-8f43-7454ec5d2a5f" />
<img width="1920" height="1080" alt="following" src="https://github.com/user-attachments/assets/6f41d294-c516-4744-bb05-7273172e3c89" />

## Admin Paneli

<img width="1920" height="1080" alt="adminDashboard" src="https://github.com/user-attachments/assets/ec9be44b-c4b4-443e-9806-88a562edce4b" />
<img width="1920" height="1080" alt="adminAlbum" src="https://github.com/user-attachments/assets/87b4b61d-265f-402e-815a-3872368ea218" />
<img width="1920" height="1080" alt="adminAlbumUpdate" src="https://github.com/user-attachments/assets/ba5d4779-f051-4e1c-857a-7bf89eb6e4bf" />
<img width="1920" height="1080" alt="adminSinger" src="https://github.com/user-attachments/assets/546ae598-69a7-4f13-81c8-dbf3889004b6" />
<img width="1920" height="1080" alt="adminSingerUpdate" src="https://github.com/user-attachments/assets/dd2583c1-cea0-40be-8cb1-dd1aaf609982" />
<img width="1920" height="1080" alt="adminSong" src="https://github.com/user-attachments/assets/5185963a-8614-4852-95c5-f97404054487" />
<img width="1920" height="1080" alt="adminSongUpdate" src="https://github.com/user-attachments/assets/aab79913-cfe4-4caa-b78b-94ede8e8d0f4" />
<img width="1920" height="1080" alt="adminUser" src="https://github.com/user-attachments/assets/3a1d8a83-fbd3-4ba3-9304-1e405d40e9e1" />
<img width="1920" height="1080" alt="adminUserUpdate" src="https://github.com/user-attachments/assets/78a28ddd-8a16-4ba5-9277-615f0319c89f" />
<img width="1920" height="1080" alt="adminGenres" src="https://github.com/user-attachments/assets/4f1438b3-f33e-4662-adc0-578a218acaf6" />
<img width="1920" height="1080" alt="adminPackage" src="https://github.com/user-attachments/assets/9c252cbc-3206-4096-a97a-6009ec751e5e" />

