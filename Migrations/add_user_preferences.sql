-- ============================================
-- 用戶偏好設定表 - 儲存每位使用者的 widget 開關與佈局設定
-- PostgreSQL
-- 樂觀鎖機制：透過 version 欄位防止並發更新衝突
-- ============================================

CREATE TABLE IF NOT EXISTS mf_user_preferences (
    id              SERIAL PRIMARY KEY,
    user_id         VARCHAR(200)    NOT NULL,
    -- widget 開關狀態，格式: ["demo_provider::Order", "demo_provider::Display"]
    enabled_widgets JSONB           NOT NULL DEFAULT '[]',
    -- grid 佈局設定，格式: [{"i":"remote_service/Order","x":0,"y":0,"w":4,"h":4,...}]
    layout_data     JSONB           NOT NULL DEFAULT '[]',
    -- 樂觀鎖版本號：每次成功 UPDATE 後遞增，
    -- UPDATE 時加上 WHERE version = :expectedVersion，若影響 0 rows 代表已被其他請求修改 → 409
    version         INT             NOT NULL DEFAULT 0,
    created_at      TIMESTAMPTZ     NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMPTZ     NOT NULL DEFAULT NOW(),

    CONSTRAINT uq_user_preferences_user_id UNIQUE (user_id)
);

-- 索引
CREATE INDEX IF NOT EXISTS idx_user_preferences_user_id ON mf_user_preferences(user_id);
