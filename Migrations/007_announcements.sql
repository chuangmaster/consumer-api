-- ============================================
-- 公告型 CMS - 資料庫 Migration
-- PostgreSQL
-- ============================================

CREATE TABLE IF NOT EXISTS announcements (
    id          SERIAL PRIMARY KEY,
    title       VARCHAR(200)  NOT NULL,
    content     TEXT          NOT NULL,
    starts_at   TIMESTAMPTZ   NOT NULL,
    ends_at     TIMESTAMPTZ   NOT NULL,
    is_active   BOOLEAN       NOT NULL DEFAULT TRUE,
    created_by  VARCHAR(100)  NOT NULL,
    created_at  TIMESTAMPTZ   NOT NULL DEFAULT NOW(),
    updated_at  TIMESTAMPTZ   NOT NULL DEFAULT NOW(),

    CONSTRAINT chk_announcement_dates CHECK (ends_at > starts_at)
);

CREATE INDEX IF NOT EXISTS idx_announcements_active_dates
    ON announcements (is_active, starts_at, ends_at);

CREATE INDEX IF NOT EXISTS idx_announcements_created_by
    ON announcements (created_by);
