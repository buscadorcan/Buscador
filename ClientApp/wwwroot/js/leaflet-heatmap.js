var map;
var heatLayer;

window.initMap = function () {
    map = L.map('map').setView([37.7749, -122.4194], 5);

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; OpenStreetMap contributors'
    }).addTo(map);

    heatLayer = new HeatmapOverlay({
        radius: 20, // Reducido para que no se expanda demasiado
        maxOpacity: 0.75,
        scaleRadius: true, // Escala el radio según el nivel de zoom
        useLocalExtrema: true, // Importante para que los puntos se muestren con intensidad real
        latField: 'lat',
        lngField: 'lng',
        valueField: 'intensity'
    });

    map.addLayer(heatLayer);

    // Evento para detectar cambios de zoom
    //map.on('zoomend', function () {
    //    window.onZoomChanged();
    //});
};

window.addMarker = function (lat, lng, text) {
    L.marker([lat, lng])
        .addTo(map)
        .bindPopup(text);
};

window.addHeatmapData = function (data) {
    if (!heatLayer) {
        console.error("heatLayer no ha sido inicializado.");
        return;
    }

    // Filtrar solo puntos válidos con intensidad mayor a 0
    var filteredData = data.filter(point => point.lat && point.lng && point.intensity > 0);

    if (filteredData.length === 0) {
        console.warn("No hay datos válidos para el heatmap.");
        return;
    }

    // Calcular el valor máximo dinámico
    var maxIntensity = Math.max(...filteredData.map(p => p.intensity));

    var heatmapData = {
        max: maxIntensity, // Ajuste dinámico
        data: filteredData
    };

    heatLayer.setData(heatmapData);

    // Calcular límites para ajustar el mapa a los puntos de calor
    var latLngs = filteredData.map(p => [p.lat, p.lng]);
    var bounds = L.latLngBounds(latLngs);
    map.fitBounds(bounds, { padding: [20, 20] }); // Añadir padding opcional
};

window.invalidateSize = function () {
    if (window.map) {
        window.map.invalidateSize();
    }
};

window.getMapZoom = function () {
    return map.getZoom(); // Retorna el nivel de zoom actual
};

// Llama a C# cuando cambia el zoom
window.onZoomChanged = function () {
    DotNet.invokeMethodAsync("ClientApp", "CargarDatos");
};
