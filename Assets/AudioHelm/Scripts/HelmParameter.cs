// Copyright 2017 Matt Tytel

using UnityEngine;

namespace AudioHelm
{
    /// <summary>
    /// A single Helm synthesizer parameter to control.
    /// </summary>
    [System.Serializable]
    public class HelmParameter
    {
        public enum ScaleType {
            kByValue,
            kByPercent,
        }

        [SerializeField]
        Param parameter_ = Param.kNone;
        /// <summary>
        /// The parameter index.
        /// </summary>
        public Param parameter
        {
            get
            {
                return parameter_;
            }
            set
            {
                if (parameter_ == value)
                    return;

                parameter_ = value;
                UpdateMinMax();
            }
        }

        /// <summary>
        /// What bounds to use for the value.
        /// </summary>
        public ScaleType scaleType = ScaleType.kByPercent;

        /// <summary>
        /// The controller this parameter belongs to.
        /// </summary>
        public HelmController parent = null;

        float lastValue = -1.0f;

        [SerializeField]
        float minimumRange = 0.0f;

        [SerializeField]
        float maximumRange = 1.0f;

        [SerializeField]
        float paramValue_ = 0.0f;
        /// <summary>
        /// The current parameter value.
        /// </summary>
        public float paramValue
        {
            get
            {
                return paramValue_;
            }
            set
            {
                if (paramValue_ == value)
                    return;

                paramValue_ = value;
                UpdateNative();
            }
        }

        public HelmParameter()
        {
            parent = null;
        }

        public HelmParameter(HelmController par)
        {
            parent = par;
        }

        public HelmParameter(HelmController par, Param param)
        {
            parent = par;
            parameter = param;
            if (scaleType == ScaleType.kByPercent)
                paramValue_ = parent.GetParameterPercent(parameter);
            else
                paramValue_ = parent.GetParameterValue(parameter);
        }

        public float GetMinimumRange()
        {
            return minimumRange;
        }

        public float GetMaximumRange()
        {
            return maximumRange;
        }

        void UpdateMinMax()
        {
            if (parent && parameter_ != Param.kNone)
            {
                minimumRange = Native.HelmGetParameterMinimum((int)parameter_);
                maximumRange = Native.HelmGetParameterMaximum((int)parameter_);
            }
        }

        void UpdateNative()
        {
            if (parent && parameter_ != Param.kNone && lastValue != paramValue_)
            {
                if (scaleType == ScaleType.kByPercent)
                {
                    float val = Mathf.Clamp(paramValue_, 0.0f, 1.0f);
                    parent.SetParameterPercent(parameter, val);
                }
                else
                {
                    float val = Mathf.Clamp(paramValue_, minimumRange, maximumRange);
                    parent.SetParameterValue(parameter, val);
                }
            }
            lastValue = paramValue_;
        }
    }
}
