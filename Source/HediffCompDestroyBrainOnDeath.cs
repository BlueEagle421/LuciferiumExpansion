using RimWorld;
using Verse;

namespace LuciferiumExpansion
{
    public class HediffCompProperties_DestroyBrainOnDeath : HediffCompProperties
    {
        public HediffCompProperties_DestroyBrainOnDeath() => compClass = typeof(HediffCompDestroyBrainOnDeath);
    }

    public class HediffCompDestroyBrainOnDeath : HediffComp
    {
        public HediffCompProperties_DestroyBrainOnDeath Props => (HediffCompProperties_DestroyBrainOnDeath)props;

        public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
        {
            base.Notify_PawnDied(dinfo, culprit);

            RemoveBrain();
        }

        private void RemoveBrain()
        {
            BodyPartRecord brain = Pawn.health.hediffSet.GetBrain();
            if (brain == null)
                return;

            Hediff hediff = HediffMaker.MakeHediff(HediffDefOf.MissingBodyPart, Pawn, brain);
            Pawn.health.AddHediff(hediff, brain, null, null);

        }
    }
}