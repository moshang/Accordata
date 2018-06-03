// Copyright 2017 Matt Tytel

using UnityEngine;
using System.Collections.Generic;

namespace AudioHelm
{
    [AddComponentMenu("")]
    public class PlatformCreator : MonoBehaviour
    {
        public Transform platformModel;
        public float minWidth = 0.1f;
        public List<Transform> allPlatforms = new List<Transform>();

        public KeyCode clearKey = KeyCode.C;
        public KeyCode undoKey = KeyCode.U;

        Transform currentPlatform;
        Vector2 startPosition;

        void TryInitialize(Vector2 position)
        {
            if (Input.GetMouseButtonDown(0))
            {
                startPosition = position;
                currentPlatform = Instantiate(platformModel) as Transform;
                currentPlatform.position = startPosition;
            }
        }

        void TryRelease(Vector2 position)
        {
            if (Input.GetMouseButtonUp(0) && currentPlatform)
            {
                if ((position - startPosition).sqrMagnitude < minWidth * minWidth)
                    Destroy(currentPlatform.gameObject);
                else
                    allPlatforms.Add(currentPlatform);
                currentPlatform = null;
            }
        }

        void TryUpdate(Vector2 position)
        {
            if (currentPlatform == null)
                return;

            Vector3 delta = position - startPosition;
            Vector3 center = (position + startPosition) / 2.0f;
            currentPlatform.position = center;
            currentPlatform.right = delta;

            Vector3 localScale = currentPlatform.localScale;
            localScale.x = delta.magnitude;
            currentPlatform.localScale = localScale;
        }

        void ClearPlatforms()
        {
            foreach (Transform platform in allPlatforms)
                Destroy(platform.gameObject);

            allPlatforms.Clear();
        }

        void UndoPlatform()
        {
            if (allPlatforms.Count == 0)
                return;

            int index = allPlatforms.Count - 1;
            Destroy(allPlatforms[index].gameObject);
            allPlatforms.RemoveAt(index);
        }

        void Update()
        {
            Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            TryInitialize(position);
            TryUpdate(position);
            TryRelease(position);

            if (Input.GetKeyDown(clearKey))
                ClearPlatforms();
            if (Input.GetKeyDown(undoKey))
                UndoPlatform();
        }
    }
}
