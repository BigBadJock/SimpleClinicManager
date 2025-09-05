window.modalHelpers = (function () {
    function getEl(sel) { return document.querySelector(sel); }
    return {
        showModal: function (selector) {
            const el = getEl(selector);
            if (!el || typeof bootstrap === 'undefined') return;
            const modal = bootstrap.Modal.getOrCreateInstance(el);
            modal.show();
        },
        hideModal: function (selector) {
            const el = getEl(selector);
            if (!el || typeof bootstrap === 'undefined') return;
            const modal = bootstrap.Modal.getInstance(el);
            if (modal) modal.hide();
        }
    };
})();