﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace ReligionsOfRimworld
{
    public class ReligionProperty_ThingObject : ReligionProperty
    {
        private ThingDef thingObject;

        public override Def GetObject()
        {
            return thingObject;
        }
    }
}
