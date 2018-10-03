using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class arrowMover : MonoBehaviour
{
    float moveTime = 2;
    public float startTime;
    public float endTime;
    Vector3 currentPosition;
    float startY = -115;
    float endY = -170;
    // Use this for initialization
    void Start()
    {
        startTime = Time.time;
        endTime = startTime + moveTime;
        currentPosition = transform.localPosition;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Time.time < startTime + (moveTime / 2))
        {
            float progress = (Time.time - startTime) / (moveTime / 2);
            progress = Easings.CubicEaseInOut(progress);
            float newY = Mathf.Lerp(startY, endY, progress);
            currentPosition.y = newY;
            transform.localPosition = currentPosition;
            }
        else if (Time.time < endTime)
        {
            float progress = (Time.time - (startTime + (moveTime / 2))) / (moveTime / 2);
            progress = Easings.CubicEaseInOut(progress);
            float newY = Mathf.Lerp(endY, startY, progress);
            currentPosition.y = newY;
            transform.localPosition = currentPosition;
        }
        else
        {
            startTime = Time.time;
            endTime = startTime + moveTime;
        }
    }
}
