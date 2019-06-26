﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace ReligionsOfRimworld
{
    public class Religion_SocialSettings : ReligionSettings
    {
        private ReligionProperty defaultPropety;
        private List<ReligionProperty> properties;

        public Religion_SocialSettings()
        {
            properties = new List<ReligionProperty>();
        }

        public ReligionProperty DefaultPropety => defaultPropety;
        public IEnumerable<ReligionProperty> Properties => properties;

        public ReligionProperty GetPropertyBySubject(Def def)
        {
            ReligionProperty property = properties.FirstOrDefault(x => x.SubjectThing == def);
            if (property != null)
                return property;
            return defaultPropety;
        }

        public override IEnumerable<ReligionInfoEntry> GetInfoEntries()
        {
            if (defaultPropety != null)
                foreach (ReligionInfoEntry entry in defaultPropety.GetInfoEntries())
                    yield return entry;

            foreach (ReligionProperty property in properties)
                foreach (ReligionInfoEntry entry in property.GetInfoEntries())
                    yield return entry;
        }
    }
}
