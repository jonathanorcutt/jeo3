
// ---------------------------------------------------------------------------------
// Contour / Schema Complexity Bar Chart
export function renderPanoramic3DChart(containerId, title, xAxisTitle, yAxisTitle, intensityLabel, xAxisData, yAxisData, chartData) {
    var chartContainer = document.getElementById(containerId);
    if (!chartContainer) return;

    var chartInstance = echarts.getInstanceByDom(chartContainer) || echarts.init(chartContainer);

    chartInstance.setOption({
        backgroundColor: "#0a0c10",
        title: {
            text: title,
            textStyle: { color: "#ffffff", fontFamily: "Consolas, monospace", fontSize: 14, fontWeight: "bold" },
            left: "center",
            top: "1%"
        },
        tooltip: {
            trigger: "item",
            backgroundColor: "rgba(10, 12, 16, 0.92)",
            borderColor: "#00ff41",
            borderWidth: 1,
            textStyle: { color: "#ffffff", fontFamily: "Consolas, monospace", fontSize: 11 },
            formatter: function (params) {
                var xName = xAxisData[params.value[0]] || params.value[0];
                var yName = yAxisData[params.value[1]] || params.value[1];
                var zVal = params.value[2].toFixed(3);
                return '<strong>' + params.seriesName + '</strong><br/>' +
                    xAxisTitle + ': ' + xName + '<br/>' +
                    yAxisTitle + ': ' + yName + '<br/>' +
                    intensityLabel + ': ' + zVal;
            }
        },
        visualMap: {
            type: "continuous",
            max: 1.0,
            min: 0.0,
            calculable: true,
            orient: "vertical",
            right: "3%",
            bottom: "15%",
            dimension: 2,
            text: ["High", "Low"],
            textStyle: { color: "#00ff41", fontFamily: "Consolas, monospace" },
            inRange: { color: ["#1e3a8a", "#2563eb", "#22c55e", "#facc15", "#ef4444"] }
        },
        xAxis3D: {
            type: "category",
            data: xAxisData,
            name: xAxisTitle,
            nameTextStyle: { color: "#00ff41", fontFamily: "Consolas, monospace" },
            axisLabel: { show: true, color: "#ffffff", fontFamily: "Consolas, monospace", interval: 0 }
        },
        yAxis3D: {
            type: "category",
            data: yAxisData,
            name: yAxisTitle,
            nameTextStyle: { color: "#00ff41", fontFamily: "Consolas, monospace" },
            axisLabel: { show: true, color: "#ffffff", fontFamily: "Consolas, monospace", interval: 0 }
        },
        zAxis3D: {
            type: "value",
            min: 0.0,
            max: 1.0,
            name: intensityLabel,
            nameTextStyle: { color: "#00ff41", fontFamily: "Consolas, monospace" },
            axisLabel: { show: true, color: "#ffffff", fontFamily: "Consolas, monospace" }
        },
        grid3D: {
            boxWidth: 160,
            boxHeight: 65,
            boxDepth: 180,
            top: -70,
            bottom: 0,
            shading: "lambert",
            viewControl: { autoRotate: false, alpha: 25, beta: 40, distance: 190 },
            light: {
                main: { intensity: 1.3, shadow: true, alpha: 40, beta: 40 },
                ambient: { intensity: 0.4 }
            }
        },
        series: [{
            type: "bar3D",
            name: title,
            data: chartData,
            shading: "lambert",
            label: { show: false },
            itemStyle: { opacity: 0.95 }
        }]
    }, true);
};


