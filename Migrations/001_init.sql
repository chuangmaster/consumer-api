-- ============================================
-- MIB 模組註冊表 - 資料庫初始化
-- PostgreSQL
-- ============================================

CREATE TABLE IF NOT EXISTS mf_remote_modules (
    id                  SERIAL PRIMARY KEY,
    module_name         VARCHAR(100) NOT NULL,
    required_claim      VARCHAR(200) NOT NULL,
    entry_url           VARCHAR(500) NOT NULL,
    exposed_component   VARCHAR(200) NOT NULL,
    display_name        VARCHAR(200) NOT NULL,
    component_type      VARCHAR(20)  NOT NULL DEFAULT 'widget',  -- 'widget' | 'menu'
    route_path          VARCHAR(200),                             -- 僅 menu 類型使用
    environment         VARCHAR(50)  NOT NULL DEFAULT 'Development',
    sort_order          INT          NOT NULL DEFAULT 0,
    is_active           BOOLEAN      NOT NULL DEFAULT TRUE,
    created_at          TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    updated_at          TIMESTAMPTZ  NOT NULL DEFAULT NOW(),

    CONSTRAINT chk_component_type CHECK (component_type IN ('widget', 'menu', 'overlay')),
    CONSTRAINT chk_menu_route CHECK (
        (component_type = 'menu' AND route_path IS NOT NULL)
        OR component_type IN ('widget', 'overlay')
    )
);

-- Widget 響應式佈局配置 (僅 component_type='widget' 需要)
-- 每個 widget 針對 lg/md/sm 三組斷點提供不同預設尺寸
CREATE TABLE IF NOT EXISTS mf_widget_layouts (
    id              SERIAL PRIMARY KEY,
    module_id       INT         NOT NULL REFERENCES mf_remote_modules(id) ON DELETE CASCADE,
    breakpoint      VARCHAR(10) NOT NULL,  -- 'lg' (≥1200px) | 'md' (≥768px) | 'sm' (<768px)
    w               INT NOT NULL,
    h               INT NOT NULL,
    x               INT NOT NULL DEFAULT 0,
    y               INT NOT NULL DEFAULT 0,
    min_w           INT NOT NULL DEFAULT 2,
    min_h           INT NOT NULL DEFAULT 2,
    max_w           INT NOT NULL DEFAULT 12,
    max_h           INT NOT NULL DEFAULT 12,

    CONSTRAINT chk_breakpoint CHECK (breakpoint IN ('lg', 'md', 'sm')),
    UNIQUE (module_id, breakpoint)
);

-- 索引
CREATE INDEX IF NOT EXISTS idx_modules_environment ON mf_remote_modules(environment);
CREATE INDEX IF NOT EXISTS idx_modules_type ON mf_remote_modules(component_type);
CREATE INDEX IF NOT EXISTS idx_modules_active ON mf_remote_modules(is_active);
