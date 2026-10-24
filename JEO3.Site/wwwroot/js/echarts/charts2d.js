

// ---------------------------------------------------------------------------------
// Sankey
const queryDepthSankeyCharts = new Map();

function queryDepthColor(depth) {
    // Deterministic palette: the same depth always gets the same color.
    const hue = (depth * 137.508 + 48) % 360;
    return `hsl(${hue}, 78%, 58%)`;
}

function queryIsUpstream(direction) {
    // Adjust this matcher to the actual TraversalDirection enum names.
    return /up|parent|ancestor/i.test(direction ?? "");
}

export function renderQueryDepthSankey(containerId, title, payload) {
    const element = document.getElementById(containerId);

    if (!element || !Array.isArray(payload) || payload.length === 0)
        return;

    // Reuse the existing instance when this component renders again.
    let chart = queryDepthSankeyCharts.get(containerId);

    if (!chart) {
        chart = echarts.init(element);
        queryDepthSankeyCharts.set(containerId, chart);
    }

    const root = payload.find(n => n.isRoot);

    if (!root)
        throw new Error("Query depth Sankey requires a root node.");

    const byId = new Map(payload.map(n => [n.id, n]));

    const upstream = payload.filter(
        n => !n.isRoot && queryIsUpstream(n.direction)
    );

    const downstream = payload.filter(
        n => !n.isRoot && !queryIsUpstream(n.direction)
    );

    const maxUp = Math.max(
        0, ...upstream.map(n => Math.max(0, n.depth))
    );

    const maxDown = Math.max(
        0, ...downstream.map(n => Math.max(0, n.depth))
    );

    // Absolute Sankey columns:
    // upstream -> root -> downstream.
    // Root is placed at the maximum upstream depth.
    const nodes = payload.map(n => {
        const isUp = !n.isRoot && queryIsUpstream(n.direction);
        const column = n.isRoot
            ? maxUp
            : isUp
                ? maxUp - n.depth
                : maxUp + n.depth;

        const fullName = [
            n.databaseName,
            n.schemaName,
            n.tableName
        ].filter(Boolean).join(".");

        return {
            name: n.id,
            depth: Math.max(0, column),
            displayName: fullName || n.id,
            traversalDepth: n.depth,
            direction: n.direction,
            isRoot: n.isRoot,
            itemStyle: {
                color: n.isRoot
                    ? "#ffffff"
                    : queryDepthColor(n.depth),
                borderColor: n.isRoot
                    ? "#facc15"
                    : "rgba(255,255,255,0.35)",
                borderWidth: n.isRoot ? 3 : 1
            },
            label: {
                color: "#f1f5f9"
            }
        };
    });

    const links = [];

    for (const n of payload) {
        if (n.isRoot || !n.parentId)
            continue;

        const parent = byId.get(n.parentId);

        if (!parent)
            continue;

        const isUp = queryIsUpstream(n.direction);

        links.push({
            source: isUp ? n.id : parent.id,
            target: isUp ? parent.id : n.id,
            value: 1,
            lineStyle: {
                color: queryDepthColor(n.depth),
                opacity: 0.55,
                curveness: 0.5
            }
        });
    }

    const maxColumn = maxUp + maxDown;

    chart.setOption({
        backgroundColor: "#080c12",
        title: {
            text: title || "JEO3 Defined",
            subtext:
                `${payload.length} traversal nodes · ` +
                `${links.length} relationships · ` +
                `up ${maxUp} / down ${maxDown}`,
            left: "center",
            top: 12,
            textStyle: { color: "#f8fafc", fontSize: 24 },
            subtextStyle: { color: "#94a3b8" }
        },
        tooltip: {
            trigger: "item",
            formatter: p => {
                const d = p.data || {};

                if (p.dataType === "edge") {
                    return `${p.data.source} → ${p.data.target}`;
                }

                return [
                    `<b>${d.displayName || d.name}</b>`,
                    `Depth: ${d.traversalDepth ?? 0}`,
                    `Direction: ${d.direction || "Root"}`,
                    d.isRoot ? "<b>ROOT</b>" : ""
                ].filter(Boolean).join("<br>");
            }
        },
        series: [{
            type: "sankey",
            orient: "horizontal",
            nodeAlign: "left",
            left: 70,
            right: 100,
            top: 82,
            bottom: 0,
            nodeWidth: 16,
            nodeGap: 12,
            layoutIterations: 32,
            draggable: true,
            emphasis: {
                focus: "adjacency"
            },
            data: nodes,
            links,
            label: {
                show: true,
                position: "inside",
                fontSize: 10,
                overflow: "truncate",
                formatter: p => p.data?.isRoot
                    ? `ROOT · ${p.data.displayName}`
                    : p.data?.displayName || p.name
            },
            lineStyle: {
                opacity: 0.55,
                curveness: 0.5
            }
        }]
    }, true);

    chart.resize();
}

