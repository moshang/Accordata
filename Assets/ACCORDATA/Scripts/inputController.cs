using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inputController : MonoBehaviour
{
    [Header("-> ACCORDATA <-")]
    public RectTransform mapPanel;
    private Vector2 mapScale;
    private Vector2 mapStartPos;
    private Vector2 mouseDownPos;
    // Use this for initialization
    void Start()
    {
        mapScale = new Vector2(1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        var d = Input.GetAxis("Mouse ScrollWheel");
        if (d > 0f)
        {
            // scroll up
            float scaleFactor = mapScale.x;
            scaleFactor = Mathf.Clamp(scaleFactor + 0.1f, 0.8f, 3);
            mapScale.x = scaleFactor;
            mapScale.y = scaleFactor;
            mapPanel.localScale = mapScale;
        }
        else if (d < 0f)
        {
            // scroll down
            float scaleFactor = mapScale.x;
            scaleFactor = Mathf.Clamp(scaleFactor - 0.1f, 0.8f, 3);
            mapScale.x = scaleFactor;
            mapScale.y = scaleFactor;
            mapPanel.localScale = mapScale;
        }
        if (Input.GetMouseButtonDown(0))
        {
            mapStartPos.x = mapPanel.anchoredPosition.x;
            mapStartPos.y = mapPanel.anchoredPosition.y;
            mouseDownPos.x = mouseDownPos.x = Input.mousePosition.x; ;
            mouseDownPos.y = Input.mousePosition.y;
        }

        if (Input.GetMouseButton(0))
        {
            /*
            float scaleFactor = mapScale.x;
            float minX = utils.map(scaleFactor, 0.8f, 3, 0, 1000);
            float maxX = utils.map(scaleFactor, 0.8f, 0, 0, -500);
            */
            Vector2 newMapPos;
            //newMapPos.x = Mathf.Clamp(mapStartPos.x + (Input.mousePosition.x - mouseDownPos.x), minX, maxX);
            newMapPos.x = mapStartPos.x + (Input.mousePosition.x - mouseDownPos.x);
            newMapPos.y = mapStartPos.y + (Input.mousePosition.y - mouseDownPos.y);
            mapPanel.anchoredPosition = newMapPos;
        }
    }
}
