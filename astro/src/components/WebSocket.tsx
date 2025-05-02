import { use, useEffect, useRef } from "react";
import type { CursorPosMsg, MaskMsg } from "./Game";

const UNITY_WEBSOCKET_URL = "ws://localhost:8080";

export default function WebSocketComponent(props: { cursorPosMsg: CursorPosMsg | null, maskMsg: MaskMsg | null }) {
    const ws = useRef<WebSocket | null>(null);

    useEffect(() => {
        if (ws.current) return;
        connectWebSocket();

        return () => ws.current?.close(); // Cleanup on unmount

        function connectWebSocket() {
            ws.current = new WebSocket(UNITY_WEBSOCKET_URL);

            ws.current.onmessage = (event) => {
                console.log("Message from Unity:", event.data);
            };

            ws.current.onopen = () => {
                console.log("WebSocket connected to Unity");
            };

            ws.current.onclose = (event) => {
                console.log("WebSocket disconnected:", event.reason);

                // Try to reconnect after a delay
                setTimeout(connectWebSocket, 5000);
            };

            ws.current.onerror = (error) => {
                console.error("WebSocket error:", error);
                if (ws.current && ws.current.readyState !== WebSocket.OPEN) {
                    ws.current.close(); // Ensure close event fires if not already closed
                }
            };
        }

    }, []);

    useEffect(() => {
        if (!ws.current) return;
        if (ws.current.readyState !== WebSocket.OPEN) return;

        if (props.cursorPosMsg) {
            ws.current.send(JSON.stringify(props.cursorPosMsg));
        }
    }, [props.cursorPosMsg]);

    useEffect(() => {
        if (!ws.current) return;
        if (ws.current.readyState !== WebSocket.OPEN) return;

        if (props.maskMsg) {
            ws.current.send(JSON.stringify(props.maskMsg));
        }
    }, [props.maskMsg])

    return <></>
}