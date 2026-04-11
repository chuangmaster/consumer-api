-- ============================================
-- Section 自訂化功能：重命名、自訂新增/刪除、Widget 跨 Section 移動
-- 對 mf_user_preferences 新增三個 JSONB 欄位
-- ============================================

ALTER TABLE mf_user_preferences
    ADD COLUMN IF NOT EXISTS section_titles     JSONB NOT NULL DEFAULT '{}',
    ADD COLUMN IF NOT EXISTS custom_sections    JSONB NOT NULL DEFAULT '[]',
    ADD COLUMN IF NOT EXISTS widget_section_map JSONB NOT NULL DEFAULT '{}',
    ADD COLUMN IF NOT EXISTS section_order      JSONB NOT NULL DEFAULT '[]';

COMMENT ON COLUMN mf_user_preferences.section_titles IS
    'Section 自訂標題 override。格式: {"sectionId": "自訂標題", ...}';

COMMENT ON COLUMN mf_user_preferences.custom_sections IS
    '使用者自行建立的 Section 清單。格式: [{"id":"uuid","title":"名稱"}, ...]';

COMMENT ON COLUMN mf_user_preferences.widget_section_map IS
    'Widget 指派的 Section override。格式: {"remote_service/Order": "sectionId", ...}';