// ---------------------------------------------------------------------------------
// ECharts Voxel Cinematic Renderer
export function renderVox3DChart(containerId, title, xAxisTitle, yAxisTitle, intensityLabel, xAxisData, yAxisData, imageSrcPath) {
    var chartContainer = document.getElementById(containerId);
    if (!chartContainer) return;

    var chartInstance = echarts.getInstanceByDom(chartContainer) || echarts.init(chartContainer);

    // Structural look variables
    var config = {
        scale: 0.15,
        roughness: 0,
        metalness: 0.0,
        projection: 'perspective',
        depthOfField: 1,
        lockY: false,
        move: true,
        sameColor: false,
        color: '#777',
        colorContrast: 1.0,
        barNumber: 80,
        barBevel: 0.18,
        barSize: 1.5
    };

    var canvas = document.createElement('canvas');
    var ctx = canvas.getContext('2d');

    // 2. Baseline environmental view (SSAO Post-processing and starting camera tilt)
    var initialOption = {
        tooltip: {},
        backgroundColor: '#0a0c10',
        xAxis3D: { type: 'value' },
        yAxis3D: { type: 'value' },
        zAxis3D: { type: 'value', min: 0, max: 100 },
        animationThreshold: 99999,
        animationDurationUpdate: 10000, // Matches fallback timeline
        grid3D: {
            show: false,
            viewControl: {
                projection: config.projection,
                alpha: 0,           // Matches starting flat alignment look
                beta: -360,         // Matches full rotation launch spin
                distance: 8,        // 🌟 FORCE starting camera location right into the mesh strings!
                // targetCoord:, // 🌟 Shifts camera center right into the physical block cluster center
                panSensitivity: config.move ? 1 : 0,
                rotateSensitivity: config.lockY ? [1, 0] : 1,
                damping: 0.95
            },
            postEffect: {
                enable: true,
                bloom: { intensity: 0.2 },
                screenSpaceAmbientOcclusion: {
                    enable: true,
                    intensity: 2.0,
                    radius: 5,
                    quality: 'high'
                },
                screenSpaceReflection: { enable: true },
                depthOfField: {
                    enable: true,
                    blurRadius: config.depthOfField,
                    fstop: 10,
                    focalDistance: 15
                }
            },
            boxDepth: 100,
            boxHeight: 20,
            environment: 'none',
            light: {
                main: { shadow: true, intensity: 2.0 },
                ambient: { intensity: 0.2 }
            }
        }
    };

    chartInstance.setOption(initialOption, true);

    // 3. Canvas image loading loop
    var img = new Image();
    img.onload = function () {
        var height = (canvas.height = Math.min(config.barNumber, img.height));
        var aspect = img.width / img.height;
        var width = (canvas.width = Math.round(height * aspect));

        ctx.drawImage(img, 0, 0, width, height);
        var imgData = ctx.getImageData(0, 0, width, height);
        var pixelData = imgData.data;

        var data = new Float32Array((pixelData.length / 4) * 3);
        var off = 0;

        for (var i = 0; i < pixelData.length / 4; i++) {
            var r = pixelData[i * 4];
            var g = pixelData[i * 4 + 1];
            var b = pixelData[i * 4 + 2];

            var lum = 0.2125 * r + 0.7154 * g + 0.0721 * b;
            lum = (lum - 125) * config.scale + 50;

            data[off++] = i % width;
            data[off++] = height - Math.floor(i / width);
            data[off++] = lum;
        }

        // 4. Inject the completed series definitions explicitly right here!
        chartInstance.setOption({
            grid3D: {
                boxWidth: (100 / height) * width,
                transitionDuration: 10000,
                transitionEasing: 'cubicOut'
            },
            series: [{
                animation: false,
                type: 'bar3D',
                shading: 'realistic',
                realisticMaterial: {
                    roughness: config.roughness,
                    metalness: config.metalness
                },
                barSize: config.barSize,
                bevelSize: config.barBevel,
                silent: true,
                dimensions: ['x', 'y', 'z'],
                itemStyle: {
                    color: config.sameColor ? config.color : function (params) {
                        var idx = params.dataIndex;
                        var pr = pixelData[idx * 4] / 255;
                        var pg = pixelData[idx * 4 + 1] / 255;
                        var pb = pixelData[idx * 4 + 2] / 255;
                        var plum = 0.2125 * pr + 0.7154 * pg + 0.0721 * pb;

                        pr *= plum * config.colorContrast;
                        pg *= plum * config.colorContrast;
                        pb *= plum * config.colorContrast;
                        return [pr, pg, pb, 1];
                    }
                },
                data: data
            }]
        });

        // 5. Run the smooth transition swoop into the flat front angle view
        setTimeout(function () {
            chartInstance.setOption({
                grid3D: {
                    viewControl: {
                        alpha: 90,                  // Perfectly flat head-on pitch
                        beta: 0,                   // Straight alignment
                        distance: 100,              // Front view framing boundary
                        targetCoord: [null, null, null] // 🌟 Resets target back to automatic layout centering
                    }
                }
            });
        }, 2000);
    };

    img.src = imageSrcPath;
}


// ---------------------------------------------------------------------------------
// 3d Schema Earth
const relationSimulations = new Map();

