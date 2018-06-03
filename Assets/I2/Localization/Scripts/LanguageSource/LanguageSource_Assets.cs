using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace I2.Loc
{
	public partial class LanguageSource
	{
        #region Variables

        public List<Object> Assets = new List<Object>();	// References to Fonts, Atlasses and other objects the localization may need

        //This is used to overcome the issue with Unity not serializing Dictionaries
        [NonSerialized] public Dictionary<string, Object> mAssetDictionary = new Dictionary<string, Object>(StringComparer.Ordinal);

        #endregion

        #region Assets

        public void UpdateAssetDictionary()
        {
            Assets.RemoveAll(x => x == null);
            mAssetDictionary = Assets.Distinct().ToDictionary(o => o.name);
        }

        public Object FindAsset( string Name )
		{
			if (Assets!=null)
			{
                if (mAssetDictionary==null || mAssetDictionary.Count!=Assets.Count)
                {
                    UpdateAssetDictionary();
                }
                Object obj;
                if (mAssetDictionary.TryGetValue(Name, out obj))
                {
                    return obj;
                }
				//for (int i=0, imax=Assets.Length; i<imax; ++i)
				//	if (Assets[i]!=null && Name.EndsWith( Assets[i].name, StringComparison.OrdinalIgnoreCase))
				//		return Assets[i];
			}
			return null;
		}
		
		public bool HasAsset( Object Obj )
		{
			return Assets.Contains(Obj);
		}

		public void AddAsset( Object Obj )
		{
            if (Assets.Contains(Obj))
                return;

            Assets.Add(Obj);
            UpdateAssetDictionary();
		}

		
		#endregion
	}
}