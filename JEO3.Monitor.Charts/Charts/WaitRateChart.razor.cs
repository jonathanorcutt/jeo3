using JEO3.Logging;
using Microsoft.AspNetCore.Components;
using PanoramicData.ECharts;

namespace JEO3.Monitor.ECharts
{
    public partial class WaitRateChart : MonitorChartBase
    {
        #region Properties
        [Parameter] public override string Width { get; set; } = "100%";
        [Parameter] public override string Height { get; set; } = "100%";

        // Separate lists for each series
        private readonly List<object> _ioData = new(MaxRetention);
        private readonly List<object> _memoryData = new(MaxRetention);
        private readonly List<object> _cpuData = new(MaxRetention);
        private readonly List<object> _waitData = new(MaxRetention);

        protected override ChartOptions Options { get; set; } = ChartOptionsLibrary.WaitRateChart_Options;

        #endregion

        #region Initialization
        protected override async Task RenderInitialAsync(MonitorSnapshot initial)
        {
            if (IsDisposed || initial == null) return;

            _memoryData.Clear();
            _ioData.Clear();
            _cpuData.Clear();
            _waitData.Clear();

            // Process Waits
            if (initial.History != null)
            {
                foreach (var sample in initial.History.OrderBy(h => h.Timestamp))
                {
                    AddWaitSample(sample);
                }
            }

            // Process Performance (CPU, Memory, IO)
            if (initial.PerformanceHistory != null)
            {
                foreach (var perf in initial.PerformanceHistory.OrderBy(h => h.Timestamp))
                {
                    AddPerformanceSample(perf);
                }
            }

            UpdateSeriesData();
            await SafeUpdateChartAsync();
        }
        #endregion

        #region Updates
        protected override async Task AppendDeltasAsync(MonitorDeltaResponse delta)
        {
            if (IsDisposed) return;

            bool hasUpdates = false;

            // Process new Waits
            if (delta.History != null && delta.History.Count > 0)
            {
                foreach (var sample in delta.History.OrderBy(h => h.Timestamp))
                {
                    AddWaitSample(sample);
                }
                hasUpdates = true;
            }

            // Process new Performance
            if (delta.PerformanceHistory != null && delta.PerformanceHistory.Count > 0)
            {
                foreach (var perf in delta.PerformanceHistory.OrderBy(h => h.Timestamp))
                {
                    AddPerformanceSample(perf);
                }
                hasUpdates = true;
            }

            if (hasUpdates)
            {
                UpdateSeriesData();
                await SafeUpdateChartAsync();
            }
        }

        private void AddWaitSample(WaitRateSample sample)
        {
            double waitValue = sample.TotalWaitMsPerSec;
            var timeStr = sample.Timestamp.ToString("yyyy-MM-dd HH:mm:ss");
            _waitData.Add(new object[] { timeStr, Math.Round(waitValue, 0) });

            if (_waitData.Count > MaxRetention) _waitData.RemoveAt(0);
        }

        private void AddPerformanceSample(MonitorHistorySample sample)
        {
            if (sample.Performance == null) return;

            var timeStr = sample.Timestamp.ToString("yyyy-MM-dd HH:mm:ss");

            var cpu = Math.Round(sample.Performance.CpuPercent, 1);
            var mem = Math.Round(sample.Performance.TargetServerMemoryMb, 1);
            var stallReads = Math.Round(sample.Performance.IoStallReadMs * .3, 1);

            _cpuData.Add(new object[] { timeStr, cpu });
            _memoryData.Add(new object[] { timeStr, mem });
            _ioData.Add(new object[] { timeStr, stallReads });

            if (_ioData.Count > MaxRetention) _ioData.RemoveAt(0);
            if (_memoryData.Count > MaxRetention) _memoryData.RemoveAt(0);
            if (_cpuData.Count > MaxRetention) _cpuData.RemoveAt(0);
        }

        private void UpdateSeriesData()
        {
            if (IsDisposed) return;

            try
            {
                if (Options.Series != null && Options.Series.Count >= 4)
                {
                    // Map each separate list to its specific line series
                    ((LineSeries)Options.Series[0]).Data = _ioData.ToArray();
                    ((LineSeries)Options.Series[1]).Data = _memoryData.ToArray();
                    ((LineSeries)Options.Series[2]).Data = _cpuData.ToArray();
                    ((LineSeries)Options.Series[3]).Data = _waitData.ToArray();
                }
            }
            catch (Exception ex)
            {
                ExceptionUtility.LogException(ex);
            }
        }
        #endregion
    }
}