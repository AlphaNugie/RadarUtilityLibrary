using System;
using System.Collections.Generic;
using System.Linq;

namespace ArsLibrary.Cluster
{
    /// <summary>
    /// 跨帧跟踪管理器，使用贪心最近邻匹配，为按给定观测量描述的目标准备跨周期的持久ID
    /// </summary>
    /// <remarks>
    /// 常用于聚类之后的跨帧关联：以某一观测量（如报警距离）作为匹配依据，将相邻周期的簇关联到同一轨迹
    /// </remarks>
    public class TrackManager
    {
        private int _nextId = 1;
        private readonly Dictionary<int, TrackState> _tracks = new Dictionary<int, TrackState>();

        /// <summary>
        /// 关联距离阈值
        /// </summary>
        public double MaxDistance { get; set; } = 3.0;

        /// <summary>
        /// 最大丢失周期数
        /// </summary>
        public int MaxLostFrames { get; set; } = 5;

        /// <summary>
        /// 当前存活轨迹数量
        /// </summary>
        public int TrackCount => _tracks.Count;

        /// <summary>
        /// 更新轨迹，返回 键→跟踪ID 的映射
        /// </summary>
        /// <param name="keyToValue">本周期各目标的键（如簇标签）及其观测量</param>
        /// <returns>键与跟踪ID的对应关系</returns>
        public Dictionary<int, int> Update(Dictionary<int, double> keyToValue)
        {
            Dictionary<int, int> keyToTrack = new Dictionary<int, int>();
            if (keyToValue == null || keyToValue.Count == 0)
            {
                IncreaseLostCounts(new HashSet<int>());
                return keyToTrack;
            }

            HashSet<int> usedTracks = new HashSet<int>();
            foreach (var pair in keyToValue)
            {
                int key = pair.Key;
                double value = pair.Value;
                double bestDist = double.PositiveInfinity;
                int bestTid = -1;

                foreach (var item in _tracks)
                {
                    TrackState state = item.Value;
                    if (usedTracks.Contains(item.Key) || state.LostCount > MaxLostFrames)
                        continue;
                    double dist = Math.Abs(value - state.Value);
                    if (dist < MaxDistance && dist < bestDist)
                    {
                        bestDist = dist;
                        bestTid = item.Key;
                    }
                }

                if (bestTid >= 0) //关联到已有轨迹
                {
                    _tracks[bestTid].Value = value;
                    _tracks[bestTid].LostCount = 0;
                    usedTracks.Add(bestTid);
                    keyToTrack[key] = bestTid;
                }
                else //分配新的ID
                {
                    int newId = _nextId++;
                    _tracks[newId] = new TrackState(value);
                    usedTracks.Add(newId);
                    keyToTrack[key] = newId;
                }
            }

            IncreaseLostCounts(usedTracks);
            return keyToTrack;
        }

        /// <summary>
        /// 未匹配的轨迹丢失计数加一，超过上限则删除
        /// </summary>
        /// <param name="usedTracks">本周期已匹配的跟踪ID集合</param>
        private void IncreaseLostCounts(HashSet<int> usedTracks)
        {
            foreach (int tid in _tracks.Keys.ToList())
            {
                if (usedTracks.Contains(tid))
                    continue;
                TrackState state = _tracks[tid];
                state.LostCount++;
                if (state.LostCount > MaxLostFrames)
                    _tracks.Remove(tid);
            }
        }

        /// <summary>
        /// 清空所有轨迹
        /// </summary>
        public void Reset()
        {
            _tracks.Clear();
            _nextId = 1;
        }
    }

    /// <summary>
    /// 单条轨迹
    /// </summary>
    public class TrackState
    {
        /// <summary>
        /// 最近一次的观测量
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// 连续丢失的周期数
        /// </summary>
        public int LostCount { get; set; }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="value">观测量</param>
        public TrackState(double value)
        {
            Value = value;
            LostCount = 0;
        }
    }
}
