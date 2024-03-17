﻿using RimWorld;
using System.Collections.Generic;
using Verse;

namespace LuciferiumExpansion
{
    public class HediffWithComps_USH_InnerOrgansHealer : HediffWithComps
    {
        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);
            List<Hediff> allHediffs = new List<Hediff>();
            pawn.health.hediffSet.GetHediffs(ref allHediffs);
            foreach (Hediff hediff in allHediffs)
            {
                if (hediff.def == HediffDef.Named("USH_InnerOrgansWeakening"))
                {
                    float firstSeverity = hediff.Severity;
                    if (firstSeverity <= 0.125f)
                    {
                        hediff.Severity = 0.01f;
                    }
                    else
                    {
                        hediff.Severity = firstSeverity - 0.125f;
                    }
                    return;
                }
            }

            pawn.health.RemoveHediff(this);
            Messages.Message("USH_LE_MechaNotWorking".Translate(pawn.Name), pawn, MessageTypeDefOf.NeutralEvent, false);
        }
    }
}