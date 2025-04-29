mergeInto(LibraryManager.library, {
    SetUpDataListener: function(gameObjNamePtr) {
        var gameObjName = UTF8ToString(gameObjNamePtr);
        console.log("Listening for messages for GameObject:", gameObjName);

        window.addEventListener("message", function(event) {
            var data = event.data;
            if (!data) return;

            if ((data.type === "mask" && data.data) || (data.type === "cursorPos" && data.data)) {
                SendMessage(gameObjName, "HandleMessage", JSON.stringify(data));
            }
        });
    }
});