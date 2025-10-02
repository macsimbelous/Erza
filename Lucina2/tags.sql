-- Table: public.tags

-- DROP TABLE public.tags;

CREATE TABLE public.tags
(
    tag_id bigserial,
    type bigint NOT NULL,
    count bigint NOT NULL,
	name_type character varying COLLATE pg_catalog."default" NOT NULL,
	language character varying COLLATE pg_catalog."default" NOT NULL,
    site character varying COLLATE pg_catalog."default" NOT NULL,
    tag character varying COLLATE pg_catalog."default" NOT NULL,
    parents character varying COLLATE pg_catalog."default",
    children character varying COLLATE pg_catalog."default",
    CONSTRAINT tags_pkey PRIMARY KEY (tag_id)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public.tags
    OWNER to erza;

-- Index: tag_index

-- DROP INDEX public.tag_index;

CREATE INDEX tag_index
    ON public.tags USING btree
    (tag_id, tag COLLATE pg_catalog."default")
    TABLESPACE pg_default;