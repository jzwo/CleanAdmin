let fullscreenChangeHandler = null;

export function isFullscreen() {
    return Boolean(document.fullscreenElement);
}

export async function request() {
    if (isFullscreen()) {
        return;
    }

    await document.documentElement.requestFullscreen();
}

export async function exit() {
    if (!isFullscreen()) {
        return;
    }

    await document.exitFullscreen();
}

export function registerFullscreenChange(dotNetRef) {
    if (fullscreenChangeHandler) {
        unregisterFullscreenChange();
    }

    fullscreenChangeHandler = () => {
        dotNetRef.invokeMethodAsync("OnBrowserFullscreenChanged", isFullscreen());
    };

    document.addEventListener("fullscreenchange", fullscreenChangeHandler);
}

export function unregisterFullscreenChange() {
    if (!fullscreenChangeHandler) {
        return;
    }

    document.removeEventListener("fullscreenchange", fullscreenChangeHandler);
    fullscreenChangeHandler = null;
}
