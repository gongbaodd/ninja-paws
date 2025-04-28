mergeInto(LibraryManager.library, {
    SetUpDataListener: function(gameObjName) {
        window.addEventListener("message", function(event) {
            var data = event.data;
            if (!data) return;

            console.log("DataReceiver: ", data);

            if (data.type === "mask" && data.mask) {
                SendMessage(UTF8ToString(gameObjName), "HandleMessage", JSON.stringify({
                    type: "mask",
                    mask: data.mask.base64,
                }));
            }


            if (data.type === "cursorPos" && data.data) {
                SendMessage(UTF8ToString(gameObjName), "HandleMessage", JSON.stringify({
                    type: "cursorPos",
                    x: data.data.x,
                    y: data.data.y,
                }));
            }

        });
    }
});