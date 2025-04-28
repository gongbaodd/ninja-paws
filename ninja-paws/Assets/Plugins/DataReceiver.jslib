mergeInto(LibraryManager.library, {
    SetUpDataListener: function(gameObjName) {
        window.addEventListener("message", function(event) {
            var data = event.data;
            if (!data) return;

            if (data.type === "mask" && data.mask) {
                SendMessage(UTF8ToString(gameObjName), "OnReceiveMask", JSON.stringify(data));
            }


            if (data.type === "cursorPos" && data.cursorPos) {
                SendMessage(UTF8ToString(gameObjName), "OnReceiveCursorPos", JSON.stringify(data));
            }

        });
    }
});