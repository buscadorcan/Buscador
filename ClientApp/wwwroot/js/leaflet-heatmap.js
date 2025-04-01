var map;
var heatLayer;

window.initMap = function () {
    map = L.map('map').setView([37.7749, -122.4194], 5);

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; OpenStreetMap contributors'
    }).addTo(map);

    heatLayer = new HeatmapOverlay({
        radius: 25,
        maxOpacity: .8,
        scaleRadius: true,
        useLocalExtrema: true,
        latField: 'lat',
        lngField: 'lng',
        valueField: 'intensity'
    });

    map.addLayer(heatLayer);
};

window.addHeatmapData = function (data) {

    if (!heatLayer) {
        console.error("heatLayer no ha sido inicializado.");
        return;
    }

    var heatmapData = {
        max: 1,
        data: data
    };

    heatLayer.setData(heatmapData);
};
