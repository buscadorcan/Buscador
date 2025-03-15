window.destroyModal = function (modalId) {
    let modal = document.getElementById(modalId);
    if (modal) {
        console.log("🗑 Eliminando modal del DOM...");
        modal.remove(); // 🔥 Eliminar el modal completamente
    }
};