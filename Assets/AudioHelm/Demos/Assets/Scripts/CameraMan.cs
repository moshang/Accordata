// Copyright 2017 Matt Tytel

using UnityEngine;

namespace AudioHelm
{
    [AddComponentMenu("")]
    public class CameraMan : MonoBehaviour
    {
        public Transform player;

        protected Vector3 diff_;

        void Start()
        {
            diff_ = transform.position - player.position;
        }

        void Update()
        {
            transform.position = player.position + diff_;
        }
    }
}
