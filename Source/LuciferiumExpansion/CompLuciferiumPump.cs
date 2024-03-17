
using RimWorld;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace LuciferiumExpansion
{
    public class CompProperties_LuciferiumPump : CompProperties
    {
        public CompProperties_LuciferiumPump() => this.compClass = typeof(CompLuciferiumPump);
    }


    [StaticConstructorOnStartup]
    public class CompLuciferiumPump : ThingComp
    {
        Map map;
        CompRefuelable compRefuelable;
        private int usagesLeft = 3;
        public CompProperties_LuciferiumPump Props => (CompProperties_LuciferiumPump)this.props;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            map = this.parent.Map;
            compRefuelable = this.parent.GetComp<CompRefuelable>();
            //compPowerTrader = this.parent.GetComp<CompPowerTrader>();
        }

        public override void CompTick()
        {
            base.CompTick();
            checkForDeadPawn();

        }
        public override string CompInspectStringExtra()
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (this != null)
            {
                if (compRefuelable != null)
                {
                    stringBuilder.Append((string)"USH_LE_UsagesLeft".Translate() + ": " + usagesLeft.ToString());
                    stringBuilder.AppendLine();
                    if (!compRefuelable.IsFull)
                        stringBuilder.Append((string)"USH_LE_CantResurrect".Translate() + ": ");
                    stringBuilder.AppendLine();
                    if (!compRefuelable.IsFull)
                    {
                        stringBuilder.Append((string)"USH_LE_NoLuciferium".Translate());
                    }
                }
            }
            return stringBuilder.ToString().TrimEnd();
        }

        public void checkForDeadPawn()
        {

            foreach (Thing thing in this.parent.GetComp<CompFacility>().LinkedBuildings)
            {
                tryResurrection(thing.Position);
                break;
            }
        }

        public void tryResurrection(IntVec3 bedPos)
        {
            foreach (Thing thing in map.thingGrid.ThingsAt(bedPos))
            {
                if (thing is Corpse corpse)
                {
                    if (corpse.InnerPawn.IsColonist && compRefuelable.IsFull && usagesLeft > 0)
                    // && compPowerTrader.PowerOn
                    {
                        Pawn pawn = corpse.InnerPawn;
                        ResurrectionUtility.Resurrect(pawn);
                        compRefuelable.ConsumeFuel(12);

                        applyHediffs(pawn);

                        usagesLeft--;
                        checkUsages();
                    }
                }
            }
        }

        public void applyHediffs(Pawn pawnForHediffs)
        {
            Hediff coma = pawnForHediffs.health.AddHediff(HediffDef.Named("USH_ScarletComa"));
            List<Hediff> allhediffs = new List<Hediff>();
            pawnForHediffs.health.hediffSet.GetHediffs(ref allhediffs);
            foreach (Hediff hediff in allhediffs)
            {
                if (hediff.def == HediffDef.Named("LuciferiumAddiction"))
                {
                    coma.Severity = 1;
                    hediff.Severity = 1;
                }
                else
                {
                    coma.Severity = 5;
                    pawnForHediffs.health.AddHediff(HediffDef.Named("LuciferiumAddiction"));
                }
                break;
            }
        }

        public void checkUsages()
        {
            if (usagesLeft == 0)
            {
                this.parent.Destroy(DestroyMode.Deconstruct);
            }
        }
    }
}
