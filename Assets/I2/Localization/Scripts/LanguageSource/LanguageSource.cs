using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace I2.Loc
{
    [AddComponentMenu("I2/Localization/Source")]
    [ExecuteInEditMode]
	public partial class LanguageSource : MonoBehaviour
    {
		#region Variables

		public bool NeverDestroy = false;  	// Keep between scenes (will call DontDestroyOnLoad )

		public bool UserAgreesToHaveItOnTheScene = false;
		public bool UserAgreesToHaveItInsideThePluginsFolder = false;

        [NonSerialized] public bool mIsGlobalSource;

        #endregion

        #region EditorVariables
#if UNITY_EDITOR

        public string Spreadsheet_LocalFileName;
		public string Spreadsheet_LocalCSVSeparator = ",";
        public string Spreadsheet_LocalCSVEncoding = "utf-8";
        public bool Spreadsheet_SpecializationAsRows = true;

#endif
        #endregion

        #region Language

        void Awake()
		{
            NeverDestroy = false;

            if (NeverDestroy)
			{
				if (ManagerHasASimilarSource())
				{
					Destroy (this);
					return;
				}
				else
				{
					if (Application.isPlaying)
						DontDestroyOnLoad (gameObject);
				}
			}
			LocalizationManager.AddSource (this);
			UpdateDictionary();
            UpdateAssetDictionary();
            LocalizationManager.LocalizeAll(true);
        }

        private void OnDestroy()
        {
            NeverDestroy = false;

            if (!NeverDestroy)
            {
                LocalizationManager.RemoveSource(this);
            }
        }
 
		public string GetSourceName()
		{
			string s = gameObject.name;
			Transform tr = transform.parent;
			while (tr)
			{
				s = string.Concat(tr.name, "_", s);
				tr = tr.parent;
			}
			return s;
		}


		public bool IsEqualTo( LanguageSource Source )
		{
			if (Source.mLanguages.Count != mLanguages.Count)
				return false;

			for (int i=0, imax=mLanguages.Count; i<imax; ++i)
				if (Source.GetLanguageIndex( mLanguages[i].Name ) < 0)
					return false;

			if (Source.mTerms.Count != mTerms.Count)
				return false;

			for (int i=0; i<mTerms.Count; ++i)
				if (Source.GetTermData(mTerms[i].Term)==null)
					return false;

			return true;
		}

		internal bool ManagerHasASimilarSource()
		{
			for (int i=0, imax=LocalizationManager.Sources.Count; i<imax; ++i)
			{
				LanguageSource source = (LocalizationManager.Sources[i] as LanguageSource);
				if (source!=null && source.IsEqualTo(this) && source!=this)
					return true;
			}
			return false;
		}

		public void ClearAllData()
		{
			mTerms.Clear ();
			mLanguages.Clear ();
			mDictionary.Clear();
            mAssetDictionary.Clear();
		}

        public bool IsGlobalSource()
        {
            return mIsGlobalSource;
        }

		#endregion
	}
}