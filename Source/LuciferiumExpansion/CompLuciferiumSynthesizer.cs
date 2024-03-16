
using PipeSystem;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;


namespace LuciferiumExpansion
{
    [StaticConstructorOnStartup]
    public class CompLuciferiumSynthesizer : ThingComp
    {
        Map map;
        CompRefuelable compRefuelable;
        private List<IntVec3> adjCells;
        private PipeNet pipeNet;
        public CompProperties_LuciferiumSynthesizer Props => (CompProperties_LuciferiumSynthesizer)this.props;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            map = this.parent.Map;
            compRefuelable = this.parent.GetComp<CompRefuelable>();

            this.adjCells = GenAdj.CellsAdjacent8Way((Thing)this.parent).ToList<IntVec3>();

        }

        public override void CompTick()
        {
            base.CompTick();

            this.pipeNet = this.parent.GetComp<CompResource>().PipeNet;
            if (this.parent.GetComp<CompResourceStorage>().AmountStored >= this.parent.GetStatValue(StatDef.Named("USH_ScarletMechanitesOffset")))
            {
                TrySpawningItem();
            }
        }
        public override string CompInspectStringExtra()
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (this != null)
            {
                stringBuilder.Append((string)"USH_LE_ProducingLuciferium".Translate() + " " + this.parent.GetStatValue(StatDef.Named("USH_ScarletMechanitesOffset")).ToString() + " " + (string)"USH_LE_ScarletMechanites".Translate());
            }
            return stringBuilder.ToString().TrimEnd();
        }

        private void TrySpawningItem()
        {
            this.pipeNet = this.parent.GetComp<CompResource>().PipeNet;
            Map map = this.parent.Map;
            if (this.parent.GetComp<CompPowerTrader>().PowerOn)
            {
                for (int index = 0; index < this.adjCells.Count; ++index)
                {
                    IntVec3 adjCell = this.adjCells[index];
                    if (adjCell.Walkable(map))
                    {
                        Thing firstThing = adjCell.GetFirstThing(map, this.Props.thing);
                        if (firstThing != null)
                        {
                            if (firstThing.stackCount + this.Props.thingCount <= firstThing.def.stackLimit)
                            {
                                firstThing.stackCount += this.Props.thingCount;
                                break;
                            }
                        }
                        else
                        {
                            Thing thing = ThingMaker.MakeThing(this.Props.thing);
                            thing.stackCount = this.Props.thingCount;
                            if (GenPlace.TryPlaceThing(thing, adjCell, map, ThingPlaceMode.Near))
                                break;
                        }
                    }
                }
                pipeNet.DrawAmongStorage(this.parent.GetStatValue(StatDef.Named("USH_ScarletMechanitesOffset")), pipeNet.storages);
            }
        }
    }
}
