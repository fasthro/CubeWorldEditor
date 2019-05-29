/*
 * @Author: fasthro
 * @Date: 2019-05-28 12:03:31
 * @Description: 场景主窗口界面
 */
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LevelEditor
{
    // 切换工具事件
    public delegate void SwitchToolsEvent(SceneToolsType stt);

    public class SceneWindow : AbstractSceneWindow, ISceneWindow
    {
        // 格子模版
        private TemplateGrid m_templateGrid;
        public TemplateGrid templateGrid { get { return m_templateGrid; } }

        // 当前选择的工具类型
        private SceneToolsType m_sceneToolType;
        public SceneToolsType sceneToolType { get { return m_sceneToolType; } }

        // 场景工具-选择器
        private SelectorTools m_selectorTools;
        public SelectorTools selectorTools { get { return m_selectorTools; } }

        // 场景工具-笔刷
        private BrushTools m_brushTools;
        public BrushTools brushTools { get { return m_brushTools; } }

        // 场景工具-吸管
        private SuckerTools m_suckerTools;
        public SuckerTools suckerTools { get { return m_suckerTools; } }

        // 场景工具-擦除
        private EraseTools m_eraseTools;
        public EraseTools eraseTools { get { return m_eraseTools; } }

        // 场景窗口-笔刷
        private BrushWindow m_brushWindow;
        public BrushWindow brushWindow { get { return m_brushWindow; } }

        // 场景窗口-选择器
        private SelectorWindow m_selecterWindow;
        public SelectorWindow selectorWindow { get { return m_selecterWindow; } }

        // 切换工具事件
        public SwitchToolsEvent switchToolsEventHandler;

        // content
        private GUIContent m_content;
        // rect
        private Rect m_rect;
        // controlId
        private int m_controlId;

        protected override void OnInitialize()
        {
            // 创建工具
            if (m_selectorTools == null) m_selectorTools = new SelectorTools();
            if (m_brushTools == null) m_brushTools = new BrushTools();
            if (m_suckerTools == null) m_suckerTools = new SuckerTools();
            if (m_eraseTools == null) m_eraseTools = new EraseTools();

            // 工具事件注册
            m_selectorTools.toolsEventHandler -= OnSelectorToolsEventHandler;
            m_selectorTools.toolsEventHandler += OnSelectorToolsEventHandler;

            m_brushTools.toolsEventHandler -= OnBrushToolsEventHandler;
            m_brushTools.toolsEventHandler += OnBrushToolsEventHandler;

            m_suckerTools.toolsEventHandler -= OnSuckerToolsEventHandler;
            m_suckerTools.toolsEventHandler += OnSuckerToolsEventHandler;

            m_eraseTools.toolsEventHandler -= OnEraseToolsEventHandler;
            m_eraseTools.toolsEventHandler += OnEraseToolsEventHandler;

            // 创建窗口
            if (m_brushWindow == null) m_brushWindow = new BrushWindow();
            if (m_selecterWindow == null) m_selecterWindow = new SelectorWindow();

            // 模版格子
            if (m_templateGrid == null) m_templateGrid = GameObject.Find(typeof(TemplateGrid).Name).gameObject.GetComponent<TemplateGrid>();
            // 注册场景事件
            m_templateGrid.SceneRenderHandler -= OnSceneRender;
            m_templateGrid.SceneRenderHandler += OnSceneRender;
        }

        protected override void OnShowWindow()
        {
            // 设置网格模版
            templateGrid.width = LevelEditorWindow.Inst.templateGridSize.x;
            templateGrid.lenght = LevelEditorWindow.Inst.templateGridSize.y;
            templateGrid.ShowView();

            // 重置工具
            SwitchTools(SceneToolsType.None);
        }

        protected override void OnCloseWindow()
        {
            // 关闭网格模版
            templateGrid.CloseView();

            // 关闭工具事件
            m_selectorTools.Close();
            m_brushTools.Close();
            m_suckerTools.Close();
            m_eraseTools.Close();

            // 关闭场景窗口
            brushWindow.CloseWindow();
            selectorWindow.CloseWindow();
        }

        /// <summary>
        /// 切换场景工具
        /// </summary>
        public void SwitchTools(SceneToolsType stt)
        {
            if (stt == m_sceneToolType) return;

            m_sceneToolType = stt;

            if (stt != SceneToolsType.Selector)
            {
                selectorTools.Close();
                selectorWindow.CloseWindow();
            }
            if (stt != SceneToolsType.Brush)
            {
                brushTools.Close();
                brushWindow.CloseWindow();
            }
            if (stt != SceneToolsType.Sucker) suckerTools.Close();
            if (stt != SceneToolsType.Erase) eraseTools.Close();


            if (stt == SceneToolsType.Selector)
            {
                selectorTools.Open();
                selectorWindow.ShowWindow();
            }
            if (stt == SceneToolsType.Brush)
            {
                brushTools.Open();
                brushWindow.ShowWindow();
            }
            if (stt == SceneToolsType.Sucker) suckerTools.Open();
            if (stt == SceneToolsType.Erase) eraseTools.Open();

            if (switchToolsEventHandler != null) switchToolsEventHandler(stt);
        }

        public void OnSceneGUI(SceneView sceneView)
        {
            if (!isShow) return;

            // 网格模版更新
            templateGrid.OnSceneGUI(sceneView);

            m_controlId = GUIUtility.GetControlID(FocusType.Passive);

            Handles.BeginGUI();

            GUI.skin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene);

            int nextInterval = SettingManager.SceneToolSize + SettingManager.SceneTooIInterval;
            float rightX = sceneView.position.width - SettingManager.SceneToolSize - 5;
            float downY = sceneView.position.height - SettingManager.SceneToolSize - 25;

            #region  左测

            // 选择工具
            m_content = new GUIContent(ResManager.Inst.GetIconTexture("iconCursor"));
            m_rect = new Rect(SettingManager.SceneTooIX, SettingManager.SceneTooIY, SettingManager.SceneToolSize, SettingManager.SceneToolSize);
            if (GUI.Toggle(m_rect, m_sceneToolType == SceneToolsType.Selector, m_content, GUI.skin.button)) SwitchTools(SceneToolsType.Selector);

            // 笔刷工具
            m_content = new GUIContent(ResManager.Inst.GetIconTexture("iconBlockMode"));
            m_rect = new Rect(SettingManager.SceneTooIX, SettingManager.SceneTooIY + nextInterval, SettingManager.SceneToolSize, SettingManager.SceneToolSize);
            if (GUI.Toggle(m_rect, m_sceneToolType == SceneToolsType.Brush, m_content, GUI.skin.button)) SwitchTools(SceneToolsType.Brush);

            // 吸管工具
            m_content = new GUIContent(ResManager.Inst.GetIconTexture("iconPicker"));
            m_rect = new Rect(SettingManager.SceneTooIX, SettingManager.SceneTooIY + nextInterval * 2, SettingManager.SceneToolSize, SettingManager.SceneToolSize);
            if (GUI.Toggle(m_rect, m_sceneToolType == SceneToolsType.Sucker, m_content, GUI.skin.button)) SwitchTools(SceneToolsType.Sucker);

            // 擦除工具
            m_content = new GUIContent(ResManager.Inst.GetIconTexture("iconErase"));
            m_rect = new Rect(SettingManager.SceneTooIX, SettingManager.SceneTooIY + nextInterval * 3, SettingManager.SceneToolSize, SettingManager.SceneToolSize);
            if (GUI.Toggle(m_rect, m_sceneToolType == SceneToolsType.Erase, m_content, GUI.skin.button)) SwitchTools(SceneToolsType.Erase);

            #endregion

            #region 底部
            // Template Grid 背景
            m_content = new GUIContent(ResManager.Inst.GetIconTexture("iconIsolate"));
            m_rect = new Rect(SettingManager.SceneTooIX, downY, SettingManager.SceneToolSize, SettingManager.SceneToolSize);
            templateGrid.transparentEnabled = GUI.Toggle(m_rect, templateGrid.transparentEnabled, m_content, GUI.skin.button);

            // Template Grid 高度
            m_content = new GUIContent(ResManager.Inst.GetIconTexture("iconGridUp"));
            m_rect = new Rect(SettingManager.SceneTooIX + nextInterval, downY, SettingManager.SceneToolSize, SettingManager.SceneToolSize);
            if (GUI.Toggle(m_rect, false, m_content, GUI.skin.button))
            {
                m_sceneToolType = SceneToolsType.None;
                templateGrid.height++;
                LevelEditorWindow.Inst.Repaint();
            }

            // GizmoPanelDown
            m_content = new GUIContent(ResManager.Inst.GetIconTexture("iconGridDown"));
            m_rect = new Rect(SettingManager.SceneTooIX + nextInterval * 2, downY, SettingManager.SceneToolSize, SettingManager.SceneToolSize);
            if (GUI.Toggle(m_rect, false, m_content, GUI.skin.button))
            {
                m_sceneToolType = SceneToolsType.None;
                templateGrid.height--;
                LevelEditorWindow.Inst.Repaint();
            }

            // 上一区域
            m_content = new GUIContent(ResManager.Inst.GetIconTexture("iconArrowUp"));
            m_rect = new Rect(SettingManager.SceneTooIX + nextInterval * 3, downY, SettingManager.SceneToolSize, SettingManager.SceneToolSize / 2);
            if (GUI.Button(m_rect, m_content))
            {
                LevelEditorWindow.Inst.area++;
            }

            // 下一区域
            m_content = new GUIContent(ResManager.Inst.GetIconTexture("iconArrowDown"));
            m_rect = new Rect(SettingManager.SceneTooIX + nextInterval * 3, downY + SettingManager.SceneToolSize / 2, SettingManager.SceneToolSize, SettingManager.SceneToolSize / 2);
            if (GUI.Button(m_rect, m_content))
            {
                LevelEditorWindow.Inst.area--;
            }
            m_rect = new Rect(SettingManager.SceneTooIX + +nextInterval * 4, downY, 50, SettingManager.SceneToolSize);
            GUILayout.BeginArea(m_rect, EditorStyles.textArea);
            {
                GUILayout.BeginVertical();
                GUILayout.Label("Area", EditorStyles.label, GUILayout.Width(50));
                GUILayout.Label(LevelEditorWindow.Inst.area.ToString(), EditorStyles.label, GUILayout.Width(50));
                GUILayout.EndVertical();
            }
            GUILayout.EndArea();
            #endregion

            Handles.EndGUI();

            HandleUtility.AddDefaultControl(m_controlId);
        }

        private void OnSceneRender(SceneView sceneView, SceneRenderState state, Vector3 mousePosition)
        {
            switch (sceneToolType)
            {
                case SceneToolsType.Selector:
                    selectorTools.OnSceneRender(sceneView, state, mousePosition);
                    selectorWindow.OnSceneGUI(sceneView);
                    break;
                case SceneToolsType.Brush:
                    brushTools.OnSceneRender(sceneView, state, mousePosition);
                    brushWindow.OnSceneGUI(sceneView);
                    break;
                case SceneToolsType.Sucker:
                    suckerTools.OnSceneRender(sceneView, state, mousePosition);
                    break;
                case SceneToolsType.Erase:
                    eraseTools.OnSceneRender(sceneView, state, mousePosition);
                    break;
            }
        }

        // 选择器工具事件
        private void OnSelectorToolsEventHandler(Grid grid)
        {

        }

        // 笔刷工具事件
        private void OnBrushToolsEventHandler(EventType eventType, Grid grid)
        {
            if (Event.current.button == 0
            && Event.current.alt == false
            && Event.current.shift == false
            && Event.current.control == false
            && grid != null)
            {
                if (Event.current.type == EventType.MouseDrag || Event.current.type == EventType.MouseDown)
                {
                    Environment.Inst.DrawGrid(grid, LevelEditorWindow.Inst.area);
                }
            }
        }

        // 吸管工具事件
        private void OnSuckerToolsEventHandler(Vector3 mousePosition)
        {
            var grid = Environment.Inst.GetGrid(mousePosition, LevelEditorWindow.Inst.area);
            if (grid != null)
            {
                LevelEditorWindow.Inst.SetViewSelected(ResManager.Inst.GetResObject(grid.resId));
                SwitchTools(SceneToolsType.Brush);
            }
        }

        // 橡皮擦工具事件
        private void OnEraseToolsEventHandler(Vector3 mousePosition)
        {
            Environment.Inst.EraseGrid(mousePosition, LevelEditorWindow.Inst.area);
        }
    }
}

