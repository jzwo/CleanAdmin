(function () {
    if (window.hasAntdFocusListener) return;
    window.hasAntdFocusListener = true;

    window.addEventListener('mouseup', function (e) {
        const button = e.target.closest('.ant-btn');

        if (button) {
            setTimeout(function () {
                button.blur();
            }, 0);
        }
    });

    console.log("Ant Design Global Focus Remover Loaded.");
})();