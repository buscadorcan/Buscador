// wwwroot/js/pdf-viewer.js
window.pdfViewer = {
    loadPdf: async function (pdfUrl) {
        const canvas = document.getElementById('pdfCanvas');
        const ctx = canvas.getContext('2d');

        // Configura el worker de PDF.js
        pdfjsLib.GlobalWorkerOptions.workerSrc = 'https://cdnjs.cloudflare.com/ajax/libs/pdf.js/3.5.141/pdf.worker.min.js';

        // Carga el PDF
        const pdf = await pdfjsLib.getDocument(pdfUrl).promise;

        // Obtén la primera página
        const page = await pdf.getPage(1);

        // Configura las dimensiones del canvas
        const viewport = page.getViewport({ scale: 1.5 });
        canvas.width = viewport.width;
        canvas.height = viewport.height;

        // Renderiza la página en el canvas
        await page.render({
            canvasContext: ctx,
            viewport: viewport
        }).promise;
    }
};
