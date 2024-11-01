-- This script was generated by the ERD tool in pgAdmin 4.
-- Please log an issue at https://github.com/pgadmin-org/pgadmin4/issues/new/choose if you find any bugs, including reproduction steps.
-- @block
BEGIN;

CREATE TYPE status_transaski_penyewaan AS ENUM ('PENDING', 'PROCESS', 'FAILED', 'DONE');
CREATE TYPE status_koleksi AS ENUM ('KOLEKSI', 'OPEN_FOR_TUKAR', 'DISEWAKAN', 'PRIVATE', 'LAGI_DISEWAKAN');
CREATE TYPE status_transaski_penukaran AS ENUM ('PENDING', 'PROCESS', 'RENT', 'FAILED', 'RETURNED');



CREATE TABLE buku (
    id INT PRIMARY KEY not null unique,
    id_koleksi INT  not null ,
    isbn TEXT  not null ,
    judul TEXT  not null ,
    penerbit TEXT  not null ,
    deskripsi TEXT,
    tahun_terbit INT,
    gambar TEXT,
    harga NUMERIC(10, 2),
    created DATE  not null ,
    last_updated DATE not null
);

-- penulis
CREATE TABLE penulis (
    id INT PRIMARY KEY not null unique,
    nama TEXT  not null,
    last_updated DATE  not null 
);

-- genre
CREATE TABLE genre (
    id INT PRIMARY KEY not null unique,
    nama TEXT  not null 
);

-- buku_ditulis
CREATE TABLE buku_ditulis (
    id INT PRIMARY KEY not null unique,
    id_buku INT not null ,
    id_penulis INT not null ,
    FOREIGN KEY (id_buku) REFERENCES buku(id),
    FOREIGN KEY (id_penulis) REFERENCES penulis(id)
);

-- genre_buku
CREATE TABLE genre_buku (
    id INT PRIMARY KEY not null unique,
    id_buku INT not null ,
    id_genre INT not null ,
    FOREIGN KEY (id_buku) REFERENCES buku(id),
    FOREIGN KEY (id_genre) REFERENCES genre(id)
);


CREATE TABLE IF NOT EXISTS public.koleksi
(
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),
    id_buku integer NOT NULL,
    id_user integer NOT NULL,
    nama text COLLATE pg_catalog."default" NOT NULL,
    lama_sewa integer NOT NULL,
    harga numeric NOT NULL,
    status status_koleksi NOT NULL DEFAULT 'KOLEKSI',
    created timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
    last_update timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT koleksi_pkey PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS public.user
(
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),
    nama text COLLATE pg_catalog."default" NOT NULL,
    deskripsi text COLLATE pg_catalog."default",
    password text COLLATE pg_catalog."default" NOT NULL,
    kota text COLLATE pg_catalog."default" NOT NULL,
    provinsi text COLLATE pg_catalog."default" NOT NULL,
    alamat_jalan text COLLATE pg_catalog."default" NOT NULL,
    kecamatan text COLLATE pg_catalog."default" NOT NULL,
    nomor_kontak text COLLATE pg_catalog."default" NOT NULL,
    gambar_profil text COLLATE pg_catalog."default",
    created timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
    last_update timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT user_pkey PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS public.wishlist
(
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),
    pembeli integer NOT NULL,
    id_koleksi integer NOT NULL,
    created timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT wishlist_pkey PRIMARY KEY (id)
);