export function renderRelations3DFlights(containerId, title, tablesNodes, foreignKeyLinks) {
    const chartContainer = document.getElementById(containerId);
    if (!chartContainer) { console.warn(`JEO3 3D chart container '${containerId}' was not found.`); return; }
    if (typeof echarts === "undefined") { console.error("JEO3 3D chart: ECharts is not loaded."); return; }

    let chartInstance = echarts.getInstanceByDom(chartContainer);
    if (!chartInstance) { chartInstance = echarts.init(chartContainer); }

    /* Bounds first, so layout x/y can be mapped into lng/lat */
    const validNodes = (tablesNodes || []).filter(node => node && node.id);
    const bounds = calculateBounds(validNodes);
    const spanX = Math.max(1, bounds.maxX - bounds.minX);
    const spanY = Math.max(1, bounds.maxY - bounds.minY);
    const toLngLat = (x, y) => [((x - bounds.minX) / spanX) * 340 - 170, ((y - bounds.minY) / spanY) * 140 - 70];

    /* 2. Node lookup + scatter data, all as [lng, lat, altitude] */
    const nodePositions = Object.create(null);
    const nodeData = [];
    for (const node of validNodes) {
        const [lng, lat] = toLngLat(Number(node.x) || 0, Number(node.y) || 0);
        nodePositions[node.id] = [lng, lat, 0];
        nodeData.push({ name: node.name || node.id, value: [lng, lat, 0] });
    }

    /* 3. Flight paths, split into two groups instead of one series per link */
    const normalPaths = [];
    const hotPaths = [];
    for (const link of foreignKeyLinks || []) {
        if (!link) { continue; }
        const source = nodePositions[link.source];
        const target = nodePositions[link.target];
        if (!source || !target) { continue; }
        const path = createFlightPath(source, target);
        if (path.length < 2) { continue; }
        (link.isHighTraffic ? hotPaths : normalPaths).push(path);
    }

    const linesSeries = (name, data, color, width, opacity) => ({
        name,
        type: "lines3D",
        coordinateSystem: "globe",
        polyline: true,
        blendMode: "lighter",
        data,
        lineStyle: { color, width, opacity },
        distance: 5,
        effect: { show: true, trailWidth: 2, trailLength: 0.15, trailOpacity: 1, trailColor: color }
    });

    var texture = '../../img/branding/world.topo.bathy.200401.jpg';
    var bath = '../../img/branding/bathymetry_bw_composite_4k.jpg';
    var star = '../../img/branding/jeo3.png';

    /* 4. Options */
    const option = {
        backgroundColor: "#000",
        title: {
            text: title || "Database Relations Flight Simulator",
            left: "center",
            top: "1%",
            textStyle: {
                //color: "#ffffff",
                fontSize: 24,
                fontWeight: "bold",
                color: "#08FC17",
                fontFamily: 'NaziTypewriterRegular, consolas',
                textBorderColor: '#000',
                textBorderType: 'solid',
                textBorderWidth: 2
            }
        },
        globe: {
            heightTexture: bath,
            // baseColor: '#000', 
            baseTexture: texture,
            environment: star,
            shading: "realistic",
            displacementScale: 0.1,
            displacementQuality: "high",
            realisticMaterial: { roughness: 0.2, metalness: 0 },
            postEffect: {
                enable: true,
                depthOfField: {
                    enable: false, focalRange: 150,
                    enable: true,
                    focalDistance: 150
                }
            },
            // temporalSuperSampling: { enable: true },
            light: {
                ambient: { intensity: 0.4 },
                main: { intensity: 0.4, shadow: false },
                ambientCubemap: {
                    texture: "../../assets/data/lake.hdr",
                    exposure: 1,
                    diffuseIntensity: 0.5,
                    specularIntensity: 2
                }
            },
            viewControl: {
                autoRotate: true, autoRotateSpeed: 1, distance: 300, minDistance: 125, maxDistance: 500,
                alpha: 12, beta: 67
                // alpha: 0, beta: 0
            },
            silent: true
        },
        yAxis3D:
        {
            offset: -180
        },
        series: [
            {
                name: "Tables",
                type: "scatter3D",
                distance: 8,
                coordinateSystem: "globe",
                data: nodeData,
                symbol: "circle",
                symbolSize: 10,
                itemStyle: { color: "#0077F2", opacity: 0.95 },
                label: {
                    show: true,
                    formatter: p => p.data.name,
                    textStyle: {
                        color: "#08FC17", fontFamily: "Consolas, monospace", fontSize: 13,
                        fontFamily: 'NaziTypewriterRegular, consolas',
                        textBorderColor: '#000',
                        textBorderType: 'solid',
                        textBorderWidth: 2
                    }
                }
            },
            linesSeries("Normal", normalPaths, "#00ff41", 1, 0.55),
            linesSeries("High Traffic", hotPaths, "#ff00ff", 2.5, 0.9)
        ]
    };

    /* One setOption, one time. No animation loop. */
    chartInstance.setOption(option, true);

    requestAnimationFrame(() => {
        if (!chartInstance.isDisposed()) { chartInstance.resize(); }
    });
}

