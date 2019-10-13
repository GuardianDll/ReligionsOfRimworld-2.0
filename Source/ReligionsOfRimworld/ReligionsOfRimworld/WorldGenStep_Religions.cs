﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace ReligionsOfRimworld
{
    public class WorldGenStep_Religions : WorldGenStep
    {
        public override int SeedPart
        {
            get
            {
                return 777998381;
            }
        }

        public override void GenerateFresh(string seed)
        {
            Log.Message("FRESH");
            ReligionManager.GetReligionManager().Initialize();
        }

        //public override void GenerateFromScribe(string seed)
        //{
        //    base.GenerateFromScribe(seed);
        //    Log.Message("SC");
        //    ReligionManager.GetReligionManager().ExposeData();
        //}
    }
}