CREATE TABLE IF NOT EXISTS public.transaksi_penukaran
(
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY,
    pembeli_id integer NOT NULL,
    penjual_id integer NOT NULL,
    is_penjual_konfirmasi boolean NOT NULL DEFAULT false,
    is_penjual_menerima boolean NOT NULL DEFAULT false,
    is_pembeli_konfirmasi boolean DEFAULT false,
    status status_transaski_penukaran NOT NULL DEFAULT 'PENDING',
    date_konfirmasi_penjual timestamp with time zone DEFAULT NULL,
    date_konfirmasi_pembeli_menerima timestamp with time zone DEFAULT NULL,
    date_transaksi_selesai timestamp with time zone DEFAULT NULL,
    last_updated timestamp with time zone NOT NULL DEFAULT NOW(),
    created timestamp with time zone NOT NULL DEFAULT NOW(),
    PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS public.transaksi_penyewaan
(
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY,
    penjual_id integer NOT NULL,
    penyewa_id integer NOT NULL,
    is_penjual_konfirmasi boolean NOT NULL DEFAULT FALSE,
    is_penyewa_menerima boolean NOT NULL DEFAULT FALSE,
    is_penjual_kembali boolean NOT NULL DEFAULT FALSE,
    is_penyewa_mengembalikan boolean NOT NULL DEFAULT FALSE,
    is_sudah_bayar boolean NOT NULL DEFAULT FALSE,
    status status_transaski_penyewaan NOT NULL DEFAULT 'PENDING',
    nomor_pembayaran integer DEFAULT NULL,
    deadline_pembayaran timestamp with time zone DEFAULT NULL,
    date_pembayaran_berhasil timestamp with time zone DEFAULT NULL,
    date_transaksi_selesai timestamp with time zone DEFAULT NULL,
    date_penyewa_menerima timestamp with time zone,
    date_penjual_mengirim timestamp with time zone,
    date_buku_kembali_penjual timestamp with time zone,
    deadline_pengembalian timestamp with time zone,
    last_updated timestamp with time zone NOT NULL DEFAULT NOW(),
    created timestamp with time zone NOT NULL DEFAULT NOW(),
    date_konfirmasi_penjual_deal timestamp with time zone,
    date_penyewa_kirim_balik timestamp with time zone,
    PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS public.koleksi_penukaran
(
    id_koleksi integer NOT NULL,
    id_transaksi integer NOT NULL,
    PRIMARY KEY (id_koleksi, id_transaksi)
);

CREATE TABLE IF NOT EXISTS public.koleksi_penyewaan
(
    id_koleksi integer NOT NULL,
    id_transaksi integer NOT NULL,
    PRIMARY KEY (id_koleksi, id_transaksi)
);

ALTER TABLE buku
    ADD CONSTRAINT fk_buku
    FOREIGN KEY (id_koleksi) REFERENCES koleksi(id);

ALTER TABLE IF EXISTS public.transaksi_penukaran
    ADD FOREIGN KEY (pembeli_id)
    REFERENCES public.user (id) MATCH SIMPLE
    ON UPDATE CASCADE
    ON DELETE CASCADE
    NOT VALID;


ALTER TABLE IF EXISTS public.transaksi_penukaran
    ADD FOREIGN KEY (penjual_id)
    REFERENCES public.user (id) MATCH SIMPLE
    ON UPDATE CASCADE
    ON DELETE CASCADE
    NOT VALID;


ALTER TABLE IF EXISTS public.transaksi_penyewaan
    ADD FOREIGN KEY (penjual_id)
    REFERENCES public.user (id) MATCH SIMPLE
    ON UPDATE CASCADE
    ON DELETE CASCADE
    NOT VALID;


ALTER TABLE IF EXISTS public.transaksi_penyewaan
    ADD FOREIGN KEY (penyewa_id)
    REFERENCES public.user (id) MATCH SIMPLE
    ON UPDATE CASCADE
    ON DELETE CASCADE
    NOT VALID;


ALTER TABLE IF EXISTS public.koleksi_penukaran
    ADD FOREIGN KEY (id_koleksi)
    REFERENCES public.koleksi (id) MATCH SIMPLE
    ON UPDATE CASCADE
    ON DELETE CASCADE
    NOT VALID;


ALTER TABLE IF EXISTS public.koleksi_penukaran
    ADD FOREIGN KEY (id_transaksi)
    REFERENCES public.transaksi_penukaran (id) MATCH SIMPLE
    ON UPDATE CASCADE
    ON DELETE CASCADE
    NOT VALID;


ALTER TABLE IF EXISTS public.koleksi_penyewaan
    ADD FOREIGN KEY (id_koleksi)
    REFERENCES public.koleksi (id) MATCH SIMPLE
    ON UPDATE CASCADE
    ON DELETE CASCADE
    NOT VALID;


ALTER TABLE IF EXISTS public.koleksi_penyewaan
    ADD FOREIGN KEY (id_transaksi)
    REFERENCES public.transaksi_penyewaan (id) MATCH SIMPLE
    ON UPDATE CASCADE
    ON DELETE CASCADE
    NOT VALID;

ALTER TABLE IF EXISTS public.koleksi
    ADD CONSTRAINT id_buku FOREIGN KEY (id_buku)
    REFERENCES public.buku (id) MATCH SIMPLE
    ON UPDATE CASCADE
    ON DELETE CASCADE;


ALTER TABLE IF EXISTS public.koleksi
    ADD CONSTRAINT id_user FOREIGN KEY (id_user)
    REFERENCES public.user (id) MATCH SIMPLE
    ON UPDATE CASCADE
    ON DELETE CASCADE;


ALTER TABLE IF EXISTS public.wishlist
    ADD CONSTRAINT id_koleksi FOREIGN KEY (id_koleksi)
    REFERENCES public.koleksi (id) MATCH SIMPLE
    ON UPDATE CASCADE
    ON DELETE CASCADE;


ALTER TABLE IF EXISTS public.wishlist
    ADD CONSTRAINT pembeli FOREIGN KEY (pembeli)
    REFERENCES public.user (id) MATCH SIMPLE
    ON UPDATE CASCADE
    ON DELETE CASCADE;

-- @block
SELECT * FROM buku;

-- @block
COMMIT;

-- @block
ROLLBACK;

-- @block
END;