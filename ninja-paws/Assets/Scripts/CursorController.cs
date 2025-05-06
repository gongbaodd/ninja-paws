using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(
    typeof(LineRenderer)
)]
public class CursorController : MonoBehaviour
{
    GameManagerController manager;
    GameSettings config;
    [SerializeField] GameObject CatPaw;
    [SerializeField] float minPointDistance = 0.1f;
    readonly float mousePosZ = 0f;
    private LineRenderer lineRenderer;
    private List<Vector3> points = new();
    private bool isDrawing = false;
    [SerializeField] float drawingSpeedThresholdMotion = 5f;
    [SerializeField] float drawingSpeedThresholdCursor = 2000f;
    [SerializeField] GameObject motionMask;
    private Vector3 lastMousePosition;
    private float mouseSpeed;

    public bool IsDrawing => isDrawing;

    private void StartDrawing()
    {
        lineRenderer.enabled = true;
        isDrawing = true;
        points.Clear();
        AddPoint(GetPawWorldPosition());
    }

    private void ContinueDrawing()
    {
        Vector3 newPos = GetPawWorldPosition();
        if (points.Count == 0 || Vector3.Distance(points[points.Count - 1], newPos) > minPointDistance)
        {
            AddPoint(newPos);
        }
    }

    private void StopDrawing()
    {
        IEnumerator StopDrawingRoutine() {
            yield return new WaitForSeconds(config.vfxTime);
            isDrawing = false;
            lineRenderer.enabled = false;
        }

        StartCoroutine(StopDrawingRoutine());
    }

    private void AddPoint(Vector3 point)
    {
        points.Add(point);
        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }

    private Vector3 GetPawWorldPosition()
    {
        if (MotionButtonController.isMotion)
        {
            var curPos = HandTrackingReceiver.DesiredCurPosition;
            return new Vector3(curPos.x, curPos.y, mousePosZ);
        }
        else
        {

            Vector3 mousePos = Input.mousePosition;
            mousePos.z = mousePosZ;

            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            worldPos.z = mousePosZ;
            return worldPos;
        }
    }

    private void LockMouse()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        CatPaw.SetActive(true);
    }

    private void UnlockMouse()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void UpdateCatPawPos()
    {
        CatPaw.transform.position = GetPawWorldPosition();
    }

    void ActivateMask() 
    {
        motionMask.SetActive(true);
    }

    void DeactivateMask() 
    {
        motionMask.SetActive(false);
    }

    void Start()
    {
        manager = GameManagerController.Instance;
        config = manager.config;

        lineRenderer = GetComponent<LineRenderer>();
        LockMouse();

        MotionButtonController.OnMotion += ActivateMask;
        MotionButtonController.OnCursor += DeactivateMask;
    }

    public static float MouseSpeed = 0f;
    public static float drawingSpeedThresholdMotionStatic = 0f;

    void Update()
    {
        Vector3 currentMousePosition = MotionButtonController.isMotion ? HandTrackingReceiver.DesiredCurPosition : Input.mousePosition;
        mouseSpeed = (currentMousePosition - lastMousePosition).magnitude / Time.unscaledDeltaTime;
        
        MouseSpeed = mouseSpeed;
        drawingSpeedThresholdMotionStatic = drawingSpeedThresholdMotion;

        lastMousePosition = currentMousePosition;

        if (!isDrawing)
        {
            if (MotionButtonController.isMotion && mouseSpeed > drawingSpeedThresholdMotion)
            { 
                StartDrawing();
            } else {
                if (mouseSpeed > drawingSpeedThresholdCursor)
                {
                    StartDrawing();
                }
            }
        }
        
        if (isDrawing)
        {
            if (MotionButtonController.isMotion && mouseSpeed < drawingSpeedThresholdMotion * 0.5f)
            { 
                StopDrawing();
            } else {
                if (mouseSpeed < drawingSpeedThresholdCursor * 0.5f)
                {
                    StopDrawing();
                }
            }

            ContinueDrawing();

        }

        UpdateCatPawPos();
    }

    void OnDestroy()
    {
        UnlockMouse();
        MotionButtonController.OnMotion -= ActivateMask;
        MotionButtonController.OnCursor -= DeactivateMask;
    }

}