/* Arc between two [lng, lat, alt] points. Altitude is in globe units (radius is about 100). */
function createFlightPath(source, target) {
    const dx = target[0] - source[0];
    const dy = target[1] - source[1];
    const distance = Math.sqrt((dx * dx) + (dy * dy));

    // const arcHeight = Math.max(4, Math.min(20, distance * 0.08));
    const arcHeight = 1;
    const steps = 32;
    const path = [];

    for (let i = 0; i <= steps; i++) {
        const t = i / steps;
        const arc = 1 * t * (1 - t);
        path.push([source[0] + (dx * t), source[1] + (dy * t), source[2] + ((target[2] - source[2]) * t) + (arcHeight * arc)]);
    }

    return path;
}

/* Bounds of the layout x/y, used to map into lng/lat */
function calculateBounds(nodes) {
    if (!nodes || nodes.length === 0) {
        return { minX: -120, maxX: 120, minY: -120, maxY: 120 };
    }

    let minX = Infinity, maxX = -Infinity, minY = Infinity, maxY = -Infinity;

    for (const node of nodes) {
        const x = Number(node.x) || 0;
        const y = Number(node.y) || 0;
        minX = Math.min(minX, x);
        maxX = Math.max(maxX, x);
        minY = Math.min(minY, y);
        maxY = Math.max(maxY, y);
    }

    const paddingX = Math.max(20, (maxX - minX) * 0.10);
    const paddingY = Math.max(20, (maxY - minY) * 0.10);

    return { minX: minX - paddingX, maxX: maxX + paddingX, minY: minY - paddingY, maxY: maxY + paddingY };
}

export function disposeRelations3D(containerId) {
    const chartContainer = document.getElementById(containerId);
    if (!chartContainer) { return; }

    const chartInstance = echarts.getInstanceByDom(chartContainer);
    if (chartInstance) { chartInstance.dispose(); }
}



// ---------------------------------------------------------------------------------

// Scatter 3D Simplex
export function renderQuerySchemaDepth3D(containerId, title, payload) {
    const dom = document.getElementById(containerId);
    if (!dom || !payload?.length) return;

    let chart = echarts.getInstanceByDom(dom);
    if (!chart) chart = echarts.init(dom, 'dark');

    const schemas = [...new Set(payload.map(p => p.schemaName || '(default)'))]
        .sort((a, b) => a.localeCompare(b));

    const depths = [...new Set(payload.map(p => p.depth))]
        .sort((a, b) => a - b);

    const maxCount = Math.max(1, ...payload.map(p => p.tableCount));

    const data = payload.map(p => [
        schemas.indexOf(p.schemaName || '(default)'),
        p.depth,
        p.tableCount,
        p.schemaName || '(default)'
    ]);

    chart.setOption({
        backgroundColor: '#000',
        title: {
            text: title || 'JEO3 Schema Depth',
            left: 'center',
            top: 12,
            textStyle: { color: '#e6b94a', fontSize: 16 }
        },
        tooltip: {
            formatter: params => {
                const d = params.value;
                return `<b>${d[3]}</b><br/>Depth: ${d[1]}<br/>Table count: ${d[2]}`;
            }
        },
        visualMap: {
            min: 0,
            max: maxCount,
            dimension: 2,
            calculable: true,
            orient: 'horizontal',
            left: 'center',
            bottom: 12,
            textStyle: { color: '#ddd' },
            inRange: {
                color: ['#163b65', '#167d9a', '#e6b94a', '#fff0a8']
            }
        },
        xAxis3D: {
            type: 'category',
            name: 'Schema',
            data: schemas,
            axisLabel: { color: '#c7d0dc', interval: 0 },
            nameTextStyle: { color: '#e6b94a' }
        },
        yAxis3D: {
            type: 'value',
            name: 'Depth',
            min: Math.min(0, ...depths),
            max: Math.max(1, ...depths),
            axisLabel: { color: '#c7d0dc' },
            nameTextStyle: { color: '#e6b94a' }
        },
        zAxis3D: {
            type: 'value',
            name: 'Table Count',
            axisLabel: { color: '#c7d0dc' },
            nameTextStyle: { color: '#e6b94a' }
        },
        grid3D: {
            boxWidth: 180,
            boxDepth: 100,
            viewControl: {
                projection: 'perspective',
                autoRotate: false,
                rotateSensitivity: 1,
                zoomSensitivity: 1,
                panSensitivity: 1
            },
            light: {
                main: { intensity: 1.1, shadow: true },
                ambient: { intensity: 0.35 }
            },
            axisLine: { lineStyle: { color: '#66758a' } },
            splitLine: { lineStyle: { color: '#283343' } }
        },
        series: [{
            type: 'scatter3D',
            data,
            symbolSize: value => Math.max(6, Math.min(24, value[2] * 3 + 5)),
            encode: { x: 0, y: 1, z: 2 },
            itemStyle: {
                opacity: 0.9,
                borderWidth: 1,
                borderColor: '#f4d77b'
            },
            emphasis: {
                itemStyle: {
                    opacity: 1,
                    borderWidth: 2,
                    borderColor: '#fff'
                }
            }
        }]
    }, true);

    chart.resize();
}

