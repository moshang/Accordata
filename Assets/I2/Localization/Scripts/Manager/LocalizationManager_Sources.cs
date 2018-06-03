using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Globalization;
using System.Collections;

namespace I2.Loc
{
    public static partial class LocalizationManager
    {

        #region Variables: Misc

        public static List<LanguageSource> Sources = new List<LanguageSource>();
        public static string[] GlobalSources = { "I2Languages" };

        #endregion

        #region Sources

        public static bool UpdateSources()
		{
			UnregisterDeletededSources();
			RegisterSourceInResources();
			RegisterSceneSources();
			return Sources.Count>0;
		}

		static void UnregisterDeletededSources()
		{
			// Delete sources that were part of another scene and not longer available
			for (int i=Sources.Count-1; i>=0; --i)
				if (Sources[i] == null)
					RemoveSource( Sources[i] );
		}

		static void RegisterSceneSources()
		{
			LanguageSource[] sceneSources = (LanguageSource[])Resources.FindObjectsOfTypeAll( typeof(LanguageSource) );
			for (int i=0, imax=sceneSources.Length; i<imax; ++i)
				if (!Sources.Contains(sceneSources[i]))
				{
					AddSource( sceneSources[i] );
				}
		}		

		static void RegisterSourceInResources()
		{
			// Find the Source that its on the Resources Folder
			foreach (string SourceName in GlobalSources)
			{
				GameObject Prefab = (ResourceManager.pInstance.GetAsset<GameObject>(SourceName));
				LanguageSource GlobalSource = (Prefab ? Prefab.GetComponent<LanguageSource>() : null);
				
				if (GlobalSource && !Sources.Contains(GlobalSource))
                {
                    GlobalSource.mIsGlobalSource = true;
                    AddSource(GlobalSource);
                }
            }
		}		

		internal static void AddSource ( LanguageSource Source )
		{
			if (Sources.Contains (Source))
				return;

            Sources.Add( Source );

			if (Source.HasGoogleSpreadsheet() && Source.GoogleUpdateFrequency != LanguageSource.eGoogleUpdateFrequency.Never)
			{
                #if !UNITY_EDITOR
                    Source.Import_Google_FromCache();
                    bool justCheck = false;
                #else
                    bool justCheck=true;
                #endif
                if (Source.GoogleUpdateDelay > 0)
						CoroutineManager.Start( Delayed_Import_Google(Source, Source.GoogleUpdateDelay, justCheck) );
				else
					Source.Import_Google(false, justCheck);
            }

            //if (force)
            {
                for (int i = 0; i < Source.mLanguages.Count(); ++i)
                    Source.mLanguages[i].SetLoaded(true);
            }

            if (Source.mDictionary.Count==0)
				Source.UpdateDictionary(true);
		}

		static IEnumerator Delayed_Import_Google ( LanguageSource source, float delay, bool justCheck )
		{
			yield return new WaitForSeconds( delay );
			source.Import_Google(false, justCheck);
		}

		internal static void RemoveSource (LanguageSource Source )
		{
			//Debug.Log ("RemoveSource " + Source+" " + Source.GetInstanceID());
			Sources.Remove( Source );
		}

		public static bool IsGlobalSource( string SourceName )
		{
			return System.Array.IndexOf(GlobalSources, SourceName)>=0;
		}

		public static LanguageSource GetSourceContaining( string term, bool fallbackToFirst = true )
		{
			if (!string.IsNullOrEmpty(term))
			{
				for (int i=0, imax=Sources.Count; i<imax; ++i)
				{
					if (Sources[i].GetTermData(term) != null)
						return Sources[i];
				}
			}
			
			return ((fallbackToFirst && Sources.Count>0) ? Sources[0] :  null);
		}

		public static Object FindAsset (string value)
		{
			for (int i=0, imax=Sources.Count; i<imax; ++i)
			{
				Object Obj = Sources[i].FindAsset(value);
				if (Obj)
					return Obj;
			}
			return null;
		}

        #endregion

 	}
}
