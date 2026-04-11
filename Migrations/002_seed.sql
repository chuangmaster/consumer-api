-- ============================================
-- MIB 模組註冊表 - Seed Data
-- 對應 demo-provider 的 exposes 設定
-- ============================================

-- 1. 模組註冊
INSERT INTO mf_remote_modules (module_name, required_claim, entry_url, exposed_component, display_name, component_type, route_path, environment, sort_order)
VALUES
    -- Widget: 訂購系統
    ('remote_service', 'transport:portal:access', 'http://localhost:3001/mf-manifest.json',
     './Order', '訂購系統', 'widget', NULL, 'Development', 1),

    -- Widget: 統計報表 (需 admin 權限)
    ('remote_service', 'transport:portal:admin', 'http://localhost:3001/mf-manifest.json',
     './Display', '統計報表', 'widget', NULL, 'Development', 2),

    -- Menu: 每週菜單 (獨立路由頁面)
    ('remote_service', 'bento:portal:access', 'http://localhost:3001/mf-manifest.json',
     './WeeklyMenu', '每週菜單', 'menu', '/weekly-menu', 'Development', 3)
ON CONFLICT DO NOTHING;

-- 2. Widget 響應式佈局配置
-- Order Widget
INSERT INTO mf_widget_layouts (module_id, breakpoint, w, h, x, y, min_w, min_h, max_w, max_h)
SELECT id, 'lg', 4, 4, 0, 0, 2, 2, 12, 8 FROM mf_remote_modules WHERE exposed_component = './Order' AND environment = 'Development';
INSERT INTO mf_widget_layouts (module_id, breakpoint, w, h, x, y, min_w, min_h, max_w, max_h)
SELECT id, 'md', 6, 4, 0, 0, 3, 2, 12, 8 FROM mf_remote_modules WHERE exposed_component = './Order' AND environment = 'Development';
INSERT INTO mf_widget_layouts (module_id, breakpoint, w, h, x, y, min_w, min_h, max_w, max_h)
SELECT id, 'sm', 12, 3, 0, 0, 12, 2, 12, 6 FROM mf_remote_modules WHERE exposed_component = './Order' AND environment = 'Development';

-- Display Widget
INSERT INTO mf_widget_layouts (module_id, breakpoint, w, h, x, y, min_w, min_h, max_w, max_h)
SELECT id, 'lg', 4, 4, 4, 0, 2, 2, 12, 8 FROM mf_remote_modules WHERE exposed_component = './Display' AND environment = 'Development';
INSERT INTO mf_widget_layouts (module_id, breakpoint, w, h, x, y, min_w, min_h, max_w, max_h)
SELECT id, 'md', 6, 4, 0, 4, 3, 2, 12, 8 FROM mf_remote_modules WHERE exposed_component = './Display' AND environment = 'Development';
INSERT INTO mf_widget_layouts (module_id, breakpoint, w, h, x, y, min_w, min_h, max_w, max_h)
SELECT id, 'sm', 12, 3, 0, 3, 12, 2, 12, 6 FROM mf_remote_modules WHERE exposed_component = './Display' AND environment = 'Development';