export function disposeQuerySchemaDepth3D(containerId) {
    const dom = document.getElementById(containerId);
    if (!dom) return;

    const chart = echarts.getInstanceByDom(dom);
    if (chart) chart.dispose();
}


// ---------------------------------------------------------------------------------

// 3d Heat v2
const panoramicEntityHeatmap3DCharts = new Map();

export function renderPanoramicEntityHeatmap3D(
    containerId,
    title,
    xAxisTitle,
    yAxisTitle,
    intensityLabel,
    xAxisCategories,
    yAxisCategories,
    chartData
) {
    const element = document.getElementById(containerId);

    if (!element || !window.echarts) {
        return;
    }

    let chart = panoramicEntityHeatmap3DCharts.get(containerId);

    if (!chart || chart.isDisposed()) {
        chart = window.echarts.init(element);
        panoramicEntityHeatmap3DCharts.set(containerId, chart);
    }

    chart.setOption({
        backgroundColor: 'transparent',

        title: {
            text: title,
            left: 'center',
            top: 8,
            textStyle: {
                color: '#e0e0e0',
                fontSize: 15,
                fontWeight: 'normal'
            }
        },

        tooltip: {
            formatter: function (params) {
                const value = params.value;

                if (!value || value.length < 3) {
                    return '';
                }

                const entityType = xAxisCategories[value[0]] ?? value[0];
                const table = yAxisCategories[value[1]] ?? value[1];
                const count = value[2];

                return `<strong>${table}</strong><br/>${entityType}: ${count}`;
            }
        },

        visualMap: {
            show: true,
            min: 0,
            max: Math.max(
                1,
                ...chartData.map(point => Number(point[2]) || 0)
            ),
            dimension: 2,
            inRange: {
                color: [
                    '#163b65',
                    '#167da8',
                    '#36c5c0',
                    '#f0d64e',
                    '#ff8738'
                ]
            },
            textStyle: {
                color: '#d0d0d0'
            },
            calculable: true,
            right: 5,
            top: 'middle'
        },

        xAxis3D: {
            type: 'category',
            name: xAxisTitle,
            data: xAxisCategories,
            nameTextStyle: { color: '#d0d0d0' },
            axisLabel: {
                color: '#bcbcbc',
                interval: 0,
                rotate: 25
            },
            axisLine: {
                lineStyle: { color: '#666666' }
            }
        },

        yAxis3D: {
            type: 'category',
            name: yAxisTitle,
            data: yAxisCategories,
            nameTextStyle: { color: '#d0d0d0' },
            axisLabel: {
                color: '#bcbcbc'
            },
            axisLine: {
                lineStyle: { color: '#666666' }
            }
        },

        zAxis3D: {
            type: 'value',
            name: intensityLabel,
            min: 0,
            nameTextStyle: { color: '#d0d0d0' },
            axisLabel: {
                color: '#bcbcbc'
            },
            axisLine: {
                lineStyle: { color: '#666666' }
            },
            splitLine: {
                lineStyle: { color: 'rgba(150,150,150,0.2)' }
            }
        },

        grid3D: {
            top: -35,
            bottom: 0,
            left: 10,
            right: 75,
            boxWidth: 180,
            boxDepth: Math.max(100, Math.min(300, yAxisCategories.length * 4)),
            environment: 'auto',
            axisPointer: {
                show: true,
                lineStyle: { color: '#f0d64e' }
            },
            viewControl: {
                projection: 'perspective',
                autoRotate: false,
                rotateSensitivity: 1,
                zoomSensitivity: 1,
                panSensitivity: 1,
                distance: 240,
                alpha: 25,
                beta: 40
            },
            light: {
                main: {
                    intensity: 1.2,
                    shadow: true,
                    alpha: 35,
                    beta: 40
                },
                ambient: {
                    intensity: 0.35
                }
            }
        },

        series: [
            {
                name: intensityLabel,
                type: 'bar3D',
                data: chartData,
                shading: 'lambert',
                barSize: 0.8,
                bevelSize: 0.15,
                minHeight: 0.2,
                itemStyle: {
                    opacity: 0.95
                },
                emphasis: {
                    itemStyle: {
                        opacity: 1
                    }
                }
            }
        ]
    }, true);

    chart.resize();
}

