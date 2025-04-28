"use client";

import { useEffect, useRef } from "react";

interface GameProps {
    cursorPos: { x: number; y: number };
}

export default function Game(props: GameProps) {
    const frameRef = useRef<HTMLIFrameElement>(null);

    useEffect(() => {
        const frameElement = frameRef.current;
        if (!frameElement) return;
        const frameWindow = frameElement.contentWindow;
        if (!frameWindow) return;

        frameWindow.postMessage(JSON.stringify({type: "cursorPos", data: props.cursorPos}), location.origin);
    }, [props.cursorPos]);

    return (
        <div>
            <iframe title="game" src="/game/index.html" width="1300" height="780" />
        </div>
    );
}