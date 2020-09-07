﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace ReligionsOfRimworld
{
    public abstract class ReligionProperty : IExposable
    {
        protected ReligionPropertyData subject;
        protected ReligionPropertyData witness;
        protected PropertyPawnCategory pawnCategory = PropertyPawnCategory.Everyone;

        public abstract Def GetObject();
        public PropertyPawnCategory PawnCategory => pawnCategory;

        protected abstract string ObjectLabel { get; }
        protected abstract string Description { get; }

        public virtual bool Contains(Def def)
        {
            return GetObject() == def;
        }

        public T GetObject<T>() where T: Def
        {
            return (T)GetObject();
        }

        public ReligionPropertyData Subject => subject;
        public ReligionPropertyData Witness => witness;

        public ReligionInfoEntry GetReligionInfoEntry()
        {
            if (GetObject() != null)
                return new ReligionInfoEntry(ObjectLabel, "", GetDescription(), GetSubjectOffsets(), GetWtinessOffsets());

            return null;
        }

        private string GetDescription()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("ReligionInfo_PawnCategory".Translate((NamedArgument)pawnCategory.ToString()));
            stringBuilder.AppendLine();

            if (Description != null)
            {
                stringBuilder.AppendLine(Description);
                stringBuilder.AppendLine();
            }

            if (subject != null)
            {
                stringBuilder.AppendLine($"{"ReligionInfo_AsSubject".Translate()}:");
                stringBuilder.Append(subject.GetInfo());
            }
                
            if (witness != null)
            {
                stringBuilder.AppendLine($"{"ReligionInfo_AsWitness".Translate()}:");
                stringBuilder.Append(witness.GetInfo());
            }
            return stringBuilder.ToString();
        }

        private Offset GetSubjectOffsets()
        {
            return subject == null || subject.Piety == null  ? null : new Offset(subject.Piety.Stages.First().PietyOffset, subject.Piety.Stages.Last().PietyOffset); 
        }
        private Offset GetWtinessOffsets()
        {
            //return null;
            return witness == null || witness.Piety == null ? null : new Offset(witness.Piety.Stages.First().PietyOffset, witness.Piety.Stages.Last().PietyOffset);
        }

        public virtual void ExposeData()
        {
            Scribe_Deep.Look<ReligionPropertyData>(ref this.subject, "subject");
            Scribe_Deep.Look<ReligionPropertyData>(ref this.witness, "witness");
            Scribe_Values.Look<PropertyPawnCategory>(ref this.pawnCategory, "pawnCategory");
        }
    }
}
