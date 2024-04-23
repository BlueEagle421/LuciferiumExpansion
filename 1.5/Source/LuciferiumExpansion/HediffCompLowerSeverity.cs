using RimWorld;
using System.Collections.Generic;
using Verse;

namespace LuciferiumExpansion
{
    public class HediffCompProperties_LowerSeverity : HediffCompProperties
    {
        public float severityToSubtract;
        public HediffDef hediffDef;
        public bool removeIfNoHediff = true;

        [MustTranslate]
        public string noHediffMessage;

        public HediffCompProperties_LowerSeverity() => compClass = typeof(HediffCompLowerSeverity);
    }
    public class HediffCompLowerSeverity : HediffComp
    {
        public HediffCompProperties_LowerSeverity Props => (HediffCompProperties_LowerSeverity)props;

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            base.CompPostPostAdd(dinfo);

            if (FindHediffToAdjust() == null)
                NoHediffFound();
        }

        public override void CompPostMake()
        {
            base.CompPostMake();

            LowerSeverity();
        }

        private void NoHediffFound()
        {
            if (Props.removeIfNoHediff)
                RemoveItself();

            Messages.Message(Props.noHediffMessage.Translate(Pawn.Name.ToStringFull), Pawn, MessageTypeDefOf.NeutralEvent, false);
        }

        private void LowerSeverity()
        {
            Hediff toAdjust = FindHediffToAdjust();

            if (toAdjust == null)
                return;

            toAdjust.Severity = NewSeverity(toAdjust.Severity, toAdjust.def.minSeverity);
        }

        private float NewSeverity(float current, float min)
        {
            if (current <= Props.severityToSubtract)
                return min;

            return current - Props.severityToSubtract;
        }

        private void RemoveItself()
        {
            Hediff firstHediffOfDef = Pawn.health.hediffSet.GetFirstHediffOfDef(parent.def, false);
            if (firstHediffOfDef == null)
                return;

            Pawn.health.RemoveHediff(firstHediffOfDef);
        }

        private Hediff FindHediffToAdjust()
        {
            List<Hediff> allHediffs = new List<Hediff>();
            Pawn.health.hediffSet.GetHediffs(ref allHediffs);

            return allHediffs.Find(x => x.def == Props.hediffDef);
        }
    }
}