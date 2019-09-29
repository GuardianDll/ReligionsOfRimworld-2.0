﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace ReligionsOfRimworld
{
    public class ReligionProperty_ReligionDef : ReligionProperty
    {
        private ReligionDef propertyObject;

        public override Def GetObject()
        {
            return propertyObject;
        }

        protected override string ObjectLabel => "ReligionInfo_Job".Translate();

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look<ReligionDef>(ref this.propertyObject, "religionDef");
        }
    }
}