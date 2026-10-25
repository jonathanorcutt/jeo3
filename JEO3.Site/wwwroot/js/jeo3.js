

// ---------------------------------------------------------------------------------

// Color Picker Set Site Theme Color
window.jeoThemeEngine = {
    setPrimaryColor: function (hexColor) {
        document.documentElement.style.setProperty('--rz-primary', hexColor);
        document.documentElement.style.setProperty('--jeo-primary', hexColor);
    }
};

// ---------------------------------------------------------------------------------

// Monaco Editor Environment
window.MonacoEnvironment = {
    baseUrl: '_content/BlazorMonaco/lib/monaco-editor/min/vs'
};

// Monaco Editor Interop
window.monacoInterop = {
    defineAndSetTheme: function (themeName, themeData) {
        monaco.editor.defineTheme(themeName, themeData);
        monaco.editor.setTheme(themeName);
    }
};

// ---------------------------------------------------------------------------------

// Site Toggle Fullscreen/F11
async function toggleSiteFullscreen() {

    try {
        if (!document.fullscreenElement) {
            // Enter fullscreen
            await document.documentElement.requestFullscreen();
        } else {
            // Exit fullscreen
            await document.exitFullscreen();
        }

        // Fullscreen Hardcoded
        var style = 'height:100vh; width:100%; border:1px solid rgba(0,0,0,.08);';
        // document.querySelectorAll('.database-viewer-splitter').forEach((pane) => { pane.style = style + 'max-height: ' + ((!document.fullscreenElement) ? 785 : 930) + 'px'; });
        document.querySelectorAll('.navbar-row div').forEach((pane) => { pane.style.display = ((!document.fullscreenElement) ? 'block' : 'none'); });
    } catch (err) {
        console.error(`Error attempting to toggle fullscreen: ${err.message}`);
    }
};

// Toggle Diagram Pane Size To/From 50% Screen (Left Side)
async function toggleElementFullscreen(fullscreenDiv, enterFullScreen) {

    try {
        if (!fullscreenDiv) return;

        var requestMethod = fullscreenDiv.requestFullscreen ||
            fullscreenDiv.webkitRequestFullscreen ||
            fullscreenDiv.mozRequestFullScreen ||
            fullscreenDiv.msRequestFullscreen;

        if (requestMethod && enterFullScreen) {
            requestMethod.call(fullscreenDiv);
        } else {
            // Exit fullscreen
            await document.exitFullscreen();
        }
    } catch (err) {
        console.error(`Error attempting to toggle fullscreen: ${err.message}`);
    }
};



// Toggle a component between its original location and a target workspace.
async function toggleElementExpandCollapse(fullscreenDiv, targetDiv, expand) {
    try {
        if (!fullscreenDiv || !targetDiv) return;

        if (expand) {
            // Capture the component's current geometry before expanding.
            const rect = targetDiv.getBoundingClientRect();

            fullscreenDiv.style.position = 'fixed';
            fullscreenDiv.style.top = `${rect.top}px`;
            fullscreenDiv.style.left = `${rect.left}px`;
            fullscreenDiv.style.width = '1500px';
            fullscreenDiv.style.height = targetDiv.height;
            fullscreenDiv.style.zIndex = '99999';
            fullscreenDiv.style.backgroundColor = 'black';

            // fullscreenDiv.classList.add('is-js-expanded');
        }
        else {
            // Restore natural positioning and sizing.
            fullscreenDiv.style.position = '';
            fullscreenDiv.style.top = '';
            fullscreenDiv.style.left = '';
            fullscreenDiv.style.width = '';
            fullscreenDiv.style.height = '';
            fullscreenDiv.style.zIndex = '';

            // fullscreenDiv.classList.remove('is-js-expanded');
        }
    }
    catch (err) {
        console.error(`Error attempting to toggle fullscreen: ${err.message}`);
    }
}

// ---------------------------------------------------------------------------------

// Download File - Base64
function downloadBase64File(contentType, base64String, fileName) {
    const link = document.createElement('a');
    link.download = fileName;
    link.href = `data:${contentType};base64,${base64String}`;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}