export function disposePanoramicEntityHeatmap3D(containerId) {
    const chart = panoramicEntityHeatmap3DCharts.get(containerId);

    if (chart && !chart.isDisposed()) {
        chart.dispose();
    }

    panoramicEntityHeatmap3DCharts.delete(containerId);
}


// ---------------------------------------------------------------------------------

// North Atlantic Current / Like a Hurricane


const relationCharts2D = new Map();

export function renderRelations2DFlights(
    containerId,
    title,
    tablesNodes,
    foreignKeyLinks
) {
    const container = document.getElementById(containerId);

    if (!container) {
        console.warn(`JEO3 2D chart container '${containerId}' was not found.`);
        return;
    }

    if (typeof echarts === "undefined") {
        console.error("JEO3 2D chart: ECharts is not loaded.");
        return;
    }

    disposeRelations2D(containerId);

    const chart = echarts.init(container);
    const nodes = (tablesNodes || []).filter(n => n && n.id);

    if (nodes.length === 0) {
        chart.dispose();
        return;
    }

    const nodeById = new Map(nodes.map(n => [n.id, n]));

    // Normalize stable layout coordinates to the available 2D viewport.
    const xs = nodes.map(n => Number(n.x) || 0);
    const ys = nodes.map(n => Number(n.y) || 0);

    const minX = Math.min(...xs);
    const maxX = Math.max(...xs);
    const minY = Math.min(...ys);
    const maxY = Math.max(...ys);

    const spanX = Math.max(1, maxX - minX);
    const spanY = Math.max(1, maxY - minY);

    const graphNodes = nodes.map(node => ({
        id: node.id,
        name: node.name || node.id,
        x: 50 + ((Number(node.x) - minX) / spanX) * 900,
        y: 50 + ((Number(node.y) - minY) / spanY) * 500,
        symbolSize: 7,
        itemStyle: {
            color: "#72dfff",
            shadowBlur: 8,
            shadowColor: "rgba(0, 190, 255, 0.55)"
        },
        label: {
            show: true,
            position: "right",
            distance: 6,
            color: "rgba(155, 224, 245, 0.8)",
            fontSize: 10,
            fontFamily: "Consolas, monospace",
            textBorderColor: "#02060b",
            textBorderWidth: 2
        }
    }));

    const links = [];

    for (const link of foreignKeyLinks || []) {
        if (!link ||
            !nodeById.has(link.source) ||
            !nodeById.has(link.target)) {
            continue;
        }

        links.push({
            source: link.source,
            target: link.target,
            isHighTraffic: !!link.isHighTraffic,
            lineStyle: {
                color: link.isHighTraffic
                    ? "rgba(255, 50, 210, 0.65)"
                    : "rgba(0, 185, 255, 0.35)",
                width: link.isHighTraffic ? 1.6 : 0.8,
                curveness: 0.18,
                opacity: 0.8
            }
        });
    }

    // Deterministic background noise: no random work on every frame.
    const noiseCanvas = document.createElement("canvas");
    noiseCanvas.width = 256;
    noiseCanvas.height = 256;

    const ctx = noiseCanvas.getContext("2d");
    const image = ctx.createImageData(256, 256);

    let seed = 90317;

    for (let i = 0; i < image.data.length; i += 4) {
        seed = (seed * 1664525 + 1013904223) >>> 0;
        const grain = seed & 255;

        image.data[i] = 35;
        image.data[i + 1] = 65;
        image.data[i + 2] = 85;
        image.data[i + 3] = grain > 224 ? 16 : 3;
    }

    ctx.putImageData(image, 0, 0);

    const noiseTexture = noiseCanvas.toDataURL("image/png");

    const option = {
        backgroundColor: "#03070d",
        animation: true,
        animationDuration: 900,
        animationDurationUpdate: 900,
        animationEasingUpdate: "linear",

        title: {
            text: title || "Schema Relations — Traffic Flow",
            left: "center",
            top: 12,
            textStyle: {
                color: "#72dfff",
                fontSize: 18,
                fontWeight: "normal",
                fontFamily: "Consolas, monospace",
                textShadowColor: "rgba(0, 180, 255, 0.4)",
                textShadowBlur: 12
            }
        },

        graphic: [
            {
                type: "rect",
                left: 0,
                top: 0,
                right: 0,
                bottom: 0,
                silent: true,
                z: -10,
                style: {
                    fill: {
                        image: noiseTexture,
                        repeat: "repeat"
                    },
                    opacity: 0.6
                }
            }
        ],

        tooltip: {
            trigger: "item",
            backgroundColor: "rgba(3, 12, 24, 0.94)",
            borderColor: "#147c9e",
            textStyle: {
                color: "#b9efff",
                fontFamily: "Consolas, monospace"
            },
            formatter: item => {
                if (item.dataType === "edge") {
                    return `${item.data.source}<br/>→ ${item.data.target}`;
                }

                return item.data.name || item.name;
            }
        },

        series: [
            {
                name: "Schema Relations",
                type: "graph",
                layout: "none",
                coordinateSystem: null,
                roam: true,
                draggable: false,
                data: graphNodes,
                links,
                symbol: "circle",
                edgeSymbol: ["none", "none"],
                lineStyle: {
                    curveness: 0.18
                },
                emphasis: {
                    focus: "adjacency",
                    lineStyle: {
                        width: 2.5,
                        opacity: 1
                    },
                    itemStyle: {
                        shadowBlur: 18,
                        shadowColor: "#00c8ff"
                    }
                }
            }
        ]
    };

    chart.setOption(option);

    // Animated traffic overlay: lightweight Canvas, independent of ECharts.
    const overlay = document.createElement("canvas");
    overlay.style.cssText =
        "position:absolute;inset:0;width:100%;height:100%;" +
        "pointer-events:none;z-index:2;";

    // Keep the overlay aligned with the chart container.
    if (getComputedStyle(container).position === "static") {
        container.style.position = "relative";
    }

    container.appendChild(overlay);

    const overlayCtx = overlay.getContext("2d");
    let frameId = 0;
    let disposed = false;
    let lastTime = 0;
    let elapsed = 0;
    let width = 0;
    let height = 0;

    const flights = links.map((link, index) => ({
        link,
        phase: (index * 0.61803398875) % 1,
        speed: link.isHighTraffic ? 0.035 : 0.018,
        size: link.isHighTraffic ? 2.1 : 1.2
    }));

    function resizeOverlay() {
        const rect = container.getBoundingClientRect();
        const dpr = Math.min(window.devicePixelRatio || 1, 2);

        width = rect.width;
        height = rect.height;

        overlay.width = Math.round(width * dpr);
        overlay.height = Math.round(height * dpr);

        overlayCtx.setTransform(dpr, 0, 0, dpr, 0, 0);
    }

    function drawNoise(time) {
        // Sparse, slow-moving grain glints.
        overlayCtx.save();
        overlayCtx.globalCompositeOperation = "lighter";

        for (let i = 0; i < 36; i++) {
            const x = ((i * 127.1) % 997) / 997 * width;
            const y = ((i * 311.7) % 991) / 991 * height;
            const pulse = (Math.sin(time * 0.00035 + i * 1.7) + 1) / 2;

            overlayCtx.fillStyle = `rgba(48,180,220,${0.015 + pulse * 0.025})`;
            overlayCtx.fillRect(x, y, 1, 1);
        }

        overlayCtx.restore();
    }

    function drawFlights(delta, time) {
        const chartRect = container.getBoundingClientRect();

        // Convert ECharts graph coordinates to container pixels.
        const plotWidth = 1000;
        const plotHeight = 600;
        const scaleX = width / plotWidth;
        const scaleY = height / plotHeight;

        overlayCtx.save();
        overlayCtx.globalCompositeOperation = "lighter";
        overlayCtx.lineCap = "round";

        for (const flight of flights) {
            const source = nodeById.get(flight.link.source);
            const target = nodeById.get(flight.link.target);

            if (!source || !target) continue;

            const x1 = (50 + ((Number(source.x) - minX) / spanX) * 900) * scaleX;
            const y1 = (50 + ((Number(source.y) - minY) / spanY) * 500) * scaleY;
            const x2 = (50 + ((Number(target.x) - minX) / spanX) * 900) * scaleX;
            const y2 = (50 + ((Number(target.y) - minY) / spanY) * 500) * scaleY;

            // Match the subtle curve used by the graph edges.
            const dx = x2 - x1;
            const dy = y2 - y1;
            const bend = 0.18;
            const cx = (x1 + x2) / 2 - dy * bend;
            const cy = (y1 + y2) / 2 + dx * bend;

            flight.phase = (flight.phase + delta * flight.speed / 1000) % 1;

            const t = flight.phase;
            const oneMinusT = 1 - t;

            const x = oneMinusT * oneMinusT * x1 +
                2 * oneMinusT * t * cx + t * t * x2;
            const y = oneMinusT * oneMinusT * y1 +
                2 * oneMinusT * t * cy + t * t * y2;

            const color = flight.link.isHighTraffic
                ? "255,50,210"
                : "0,200,255";

            const radius = flight.size;

            // Soft halo.
            const glow = overlayCtx.createRadialGradient(
                x, y, 0, x, y, radius * 5
            );

            glow.addColorStop(0, `rgba(${color},0.85)`);
            glow.addColorStop(0.25, `rgba(${color},0.28)`);
            glow.addColorStop(1, `rgba(${color},0)`);

            overlayCtx.fillStyle = glow;
            overlayCtx.beginPath();
            overlayCtx.arc(x, y, radius * 5, 0, Math.PI * 2);
            overlayCtx.fill();

            // Small luminous particle.
            overlayCtx.fillStyle = `rgba(${color},0.95)`;
            overlayCtx.beginPath();
            overlayCtx.arc(x, y, radius, 0, Math.PI * 2);
            overlayCtx.fill();
        }

        overlayCtx.restore();
    }

    function animate(time) {
        if (disposed) return;

        const delta = lastTime === 0 ? 16 : Math.min(time - lastTime, 50);
        lastTime = time;
        elapsed += delta;

        overlayCtx.clearRect(0, 0, width, height);

        drawNoise(elapsed);
        drawFlights(delta, elapsed);

        frameId = requestAnimationFrame(animate);
    }

    resizeOverlay();
    frameId = requestAnimationFrame(animate);

    const resizeObserver = new ResizeObserver(() => {
        chart.resize();
        resizeOverlay();
    });

    resizeObserver.observe(container);

    relationCharts2D.set(containerId, {
        chart,
        overlay,
        resizeObserver,
        stop: () => {
            disposed = true;
            cancelAnimationFrame(frameId);
            resizeObserver.disconnect();
            overlay.remove();
        }
    });
}

