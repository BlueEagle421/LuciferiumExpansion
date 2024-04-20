using PipeSystem;
using RimWorld;
using UnityEngine;
using Verse;

namespace LuciferiumExpansion
{
    public class CompProperties_ScarletRefinery : CompProperties
    {
        public Vector2 fireDrawPositionOffset = new Vector2(0.04f, 1.05f);
        public CompProperties_ScarletRefinery() => compClass = typeof(CompScarletRefinery);
    }

    public class CompScarletRefinery : ThingComp
    {
        private CompResourceProcessor _resourceProcessor;
        private Vector3 _fireDrawPos;
        public CompProperties_ScarletRefinery Props => (CompProperties_ScarletRefinery)props;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            _resourceProcessor = parent.GetComp<CompResourceProcessor>();
            _fireDrawPos = parent.DrawPos;
            _fireDrawPos.y += Props.fireDrawPositionOffset.x;
            _fireDrawPos.z += Props.fireDrawPositionOffset.y;
        }

        public override void PostDraw()
        {
            if (!_resourceProcessor.Working)
                return;

            CompFireOverlay.FireGraphic.Draw(_fireDrawPos, Rot4.North, parent);
        }
    }
}