export function disposeQueryDepthSankey(containerId) {
    const chart = queryDepthSankeyCharts.get(containerId);

    if (chart) {
        chart.dispose();
        queryDepthSankeyCharts.delete(containerId);
    }
}


// ---------------------------------------------------------------------------------
// Ancestry Tree
const queryDepthTreeCharts = new Map();

function queryBuildDepthTree(payload) {
    const root = payload.find(n => n.isRoot);

    if (!root)
        throw new Error("Query depth tree requires a root node.");

    const treeNodes = new Map();

    for (const n of payload) {
        const fullName = [n.schemaName, n.tableName].filter(Boolean).join(".") || n.id;

        treeNodes.set(n.id, {
            name: fullName,
            depth: n.depth,
            direction: n.isRoot ? "Root" : n.direction,
            isRoot: n.isRoot,
            itemStyle: {
                color: n.isRoot ? "#ffffff" : queryDepthColor(n.depth),
                borderColor: n.isRoot ? "#facc15" : "rgba(255,255,255,0.35)",
                borderWidth: n.isRoot ? 3 : 1
            },
            children: []
        });
    }

    const rootNode = treeNodes.get(root.id);

    // Direction is ignored for structure: every node hangs off its Parent, so up and down nodes are all just descendants of the root.
    for (const n of payload) {
        if (n.isRoot)
            continue;

        const parent = (n.parentId && treeNodes.get(n.parentId)) || rootNode;
        parent.children.push(treeNodes.get(n.id));
    }

    return rootNode;
}

export function renderQueryDepthTree(containerId, title, payload) {
    const element = document.getElementById(containerId);

    if (!element || !Array.isArray(payload) || payload.length === 0)
        return;

    let chart = queryDepthTreeCharts.get(containerId);

    if (!chart || chart.isDisposed()) {
        chart = echarts.init(element);
        queryDepthTreeCharts.set(containerId, chart);
    }

    const tree = queryBuildDepthTree(payload);

    const maxUp = Math.max(0, ...payload.filter(n => !n.isRoot && queryIsUpstream(n.direction)).map(n => Math.max(0, n.depth)));
    const maxDown = Math.max(0, ...payload.filter(n => !n.isRoot && !queryIsUpstream(n.direction)).map(n => Math.max(0, n.depth)));

    chart.setOption({
        backgroundColor: "#080c12",
        title: {
            text: title || "JEO3 Metric",
            subtext: `${payload.length} traversal nodes · ${payload.length - 1} relationships · up ${maxUp} / down ${maxDown}`,
            left: "center",
            top: 12,
            textStyle: { color: "#f8fafc", fontSize: 24 },
            subtextStyle: { color: "#94a3b8" }
        },
        tooltip: {
            trigger: "item",
            triggerOn: "mousemove",
            formatter: p => {
                const d = p.data || {};
                return [`<b>${d.name}</b>`, `Depth: ${d.depth ?? 0}`, `Direction: ${d.direction || "Root"}`].join("<br>");
            }
        },
        series: [{
            type: "tree",
            data: [tree],
            layout: "orthogonal",
            orient: "LR",
            top: 82,
            bottom: 24,
            left: 170,
            right: 290,
            symbol: "circle",
            symbolSize: 9,
            edgeShape: "curve",
            edgeForkPosition: "50%",
            initialTreeDepth: -1,
            expandAndCollapse: true,
            roam: true,
            animationDurationUpdate: 300,
            lineStyle: { color: "rgba(148,163,184,0.45)", width: 1.5, curveness: 0.5 },
            label: { position: "left", verticalAlign: "middle", align: "right", color: "#f1f5f9", fontSize: 10 },
            leaves: { label: { position: "right", verticalAlign: "middle", align: "left" } },
            emphasis: { focus: "descendant" }
        }]
    }, true);

    chart.resize();
}

export function disposeQueryDepthTree(containerId) {
    const chart = queryDepthTreeCharts.get(containerId);

    if (chart) {
        if (!chart.isDisposed()) chart.dispose();
        queryDepthTreeCharts.delete(containerId);
    }
}