export function disposeRelations2D(containerId) {
    const state = relationCharts2D.get(containerId);

    if (state) {
        state.stop();
        state.chart.dispose();
        relationCharts2D.delete(containerId);
    }

    const container = document.getElementById(containerId);

    if (container) {
        const chart = echarts.getInstanceByDom(container);

        if (chart) chart.dispose();
    }
}



// ---------------------------------------------------------------------------------

// Buildings - Doesnt Currently Work
export async function renderPanoramicBuildings3D(containerId, title, geoJsonPath, tableMetrics) {
    var chartContainer = document.getElementById(containerId);
    if (!chartContainer) return;

    var chartInstance = echarts.getInstanceByDom(chartContainer) || echarts.init(chartContainer);

    try {
        var response = await fetch(geoJsonPath);
        var buildingsGeoJson = await response.json();

        echarts.registerMap('buildings', buildingsGeoJson);

        // Convert incoming metrics collection into a quick-lookup map array
        // keyed completely on dynamic TablePath identifiers
        const metricLookup = {};
        if (tableMetrics && Array.isArray(tableMetrics)) {
            tableMetrics.forEach(m => {
                metricLookup[m.tablePath] = m.value;
            });
        }

        var regionsData = buildingsGeoJson.features.map(function (feature) {
            const featureName = feature.properties.name;
            // Extract the live complex metrics value from database model instance, fallback to 0.1 if missing
            const activeIntensity = metricLookup[featureName] !== undefined ? metricLookup[featureName] : 0.1;

            return {
                name: featureName,
                value: activeIntensity,
                // Scales the physical height dynamically based on live database telemetry matrix
                height: (feature.properties.height * activeIntensity) / 10
            };
        });

        chartInstance.setOption({
            backgroundColor: "#0a0c10",
            title: {
                text: title,
                textStyle: { color: "#ffffff", fontFamily: "Consolas, monospace", fontSize: 14, fontWeight: "bold" },
                left: "center",
                top: "1%"
            },
            visualMap: {
                show: true,
                min: 0.0,
                max: 1.0,
                orient: "vertical",
                right: "3%",
                bottom: "15%",
                text: ["High", "Low"],
                textStyle: { color: "#00ff41", fontFamily: "Consolas, monospace" },
                inRange: {
                    color: [
                        '#313695', '#4575b4', '#74add1', '#abd9e9', '#e0f3f8',
                        '#ffffbf', '#fee090', '#fdae61', '#f46d43', '#d73027', '#a50026'
                    ]
                }
            },
            series: [{
                type: 'map3D',
                map: 'buildings',
                shading: 'realistic',
                environment: '#0a0c10',
                realisticMaterial: { roughness: 0.6, textureTiling: 20 },
                postEffect: {
                    enable: true,
                    SSAO: { enable: true, radius: 1, intensity: 1.2 }
                },
                viewControl: { alpha: 40, beta: 180, distance: 120, autoRotate: false },
                itemStyle: { opacity: 1.0 },
                data: regionsData
            }]
        }, true);

        setTimeout(function () {
            chartInstance.setOption({ series: [{ silent: true, progressive: true, progressiveThreshold: 500 }] });
        }, 2500);

    } catch (err) {
        console.error("Failed to execute urban WebGL maps render pass: ", err);
    }
}

