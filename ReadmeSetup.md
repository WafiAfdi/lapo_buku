# Cara setup aplikasi dengan benar
Untuk dapat menjalankan aplikasi Lapo Buku, pertama pastikan bahwa postgresql telah diinstall pada OS. Jika sudah, maka pastikan sudah ada user dan database. Soalnya ada diperlukan appsettings.json yang mengarah pada database. appsettings.json akan berisi : 
````
    {
        "Host": "localhost",
        "Port": 5432,
        "Name": "Junpro",
        "User": "postgres",
        "Password": "password"
    }
````
Data di atas disesuaikan dengan databasenya masing - masing, jika tidak akan menyebabkan error pada aplikasi. Selanjutnya, pengguna harus jalankan script DDL yang disediakn dengan nama `ddl_revisi.sql` agar skema database terbentuk secara fisik.

Sebagai tambahan, ada tambahan jika ingin ada data pada awalnya dengan menjalankan beberapa command di bawah : 

 1. Dummy User
 Jalankan script untuk buat dua dummy user
````
      INSERT  INTO public.user (username, email, deskripsi, password, kota,provinsi,alamat_jalan,kecamatan,nomor_kontak) 
        VALUES
    ('abdul_hamid', 'abdulhamid@gmail.com', 'Aku menyukai buku yang tipenya light novel dan genrenya isekai', 'test', 'Sleman', 'Yogyakarta', 'Jl. Kemerdekaan C20', 'Condongcatur', '081232131321'), ('alice putri', 'alice-putri-24@gmail.com', 'Aku menyukai buku yang biasanya memiliki cerita romantis dan konflik cinta segitiga', 'test', 'Bandung', 'Jawa Barat', 'Jl. Bojongsoang A10', 'Bukit Bugis', '081232131321');
````




 2. Dummy Buku
 Jalankan script untuk buat dummy buku dan sesuaikan pemilik_id dengan id pada database postgresql
````
    INSERT  INTO public.buku (isbn, judul, penerbit, deskripsi, tahun_terbit, status, id_pemilik, rating_buku) VALUES ('2311241321', 'Petualangan Harun', 'Citra Publication','Harun adalah sang petualang yang lagi menelusuri Keraajaan Majapahit', 2010, 'OPEN_FOR_TUKAR', 1, 80), ('3213123', 'Kehidupan Selepas SMA', 'Bulaksumur Publication','Kehidupan mudah seorang anak SMA pada saat dia menapak di kehidupan kuliah', 2020, 'OPEN_FOR_TUKAR', 2, 80), ('21314143', 'Rumah Hantu 98', 'Bulaksumur Publication','Rumah terbengkalai milik seorang korban pembunuhan 98', 2001, 'OPEN_FOR_TUKAR', 2, 80);
````
 5. Dummy Genre
  Insert data genre sesuai dengan kemauan pengguna
  
````
    INSERT  INTO public.genre (nama) VALUES ('romantis'), ('horror'), ('petualangan');
````

 7. Dummy Penulis
 Insert data genre sesuai dengan kemauan pengguna
 ```` 
	INSERT  INTO public.penulis (nama) VALUES ('Hari Proton'), ('Edward Hartono'), ('Prastetu Wongosari'), ('Raden A. R. Pandjiwongso');
	
````

 8. Relasi
 Untuk data relasi sesuaikan dengan id yang dimiliki pada database 
````
-- @block

INSERT  INTO public.genre_buku (id_buku, id_genre) VALUES (2,1), (1,3), (3,2);

  

-- @block

INSERT  INTO public.buku_ditulis (id_buku, id_penulis) VALUES (1,1), (1,2), (2,3), (3,4), (2,4);
````

