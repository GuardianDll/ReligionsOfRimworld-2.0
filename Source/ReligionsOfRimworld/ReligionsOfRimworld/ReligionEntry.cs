﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace ReligionsOfRimworld
{
    public class ReligionConfiguration : IExposable
    {
        private string label;
        private string description;
        private ReligionGroupTagDef groupTag;
        private List<ReligionSettings> settings;

        public ReligionConfiguration(string label, string description, ReligionGroupTagDef groupTag, IEnumerable<ReligionSettings> settings)
        {
            if(Scribe.mode == LoadSaveMode.Inactive)
            {
                this.label = label;
                this.description = description;
                this.settings = new List<ReligionSettings>();
                this.settings.AddRange(settings);
                this.groupTag = groupTag;
            }
        }

        public string Label => label;
        public string Description => description;
        public ReligionGroupTagDef GroupTag => groupTag;
        public IEnumerable<ReligionSettings> Settings => settings;

        public IEnumerable<ReligionInfo> GetInfo()
        {
            ReligionInfo category = new ReligionInfo("ReligionInfo_Overall".Translate());
            category.Add(new ReligionInfoEntry("ReligionInfo_Description".Translate(), "", description));
            category.Add(new ReligionInfoEntry("ReligionInfo_GroupTag".Translate(), groupTag.LabelCap, groupTag.description));
            yield return category;
            foreach (ReligionSettings setting in settings)
                yield return setting.GetInfoCategory();
        }

        public T FindByTag<T>(SettingsTagDef tag) where T : ReligionSettings
        {
            return (T)settings.FirstOrDefault(x => x.Tag == tag);
        }

        public void ExposeData()
        {
            Scribe_Values.Look<string>(ref this.label, "label");
            Scribe_Values.Look<string>(ref this.description, "descrtiption");
            Scribe_Defs.Look<ReligionGroupTagDef>(ref this.groupTag, "groupTag");
            Scribe_Collections.Look<ReligionSettings>(ref this.settings, "settings", LookMode.Deep);
        }
    }
}
