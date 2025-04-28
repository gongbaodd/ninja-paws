"use client";

import { useEffect, useRef, useState } from "react";
import WebSocketComponent from "./WebSocket";

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
    const [cursorPosMsg, setCursorPosMsg] = useState<CursorPosMsg | null>(null);
    const [maskMsg, setMaskMsg] = useState<MaskMsg | null>(null);

    useEffect(() => {
        const frameElement = frameRef.current;
        if (!frameElement) return;
        const frameWindow = frameElement.contentWindow;
        if (!frameWindow) return;

        frameWindow.postMessage({type: "cursorPos", data: props.cursorPos}, location.origin);

    }, [props.cursorPos, props.mask]);

    useEffect(() => {
        const frameElement = frameRef.current;
        if (!frameElement) return;
        const frameWindow = frameElement.contentWindow;
        if (!frameWindow) return;

        frameWindow.postMessage({type: "mask", data: props.mask}, location.origin);
    }, [props.mask]);

    return (
        <div>
            <iframe ref={frameRef} title="game" src="/game/index.html" width="1300" height="780" />
            <WebSocketComponent cursorPosMsg={cursorPosMsg} maskMsg={maskMsg} />
        </div>
    );
}