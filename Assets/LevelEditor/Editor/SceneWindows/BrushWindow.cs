/*
 * @Author: fasthro
 * @Date: 2019-05-27 14:48:51
 * @Description: 笔刷操作窗口
 */
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LevelEditor
{
    public class BrushWindow : AbstractSceneWindow, ISceneWindow
    {
        // 位置偏移
        private Vector3 m_positionOffset;
        // 角度偏移
        private Vector3 m_angleOffset;

        // 笔刷工具
        private BrushTools m_brushTools;

        protected override void OnInitialize()
        {
            title = "Brush Setting";
            w = 180;
            h = 105;
            m_positionOffset = Vector3.zero;
            m_angleOffset = Vector3.zero;

            m_brushTools = LevelEditorWindow.Inst.sceneWindow.brushTools;
        }

        public void OnSceneGUI(SceneView sceneView)
        {
            if (!isShow) return;
            if(m_brushTools.gird == null) return;

            x = sceneView.position.width - w - 2;
            y = sceneView.position.height - h - 5;
            GUI.Window(1, new Rect(x, y, w, h), OnGUI, title);
        }

        protected override void OnGUI(int id)
        {
            EditorGUILayout.BeginVertical("box");
            m_positionOffset = EditorGUILayout.Vector3Field("Position Offset", m_positionOffset);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("box");
            m_angleOffset = EditorGUILayout.Vector3Field("Angle Offset", m_angleOffset);
            EditorGUILayout.EndVertical();

            // 设置笔刷
            m_brushTools.positionOffset = m_positionOffset;
            m_brushTools.angleOffset = m_angleOffset;
        }
    }
}
