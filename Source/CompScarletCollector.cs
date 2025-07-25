﻿using PipeSystem;
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

    public class CompScarletCollector : ThingComp
    {
        private CompPowerTrader _powerComp;
        private CompResource _resourceComp;
        private int _nextProduceTick = -1;
        internal List<IntVec3> _lumpCells;
        private Map _currentMap;

        public CompProperties_ScarletCollector Props => (CompProperties_ScarletCollector)props;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            _currentMap = parent.Map;
            _powerComp = parent.GetComp<CompPowerTrader>();
            _resourceComp = parent.GetComp<CompResource>();
        }

        public override void PostPostMake()
        {
            base.PostPostMake();

            _nextProduceTick = -1;
        }



        public override void CompTick()
        {
            base.CompTick();
            int ticksGame = Find.TickManager.TicksGame;
            if (_nextProduceTick == -1)
            {
                _nextProduceTick = ticksGame + Props.ticksPerPortion;
                return;
            }
            if (ticksGame >= _nextProduceTick)
            {
                TryProducePortion();
                _nextProduceTick = ticksGame + Props.ticksPerPortion;
            }
        }

        public override string CompInspectStringExtra()
        {
            StringBuilder stringBuilder = new StringBuilder();

            float sludgeLeft = ScarletSludgeManager.Instance.ScarletSludgeAmount;
            stringBuilder.AppendLine("USH_LE_ScarletSludgeLeft".Translate(sludgeLeft, sludgeLeft.ConvertToLuciferium().ToString("0.0")));

            if (!ProductionReport().Accepted)
            {
                stringBuilder.AppendLine("USH_LE_CantProduce".Translate(ProductionReport().Reason).Colorize(ColorLibrary.RedReadable));
                return stringBuilder.ToString().TrimEnd();
            }

            float litersPerDay = LitersPerDay();
            stringBuilder.AppendLine("USH_LE_Producing".Translate(litersPerDay, (litersPerDay * 60f).ConvertToLuciferium().ToString("0.0")));

            stringBuilder.AppendLine("USH_LE_Efficiency".Translate(Efficiency() * 100f));
            return stringBuilder.ToString().TrimEnd();
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo gizmo in base.CompGetGizmosExtra())
                yield return gizmo;

            if (!DebugSettings.ShowDevGizmos)
                yield break;

            yield return new CommandSetScarletSludge
            {
                defaultLabel = "DEV: Set scarlet sludge",
            };
        }

        private void TryProducePortion()
        {
            if (!ProductionReport().Accepted)
                return;

            ScarletSludgeManager.Instance.ScarletSludgeAmount -= DistributeAmount();
            _resourceComp.PipeNet.DistributeAmongStorage(DistributeAmount(), out var _);
        }

        private AcceptanceReport ProductionReport()
        {
            if (_powerComp != null && !_powerComp.PowerOn)
                return "NoPower".Translate().CapitalizeFirst();

            if (!parent.def.canBeUsedUnderRoof && _currentMap.roofGrid.Roofed(parent.Position))
                return "Roofed".Translate().CapitalizeFirst();

            if (_resourceComp.PipeNet != null && (_resourceComp.PipeNet.storages.Count == 0 || _resourceComp.PipeNet.AvailableCapacity < DistributeAmount()))
                return "USH_LE_NoStorage".Translate().CapitalizeFirst();

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
            return Efficiency() * Props.portionSize * LuciferiumExpansionMod.Settings.CollectionSpeedMultiplier;
        }

        private float LitersPerDay()
        {
            return 60000 / Props.ticksPerPortion * DistributeAmount();
        }
    }
}
