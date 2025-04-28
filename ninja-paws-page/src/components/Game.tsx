"use client";

import { useEffect, useRef } from "react";

interface GameProps {
    cursorPos: { x: number; y: number };
    mask: { base64: string } | null;
}

export default function Game(props: GameProps) {
    const frameRef = useRef<HTMLIFrameElement>(null);

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
        </div>
    );
}