// Download File - Stream
window.downloadFileFromStream = async (fileName, contentStreamReference) => {
    const arrayBuffer = await contentStreamReference.arrayBuffer();

    // FIX 1: Explicitly force "application/octet-stream" so the browser doesn't try to view it
    const blob = new Blob([arrayBuffer], { type: 'application/octet-stream' });

    const url = URL.createObjectURL(blob);
    const anchorElement = document.createElement('a');
    anchorElement.href = url;
    anchorElement.download = fileName ?? '';

    // FIX 2: Temporarily attach to body (required by some browsers for the download attribute to work)
    document.body.appendChild(anchorElement);
    anchorElement.click();

    // Clean up
    anchorElement.remove();
    URL.revokeObjectURL(url);
}

// ---------------------------------------------------------------------------------

// EGauge Crosshairs
function updateCoordinates(e) {
    const container = e.currentTarget; // This is now .monitor-panel
    const rect = container.getBoundingClientRect();

    // Calculate relative mouse positions within the panel frame
    const x = e.clientX - rect.left;
    const y = e.clientY - rect.top;

    container.style.setProperty('--mouse-x', `${x}px`);
    container.style.setProperty('--mouse-y', `${y}px`);
}

// ---------------------------------------------------------------------------------

// Tables Nav Auto-Scroll
// Check if it already exists on the window to prevent redeclaration errors
if (!window.gridObserver) {
    window.gridObserver = new MutationObserver((mutations) => {
        for (const mutation of mutations) {
            if (mutation.type === 'attributes' && mutation.attributeName === 'class') {
                const target = mutation.target;

                // Check if this specific element just gained the active selection highlight class
                if (target && target.classList && target.classList.contains('rz-state-highlight')) {
                    target.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
                }
            }
        }
    });
}

// Safely start or restart watching the layout tree
window.initializeGlobalGridScroller = () => {
    // ALWAYS fall back to document.body if the layout container isn't ready.
    // Document.body is never destroyed during Blazor page changes.
    const workspaceContainer = document.querySelector('.dashboard-full-absolute')
        || document.querySelector('.rz-layout')
        || document.body;

    if (workspaceContainer) {
        // Safe disconnect
        window.gridObserver.disconnect();

        window.gridObserver.observe(workspaceContainer, {
            attributes: true,
            subtree: true,
            attributeFilter: ['class']
        });
    }
};


// ---------------------------------------------------------------------------------

// 🌟 THE ZOOM PROTECTION SHIELD
// Intercepts the mouse wheel event right at the browser window root level
window.addEventListener('wheel', function (e) {
    // Check if the mouse cursor is currently hovering over Blazor diagram canvases
    const target = e.target.closest('.sample-tile-layout .canvas-container');

    if (target) {
        // Stop the browser window from moving up or down!
        e.preventDefault();

        // 3. Manually bubble or dispatch the wheel delta event straight to the diagram handler if needed, 
        // though Blazor Diagram's native wheel event listener will now catch it cleanly.
    }
}, { passive: false }); // 🌟 CRUCIAL: passive must be false so preventDefault()


// ---------------------------------------------------------------------------------

// 3d ECharts Resize
window.jeoResize = (() => {
    // Looks directly at the container, or searches down into child elements to find the ECharts init block
    const find = el => {
        if (!el) return null;
        if (echarts.getInstanceByDom(el)) return echarts.getInstanceByDom(el);
        
        // Scan for nested div elements that ECharts might have attached to
        const children = el.getElementsByTagName('div');
        for (const child of children) {
            const inst = echarts.getInstanceByDom(child);
            if (inst) return inst;
        }
        return null;
    };

    const ro = new ResizeObserver(entries => {
        if (typeof echarts === 'undefined') return;
        for (const e of entries) {
            if (!e.contentRect.width || !e.contentRect.height) continue;
            const inst = find(e.target);
            if (inst && !inst.isDisposed()) {
                inst.resize();
            }
        }
    });

    return {
        observe: id => { const el = document.getElementById(id); if (el) ro.observe(el); },
        unobserve: id => { const el = document.getElementById(id); if (el) ro.unobserve(el); }
    };
})();

// ---------------------------------------------------------------------------------

// Treeview
window.awesomeTreeView = {
    init: function () {
        // reserved for future global setup (e.g. resize observers)
    },
    setIndeterminate: function (elements, states) {
        elements.forEach((el, i) => { if (el) el.indeterminate = !!states[i]; });
    }
};


// ---------------------------------------------------------------------------------

// Pacman
function deleteNode(obj) {
    obj.deleteNode();
}

