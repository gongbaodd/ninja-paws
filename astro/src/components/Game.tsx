"use client";

import { useEffect, useRef } from "react";
import WebSocketComponent from "./WebSocket";
const isDEV = import.meta.env.DEV;

interface GameProps {
    cursorPos: { x: number; y: number };
    mask: { base64: string } | null;
}

export interface CursorPosMsg {
    type: "cursorPos";
    data: GameProps["cursorPos"];
}

export interface MaskMsg {
    type: "mask";
    data: GameProps["mask"];
}

export default function Game(props: GameProps) {
    const frameRef = useRef<HTMLIFrameElement>(null);

    const cursorPosMsg: CursorPosMsg = {
        type: "cursorPos",
        data: props.cursorPos,
    };

    const maskMsg: MaskMsg = {
        type: "mask",
        data: props.mask,
    };

    useEffect(() => {
        const frameWindow = frameRef.current?.contentWindow;
        if (frameWindow) {
            frameWindow.postMessage(cursorPosMsg, location.origin);
        }
    }, [cursorPosMsg]);

    useEffect(() => {
        const frameWindow = frameRef.current?.contentWindow;
        if (frameWindow) {
            frameWindow.postMessage(maskMsg, location.origin);
        }
    }, [maskMsg]);

    return (
        <div>
            {!isDEV &&<iframe ref={frameRef} title="game" src="./game/index.html" width="1300" height="780" />}
            {isDEV && <WebSocketComponent cursorPosMsg={cursorPosMsg} maskMsg={maskMsg} />}
        </div>
    );
}