using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelEditor
{
    public class Grid : MonoBehaviour
    {
        // key
        public string key;
        // 资源Id
        public string resId;
        // 所在地块
        public int areaIndex;
        // 位置
        public Vector3 position;
        // 位置偏移
        public Vector3 positionOffset;
        // 旋转角度
        public Vector3 angle;
        // 角度偏移
        public Vector3 angleOffset;
        // 材料类型
        public MaterialType materialType;

        // box line
        private LEBoxLine m_boxLine;

        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialize()
        {
            DrawBoxLineView(false);
        }

        /// <summary>
        /// 画 box line
        /// </summary>
        /// <param name="drawing"></param>
        /// <param name="color"></param>
        public void DrawBoxLineView(bool drawing, Color color)
        {
            if (m_boxLine == null)
            {
                m_boxLine = gameObject.GetComponent<LEBoxLine>();
                if(m_boxLine == null)
                {
                    m_boxLine = gameObject.AddComponent<LEBoxLine>();
                }
            }

            m_boxLine.Draw(drawing);

            if (drawing)
            {
                m_boxLine.SetColor(color);
            }
        }

        /// <summary>
        /// 画 box line
        /// </summary>
        /// <param name="drawing"></param>
        public void DrawBoxLineView(bool drawing)
        {
            if (m_boxLine == null)
            {
                m_boxLine = gameObject.GetComponent<LEBoxLine>();
                if(m_boxLine == null)
                {
                    m_boxLine = gameObject.AddComponent<LEBoxLine>();
                }
            }
            
            m_boxLine.Draw(drawing);
        }
    }
}
