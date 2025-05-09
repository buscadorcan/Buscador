window.initSortable = function(dotNetHelper) {
    var gridElement = document.getElementById('sortableGrid');
    if (gridElement) {
        new Sortable(gridElement.querySelector('tbody'), {
            animation: 150,
            draggable: 'tr',
            onEnd: function(evt) {
                var sortedIds = Array.from(evt.from.children).map(item => {
                    return item.querySelector('td:first-child input[type="hidden"]').value.trim();
                });
                dotNetHelper.invokeMethodAsync('OnDragEnd', sortedIds);
            }
        });
    } else {
        console.error('Elemento no encontrado: #sortableGrid');
    }
};
