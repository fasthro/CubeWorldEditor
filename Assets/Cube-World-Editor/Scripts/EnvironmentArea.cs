﻿/*
 * @Author: fasthro
 * @Date: 2019-05-17 14:28:57
 * @Description: Cube World 区域
 */
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace CubeWorldEditor
{
    public class EnvironmentArea : MonoBehaviour
    {
        // 区域索引
        private int m_areaIndex;
        public int areaIndex { get { return m_areaIndex; } }

        // 区域所有格子字典
        private Dictionary<string, Grid> grids;
        // 区域装饰品字典
        public Dictionary<string, Grid> ornaments;

        public void Initialize(int index)
        {
            m_areaIndex = index;

            grids = new Dictionary<string, Grid>();
            ornaments = new Dictionary<string, Grid>();


            // 创建区域材料节点
            FieldInfo[] fields = typeof(MaterialType).GetFields();
            for (int i = 0; i < fields.Length; i++)
            {
                var name = fields[i].Name;
                if (!name.Equals("value__"))
                {
                    var mt = (MaterialType)fields[i].GetValue(fields[i]);
                    var materialTransform = transform.Find(name);
                    if (materialTransform != null)
                    {
                        int gridCount = materialTransform.childCount;
                        for (int k = 0; k < gridCount; k++)
                        {
                            var gridTransform = materialTransform.GetChild(k);

                            var key = gridTransform.gameObject.name;
                            var grid = gridTransform.GetComponent<Grid>();

                            if (grid != null)
                            {
                                grid.Initialize();
                                grids.Add(key, grid);

                                if (mt == MaterialType.Ornament)
                                {
                                    ornaments.Add(key, grid);
                                }
                            }
                        }
                    }
                }
            }
        }

        #region grid
        /// <summary>
        /// 获取格子
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Grid GetGrid(string key)
        {
            Grid grid = null;
            if (grids.TryGetValue(key, out grid))
            {
                return grid;
            }
            return null;
        }

        /// <summary>
        /// 添加格子
        /// </summary>
        /// <param name="key"></param>
        /// <param name="grid"></param>
        public bool AddGrid(string key, Grid grid)
        {
            if (!grids.ContainsKey(key) && grid != null)
            {
                grids.Add(key, grid);

                if (grid.materialType == MaterialType.Ornament)
                {
                    ornaments.Add(key, grid);
                }

                return true;
            }
            return false;
        }

        /// <summary>
        /// 移除格子
        /// </summary>
        /// <param name="key"></param>
        public void RemoveGrid(string key)
        {
            var grid = GetGrid(key);
            if (grid != null)
            {
                grids.Remove(key);

                if (grid.materialType == MaterialType.Ornament)
                {
                    ornaments.Remove(key);
                }
            }
        }
        #endregion

        #region export
        /// <summary>
        /// 导出xml
        /// </summary>
        /// <returns></returns>
        public string ExportXml()
        {
            string groundContent = string.Empty;
            string ornamentContent = string.Empty;

            foreach (KeyValuePair<string, Grid> gridItem in grids)
            {
                var grid = gridItem.Value;
                if (grid.materialType == MaterialType.Cube)
                {
                    groundContent += ExporGridtXml(grid);
                }
                else if (grid.materialType == MaterialType.Ornament)
                {
                    ornamentContent += ExporGridtXml(grid);
                }
            }

            string template = Utils.LoadTemplate("Area.txt");
            template = template.Replace("{#area_index}", areaIndex.ToString());
            template = template.Replace("{#cube_content}", groundContent);
            template = template.Replace("{#ornament_content}", ornamentContent);

            return template;
        }

        /// <summary>
        /// 导出格子模版  
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        private string ExporGridtXml(Grid grid)
        {
            string template = "<grid ab='{#ab}' asset='{#asset}' pos='{#pos_x},{#pos_y},{#pos_z}' o_pos='{#o_pos_x},{#o_pos_y},{#o_pos_z}' angle='{#angle_x},{#angle_y},{#angle_z}' o_angle='{#o_angle_x},{#o_angle_y},{#o_angle_z}'></grid>";
            var resObject = ResManager.Inst.GetResObject(grid.resId);
            template = template.Replace("{#ab}", resObject.GetAssetBundleName());
            template = template.Replace("{#asset}", resObject.GetFileNameWithoutExtension());
            template = template.Replace("{#pos_x}", grid.position.x.ToString());
            template = template.Replace("{#pos_y}", grid.position.y.ToString());
            template = template.Replace("{#pos_z}", grid.position.z.ToString());
            template = template.Replace("{#o_pos_x}", grid.positionOffset.x.ToString());
            template = template.Replace("{#o_pos_y}", grid.positionOffset.y.ToString());
            template = template.Replace("{#o_pos_z}", grid.positionOffset.z.ToString());
            template = template.Replace("{#angle_x}", grid.angleOffset.x.ToString());
            template = template.Replace("{#angle_y}", grid.angleOffset.y.ToString());
            template = template.Replace("{#angle_z}", grid.angleOffset.z.ToString());
            template = template.Replace("{#o_angle_x}", grid.angleOffset.x.ToString());
            template = template.Replace("{#o_angle_y}", grid.angleOffset.y.ToString());
            template = template.Replace("{#o_angle_z}", grid.angleOffset.z.ToString());

            return template;
        }
        #endregion
    }
}
