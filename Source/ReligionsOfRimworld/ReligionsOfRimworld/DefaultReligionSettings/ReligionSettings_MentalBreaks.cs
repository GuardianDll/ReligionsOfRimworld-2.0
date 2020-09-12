﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace ReligionsOfRimworld
{
    public class ReligionSettings_MentalBreaks : ReligionSettings
    {
        private List<MentalBreakDef> mentalBreaks;

        public ReligionSettings_MentalBreaks()
        {
            if(Scribe.mode == LoadSaveMode.Inactive)
            mentalBreaks = new List<MentalBreakDef>();
        }

        public IEnumerable<MentalBreakDef> MentalBreaks => mentalBreaks;

        //public override IEnumerable<ReligionInfoEntry> GetInfoEntries()
        //{
        //    foreach (MentalBreakDef mentalBreak in mentalBreaks)
        //        yield return new ReligionInfoEntry("ReligionInfo_MentalBreak".Translate(), mentalBreak.mentalState.LabelCap, mentalBreak.mentalState.description);
        //}

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look<MentalBreakDef>(ref this.mentalBreaks, "mentalBreaks", LookMode.Def);
        }
    }
}
