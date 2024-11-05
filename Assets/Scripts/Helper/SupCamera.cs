using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupCamera : MonoBehaviour
{

    [SerializeField] private LevelFocus levelFocus;

    public List<GameObject> Players;

    [SerializeField] private float AngleUpdateSpeed = 7f;
    [SerializeField] private float PositionUpdateSpeed = 5f;

    [SerializeField] private float DepthMax = -10f;
    [SerializeField] private float DepthMin = -22f;

    [SerializeField] private float AngleMax = 11f;
    [SerializeField] private float AngleMin = 3f;

    [SerializeField] private Vector3 minBounds, maxBounds;

    private float CameraEulerX;
    private Vector3 CameraPosition;

    void Start()
    {
        Players.Add(levelFocus.gameObject);
    }

    void LateUpdate()
    {
        CalculateCameraLocations();
        MoveCamera();
    }

    private void MoveCamera()
    {
        Vector3 position = gameObject.transform.position;
        if (position != CameraPosition)
        {
            Vector3 targetPosition = Vector3.zero;
            targetPosition.x = Mathf.MoveTowards(position.x, CameraPosition.x, PositionUpdateSpeed * Time.deltaTime);
            targetPosition.y = Mathf.MoveTowards(position.y, CameraPosition.y, PositionUpdateSpeed * Time.deltaTime);
            targetPosition.z = Mathf.MoveTowards(position.z, CameraPosition.z, PositionUpdateSpeed * Time.deltaTime);
            gameObject.transform.position = targetPosition;
        }

        Vector3 localEulerAngles = gameObject.transform.eulerAngles;
        if (localEulerAngles.x != CameraEulerX)
        {
            Vector3 targetEulerAngles = new Vector3(CameraEulerX, localEulerAngles.y, localEulerAngles.z);
            gameObject.transform.localEulerAngles = Vector3.MoveTowards(localEulerAngles, targetEulerAngles, AngleUpdateSpeed * Time.deltaTime);
        }
    }

    private void CalculateCameraLocations()
    {
        Vector3 averageCenter = Vector3.zero;
        Vector3 totalPositions = Vector3.zero;
        Bounds playerBounds = new Bounds();
        foreach (GameObject player in Players)
        {
            if (player == null)
            {
                Players.Remove(player);
                return;
            }
        }
        foreach (GameObject player in Players)
        {
            Vector3 playerPosition = player.transform.position;
            if (!levelFocus.FocusBounds.Contains(playerPosition))
            {
                float playerX = Mathf.Clamp(playerPosition.x, levelFocus.FocusBounds.min.x, levelFocus.FocusBounds.max.x);
                float playerY = Mathf.Clamp(playerPosition.y, levelFocus.FocusBounds.min.y, levelFocus.FocusBounds.max.y);
                float playerZ = Mathf.Clamp(playerPosition.z, levelFocus.FocusBounds.min.z, levelFocus.FocusBounds.max.z);
                playerX = Mathf.Clamp(playerX, minBounds.x, maxBounds.x);
                playerY = Mathf.Clamp(playerY, minBounds.y, maxBounds.y);
                playerZ = Mathf.Clamp(playerZ, minBounds.z, maxBounds.z);
                playerPosition = new Vector3(playerX, playerY, playerZ);
            }
            totalPositions += playerPosition;
            playerBounds.Encapsulate(player.transform.position);
        }

        averageCenter = (totalPositions / Players.Count);

        float extents = (playerBounds.extents.x + playerBounds.extents.y);
        float lerpPercent = Mathf.InverseLerp(0, (levelFocus.HalfXBounds + levelFocus.HalfYBounds)/2, extents);

        float depth = Mathf.Lerp(DepthMax, DepthMin, lerpPercent);
        float angle = Mathf.Lerp(AngleMax, AngleMin, lerpPercent);

        CameraEulerX = angle;
        CameraPosition = new Vector3(averageCenter.x, averageCenter.y, depth);
    }

    public IEnumerator ShakeCamera(float duration, float magnitude, AnimationCurve curve)
    {
        Vector3 originalpos =  transform.localPosition;
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            float strength = curve.Evaluate(elapsed / duration);

            transform.localPosition += new Vector3(x, y, originalpos.z) * strength;

            elapsed += Time.deltaTime;
            
            yield return null;
        }
        float fadeOut = 1.0f;
        elapsed = 0.0f;
        while (elapsed < fadeOut)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalpos, elapsed);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalpos;
    }
}
