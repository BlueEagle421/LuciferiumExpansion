using PipeSystem;
using RimWorld;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;

namespace LuciferiumExpansion
{
    public class CompProperties_ScarletCollector : CompProperties
    {
        public int ticksPerPortion = 60;
        public float portionSize = 0.025f;
        public SoundDef ambientSound;

        public CompProperties_ScarletCollector() => compClass = typeof(CompScarletCollector);
    }

    [StaticConstructorOnStartup]
    public class CompScarletCollector : ThingComp
    {
        private CompPowerTrader _powerComp;
        private CompResource _resourceComp;
        private int _nextProduceTick = -1;
        internal List<IntVec3> _lumpCells;
        private Map _currentMap;

        public CompProperties_ScarletCollector CollectorProperties => (CompProperties_ScarletCollector)props;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            _currentMap = parent.Map;
            _powerComp = parent.GetComp<CompPowerTrader>();
            _resourceComp = parent.GetComp<CompResource>();
        }


        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);
            _nextProduceTick = -1;
        }

        public override void CompTick()
        {
            base.CompTick();
            int ticksGame = Find.TickManager.TicksGame;
            if (_nextProduceTick == -1)
            {
                _nextProduceTick = ticksGame + CollectorProperties.ticksPerPortion;
                return;
            }
            if (ticksGame >= _nextProduceTick)
            {
                TryProducePortion();
                _nextProduceTick = ticksGame + CollectorProperties.ticksPerPortion;
            }
        }

        public override string CompInspectStringExtra()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("USH_LE_ScarletSludgeLeft".Translate(_resourceComp.Props.Resource.name, ScarletSludgeManager.Instance.ScarletSludgeAmount));

            if (!CanProduce().Accepted)
            {
                stringBuilder.AppendLine(("DisabledCommand".Translate() + ": " + CanProduce().Reason).Colorize(ColorLibrary.RedReadable));
                return stringBuilder.ToString().TrimEnd();
            }

            stringBuilder.AppendLine("USH_LE_Efficiency".Translate((Efficiency() * 100f).ToString()));

            stringBuilder.AppendLine("USH_LE_Producing".Translate(LitersPerDay().ToString()));

            return stringBuilder.ToString().TrimEnd();
        }

        private void TryProducePortion()
        {
            if (!CanProduce().Accepted)
                return;

            _resourceComp.PipeNet.DistributeAmongStorage(DistributeAmount(), out var _);
        }

        private AcceptanceReport CanProduce()
        {
            if (_powerComp != null && !_powerComp.PowerOn)
                return "NoPower".Translate().CapitalizeFirst();

            if (!parent.def.canBeUsedUnderRoof && _currentMap.roofGrid.Roofed(parent.Position))
                return "Roofed".Translate().CapitalizeFirst();

            if (_resourceComp.PipeNet != null && _resourceComp.PipeNet.storages.Count == 0)
                return "PipeSystem_NoStorageInNet".Translate().CapitalizeFirst();

            if (ScarletSludgeManager.Instance.ScarletSludgeAmount <= 0)
                return "USH_LE_NoScarletSludge".Translate(_resourceComp.PipeNet.def.resource.name).CapitalizeFirst();

            return true;
        }

        private float SpeedFactor()
        {
            return parent.GetStatValue(StatDef.Named("USH_ScarlettCollectorCollectingSpeedFactor"), true, -1);
        }

        private float Efficiency()
        {
            return (float)(1.0 * (Mathf.Round(SpeedFactor() * 100f) / 100.0));
        }

        private float DistributeAmount()
        {
            return Efficiency() * CollectorProperties.portionSize;
        }

        private float LitersPerDay()
        {
            return 60000 / CollectorProperties.ticksPerPortion * DistributeAmount();
        }
    }
}
