window.inicializarMapa = function () {
    console.log("Intentando inicializar Google Maps...");

    if (typeof google !== "undefined" && google.maps) {
        console.log("Google Maps detectado. Creando mapa...");
        var mapElement = document.getElementById("map");
        if (mapElement) {
            new google.maps.Map(mapElement, {
                center: { lat: -0.220164, lng: -78.512327 },
                zoom: 5,
            });
        } else {
            console.error("No se encontró el elemento #map en el DOM.");
        }
    } else {
        console.error("Google Maps no está disponible. ¿Se cargó la API correctamente?");
    }
};
