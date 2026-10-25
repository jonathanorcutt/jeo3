using JEO3.Site.Dashboard;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace JEO3.Site.Components.Charting;

/// <summary>
/// Shared lifecycle for every ECharts-backed component: sizing parameters, one-time registration with the
/// shared JS ResizeObserver (wwwroot/js/echartsResize.js), and teardown. Concrete charts supply ContainerId
/// and keep their own render logic. Resizing happens entirely in JS: no C# callbacks, no timers, no polling.
/// </summary>
public abstract class EChartBase : ComponentBase, IAsyncDisposable
{
    #region Properties
    [Inject] protected WSWorkspace W { get; set; }
    [Parameter] public string Width { get; set; } = "100%";
    [Parameter] public string Height { get; set; } = "100%";
    [Parameter] public string LabelText { get; set; } = string.Empty;

    [Parameter] public string PaddingStyle { get; set; } = "0px 2px 50px 0px";

    [Inject] protected IJSRuntime JSRuntime { get; set; } = default!;

    /// <summary>DOM id of the element ECharts renders into. May be null until the chart exists (e.g. a PanoramicData Chart reference).</summary>
    protected abstract string? ContainerId { get; }

    protected abstract string JavascriptFilePath { get; set; }

    protected bool IsDisposed { get; private set; }

    private string? _observedId;
    #endregion

    #region Lifecycle
    protected override void OnInitialized()
    {
        W.OnDashboardStateChanged += HandleStateChanged;
    }
    private async void HandleStateChanged()
    {
        if (IsDisposed) return;
        await InvokeAsync(StateHasChanged);
    }
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        // After the first successful registration this is just a string comparison per render.
        if (IsDisposed || string.IsNullOrEmpty(ContainerId) || ContainerId == _observedId) return;

        _observedId = ContainerId;

        try { await JSRuntime.InvokeVoidAsync("jeoResize.observe", ContainerId); }
        catch (JSDisconnectedException) { }
        catch (JSException ex) { Console.Error.WriteLine($"EChartBase resize registration failed for '{ContainerId}': {ex.Message}"); }
    }

    /// <summary>Derived charts override this for their own teardown (module disposal, etc.). Runs after the resize observer is detached.</summary>
    protected virtual ValueTask OnDisposeAsync() => ValueTask.CompletedTask;

    public async ValueTask DisposeAsync()
    {
        if (IsDisposed) return;

        IsDisposed = true;

        if (_observedId is not null)
        {
            try { await JSRuntime.InvokeVoidAsync("jeoResize.unobserve", _observedId); }
            catch { /* circuit already gone */ }
        }

        if (W != null)
        {
            W.OnDashboardStateChanged -= HandleStateChanged;
        }

        await OnDisposeAsync();

        GC.SuppressFinalize(this);
    }
    #endregion
}
