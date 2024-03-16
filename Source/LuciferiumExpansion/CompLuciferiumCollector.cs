// Decompiled with JetBrains decompiler
// Type: VCHE.CompPumpjack
// Assembly: VCHE, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 872512D5-06CA-4BFC-9B2D-EE9D4D7F939F
// Assembly location: C:\Users\Joachim\Desktop\RimWorld.v1.3.3200\RimWorld.v1.3.3200\Mods\2792917473_vanilla_chemfuel_expanded\1.3\Assemblies\VCHE.dll

using PipeSystem;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace LuciferiumExpansion
{
    [StaticConstructorOnStartup]
    public class CompScarlettCollector : ThingComp
    {
        private CompResourceStorage resource;
        private CompPowerTrader powerComp;
        private int nextProduceTick = -1;
        private bool noCapacity = false;

        public CompProperties_ScarlettCollector Props => (CompProperties_ScarlettCollector)this.props;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            this.resource = this.parent.GetComp<CompResourceStorage>();
            this.powerComp = this.parent.GetComp<CompPowerTrader>();
        }

        public override void PostDeSpawn(Map map) => this.nextProduceTick = -1;

        public override void PostExposeData()
        {
            Scribe_Values.Look<int>(ref this.nextProduceTick, "nextProduceTick");
            Scribe_Values.Look<bool>(ref this.noCapacity, "noCapacity");
        }

        public override void CompTick()
        {
            base.CompTick();
            if (this.parent.Spawned)
            {
                int ticksGame = Find.TickManager.TicksGame;
                if (this.nextProduceTick == -1)
                    this.nextProduceTick = ticksGame + this.Props.ticksPerPortion;
                else if (ticksGame >= this.nextProduceTick)
                {
                    this.TryProducePortion();
                    this.nextProduceTick = ticksGame + this.Props.ticksPerPortion;
                }
            }
        }

        public override string CompInspectStringExtra()
        {
            if (this.parent.Spawned)
            {
                if (this.noCapacity)
                    return (string)"VCHE_CantPump".Translate();
                if (this.parent.Spawned)
                    return (string)"YourMom".Translate();
            }
            return (string)null;
        }

        public override void PostDraw()
        {
            base.PostDraw();
        }

        private void TryProducePortion()
        {
            if (this.powerComp != null && this.powerComp.PowerOn)
            {
                if (this.resource != null)
                {
                    PipeNet pipeNet = this.resource.PipeNet;
                    if (pipeNet.connectors.Count > 1)
                    {
                        this.noCapacity = (double)pipeNet.AvailableCapacity <= 0.0;
                        if (!this.noCapacity)
                        {
                            pipeNet.DistributeAmongStorage(1f);
                            return;
                        }
                        return;
                    }
                }
            }
        }

        private void DrawMat(Material mat, Vector3 drawPos) => Graphics.DrawMesh(MeshPool.plane10, Matrix4x4.TRS(drawPos, this.parent.Rotation.AsQuat, new Vector3(3f, 1f, 3f)), mat, 0);

    }
}
