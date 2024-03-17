using PipeSystem;
using RimWorld;
using UnityEngine;
using Verse;

namespace LuciferiumExpansion
{
    public class CompProperties_ScarlettRefinery : CompProperties
    {
        public CompProperties_ScarlettRefinery() => this.compClass = typeof(CompScarlettRefinery);
    }

    public class CompScarlettRefinery : ThingComp
    {
        private CompResourceProcessor resource;
        private Vector3 fireDrawPos;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            this.resource = this.parent.GetComp<CompResourceProcessor>();
            this.fireDrawPos = this.parent.DrawPos;
            this.fireDrawPos.y += 0.04054054f;
            this.fireDrawPos.z += 1.91f;
        }

        public override void PostDraw()
        {
            if (!this.resource.Working)
                return;
            CompFireOverlay.FireGraphic.Draw(this.fireDrawPos, Rot4.North, (Thing)this.parent);
        }
    }
}
