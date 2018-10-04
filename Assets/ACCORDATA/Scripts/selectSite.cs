/* Copyright (c) Jean Marais / MoShang 2018. Licensed under GPLv3.
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class selectSite : MonoBehaviour
{
    // -> ACCORDATA <-
    public int siteIndex;
    private Toggle toggle;
    private seqGenerator seqGen;

    private void OnEnable()
    {
        toggle = GetComponent<Toggle>();
        if (toggle == null)
            toggle = GetComponent<Toggle>();
        if (seqGen == null)
            seqGen = GameObject.Find("AccordataController").GetComponent<seqGenerator>();
    }

    public void setSite()
    {
        seqGen.data.getData72HR(siteIndex);
        seqGen.thisSiteIndex = siteIndex;
        seqGen.uiCtrl.currentSiteIndex = siteIndex;
        seqGen.nextSiteIndex = (siteIndex + 1) % dataLoader.numSitesToUse;
    }
}
