window.desmarcarTodosLosCheckboxes = function () {
    document.querySelectorAll("input[type='checkbox']").forEach(cb => cb.checked = false);
};
