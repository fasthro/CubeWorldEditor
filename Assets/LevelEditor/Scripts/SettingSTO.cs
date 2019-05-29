/*
 * @Author: fasthro
 * @Date: 2019-05-18 10:56:35
 * @Description: 设置参数
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelEditor
{
    [System.Serializable]
    public class SettingSTO : ScriptableObject
    {
        [Header("关卡场景保存路径")]
        public string ArtPath = "Assets/Art/Levels/Prefabs/";

        [Header("关卡场景保存路径")]
        public string LevelSceneSavePath = "Levels/Scenes/";

        [Header("cube offset")]
        public Vector3 CubeAnchorOffset = new Vector3(0, -0.5f, 0);
    }

}
