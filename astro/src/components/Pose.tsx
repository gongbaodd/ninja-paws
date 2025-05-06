"use client";

import { useCallback, useRef, useState } from "react";
import PosePkg from "@mediapipe/pose";
import CameraPkg from "@mediapipe/camera_utils";
import Game from "./Game";

const { Pose } = PosePkg;
const { Camera } = CameraPkg;
const RIGHT_WRIST_INDEX = 16;
const isDEV = import.meta.env.DEV;

export default function PoseDetection() {
  const videoRef = useRef<HTMLVideoElement>(null);
  const canvasRef = useRef<HTMLCanvasElement>(null);
  const [cursorPos, setCursorPos] = useState<
    Parameters<typeof Game>[0]["cursorPos"]
  >({ x: 0.5, y: 0.5 });
  const [mask, setMask] = useState<Parameters<typeof Game>[0]["mask"]>(null);

  let camera: CameraPkg.Camera | null = null;

  const onSetMotion = useCallback((isOpen: boolean) => {
    const videoElement = videoRef.current;
    const canvasElement = canvasRef.current;

    if (!videoElement || !canvasElement) return;

    if (isOpen) {
      detectPose(
        videoElement,
        canvasElement,
        (x, y) => setCursorPos({ x, y }),
        (mask) => setMask({ base64: mask })
      );
    } else {
        if (camera) {
            camera.stop();
            camera = null;
        }
    }
  }, []);

  return (
    <>
      <Game cursorPos={cursorPos} mask={mask} onSetMotion={onSetMotion} />
      <div className={"flex pose " + (!isDEV && "hide")}>
        <video ref={videoRef} autoPlay playsInline></video>
        <canvas ref={canvasRef} width={1280} height={720}></canvas>
      </div>
    </>
  );

  function detectPose(
    videoElement: HTMLVideoElement,
    canvasElement: HTMLCanvasElement,
    onCursor: (x: number, y: number) => void,
    onMask: (mask: string) => void = () => {}
  ) {
    const canvasCtx = canvasElement.getContext("2d");
    if (!canvasCtx) return;

    const pose = new Pose({
      locateFile: (file) => `./@mediapipe/pose/${file}`,
    });

    pose.setOptions({
      modelComplexity: 1,
      smoothLandmarks: true,
      enableSegmentation: true,
      smoothSegmentation: true,
      minDetectionConfidence: 0.5,
      minTrackingConfidence: 0.5,
    });

    pose.onResults((result) => {
      canvasCtx.save();
      canvasCtx.clearRect(0, 0, canvasElement.width, canvasElement.height);
      canvasCtx.drawImage(
        result.segmentationMask,
        0,
        0,
        canvasElement.width,
        canvasElement.height
      );

      // Only overwrite existing pixels.
      canvasCtx.globalCompositeOperation = "source-in";
      canvasCtx.fillStyle = "#A66C48";
      canvasCtx.fillRect(0, 0, canvasElement.width, canvasElement.height);

      canvasCtx.restore();

      getRightWristPos(result);
      onMask(canvasElement.toDataURL("image/png"));
    });

    camera = new Camera(videoElement, {
      onFrame: async () => {
        await pose.send({ image: videoElement });
      },
      width: 1280,
      height: 720,
    });
    camera.start();

    function getRightWristPos(results: PosePkg.Results) {
      const rightWrist = results.poseLandmarks[RIGHT_WRIST_INDEX];

      if (rightWrist) {
        const x = rightWrist.x;
        const y = rightWrist.y;
        onCursor(x, y);
      }
    }
  }
}
