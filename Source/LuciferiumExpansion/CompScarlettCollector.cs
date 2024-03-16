using PipeSystem;
using RimWorld;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;

namespace LuciferiumExpansion
{
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
                _nextProduceTick = ticksGame + CollectorProperties.TicksPerPortion;
                return;
            }
            if (ticksGame >= _nextProduceTick)
            {
                TryProducePortion();
                _nextProduceTick = ticksGame + CollectorProperties.TicksPerPortion;
            }
        }

        public override string CompInspectStringExtra()
        {
            StringBuilder stringBuilder = new StringBuilder();

            if (this == null)
                return string.Empty;

            stringBuilder.Append("USH_LE_Efficiency".Translate() + ": " + (1.0 * (Mathf.Round(SpeedFactor() * 100f) / 100.0)).ToString() + "%");
            stringBuilder.AppendLine();
            if (_powerComp.PowerOn)
            {
                bool flag2 = !_currentMap.roofGrid.Roofed(parent.Position);
                if (flag2)
                {
                    stringBuilder.Append("USH_LE_Producing".Translate() + ": " + ((float)(60000 / CollectorProperties.TicksPerPortion) * (Mathf.Round(parent.GetStatValue(StatDef.Named("USH_ScarlettCollectorCollectingSpeedFactor"), true, -1) * 100f) / 100f)).ToString() + " l/d");
                }
                else
                {
                    stringBuilder.Append("USH_LE_Not_Producing_Roofed".Translate());
                }
            }
            char[] empty = { };
            return stringBuilder.ToString().TrimEnd(empty);
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

            float toDistribute = (float)(1.0 * (Mathf.Round(SpeedFactor() * 100f) / 100.0));
            _currentPipeNet.DistributeAmongStorage(toDistribute, out var _);
        }

        private void UpdatePipeNet()
        {
            _currentPipeNet = parent.GetComp<CompResource>().PipeNet;
        }

        private float SpeedFactor()
        {
            return parent.GetStatValue(StatDef.Named("USH_ScarlettCollectorCollectingSpeedFactor"), true, -1);
        }
    }
}
