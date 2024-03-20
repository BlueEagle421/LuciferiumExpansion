
using RimWorld;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace LuciferiumExpansion
{
    public class CompProperties_LuciferiumPump : CompProperties
    {
        public int baseUsages = 3;
        public HediffDef hediffComa;
        public HediffDef hediffAddiction;
        public CompProperties_LuciferiumPump() => compClass = typeof(CompLuciferiumPump);
    }

    [StaticConstructorOnStartup]
    public class CompLuciferiumPump : ThingComp
    {
        Map _currentMap;
        CompRefuelable _compRefuelable;
        CompPowerTrader _compPowerTrader;
        private int _usagesLeft;
        public CompProperties_LuciferiumPump Props => (CompProperties_LuciferiumPump)props;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            _currentMap = parent.Map;
            _usagesLeft = Props.baseUsages;

            _compRefuelable = parent.GetComp<CompRefuelable>();
            _compPowerTrader = parent.GetComp<CompPowerTrader>();
        }

        private void BeginCorpseTargeting()
        {
            Find.Targeter.BeginTargeting(TargetingParameters(), delegate (LocalTargetInfo t)
            {
                if (!TryToResurrectAt(t.Thing.Position))
                    Messages.Message((string)"USH_LE_CantResurrect".Translate(WorkStateCode().Item2), parent, MessageTypeDefOf.CautionInput, true);

                Find.Targeter.StopTargeting();
            });
        }

        public override string CompInspectStringExtra()
        {
            StringBuilder stringBuilder = new StringBuilder();

            if (this == null)
                return string.Empty;

            var workState = WorkStateCode();

            bool canWork = workState.Item1;
            string cantWorkCode = workState.Item2;

            if (!canWork)
                stringBuilder.AppendLine((string)"USH_LE_CantResurrect".Translate(cantWorkCode));

            stringBuilder.AppendLine((string)"USH_LE_UsagesLeft".Translate(_usagesLeft));

            return stringBuilder.ToString().TrimEnd();
        }

        private (bool, string) WorkStateCode()
        {
            bool canWork = true;
            string cantWorkCode = string.Empty;

            if (_compRefuelable != null && !_compRefuelable.IsFull)
            {
                canWork = false;
                cantWorkCode += string.Format("{0}, ", (string)"USH_LE_NoLuciferium".Translate());
            }

            if (_compPowerTrader != null && !_compPowerTrader.PowerOn)
            {
                canWork = false;
                cantWorkCode += string.Format("{0}, ", (string)"USH_LE_NoPower".Translate());
            }

            cantWorkCode = cantWorkCode.TrimEnd(',', ' ') + '.';

            return (canWork, cantWorkCode);
        }

        public bool TryToResurrectAt(IntVec3 position)
        {
            Corpse toRessurect = FirstCorpseAt(position);

            if (toRessurect == null)
                return false;

            if (!CanResurrect(toRessurect))
                return false;

            Resurrect(toRessurect.InnerPawn);
            return true;
        }

        private void Resurrect(Pawn pawn)
        {
            ResurrectionUtility.Resurrect(pawn);
            HandleHediffs(pawn);

            _compRefuelable.ConsumeFuel(12);
            _usagesLeft--;
            CheckForSelfDeconstruct();
        }

        private bool CanResurrect(Corpse corpse)
        {
            if (!corpse.InnerPawn.IsColonist)
                return false;

            if (!WorkStateCode().Item1)
                return false;

            return true;
        }

        private Corpse FirstCorpseAt(IntVec3 position)
        {
            foreach (Thing thing in _currentMap.thingGrid.ThingsAt(position))
                if (thing is Corpse corpse)
                    return corpse;

            return null;
        }

        public void HandleHediffs(Pawn pawn)
        {
            List<Hediff> allHediffs = new List<Hediff>();
            pawn.health.hediffSet.GetHediffs(ref allHediffs);

            Hediff addedComa = pawn.health.AddHediff(Props.hediffComa);
            Hediff foundAddiction = allHediffs.Find(x => x.def == Props.hediffAddiction);

            if (foundAddiction == null)
            {
                addedComa.Severity = 5;
                pawn.health.AddHediff(Props.hediffAddiction);
            }
            else
            {
                addedComa.Severity = 1;
                foundAddiction.Severity = 1;
            }
        }

        public void CheckForSelfDeconstruct()
        {
            if (_usagesLeft > 0)
                return;

            parent.Destroy(DestroyMode.Deconstruct);
        }

        private TargetingParameters TargetingParameters()
        {
            return new TargetingParameters
            {
                canTargetPawns = false,
                canTargetBuildings = false,
                canTargetItems = true,
                mapObjectTargetsMustBeAutoAttackable = false,
                validator = ((TargetInfo x) => x.Thing is Corpse)
            };
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo gizmo in base.CompGetGizmosExtra())
                yield return gizmo;

            if (!Prefs.DevMode)
                yield break;

            yield return new Command_Action
            {
                defaultLabel = "DEBUG: Resurrect",
                action = delegate
                {
                    BeginCorpseTargeting();
                }
            };
        }
    }
}