// ---------------------------------------------------------------------------------
//Dataset-Link
export function renderQuerySchemaDepth(containerId, title, rows) {
    const el = document.getElementById(containerId);
    if (!el || !window.echarts || !rows || !rows.length) return;
    const old = echarts.getInstanceByDom(el);
    if (old) old.dispose();
    const chart = echarts.init(el);

    const maxDepth = Math.max(...rows.map(r => r.depth));
    const bySchema = new Map();
    rows.forEach(r => {
        const k = r.schemaName || '(none)';
        if (!bySchema.has(k)) bySchema.set(k, new Array(maxDepth + 1).fill(0));
        bySchema.get(k)[r.depth]++;
    });
    const schemas = [...bySchema.entries()].sort((a, b) => b[1].reduce((x, y) => x + y, 0) - a[1].reduce((x, y) => x + y, 0));
    const header = ['schema', ...Array.from({ length: maxDepth + 1 }, (_, i) => String(i))];
    const source = [header, ...schemas.map(([name, counts]) => [name, ...counts])];

    const pieLabel = dim => ({ formatter: '{b}: {@[' + dim + ']} ({d}%)' });
    chart.setOption({
        title: {
            text: "JEO3 Metric",
            subtext: title,
            left: "center",
            top: 12,
            textStyle: { color: "#f8fafc", fontSize: 24 },
            subtextStyle: { color: "#94a3b8" }
        },
        backgroundColor: '#000',
        textStyle: { color: '#ccc' },
        legend: { type: 'scroll', bottom: 10, textStyle: { color: '#ccc' } },
        tooltip: { trigger: 'axis', showContent: false },
        dataset: { source },
        xAxis: { type: 'category', name: 'Depth', nameLocation: 'middle', nameGap: 28 },
        yAxis: { gridIndex: 0, name: 'Tables', minInterval: 1 },
        grid: {
            top: '55%', bottom: 100,
            left: 50,           // Reduce left margin
            containLabel: true  // Measure the text labels so they never overflow the canvas
        },
        series: [
            ...schemas.map(() => ({ type: 'line', smooth: true, seriesLayoutBy: 'row', emphasis: { focus: 'series' } })),
            { type: 'pie', id: 'pie', radius: '30%', top: 30, center: ['50%', '25%'], label: pieLabel(1), encode: { itemName: 'schema', value: 1, tooltip: 1 } }
        ]
    });

    chart.on('updateAxisPointer', e => {
        const info = e.axesInfo && e.axesInfo[0];
        if (!info) return;
        const dim = info.value + 1;
        chart.setOption({ series: { id: 'pie', label: pieLabel(dim), encode: { value: dim, tooltip: dim } } });
    });

}

export function disposeQuerySchemaDepth(containerId) {
    const el = document.getElementById(containerId);
    const c = el && window.echarts ? echarts.getInstanceByDom(el) : null;
    if (c) c.dispose();
}


// ---------------------------------------------------------------------------------
// Circular
export function renderQueryCircular(containerId, title, nodes) {
    const el = document.getElementById(containerId);
    if (!el || !window.echarts || !nodes || !nodes.length) return;
    const old = echarts.getInstanceByDom(el);
    if (old) old.dispose();
    const chart = echarts.init(el);

    const schemas = [...new Set(nodes.map(n => n.schemaName || '(none)'))].sort();
    const degree = new Map(nodes.map(n => [n.id, 0]));
    const links = [];
    nodes.forEach(n => {
        if (n.parentId && degree.has(n.parentId)) {
            links.push({ source: n.parentId, target: n.id });
            degree.set(n.id, degree.get(n.id) + 1);
            degree.set(n.parentId, degree.get(n.parentId) + 1);
        }
    });
    const data = nodes.map(n => ({
        id: n.id,
        name: (n.schemaName ? n.schemaName + '.' : '') + n.tableName,
        category: schemas.indexOf(n.schemaName || '(none)'),
        symbolSize: n.isRoot ? 40 : Math.min(8 + degree.get(n.id) * 4, 32),
        value: n.depth,
        label: { show: true }
    }));

    chart.setOption({
        backgroundColor: '#000',
        title: {
            text: "JEO3 Metric",
            subtext: title,
            left: "center",
            top: 12,
            textStyle: { color: "#f8fafc", fontSize: 24 },
            subtextStyle: { color: "#94a3b8" }
        },
        tooltip: { formatter: p => p.dataType === 'node' ? p.name + '<br/>Depth: ' + p.value : '' },
        legend: [{ data: schemas, bottom: 8, textStyle: { color: '#ccc' } }],
        animationDuration: 1500,
        animationEasingUpdate: 'quinticInOut',
        series: [{
            type: 'graph', layout: 'circular', circular: { rotateLabel: true },
            zoom: 0.7, 
            data, links, categories: schemas.map(name => ({ name })),
            roam: true, label: { position: 'right', formatter: '{b}', color: '#ccc' },
            lineStyle: { color: 'source', curveness: 0.3, opacity: 0.7 },
            emphasis: { focus: 'adjacency', lineStyle: { width: 6 } }
        }]
    });
}

