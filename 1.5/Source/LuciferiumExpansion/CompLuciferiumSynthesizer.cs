
using PipeSystem;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;


namespace LuciferiumExpansion
{
    public class CompProperties_LuciferiumSynthesizer : CompProperties
    {
        public ThingDef thing;
        public int thingCount;
        public CompProperties_LuciferiumSynthesizer() => compClass = typeof(CompLuciferiumSynthesizer);
    }


    public class CompLuciferiumSynthesizer : ThingComp
    {
        private CompResourceStorage _compResourceStorage;
        private List<IntVec3> adjCells;
        public CompProperties_LuciferiumSynthesizer Props => (CompProperties_LuciferiumSynthesizer)props;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            _compResourceStorage = parent.GetComp<CompResourceStorage>();
            adjCells = GenAdj.CellsAdjacent8Way(parent).ToList<IntVec3>();
        }

        public override void CompTickRare()
        {
            base.CompTickRare();

            if (_compResourceStorage.AmountStored >= parent.GetStatValue(StatDef.Named("USH_ScarletMechanitesOffset")))
                TrySpawningItem();
        }
        public override string CompInspectStringExtra()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine();

            stringBuilder.AppendLine((string)"USH_LE_LuciStored".Translate(_compResourceStorage.Resource.name) + ": " + ((int)_compResourceStorage.AmountStored).ToString());

            stringBuilder.AppendLine((string)"USH_LE_LuciNeeded".Translate(_compResourceStorage.Resource.name) + ": " + AmountNeeded().ToString());

            return base.CompInspectStringExtra() + stringBuilder.ToString().Trim();
        }

        private int AmountNeeded() => (int)parent.GetStatValue(StatDef.Named("USH_ScarletMechanitesOffset"));

        private void TrySpawningItem()
        {
            Map map = parent.Map;

            if (!parent.GetComp<CompPowerTrader>().PowerOn)
                return;

            for (int index = 0; index < adjCells.Count; ++index)
            {
                IntVec3 adjCell = adjCells[index];
                if (!adjCell.Walkable(map))
                    continue;

                Thing firstThing = adjCell.GetFirstThing(map, Props.thing);
                if (firstThing != null)
                {
                    if (firstThing.stackCount + Props.thingCount <= firstThing.def.stackLimit)
                    {
                        firstThing.stackCount += Props.thingCount;
                        DrawFromStorage();
                        break;
                    }
                }
                else
                {
                    Thing thing = ThingMaker.MakeThing(Props.thing);
                    thing.stackCount = Props.thingCount;
                    if (GenPlace.TryPlaceThing(thing, adjCell, map, ThingPlaceMode.Near))
                    {
                        DrawFromStorage();
                        break;
                    }
                }
            }
        }

        private void DrawFromStorage()
        {
            PipeNet pipeNet = parent.GetComp<CompResource>().PipeNet;
            pipeNet.DrawAmongStorage(parent.GetStatValue(StatDef.Named("USH_ScarletMechanitesOffset")), pipeNet.storages);
        }
    }

}
