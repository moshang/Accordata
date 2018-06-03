// Copyright 2017 Matt Tytel

using UnityEngine;

namespace AudioHelm
{
    [AddComponentMenu("")]
    public class ControlTorque : MonoBehaviour
    {
        public float torque = 1.0f;
        public float maxAngularVelocity = 50.0f;
        public Vector3 forward = Vector3.forward;
        public Vector3 right = Vector3.right;

        void Start()
        {
            GetComponent<Rigidbody>().maxAngularVelocity = maxAngularVelocity;
        }

        void Update()
        {
            if (Input.GetKey("w") || Input.GetKey("up"))
                GetComponent<Rigidbody>().AddTorque(torque * right);
            if (Input.GetKey("s") || Input.GetKey("down"))
                GetComponent<Rigidbody>().AddTorque(-torque * right);
            if (Input.GetKey("a") || Input.GetKey("left"))
                GetComponent<Rigidbody>().AddTorque(torque * forward);
            if (Input.GetKey("d") || Input.GetKey("right"))
                GetComponent<Rigidbody>().AddTorque(-torque * forward);
        }
    }
}