export function disposeQueryCircular(containerId) {
    const el = document.getElementById(containerId);
    const c = el && window.echarts ? echarts.getInstanceByDom(el) : null;
    if (c) c.dispose();
}


// ---------------------------------------------------------------------------------
// Treemap


const groupedTreemaps = new Map();

const DEFAULT_TREEMAP_PALETTE = ["#39ff14", "#00d9ff", "#ff3b6b", "#b388ff", "#ffd54f", "#ff8a3d", "#4dffdf", "#7c8cff"];

export function renderGroupedTreemap(containerId, title, items, palette, dotNetRef) {
    const container = document.getElementById(containerId);
    if (!container || typeof echarts === "undefined") return;

    let chart = echarts.getInstanceByDom(container);
    if (!chart) chart = echarts.init(container);

    const colors = palette && palette.length ? palette : DEFAULT_TREEMAP_PALETTE;
    const colorOf = new Map();
    const colorFor = key => {
        if (!colorOf.has(key)) colorOf.set(key, colors[colorOf.size % colors.length]);
        return colorOf.get(key);
    };

    // Items with a groupKey nest under a group node; items without one stand alone, each with its own color.
    const groups = new Map();
    const loose = [];
    let total = 0;

    for (const it of items || []) {
        if (!it || !it.name) continue;
        total += it.value;
        const color = colorFor(it.groupKey || it.name);
        const leaf = { name: it.name, value: it.value, subLabel: it.subLabel, index: it.index, itemStyle: { color } };

        if (!it.groupKey) { loose.push(leaf); continue; }
        if (!groups.has(it.groupKey)) groups.set(it.groupKey, { name: it.groupKey, itemStyle: { color }, children: [] });
        groups.get(it.groupKey).children.push(leaf);
    }

    const fmt = v => Number(v || 0).toLocaleString("en-US", { maximumFractionDigits: 2 });
    const mono = "Consolas, monospace";

    chart.setOption({
        backgroundColor: "#101318",
        animationDurationUpdate: 450,
        title: { text: title || "Treemap", left: "center", top: 8, textStyle: { color: "#e5eaf2", fontSize: 17, fontWeight: 500, fontFamily: mono } },
        tooltip: {
            confine: true,
            backgroundColor: "rgba(15, 19, 28, 0.96)",
            borderColor: "#56677f",
            textStyle: { color: "#edf3ff", fontFamily: mono },
            formatter: p => {
                const d = p.data || {};
                const pct = total > 0 ? (Number(p.value) / total * 100).toFixed(1) : "0.0";
                return [`<strong>${escapeHtml(d.name || "")}</strong>`, d.subLabel ? escapeHtml(d.subLabel) : fmt(p.value), `${pct}% of total`].join("<br/>");
            }
        },
        series: [{
            type: "treemap",
            left: 8, right: 8, top: 46, bottom: 26,
            data: [...groups.values(), ...loose],
            roam: false,
            nodeClick: "zoomToNode",
            breadcrumb: { show: true, height: 20, bottom: 2, itemStyle: { color: "#252c38", borderColor: "#101318", borderWidth: 1 }, textStyle: { color: "#cbd5e1", fontSize: 10 } },
            squareRatio: 1.35,
            label: { show: true, color: "#f5f7fb", fontFamily: mono, fontSize: 11, overflow: "truncate", formatter: p => p.data && p.data.subLabel ? `${p.name}\n${p.data.subLabel}` : p.name },
            upperLabel: { show: true, height: 18, color: "#f5f7fb", fontSize: 11, fontFamily: mono },
            itemStyle: { borderColor: "#101318", borderWidth: 2, gapWidth: 2 },
            emphasis: { itemStyle: { borderColor: "#ffffff", borderWidth: 2 } },
            levels: [{ itemStyle: { borderWidth: 0, gapWidth: 4 } }, { itemStyle: { borderWidth: 2, gapWidth: 2 } }]
        }]
    }, true);

    // Leaves carry the index of the original item; group nodes don't, so only cell clicks call back.
    chart.off("click");
    chart.on("click", p => {
        if (dotNetRef && p.data && p.data.index !== undefined)
            dotNetRef.invokeMethodAsync("HandleCellClick", p.data.index);
    });

    const resizeObserver = new ResizeObserver(() => { if (!chart.isDisposed()) chart.resize(); });
    resizeObserver.observe(container);

    const previous = groupedTreemaps.get(containerId);
    if (previous) previous.resizeObserver.disconnect();
    groupedTreemaps.set(containerId, { chart, resizeObserver });
}

export function disposeGroupedTreemap(containerId) {
    const state = groupedTreemaps.get(containerId);

    if (state) {
        state.resizeObserver.disconnect();
        if (!state.chart.isDisposed()) state.chart.dispose();
        groupedTreemaps.delete(containerId);
    }
}
