using UnityEngine;
using UnityEditor;

namespace I2.Loc
{
	[CustomEditor(typeof(LanguageSource))]
	public partial class LocalizationEditor : Editor
	{
		#region Variables
		
		SerializedProperty 	mProp_Assets, mProp_Languages, 
							mProp_Google_WebServiceURL, mProp_GoogleUpdateFrequency, mProp_GoogleUpdateDelay, mProp_Google_SpreadsheetKey, mProp_Google_SpreadsheetName, 
							mProp_Spreadsheet_LocalFileName, mProp_Spreadsheet_LocalCSVSeparator, mProp_CaseInsensitiveTerms, mProp_Spreadsheet_LocalCSVEncoding,
							mProp_OnMissingTranslation, mProp_AppNameTerm, mProp_IgnoreDeviceLanguage, mProp_Spreadsheet_SpecializationAsRows, mProp_GoogleInEditorCheckFrequency;

		public static LanguageSource mLanguageSource;
        public static LocalizationEditor mLanguageSourceEditor;

        static bool mIsParsing = false;  // This is true when the editor is opening several scenes to avoid reparsing objects

		#endregion
		
		#region Variables GUI
		
		GUIStyle Style_ToolBar_Big, Style_ToolBarButton_Big;

 		
		public GUISkin CustomSkin;

		static Vector3 mScrollPos_Languages;
		public static string mLanguages_NewLanguage = "";

		#endregion

        #region Styles

        public static GUIStyle Style_WrapTextField {
            get{ 
                if (mStyle_WrapTextField==null)
                {
                    mStyle_WrapTextField = new GUIStyle("textField");
                    mStyle_WrapTextField.wordWrap = true;
                }
                return mStyle_WrapTextField;
            }
        }
        static GUIStyle mStyle_WrapTextField;

        #endregion

		#region Inspector

		void OnEnable()
		{
			var newSource = target as LanguageSource;
			bool ForceParse = (mLanguageSource != newSource);
			mLanguageSource = newSource;
            mLanguageSourceEditor = this;


            if (!LocalizationManager.Sources.Contains(mLanguageSource))
				LocalizationManager.UpdateSources();
            mProp_Assets                           = serializedObject.FindProperty("Assets");
            mProp_Languages                        = serializedObject.FindProperty("mLanguages");
            mProp_Google_WebServiceURL             = serializedObject.FindProperty("Google_WebServiceURL");
            mProp_GoogleUpdateFrequency            = serializedObject.FindProperty("GoogleUpdateFrequency");
            mProp_GoogleInEditorCheckFrequency     = serializedObject.FindProperty("GoogleInEditorCheckFrequency");
            mProp_GoogleUpdateDelay                = serializedObject.FindProperty("GoogleUpdateDelay");
            mProp_Google_SpreadsheetKey            = serializedObject.FindProperty("Google_SpreadsheetKey");
            mProp_Google_SpreadsheetName           = serializedObject.FindProperty("Google_SpreadsheetName");
            mProp_CaseInsensitiveTerms             = serializedObject.FindProperty("CaseInsensitiveTerms");
            mProp_Spreadsheet_LocalFileName        = serializedObject.FindProperty("Spreadsheet_LocalFileName");
            mProp_Spreadsheet_LocalCSVSeparator    = serializedObject.FindProperty("Spreadsheet_LocalCSVSeparator");
            mProp_Spreadsheet_LocalCSVEncoding     = serializedObject.FindProperty("Spreadsheet_LocalCSVEncoding");
            mProp_Spreadsheet_SpecializationAsRows = serializedObject.FindProperty("Spreadsheet_SpecializationAsRows");
            mProp_OnMissingTranslation             = serializedObject.FindProperty("OnMissingTranslation");
			mProp_AppNameTerm					   = serializedObject.FindProperty("mTerm_AppName");
			mProp_IgnoreDeviceLanguage			   = serializedObject.FindProperty("IgnoreDeviceLanguage");

            if (!mIsParsing)
			{
				if (string.IsNullOrEmpty(mLanguageSource.Google_SpreadsheetKey))
					mSpreadsheetMode = eSpreadsheetMode.Local;
				else
					mSpreadsheetMode = eSpreadsheetMode.Google;

				mCurrentViewMode = (mLanguageSource.mLanguages.Count>0 ? eViewMode.Keys : eViewMode.Languages);

				UpdateSelectedKeys();

                if (ForceParse || mParsedTerms.Count < mLanguageSource.mTerms.Count)
                {
                    mSelectedCategories.Clear();
                    ParseTerms(true);
                }
			}
            ScheduleUpdateTermsToShowInList();
			LoadSelectedCategories();
            //UpgradeManager.EnablePlugins();
        }

		void OnDisable()
		{
			//LocalizationManager.LocalizeAll();
			SaveSelectedCategories();
		}


        void UpdateSelectedKeys()
		{
			// Remove all keys that are not in this source
			string trans;
			for (int i=mSelectedKeys.Count-1; i>=0; --i)
				if (!mLanguageSource.TryGetTranslation(mSelectedKeys[i], out trans))
					mSelectedKeys.RemoveAt(i);

			// Remove all Categories that are not in this source
			/*var mCateg = mLanguageSource.GetCategories();
			for (int i=mSelectedCategories.Count-1; i>=0; --i)
				if (!mCateg.Contains(mSelectedCategories[i]))
					mSelectedCategories.RemoveAt(i);
			if (mSelectedCategories.Count==0)
				mSelectedCategories = mCateg;*/

			if (mSelectedScenes.Count==0)
				mSelectedScenes.Add (Editor_GetCurrentScene());
        }

        public override void OnInspectorGUI()
		{
			// Load Test:
			/*if (mLanguageSource.mTerms.Count<40000)
			{
				mLanguageSource.mTerms.Clear();
				for (int i=0; i<40020; ++i)
					mLanguageSource.AddTerm("ahh"+i.ToString("00000"), eTermType.Text, false);
				mLanguageSource.UpdateDictionary();
			}*/
            //Profiler.maxNumberOfSamplesPerFrame = -1;    // REMOVE ---------------------------------------------------

			mIsParsing = false;

			//#if UNITY_5_6_OR_NEWER
			//	serializedObject.UpdateIfRequiredOrScript();
			//#else
			//	serializedObject.UpdateIfDirtyOrScript();
			//#endif

			if (mLanguageSource.mTerms.Count<1000)
				Undo.RecordObject(target, "LanguageSource");

			GUI.backgroundColor = Color.Lerp (Color.black, Color.gray, 1);
			GUILayout.BeginVertical(LocalizeInspector.GUIStyle_Background);
			GUI.backgroundColor = Color.white;
			
			if (GUILayout.Button("Language Source", LocalizeInspector.GUIStyle_Header))
			{
				Application.OpenURL(LocalizeInspector.HelpURL_Documentation);
			}

				InitializeStyles();

				GUILayout.Space(10);

				OnGUI_Main();

			GUILayout.Space (10);
			GUILayout.FlexibleSpace();

			GUITools.OnGUI_Footer("I2 Localization", LocalizationManager.GetVersion(), LocalizeInspector.HelpURL_forum, LocalizeInspector.HelpURL_Documentation, LocalizeInspector.HelpURL_AssetStore);

			GUILayout.EndVertical();

			serializedObject.ApplyModifiedProperties();
            if (Event.current.type == EventType.Repaint)
            {
                mTestAction = eTest_ActionType.None;
                mTestActionArg = null;
                mTestActionArg2 = null;
            }
        }

		#endregion
	}
}