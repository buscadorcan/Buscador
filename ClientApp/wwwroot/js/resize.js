window.initDetectMobile = function(dotNetHelper) {
    dotNetHelper.invokeMethodAsync('UpdateIsMobile', window.innerWidth <= 768);

    window.addEventListener('resize', () => {
        dotNetHelper.invokeMethodAsync('UpdateIsMobile', window.innerWidth <= 768);
    });
};
