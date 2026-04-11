-- Migration: 將 section_titles 和 custom_sections 合併成統一的 sections 欄位
-- sections JSONB 陣列格式: [{"id":"...","title":"...","isCustom":true}]
-- Provider 改名覆蓋只存 {"id":"...","title":"..."} (不含 isCustom)
-- 自訂 Section 存 {"id":"...","title":"...","isCustom":true}

ALTER TABLE mf_user_preferences
    ADD COLUMN IF NOT EXISTS sections JSONB NOT NULL DEFAULT '[]';

-- 將現有資料從舊欄位遷移到新欄位
UPDATE mf_user_preferences
SET sections = (
    SELECT COALESCE(jsonb_agg(entry), '[]'::jsonb)
    FROM (
        -- Provider section 改名覆蓋（section_titles: {"id": "新名稱"}）
        SELECT jsonb_build_object('id', key, 'title', value) AS entry
        FROM jsonb_each_text(section_titles)

        UNION ALL

        -- 自訂 Section（custom_sections: [{"id":"...","title":"..."}]）加上 isCustom: true
        SELECT jsonb_set(elem, '{isCustom}', 'true'::jsonb) AS entry
        FROM jsonb_array_elements(custom_sections) AS elem
    ) combined
)
WHERE section_titles != '{}'::jsonb
   OR custom_sections != '[]'::jsonb;

-- 刪除舊欄位
ALTER TABLE mf_user_preferences
    DROP COLUMN IF EXISTS section_titles,
    DROP COLUMN IF EXISTS custom_sections;
