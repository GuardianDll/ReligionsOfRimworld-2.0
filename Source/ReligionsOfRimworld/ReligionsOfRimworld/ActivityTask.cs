﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace ReligionsOfRimworld
{
    public class ActivityTask : IExposable, ILoadReferenceable
    {
        private int loadID = -1;
        private ScheduledDay dayOfTask;
        private SimpleFilter filter;
        private float ingredientSearchRadius = 999f;
        private int lastIngredientSearchFailTicks = -99999;
        private bool suspended;
        private Pawn pawnRestriction;
        private ReligionProperty property;
        private int startHour;
        private IngredientPawn humanlike;
        private IngredientPawn animal;
        private int lastCompletedTick;

        public ActivityTask(ScheduledDay dayOfTask, ReligionProperty property)
        {
            this.dayOfTask = dayOfTask;
            if (Scribe.mode == LoadSaveMode.Inactive)
            {
                this.loadID = Find.UniqueIDsManager.GetNextThingID();
                startHour = 18;
                this.property = property;
                this.loadID = Find.UniqueIDsManager.GetNextBillID();
                humanlike = new IngredientPawn();
                animal = new IngredientPawn();
                List<ThingDef> defs = new List<ThingDef>();
                foreach (ThingDefsCount tcd in ActivityTaskDef.ThingDefsCount)
                    defs.Add(tcd.Thing);
                filter = new SimpleFilter(defs);
            }
        }

        public int StartHour
        {
            get => startHour;
            set
            {
                startHour = value;
                dayOfTask.Reorder();
            }
        }
        public Building_ReligiousBuildingFacility ParentFacility => dayOfTask.ParentSchedule.Facility;
        public bool Suspended { get => suspended; set => suspended = value; }
        public SimpleFilter ThingFilter => filter;
        public Pawn PawnRestriction { get => pawnRestriction; set => pawnRestriction = value; }
        public float IngredientSearchRadius { get => ingredientSearchRadius; set => ingredientSearchRadius = value; }
        public int LastIngredientSearchFailTicks { get => lastIngredientSearchFailTicks; set => lastIngredientSearchFailTicks = value; }
        public IngredientPawn HumanlikeIngredient => humanlike;
        public IngredientPawn AnimalIngredient => animal;
        public ReligionProperty Property => property;
        public ActivityTaskDef ActivityTaskDef => property.GetObject<ActivityTaskDef>();
        public string Label => ActivityTaskDef.LabelCap;
        public string Description => ActivityTaskDef.description;

        public void ValidateSettings()
        {
            if (pawnRestriction != null && pawnRestriction.Dead)
                pawnRestriction = null;
            if (humanlike != null)
                humanlike.ValidateSettings();
            if(animal != null)
                animal.ValidateSettings();
        }

        public bool ShouldDoNow()
        {
            if (!suspended)
            {
                int currentHour = GenLocalDate.HourOfDay(Find.CurrentMap);
                if(Math.Abs(startHour - currentHour) <= 1)
                {
                    if (lastCompletedTick == 0)
                        return true;
                    else
                        return Find.TickManager.TicksGame >= lastCompletedTick + 7500;
                }
            }
            return false;
        }

        public void Notify_IterationCompleted(Pawn pawn)
        {
            lastCompletedTick = Find.TickManager.TicksGame;
        }

        public bool PawnAllowedToStartAnew(Pawn p)
        {
            if (humanlike.ConcretePawn == p)
                return false;
            if (this.pawnRestriction != null)
                return this.pawnRestriction == p;
            return p.GetReligionComponent().Religion == ParentFacility.AssignedReligion;
        }

        public Rect DoInterface(float x, float y, float width, int index)
        {
            Rect rect1 = new Rect(x, y, width, 53f);
            Color baseColor = Color.white;
            if (!this.ShouldDoNow())
                baseColor = new Color(1f, 0.7f, 0.7f, 0.7f);
            GUI.color = baseColor;
            Text.Font = GameFont.Small;
            if (index % 2 == 0)
                Widgets.DrawAltRect(rect1);
            GUI.BeginGroup(rect1);
            Widgets.Label(new Rect(20, 0.0f, (float)((double)rect1.width - 48.0 - 20.0), rect1.height + 5f), this.startHour.ToString());
            Widgets.Label(new Rect(50f, 0.0f, (float)((double)rect1.width - 48.0 - 20.0), rect1.height + 5f), this.ActivityTaskDef.LabelCap);
            this.DoConfigInterface(rect1.AtZero(), baseColor);
            Rect rect5 = new Rect(rect1.width - 24f, 0.0f, 24f, 24f);
            rect5.x -= rect5.width + 4f;
            if (Widgets.ButtonImage(rect5, GraphicsCache.Suspend, baseColor))
            {
                this.suspended = !this.suspended;
                SoundDefOf.Click.PlayOneShotOnCamera((Map)null);
            }
            TooltipHandler.TipRegion(rect5, (TipSignal)"SuspendBillTip".Translate());
            GUI.EndGroup();
            if (this.suspended)
            {
                Text.Font = GameFont.Medium;
                Text.Anchor = TextAnchor.MiddleCenter;
                Rect rect3 = new Rect((float)((double)rect1.x + (double)rect1.width / 2.0 - 70.0), (float)((double)rect1.y + (double)rect1.height / 2.0 - 20.0), 140f, 40f);
                GUI.DrawTexture(rect3, (Texture)TexUI.GrayTextBG);
                Widgets.Label(rect3, "SuspendedCaps".Translate());
                Text.Anchor = TextAnchor.UpperLeft;
                Text.Font = GameFont.Small;
            }
            Text.Font = GameFont.Small;
            GUI.color = Color.white;
            return rect1;
        }

        public void TryDrawIngredientSearchRadiusOnMap()
        {
            if ((double)ingredientSearchRadius >= (double)GenRadial.MaxRadialPatternRadius)
                return;
            GenDraw.DrawRadiusRing(ParentFacility.Position, ingredientSearchRadius);
        }

        private void DoConfigInterface(Rect baseRect, Color baseColor)
        {
            Rect rect = new Rect(28f, 32f, 100f, 30f);
            GUI.color = new Color(1f, 1f, 1f, 0.65f);
            //Widgets.Label(rect, this.RepeatInfoText);
            GUI.color = baseColor;
            WidgetRow widgetRow = new WidgetRow(baseRect.xMax, baseRect.y + 29f, UIDirection.LeftThenUp, 99999f, 4f);
            if (widgetRow.ButtonText("Details".Translate() + "...", (string)null, true, false))
                Find.WindowStack.Add((Window)new Dialog_ActivityTaskConfig(this));
        }

        public string GetUniqueLoadID()
        {
            return "ActivityTask_" + (object)this.loadID;
        }

        public void ExposeData()
        {
            Scribe_Values.Look<int>(ref loadID, "loadID");
            Scribe_Deep.Look<ReligionProperty>(ref this.property, "property");
            Scribe_Deep.Look<SimpleFilter>(ref this.filter, "filter", Enumerable.Empty<ThingDef>());
            Scribe_Values.Look<float>(ref ingredientSearchRadius, "ingredientSearchRadius");
            Scribe_Values.Look<int>(ref lastIngredientSearchFailTicks, "lastIngredientSearchFailTicks");
            Scribe_Values.Look<bool>(ref suspended, "suspended");
            Scribe_References.Look<Pawn>(ref this.pawnRestriction, "pawnRestriction");
            Scribe_Values.Look<int>(ref startHour, "startHour");
            Scribe_Deep.Look<IngredientPawn>(ref this.humanlike, "humanlikeIngredient");
            Scribe_Deep.Look<IngredientPawn>(ref this.animal, "animalIngredient");
            Scribe_Values.Look<int>(ref this.lastCompletedTick, "lastCompletedTick");
        }
    }
}
