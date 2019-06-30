﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace ReligionsOfRimworld
{
    public class ThoughtWorker_NeedPiety : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            CompReligion comp = p.GetReligionComponent();
            if (comp == null)
                return ThoughtState.Inactive;

            ReligionSettings_Need settings = comp.Religion.NeedSettings;
            if(settings == null)
                return ThoughtState.Inactive;

            if (settings.NeedThought != null && settings.NeedThought == this.def)
                return ThoughtState.ActiveAtStage(comp.PietyTracker.Piety.CurCategoryInt);

            return ThoughtState.Inactive;
        }
    }
}
