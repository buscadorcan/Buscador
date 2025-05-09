window.abrirVentanaPDF = (url) => {
    let popup = window.open(
        url,
        "VisorPDF",
        "width=900,height=700,scrollbars=yes,resizable=yes,toolbar=no,location=no,status=no,menubar=no"
    );

    if (!popup) {
        alert("No se pudo abrir la ventana emergente. Verifique los permisos del navegador.");
    }
};