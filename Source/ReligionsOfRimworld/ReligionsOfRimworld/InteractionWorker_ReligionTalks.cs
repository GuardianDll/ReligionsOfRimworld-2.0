﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace ReligionsOfRimworld
{
    public class InteractionWorker_ReligionTalks : InteractionWorker
    {
        public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
        {
            ReligionSettings_ReligionTalks settings = initiator.GetReligionComponent().Religion.GetSettings<ReligionSettings_ReligionTalks>(SettingsTagDefOf.TalksTag);
            if (settings != null && settings.Interaction == this.interaction)
                return settings.BaseChanceOfConversation;
            return 0f;
        }

        public override void Interacted(Pawn initiator, Pawn recipient, List<RulePackDef> extraSentencePacks, out string letterText, out string letterLabel, out LetterDef letterDef, out LookTargets lookTargets)
        {
            base.Interacted(initiator, recipient, extraSentencePacks, out letterText, out letterLabel, out letterDef, out lookTargets);
            CompReligion compReligion = initiator.GetReligionComponent();
            float successChance = ChanceToConvert(initiator, recipient);
            if ((float)new Random().NextDouble() <= successChance)
            {
                letterText = recipient.ToString() + " " + "ReligionInfo_NowBelieveIn".Translate() + " " + compReligion.Religion.Label;
                letterLabel = "ReligionInfo_IsNowReligious".Translate();
                letterDef = LetterDefOf.NeutralEvent;
                recipient.GetReligionComponent().TryChangeReligion(compReligion.Religion);
            }
        }

        private float ChanceToConvert(Pawn initiator, Pawn recipient)
        {
            CompReligion initiatorComp = initiator.GetReligionComponent();
            CompReligion recipientComp = recipient.GetReligionComponent();
            ReligionSettings_ReligionTalks settings = initiatorComp.Religion.GetSettings<ReligionSettings_ReligionTalks>(SettingsTagDefOf.TalksTag);

            if (recipientComp.ReligionRestrictions.MayConvertByTalking)
            {
                float opinionFactor = settings.OpinionFactorCurve.Curve != null ? settings.OpinionFactorCurve.Curve.Evaluate((float)recipient.relations.OpinionOf(initiator)) : 1f;
                float moodFactor = settings.MoodFactorCurve.Curve != null ? settings.MoodFactorCurve.Curve.Evaluate((float)recipient.needs.mood.CurLevel) : 1f;
                float compabilityFactor = recipientComp.ReligionCompability.CompabilityFor(initiatorComp.Religion);

                return 1f * opinionFactor * moodFactor * compabilityFactor;
            }
            else
                return 0f;
        }
    }
}
