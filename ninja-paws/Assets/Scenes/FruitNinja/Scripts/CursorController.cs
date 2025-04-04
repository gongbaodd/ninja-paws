using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scenes.FruitNinja.Scripts
{
    [RequireComponent(
        typeof(LineRenderer)
    )]
    public class CursorController : MonoBehaviour
    {
        [SerializeField] GameObject CatPaw;
        [SerializeField] float mousePosZ = 10f;
        [SerializeField] float minPointDistance = 0.1f;
        private LineRenderer lineRenderer;
        private List<Vector3> points = new();
        private bool isDrawing = false;

        public bool IsDrawing => isDrawing;

        private void StartDrawing()
        {
            lineRenderer.enabled = true;
            isDrawing = true;
            points.Clear();
            AddPoint(GetMouseWorldPosition());
        }

        private void ContinueDrawing()
        {
            Vector3 newPos = GetMouseWorldPosition();
            if (points.Count == 0 || Vector3.Distance(points[points.Count - 1], newPos) > minPointDistance)
            {
                AddPoint(newPos);
            }
        }

        private void StopDrawing()
        {
            isDrawing = false;
            lineRenderer.enabled = false;
        }

        private void AddPoint(Vector3 point)
        {
            points.Add(point);
            lineRenderer.positionCount = points.Count;
            lineRenderer.SetPositions(points.ToArray());
        }

        private Vector3 GetMouseWorldPosition()
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = mousePosZ;
            return Camera.main.ScreenToWorldPoint(mousePos);
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
            CatPaw.transform.position = GetMouseWorldPosition();
        }

        void Start()
        {
            lineRenderer = GetComponent<LineRenderer>();
            LockMouse();
        }

        void Update()
        {

            if (Input.GetMouseButtonDown(0))
            {
                StartDrawing();
            }

            if (Input.GetMouseButton(0) && isDrawing)
            {
                ContinueDrawing();
            }

            if (Input.GetMouseButtonUp(0))
            {
                StopDrawing();
            }

            UpdateCatPawPos();
        }

        void OnDestroy()
        {
            UnlockMouse();
        }

    }
}