"use client";

import { useEffect, useRef } from "react";
import WebSocketComponent from "./WebSocket";
const isDEV = import.meta.env.DEV;

interface GameProps {
    cursorPos: { x: number; y: number };
    mask: { base64: string } | null;
    onSetMotion: (motion: boolean) => void;
}

export interface CursorPosMsg {
    type: "cursorPos";
    data: GameProps["cursorPos"];
}

export interface MaskMsg {
    type: "mask";
    data: GameProps["mask"];
}

export interface MotionMsg {
    type: "motion";
    data: boolean;
}

export default function Game(props: GameProps) {
    const frameRef = useRef<HTMLIFrameElement>(null);
    const gameRef = useRef<HTMLDivElement>(null);

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

    useEffect(() => {
        const handleMessage = (event: MessageEvent) => {
            if (event.origin !== location.origin) return;
            const { type, data } = event.data;
    
            if (type === "motion") {
                props.onSetMotion(data);
            }
        };
    
        window.addEventListener("message", handleMessage);
        return () => window.removeEventListener("message", handleMessage);
    }, []);

    return (
        <div className="game">
            {<iframe ref={frameRef} title="game" src="./game/index.html" width="1280" height="720" />}
            {isDEV && <WebSocketComponent cursorPosMsg={cursorPosMsg} maskMsg={maskMsg} onSetMotion={props.onSetMotion} />}
        </div>
    );
}