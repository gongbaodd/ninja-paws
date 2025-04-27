"use client";

import { useEffect, useRef } from "react";
import PosePkg from "@mediapipe/pose";
import CameraPkg from "@mediapipe/camera_utils"
import DrawUtilsPkg from "@mediapipe/drawing_utils";

const { Pose, POSE_CONNECTIONS } = PosePkg;
const { Camera } = CameraPkg;
const { drawConnectors, drawLandmarks } = DrawUtilsPkg;

export default function PoseDetection() {

    const videoRef = useRef<HTMLVideoElement>(null);
    const canvasRef = useRef<HTMLCanvasElement>(null);

    useEffect(() => {
        const videoElement = videoRef.current;
        const canvasElement = canvasRef.current;

        if (!videoElement || !canvasElement) return;

        detectPose(videoElement, canvasElement);
    }, [])

    return (
        <div className="flex pose">
            <video ref={videoRef} autoPlay playsInline></video>
            <canvas ref={canvasRef} width={1280} height={720}></canvas>
        </div>
    );
}

function detectPose(videoElement: HTMLVideoElement, canvasElement: HTMLCanvasElement) {
    const canvasCtx = canvasElement.getContext("2d");
    if (!canvasCtx) return;

    const pose = new Pose({
        locateFile: (file) => `/@mediapipe/pose/${file}`,
    });

    pose.setOptions({
        modelComplexity: 1,
        smoothLandmarks: true,
        enableSegmentation: true,
        smoothSegmentation: true,
        minDetectionConfidence: 0.5,
        minTrackingConfidence: 0.5,
    });

    pose.onResults(result => {
        canvasCtx.save();
        canvasCtx.clearRect(0, 0, canvasElement.width, canvasElement.height);
        canvasCtx.drawImage(
            result.segmentationMask,
            0,
            0,
            canvasElement.width,
            canvasElement.height,
        );

        // Only overwrite existing pixels.
        canvasCtx.globalCompositeOperation = "source-in";
        canvasCtx.fillStyle = "#0000FF";
        canvasCtx.fillRect(0, 0, canvasElement.width, canvasElement.height);

        // Only overwrite missing pixels.
        canvasCtx.globalCompositeOperation = "destination-atop";
        canvasCtx.globalCompositeOperation = "source-over";
        drawConnectors(canvasCtx, result.poseLandmarks, POSE_CONNECTIONS, {
            color: "#00FF00",
            lineWidth: 4,
        });
        drawLandmarks(canvasCtx, result.poseLandmarks, {
            color: "#FF0000",
            lineWidth: 2,
        });
        canvasCtx.restore();

    });

    const camera = new Camera(videoElement, {
        onFrame: async () => {
            await pose.send({ image: videoElement });
        },
        width: 1280,
        height: 720,
    });
    camera.start();

    return () => {
        camera.stop();
    };
}
