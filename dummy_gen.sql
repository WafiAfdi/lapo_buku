-- @block 
INSERT INTO public.user (username, email, deskripsi, password, kota,provinsi,alamat_jalan,kecamatan,nomor_kontak) VALUES 
('abdul_hamid', 'abdulhamid@gmail.com', 'Aku menyukai buku yang tipenya light novel dan genrenya isekai', 'test', 'Sleman', 'Yogyakarta', 'Jl. Kemerdekaan C20', 'Condongcatur', '081232131321'),
('alice putri', 'alice-putri-24@gmail.com', 'Aku menyukai buku yang biasanya memiliki cerita romantis dan konflik cinta segitiga', 'test', 'Bandung', 'Jawa Barat', 'Jl. Bojongsoang A10', 'Bukit Bugis', '081232131321');


-- @block
INSERT INTO public.genre (nama) VALUES ('romantis'), ('horror'), ('petualangan');

-- @block
INSERT INTO public.penulis (nama) VALUES ('Hari Proton'), ('Edward Hartono'), ('Prastetu Wongosari'), ('Raden A. R. Pandjiwongso');

-- @block 
INSERT INTO public.buku (isbn, judul, penerbit, deskripsi, tahun_terbit, status, id_pemilik, rating_buku)
VALUES ('2311241321', 'Petualangan Harun', 'Citra Publication','Harun adalah sang petualang yang lagi menelusuri Keraajaan Majapahit', 2010, 'OPEN_FOR_TUKAR', 1, 80),
 ('3213123', 'Kehidupan Selepas SMA', 'Bulaksumur Publication','Kehidupan mudah seorang anak SMA pada saat dia menapak di kehidupan kuliah', 2020, 'OPEN_FOR_TUKAR', 2, 80),
 ('21314143', 'Rumah Hantu 98', 'Bulaksumur Publication','Rumah terbengkalai milik seorang korban pembunuhan 98', 2001, 'OPEN_FOR_TUKAR', 2, 80);


-- @block
INSERT INTO public.genre_buku (id_buku, id_genre) VALUES (2,1), (1,3), (3,2);

-- @block
INSERT INTO public.buku_ditulis (id_buku, id_penulis) VALUES (1,1), (1,2), (2,3), (3,4), (2,4);
