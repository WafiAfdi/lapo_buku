ALTER TABLE buku
S id_pemilik INT;

-- @block
ALTER TABLE IF EXISTS public.buku
    ADD FOREIGN KEY (id_pemilik)
    REFERENCES public.user (id) MATCH SIMPLE
    ON UPDATE CASCADE
    ON DELETE CASCADE
    NOT VALID;


-- @block
SELECT * FROM public.user;
-- @block
UPDATE buku
SET id_pemilik = 2
WHERE true;