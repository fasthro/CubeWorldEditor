/*
 * @Author: fasthro
 * @Date: 2019-05-17 14:28:57
 * @Description: 环境区域
 */
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace LevelEditor
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


        // 区域坐标描述(使用区域描述之前请先调用 CalculateCoord 计算)
        public Vector3 coordLD = Vector3.zero;
        public Vector3 coordLU = Vector3.zero;
        public Vector3 coordRU = Vector3.zero;
        public Vector3 coordRD = Vector3.zero;

        // 是否导出格子四周的格子
        public bool exportAround = true;
        // 动画开始位置
        public Vector3Int animationStartPosition;

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

        /// <summary>
        ///  计算区域描述
        /// </summary>
        public void CalculateCoord()
        {
            int index = 0;
            foreach (KeyValuePair<string, Grid> item in grids)
            {
                var grid = item.Value;
                if (index == 0)
                {
                    coordLD.x = grid.position.x;
                    coordLD.y = 0;
                    coordLD.z = grid.position.z;

                    coordLU.x = grid.position.x;
                    coordLU.y = 0;
                    coordLU.z = grid.position.z;

                    coordRU.x = grid.position.x;
                    coordRU.y = 0;
                    coordRU.z = grid.position.z;

                    coordRD.x = grid.position.x;
                    coordRD.y = 0;
                    coordRD.z = grid.position.z;
                }
                else
                {
                    // coordLD
                    if (grid.position.x < coordLD.x)
                        coordLD.x = grid.position.x;
                    if (grid.position.z < coordLD.z)
                        coordLD.z = grid.position.z;

                    // coordLU
                    if (grid.position.x < coordLU.x)
                        coordLU.x = grid.position.x;
                    if (grid.position.z > coordLU.z)
                        coordLU.z = grid.position.z;

                    // coordRU
                    if (grid.position.x > coordRU.x)
                        coordRU.x = grid.position.x;
                    if (grid.position.z > coordRU.z)
                        coordRU.z = grid.position.z;

                    // coordRD
                    if (grid.position.x > coordRD.x)
                        coordRD.x = grid.position.x;
                    if (grid.position.z < coordRD.z)
                        coordRD.z = grid.position.z;
                }
                index++;
            }
        }

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
            template = template.Replace("{#animation_start_id}", Environment.GetRunTimeKey(animationStartPosition));
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
            string template = "<grid bundle_name='{#bundle_name}' asset_name='{#asset_name}' pos_x='{#pos_x}' pos_y='{#pos_y}' pos_z='{#pos_z}' op_x='{#op_x}' op_y='{#op_y}' op_z='{#op_z}' angle_x='{#angle_x}' angle_y='{#angle_y}' angle_z='{#angle_z}' oa_x='{#oa_x}' oa_y='{#oa_y}' oa_z='{#oa_z}' {#adjacent}></grid>";
            var resObject = ResManager.Inst.GetResObject(grid.resId);
            template = template.Replace("{#bundle_name}", resObject.GetAssetBundleName());
            template = template.Replace("{#asset_name}", resObject.GetFileNameWithoutExtension());
            template = template.Replace("{#pos_x}", grid.position.x.ToString());
            template = template.Replace("{#pos_y}", grid.position.y.ToString());
            template = template.Replace("{#pos_z}", grid.position.z.ToString());
            template = template.Replace("{#op_x}", grid.positionOffset.x.ToString());
            template = template.Replace("{#op_y}", grid.positionOffset.y.ToString());
            template = template.Replace("{#op_z}", grid.positionOffset.z.ToString());
            template = template.Replace("{#angle_x}", grid.angleOffset.x.ToString());
            template = template.Replace("{#angle_y}", grid.angleOffset.y.ToString());
            template = template.Replace("{#angle_z}", grid.angleOffset.z.ToString());
            template = template.Replace("{#oa_x}", grid.angleOffset.x.ToString());
            template = template.Replace("{#oa_y}", grid.angleOffset.y.ToString());
            template = template.Replace("{#oa_z}", grid.angleOffset.z.ToString());

            // around direction
            string adjacent = string.Empty;
            if (exportAround)
            {
                string px = ExportGridAdjacentXml(grid, AroundDirection.PositiveX);
                if (!string.IsNullOrEmpty(px))
                {
                    adjacent += string.Format(" {0}", px);
                }
                string nx = ExportGridAdjacentXml(grid, AroundDirection.NegativeX);
                if (!string.IsNullOrEmpty(nx))
                {
                    adjacent += string.Format(" {0}", nx);
                }
                string pz = ExportGridAdjacentXml(grid, AroundDirection.PositiveZ);
                if (!string.IsNullOrEmpty(pz))
                {
                    adjacent += string.Format(" {0}", pz);
                }
                string nz = ExportGridAdjacentXml(grid, AroundDirection.NegativeZ);
                if (!string.IsNullOrEmpty(nz))
                {
                    adjacent += string.Format(" {0}", nz);
                }
            }
            template = template.Replace("{#adjacent}", adjacent);

            return template;
        }

        /// <summary>
        /// 导出相邻格子id
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        private string ExportGridAdjacentXml(Grid grid, AroundDirection direction)
        {
            if (grid.materialType != MaterialType.Cube)
                return "";

            Vector3 position = Vector3.zero;
            position.x = grid.position.x;
            position.y = grid.position.y;
            position.z = grid.position.z;

            string dirstr = string.Empty;

            if (direction == AroundDirection.PositiveX)
            {
                position.x = grid.position.x + 1f;
                dirstr = "adjacent_px";
            }
            else if (direction == AroundDirection.NegativeX)
            {
                position.x = grid.position.x - 1f;
                dirstr = "adjacent_nx";
            }
            else if (direction == AroundDirection.PositiveZ)
            {
                position.z = grid.position.z + 1f;
                dirstr = "adjacent_pz";
            }
            else if (direction == AroundDirection.NegativeZ)
            {
                position.z = grid.position.z - 1f;
                dirstr = "adjacent_nz";
            }

            string key = Environment.GetKey(position, areaIndex);

            Grid adjacentGrid = GetGrid(key);
            if (adjacentGrid != null)
            {
                return string.Format("{0}='{1}'", dirstr, Environment.GetRunTimeKey(adjacentGrid.position));
            }
            return "";
        }
        #endregion
    }
}
