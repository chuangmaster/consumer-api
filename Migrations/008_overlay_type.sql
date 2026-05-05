-- ============================================
-- Migration 008: 新增 overlay component type
-- 支援右下角浮動 widget（不走 grid 容器）
-- 注意：001_init.sql 已含此變更，此檔僅供既有 DB 升級用
-- ============================================

ALTER TABLE mf_remote_modules
    DROP CONSTRAINT IF EXISTS chk_component_type;

ALTER TABLE mf_remote_modules
    ADD CONSTRAINT chk_component_type
    CHECK (component_type IN ('widget', 'menu', 'overlay'));
-- chk_menu_route 原本對間 widget/menu，一併更新支援 overlay
ALTER TABLE mf_remote_modules
    DROP CONSTRAINT IF EXISTS chk_menu_route;

ALTER TABLE mf_remote_modules
    ADD CONSTRAINT chk_menu_route CHECK (
        (component_type = 'menu' AND route_path IS NOT NULL)
        OR component_type IN ('widget', 'overlay')
    );
