mergeInto(LibraryManager.library, {
    SetUpDataListener: function(gameObjName) {
        window.addEventListener("message", function(event) {
            var data = event.data;
            if (!data) return;

            console.log("DataReceiver: ", data);

            if (data.type === "mask" && data.mask) {
                SendMessage(UTF8ToString(gameObjName), "OnReceiveMask", data.mask.base64);
            }


            if (data.type === "cursorPos" && data.data) {
                SendMessage(UTF8ToString(gameObjName), "OnReceiveCursorPos", JSON.stringify(data.data));
            }

        });
    }
});