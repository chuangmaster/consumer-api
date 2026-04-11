-- Migration: 將 enabled_widgets 從 string[] 改為 WidgetEntryDto[]，並移除 widget_section_map 欄位
-- 新格式: [{"id":"demo_provider::Order","sectionId":"demo_provider"}, ...]
-- sectionId 由 id 的 "::" 前半部推導（即 provider id）

UPDATE mf_user_preferences
SET enabled_widgets = (
    SELECT COALESCE(jsonb_agg(
        jsonb_build_object(
            'id', widget_id,
            'sectionId', split_part(widget_id, '::', 1)
        )
    ), '[]'::jsonb)
    FROM jsonb_array_elements_text(enabled_widgets) AS widget_id
);

ALTER TABLE mf_user_preferences
    DROP COLUMN IF EXISTS widget_section_map;
