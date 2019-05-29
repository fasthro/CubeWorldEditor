/*
 * @Author: fasthro
 * @Date: 2019-05-27 17:37:12
 * @Description: 选择器窗口
 */
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LevelEditor
{
    public class SelectorWindow : AbstractSceneWindow, ISceneWindow
    {
        // 位置偏移
        private Vector3 m_positionOffset;
        // 角度偏移
        private Vector3 m_angleOffset;

        // 选择器工具
        private SelectorTools m_selectorTools;

        protected override void OnInitialize()
        {
            title = "Selecter Setting";
            w = 180;
            h = 105;

            m_selectorTools = LevelEditorWindow.Inst.sceneWindow.selectorTools;
            if (m_selectorTools.selectGrid != null)
            {
                m_positionOffset = m_selectorTools.selectGrid.positionOffset;
                m_angleOffset = m_selectorTools.selectGrid.angleOffset;
            }
        }

        public void OnSceneGUI(SceneView sceneView)
        {
            if (!isShow) return;
            if (m_selectorTools.selectGrid == null) return;

            x = sceneView.position.width - w - 2;
            y = sceneView.position.height - h - 5;
            GUI.Window(1, new Rect(x, y, w, h), OnGUI, title);
        }

        protected override void OnGUI(int id)
        {
            EditorGUILayout.BeginVertical("box");
            m_positionOffset = EditorGUILayout.Vector3Field("Offset", m_positionOffset);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("box");
            m_angleOffset = EditorGUILayout.Vector3Field("Angle", m_angleOffset);
            EditorGUILayout.EndVertical();

            // 设置选择器选择的格子参数
            var gridTransform = m_selectorTools.selectGrid.transform;
            
            m_selectorTools.selectGrid.positionOffset = m_positionOffset;
            m_selectorTools.selectGrid.angleOffset = m_angleOffset;
            
            gridTransform.position = m_selectorTools.selectGrid.position + SettingManager.Inst.Setting.CubeAnchorOffset + m_positionOffset;
            gridTransform.localEulerAngles = m_selectorTools.selectGrid.angleOffset + m_angleOffset;
        }
    }
}
