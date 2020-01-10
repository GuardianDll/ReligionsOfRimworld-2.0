﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace ReligionsOfRimworld
{
    public class ReligionConfiguration : IExposable
    {
        private ReligionDef religionDef;
        private string label;
        private string description;
        private ReligionGroupTagDef groupTag;
        private List<ReligionSettings> allSettings;

        //public ReligionConfiguration(string label, string description, ReligionGroupTagDef groupTag, IEnumerable<ReligionSettings> settings)
        //{
        //    if(Scribe.mode == LoadSaveMode.Inactive)
        //    {
        //        this.label = label;
        //        this.description = description;
        //        this.allSettings = new List<ReligionSettings>();
        //        this.allSettings.AddRange(settings);
        //        this.groupTag = groupTag;
        //    }
        //}

        public ReligionConfiguration()
        { }

        public ReligionConfiguration(ReligionDef def)
        {
            this.religionDef = def;
            this.label = def.LabelCap;
            this.description = def.description;
            this.groupTag = def.GroupTag;
            this.allSettings = new List<ReligionSettings>();

            foreach (ReligionSettingsDef settingsDef in def.Settings)
                allSettings.Add(settingsDef.Settings);
        }

        public ReligionDef Def => religionDef;
        public string Label => label;
        public string Description => description;
        public ReligionGroupTagDef GroupTag => groupTag;
        public IEnumerable<ReligionSettings> Settings => allSettings;

        public void TryAddSettings(ReligionSettings settings)
        {
            ReligionSettings actualSettings = FindByTag(settings.Tag);
            if (actualSettings == null)
                allSettings.Add(settings);
            else
                Log.Warning("Tried to add settings with tag that already used in configuration");
        }

        public void TryRemoveSettings(SettingsTagDef settingsTag)
        {
            ReligionSettings actualSettings = FindByTag(settingsTag);
            if (actualSettings != null)
                allSettings.Remove(actualSettings);
            else
                Log.Warning("Tried to remove settings with tag that never used in configuration");
        }

        public void TryChangeSettings(ReligionSettings settings)
        {
            TryRemoveSettings(settings.Tag);
            TryAddSettings(settings);
        }

        public ReligionSettings FindByTag(SettingsTagDef tag)
        {
            return allSettings.FirstOrDefault(x => x.Tag == tag);
        }

        public T FindByTag<T>(SettingsTagDef tag) where T : ReligionSettings
        {
            return (T)allSettings.FirstOrDefault(x => x.Tag == tag);
        }

        public IEnumerable<ReligionInfoCategory> GetInfo()
        {
            ReligionInfoCategory category = new ReligionInfoCategory("ReligionInfo_Overall".Translate(), description);

            if(groupTag != null)
            {
                category.Add(new ReligionInfoEntry("ReligionInfo_GroupTag".Translate(), groupTag.LabelCap, groupTag.description));
                yield return category;
            }

            foreach (ReligionSettings setting in allSettings)
                yield return setting.GetInfoCategory();
        }

        public bool TryToRecache()
        {
            if (religionDef != null)
            {
                Log.Message("OK");
                this.allSettings = new List<ReligionSettings>();
                foreach (ReligionSettingsDef settingsDef in religionDef.Settings)
                    allSettings.Add(settingsDef.Settings);
                return true;
            }
            else
                return false;
        }

        public void ExposeData()
        {
            Scribe_Defs.Look<ReligionDef>(ref this.religionDef, "religionDef");
            Scribe_Values.Look<string>(ref this.label, "label");
            Scribe_Values.Look<string>(ref this.description, "descrtiption");
            Scribe_Defs.Look<ReligionGroupTagDef>(ref this.groupTag, "groupTag");
            Scribe_Collections.Look<ReligionSettings>(ref this.allSettings, "settings", LookMode.Deep);
        }
    }
}
