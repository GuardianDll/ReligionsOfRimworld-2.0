﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace ReligionsOfRimworld
{
    public class ReligionDef : Def
    {
        private List<ReligionSettingsDef> settingsDefs;

        public ReligionDef()
        {
            settingsDefs = new List<ReligionSettingsDef>();
        }

        public IEnumerable<ReligionSettingsDef> SettingsDefs => settingsDefs;
    }
}
