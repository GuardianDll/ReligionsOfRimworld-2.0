﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace ReligionsOfRimworld
{
    public abstract class ReligionSettings
    {
        public abstract IEnumerable<ReligionInfoEntry> GetInfoEntries();
    }
}
