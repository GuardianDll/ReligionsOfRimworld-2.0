﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace ReligionsOfRimworld
{
    public class ReligionDef : Def
    {
        public List<ReligionSettingsDef> settingsDefs;

        public ReligionDef()
        {
            settingsDefs = new List<ReligionSettingsDef>();
        }

        public IEnumerable<ReligionInfo> GetInfo()
        {
            foreach (ReligionSettingsDef def in settingsDefs)
                yield return def.GetInfoCategory();
        }

        public ReligionSettings FindByTag(SettingsTagDef tag)
        {
            return settingsDefs.FirstOrDefault(x => x.tag == tag).settings;
        }

        public T FindByTag<T>(SettingsTagDef tag) where T : ReligionSettings
        {
            return (T)settingsDefs.FirstOrDefault(x => x.tag == tag).settings;
        }
    }
}
