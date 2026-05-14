create table public.account (
  gdkey text primary key,
  ywp_user_tables jsonb,
  last_lgn_time text,
  opening_tutorial_flag boolean,
  start_date text,
  character_id text unique,
  user_id text
);

create table public.device (
  udkey text primary key,
  gdkeys text[] not null
);