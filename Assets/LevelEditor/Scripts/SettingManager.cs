using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LevelEditor
{

    public class SettingManager
    {
        // 单例模式引用
        private static SettingManager _inst;
        public static SettingManager Inst
        {
            get
            {
                if (_inst == null)
                {
                    _inst = new SettingManager();
                    _inst.Initialize();
                }
                return _inst;
            }
        }

        // 设置 STO
        public SettingSTO Setting;

        // 初始化
        public void Initialize()
        {
            Setting = AssetDatabase.LoadAssetAtPath("Assets/LevelEditor/STO/SettingSTO.asset", typeof(SettingSTO)) as SettingSTO;
        }
    }
}
