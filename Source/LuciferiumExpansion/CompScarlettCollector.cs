using PipeSystem;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;

namespace LuciferiumExpansion
{
    public class CompProperties_ScarlettCollector : CompProperties
    {
        public int ticksPerPortion = 60;
        public float portionSize = 0.025f;
        public SoundDef ambientSound;

        public CompProperties_ScarlettCollector() => compClass = typeof(CompScarlettCollector);
    }

    [StaticConstructorOnStartup]
    public class CompScarlettCollector : ThingComp
    {
        private CompPowerTrader _powerComp;
        private int _nextProduceTick = -1;
        internal List<IntVec3> _lumpCells;
        private PipeNet _currentPipeNet;
        private Map _currentMap;

        public CompProperties_ScarlettCollector CollectorProperties => (CompProperties_ScarlettCollector)props;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            _currentMap = parent.Map;
            _powerComp = parent.GetComp<CompPowerTrader>();
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

            if (this == null)
                return string.Empty;

            if (!_powerComp.PowerOn)
                return string.Empty;

            if (_currentMap.roofGrid.Roofed(parent.Position))
                return "USH_LE_Not_Producing_Roofed".Translate();

            stringBuilder.Append("USH_LE_Efficiency".Translate() + ": " + (Efficiency() * 100f).ToString() + "%");
            stringBuilder.AppendLine();

            string litersPerDay = (60000 / CollectorProperties.ticksPerPortion * DistributeAmount()).ToString() + " l/d";
            stringBuilder.Append("USH_LE_Producing".Translate() + ": " + litersPerDay);

            return stringBuilder.ToString().TrimEnd(Array.Empty<char>());
        }

        private void TryProducePortion()
        {
            UpdatePipeNet();

            if (!_powerComp.PowerOn)
                return;

            if (_currentMap.roofGrid.Roofed(parent.Position))
                return;

            if (parent.def.canBeUsedUnderRoof)
                return;

            if (_currentPipeNet == null)
                return;

            _currentPipeNet.DistributeAmongStorage(DistributeAmount(), out var _);
        }

        private void UpdatePipeNet()
        {
            _currentPipeNet = parent.GetComp<CompResource>().PipeNet;
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
    }
}
