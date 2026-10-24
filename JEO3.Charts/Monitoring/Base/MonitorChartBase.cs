using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using PanoramicData.ECharts;
using System;
using System.Threading.Tasks;

namespace JEO3.Monitor.ECharts
{
    /// <summary>
    /// Shared lifecycle for every live monitoring chart.
    /// Inherits the delta-polling engine from MonitorBase and provides the static ECharts Options shell with adaptive resizing.
    /// Concrete charts must supply the initial ChartOptions shell and map delta data into it.
    /// </summary>
    public abstract class MonitorChartBase : MonitorBase, IAsyncDisposable
    {
        #region Properties

        [Inject] protected IJSRuntime JSRuntime { get; set; } = default!;
        [CascadingParameter] public ChartThemeType ActiveTheme { get; set; } = ChartThemeType.Default;
        protected EChart? Chart;
        protected abstract ChartOptions Options { get; set; }
        private ChartThemeType _lastTheme;

        #endregion

        #region Initialization

        protected override void OnParametersSet()
        {
            // Only rebuild options if the theme actually changed
            if (ActiveTheme != _lastTheme || Options == null)
            {
                _lastTheme = ActiveTheme;
                var chartType = this.GetType().Name;
                var chartTheme = ChartTheme.FromEnum(ActiveTheme);
                switch (chartType)
                {
                    case nameof(ActiveRequestsChart):
                        this.Options = ChartOptionsLibrary.BuildActiveRequestsChartOptions(chartTheme);
                        break;
                    case nameof(BlockingChart):
                        this.Options = ChartOptionsLibrary.BuildBlockingChartOptions(chartTheme);
                        break;
                    case nameof(CorrelationChart):
                        this.Options = ChartOptionsLibrary.BuildCorrelationChartOptions(chartTheme);
                        break;
                    case nameof(DeadlockChart):
                        this.Options = ChartOptionsLibrary.BuildDeadlockChartOptions(chartTheme);
                        break;
                    case nameof(EGauge):
                        this.Options = ChartOptionsLibrary.BuildEGaugeOptions(chartTheme);
                        break;
                    case nameof(TaskRateChart):
                        this.Options = ChartOptionsLibrary.BuildTaskRateChartOptions(chartTheme);
                        break;
                    case nameof(TopWaitsChart):
                        this.Options = ChartOptionsLibrary.BuildTopWaitsChartOptions(chartTheme);
                        break;
                    case nameof(WaitCandleChart):
                        this.Options = ChartOptionsLibrary.BuildWaitCandleChartOptions(chartTheme);
                        break;
                    case nameof(WaitRateChart):
                        this.Options = ChartOptionsLibrary.BuildWaitRateChartOptions(chartTheme);
                        break;
                    case nameof(WaitDistributionChart):
                        this.Options = ChartOptionsLibrary.BuildWaitDistributionChartOptions(chartTheme);
                        break;
                    case nameof(WaitRateSparklineChart):
                        this.Options = ChartOptionsLibrary.BuildWaitRateSparklineChartOptions(chartTheme);
                        break;
                    case nameof(WaitRateStackedAreaChart):
                        this.Options = ChartOptionsLibrary.BuildWaitRateStackedAreaChartOptions(chartTheme);
                        break;
                }
                StateHasChanged();
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                // CRITICAL SAFETY CHECK: Wait until the wrapped chart ref binds completely to avoid null references
                if (Chart == null) return;

                try
                {
                    // Hand off observation entirely to the global JavaScript observer routine
                    await JSRuntime.InvokeVoidAsync("jeoResize.observe", Chart.Id);
                }
                catch (JSDisconnectedException)
                {
                    // Circuit broken mid-initialization
                }
            }
        }

        public override async ValueTask DisposeAsync()
        {
            if (IsDisposed) return;
            IsDisposed = true;

            if (Chart != null && !string.IsNullOrEmpty(Chart.Id))
            {
                try
                {
                    // Clean up the global ResizeObserver registration tracking this specific DOM node
                    await JSRuntime.InvokeVoidAsync("jeoResize.unobserve", Chart.Id);
                }
                catch { }
            }

            // FIX: Chain back up to MonitorBase to guarantee DataService.OnDelta -= HandleDelta executes!
            await base.DisposeAsync();

            GC.SuppressFinalize(this);
        }
        #endregion

        #region Update
        /// <summary>
        /// Wraps UI updates and JS Interop calls inside an isolated, safe execution envelope.
        /// </summary>
        protected async Task SafeUpdateChartAsync()
        {
            if (IsDisposed) return;

            try
            {
                await InvokeAsync(StateHasChanged);

                // Final confirmation that the browser context hasn't died during state rendering
                if (!IsDisposed && Chart != null)
                {
                    await Chart.UpdateAsync();
                }
            }
            catch (JSDisconnectedException)
            {
                // Silently ignore: The browser circuit dropped right mid-render execution.
            }
            catch (Exception ex) when (ex.Message.Contains("JavaScript interop"))
            {
                // Fallback catch-all boundary for general interop disconnect errors
            }
        }
        #endregion
    }
}