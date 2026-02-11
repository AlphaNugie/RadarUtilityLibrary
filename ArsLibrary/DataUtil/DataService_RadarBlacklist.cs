using ArsLibrary.Core;
using ArsLibrary.Model;
using CommonLib.DataUtil;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArsLibrary.DataUtil
{
    /// <summary>
    /// 雷达黑名单实体类的sqlite数据服务类
    /// </summary>
    public class DataService_RadarBlacklist : BaseDataServiceSqlite
    {
        /// <summary>
        /// 构造器
        /// </summary>
        public DataService_RadarBlacklist() : base(ArsConst.SqliteFileDir, ArsConst.SqliteFileName) { }

        /// <inheritdoc/>
        protected override string GetTableName()
        {
            return "t_base_radar_blacklist";
        }

        /// <inheritdoc/>
        protected override List<SqliteColumnMapping> GetColumnsMustHave()
        {
            return new List<SqliteColumnMapping>()
            {
                new SqliteColumnMapping("record_id", SqliteSqlType.INTEGER, null, true, ConflictClause.FAIL, null, false, ConflictClause.NONE, true, ConflictClause.FAIL, true),
                new SqliteColumnMapping("radar_id", SqliteSqlType.INTEGER, null, true, ConflictClause.FAIL),
                new SqliteColumnMapping("radar_coors_limited", SqliteSqlType.INTEGER, null, true, ConflictClause.FAIL, 0),
                new SqliteColumnMapping("radar_x_min", SqliteSqlType.NUMBER, null, true, ConflictClause.FAIL, 0),
                new SqliteColumnMapping("radar_x_max", SqliteSqlType.NUMBER, null, true, ConflictClause.FAIL, 0),
                new SqliteColumnMapping("radar_y_min", SqliteSqlType.NUMBER, null, true, ConflictClause.FAIL, 0),
                new SqliteColumnMapping("radar_y_max", SqliteSqlType.NUMBER, null, true, ConflictClause.FAIL, 0),
                new SqliteColumnMapping("claimer_coors_limited", SqliteSqlType.INTEGER, null, true, ConflictClause.FAIL, 0),
                new SqliteColumnMapping("claimer_x_min", SqliteSqlType.NUMBER, null, true, ConflictClause.FAIL, 0),
                new SqliteColumnMapping("claimer_x_max", SqliteSqlType.NUMBER, null, true, ConflictClause.FAIL, 0),
                new SqliteColumnMapping("claimer_y_min", SqliteSqlType.NUMBER, null, true, ConflictClause.FAIL, 0),
                new SqliteColumnMapping("claimer_y_max", SqliteSqlType.NUMBER, null, true, ConflictClause.FAIL, 0),
                new SqliteColumnMapping("claimer_z_min", SqliteSqlType.NUMBER, null, true, ConflictClause.FAIL, 0),
                new SqliteColumnMapping("claimer_z_max", SqliteSqlType.NUMBER, null, true, ConflictClause.FAIL, 0),
                new SqliteColumnMapping("rcs_limited", SqliteSqlType.INTEGER, null, true, ConflictClause.FAIL, 0),
                new SqliteColumnMapping("rcs_min", SqliteSqlType.INTEGER, null, true, ConflictClause.FAIL, 0),
                new SqliteColumnMapping("rcs_max", SqliteSqlType.INTEGER, null, true, ConflictClause.FAIL, 0),
                new SqliteColumnMapping("angle_limited", SqliteSqlType.INTEGER, null, true, ConflictClause.FAIL, 0),
                new SqliteColumnMapping("angle_min", SqliteSqlType.NUMBER, null, true, ConflictClause.FAIL, 0),
                new SqliteColumnMapping("angle_max", SqliteSqlType.NUMBER, null, true, ConflictClause.FAIL, 0),
                new SqliteColumnMapping("claimer_posture_limited", SqliteSqlType.INTEGER, null, true, ConflictClause.FAIL, 0),
                new SqliteColumnMapping("walk_pos_min", SqliteSqlType.NUMBER, null, true, ConflictClause.FAIL, 0),
                new SqliteColumnMapping("walk_pos_max", SqliteSqlType.NUMBER, null, true, ConflictClause.FAIL, 0),
                new SqliteColumnMapping("pitch_angle_min", SqliteSqlType.NUMBER, null, true, ConflictClause.FAIL, 0),
                new SqliteColumnMapping("pitch_angle_max", SqliteSqlType.NUMBER, null, true, ConflictClause.FAIL, 0),
                new SqliteColumnMapping("yaw_angle_min", SqliteSqlType.NUMBER, null, true, ConflictClause.FAIL, 0),
                new SqliteColumnMapping("yaw_angle_max", SqliteSqlType.NUMBER, null, true, ConflictClause.FAIL, 0),
                new SqliteColumnMapping("stretch_len_min", SqliteSqlType.NUMBER, null, true, ConflictClause.FAIL, 0),
                new SqliteColumnMapping("stretch_len_max", SqliteSqlType.NUMBER, null, true, ConflictClause.FAIL, 0),
                new SqliteColumnMapping("remark", SqliteSqlType.VARCHAR2, 128),
            };
        }

        /// <summary>
        /// 查询所有黑名单
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllRecordsOrderById()
        {
            return GetRecordsOrderById(0);
        }

        /// <summary>
        /// 根据给定雷达ID查询黑名单
        /// </summary>
        /// <param name="radarId"></param>
        /// <returns></returns>
        public DataTable GetRecordsOrderById(int radarId)
        {
            string sqlString = string.Format("select *, 0 changed from {0} where ({1} = 0 or radar_id = {1}) order by record_id", TableName, radarId);
            return Provider.Query(sqlString);
            //return table == null || table.Rows.Count == 0 ? null : table.Rows.Cast<DataRow>().Select(dataRow => new RadarBlacklist(dataRow)).ToList();
        }

        /// <summary>
        /// 将DataTable转换为List
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static List<RadarBlacklist> CastDataTableToList(DataTable table)
        {
            return table == null || table.Rows.Count == 0 ? null : table.Rows.Cast<DataRow>().Select(dataRow => new RadarBlacklist(dataRow)).ToList();
        }

        #region 增删改
        /// <summary>
        /// 根据ID删除
        /// </summary>
        /// <param name="id">记录的ID</param>
        /// <returns></returns>
        public int DeleteRecordById(int id)
        {
            string sql = string.Format("delete from {0} where record_id = {1}", TableName, id);
            return Provider.ExecuteSql(sql);
        }

        /// <summary>
        /// 保存记录信息
        /// </summary>
        /// <param name="record">记录对象</param>
        /// <returns></returns>
        public int SaveRecord(RadarBlacklist record)
        {
            return Provider.ExecuteSql(GetRecordSqlString(record));
        }

        /// <summary>
        /// 批量保存记录信息
        /// </summary>
        /// <param name="records">多个记录对象</param>
        /// <returns></returns>
        public bool SaveRecords(IEnumerable<RadarBlacklist> records)
        {
            string[] sqls = records == null ? null : records.Select(record => GetRecordSqlString(record)).ToArray();
            return Provider.ExecuteSqlTrans(sqls);
        }
        #endregion

        /// <summary>
        /// 获取SQL字符串
        /// </summary>
        /// <param name="record">记录对象</param>
        /// <returns></returns>
        private string GetRecordSqlString(RadarBlacklist record)
        {
            string sql = string.Empty;
            if (record != null)
                //sql = string.Format(record.Id <= 0 ? "insert into {0} (radar_id, rcs_limited, rcs_min, rcs_max, radar_coors_limited, radar_x_min, radar_x_max, radar_y_min, radar_y_max, claimer_coors_limited, claimer_x_min, claimer_x_max, claimer_y_min, claimer_y_max, claimer_z_min, claimer_z_max, angle_limited, angle_min, angle_max, remark) values ({2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, {17}, {18}, {19}, {20}, '{21}')" : "update {0} set radar_id = {2}, rcs_limited = {3}, rcs_min = {4}, rcs_max = {5}, radar_coors_limited = {6}, radar_x_min = {7}, radar_x_max = {8}, radar_y_min = {9}, radar_y_max = {10}, claimer_coors_limited = {11}, claimer_x_min = {12}, claimer_x_max = {13}, claimer_y_min = {14}, claimer_y_max = {15}, claimer_z_min = {16}, claimer_z_max = {17}, angle_limited = {18}, angle_min = {19}, angle_max = {20}, remark = '{21}' where record_id = {1}",
                //    TableName, record.Id, record.RadarId, record.RcsLimited ? 1 : 0, record.RcsMin, record.RcsMax, record.RadarCoorsLimited ? 1 : 0, record.RadarxMin, record.RadarxMax, record.RadaryMin, record.RadaryMax, record.ClaimerCoorsLimited ? 1 : 0, record.ClaimerxMin, record.ClaimerxMax, record.ClaimeryMin, record.ClaimeryMax, record.ClaimerzMin, record.ClaimerzMax, record.AngleLimited ? 1 : 0, record.AngleMin, record.AngleMax, record.Remark);
                sql = string.Format(record.Id <= 0 ? @"insert into {0} (
radar_id,
rcs_limited, rcs_min, rcs_max,
radar_coors_limited, radar_x_min, radar_x_max, radar_y_min, radar_y_max,
claimer_coors_limited, claimer_x_min, claimer_x_max, claimer_y_min, claimer_y_max, claimer_z_min, claimer_z_max,
angle_limited, angle_min, angle_max,
claimer_posture_limited, walk_pos_min, walk_pos_max, pitch_angle_min, pitch_angle_max, yaw_angle_min, yaw_angle_max, stretch_len_min, stretch_len_max,
remark) values (
{2},
{3}, {4}, {5},
{6}, {7}, {8}, {9}, {10},
{11}, {12}, {13}, {14}, {15}, {16}, {17},
{18}, {19}, {20},
{21}, {22}, {23}, {24}, {25}, {26}, {27}, {28}, {29},
'{30}')"
:
@"update {0} set
radar_id = {2},
rcs_limited = {3}, rcs_min = {4}, rcs_max = {5},
radar_coors_limited = {6}, radar_x_min = {7}, radar_x_max = {8}, radar_y_min = {9}, radar_y_max = {10},
claimer_coors_limited = {11}, claimer_x_min = {12}, claimer_x_max = {13}, claimer_y_min = {14}, claimer_y_max = {15}, claimer_z_min = {16}, claimer_z_max = {17},
angle_limited = {18}, angle_min = {19}, angle_max = {20},
claimer_posture_limited = {21}, walk_pos_min = {22}, walk_pos_max = {23}, pitch_angle_min = {24}, pitch_angle_max = {25}, yaw_angle_min = {26}, yaw_angle_max = {27}, stretch_len_min = {28}, stretch_len_max = {29},
remark = '{30}' where record_id = {1}",
                    TableName,
                    record.Id,
                    record.RadarId,
                    record.RcsLimited ? 1 : 0, record.RcsMin, record.RcsMax,
                    record.RadarCoorsLimited ? 1 : 0, record.RadarxMin, record.RadarxMax, record.RadaryMin, record.RadaryMax,
                    record.ClaimerCoorsLimited ? 1 : 0, record.ClaimerxMin, record.ClaimerxMax, record.ClaimeryMin, record.ClaimeryMax, record.ClaimerzMin, record.ClaimerzMax,
                    record.AngleLimited ? 1 : 0, record.AngleMin, record.AngleMax,
                    record.ClaimerPostureLimited ? 1 : 0, record.WalkPosMin, record.WalkPosMax, record.PitchAngleMin, record.PitchAngleMax, record.YawAngleMin, record.YawAngleMax, record.StretchLenMin, record.StretchLenMax,
                    record.Remark);
            return sql;
        }
    }
}
