using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.Sound;

namespace LuciferiumExpansion
{
    public class CompProperties_ScarletAttractor : CompProperties
    {
        public float scarletSludgeToAdd = 0.025f;
        public SoundDef soundDef; //USH_ScarletBeam

        public CompProperties_ScarletAttractor() => compClass = typeof(CompScarletAttractor);
    }

    [StaticConstructorOnStartup]
    public class CompScarletAttractor : CompActivable
    {
        private CompPowerTrader _powerComp;
        internal List<IntVec3> _lumpCells;
        private Map _currentMap;

        public CompProperties_ScarletAttractor Props => (CompProperties_ScarletAttractor)props;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            _powerComp = parent.GetComp<CompPowerTrader>();
        }

        public override void Activate()
        {
            base.Activate();

            AttractScarletSludge();
        }

        protected override bool TryUse()
        {
            return true;
        }

        private void AttractScarletSludge()
        {
            PlayAttractionSound();
            SpawnAttractionBeam();

            ScarletSludgeManager.Instance.ScarletSludgeAmount += Props.scarletSludgeToAdd;
            parent.Destroy(DestroyMode.Vanish);
        }

        private void PlayAttractionSound()
        {
            SoundDef.Named("USH_ScarletBeam").PlayOneShot(new TargetInfo(this.parent.Position, this.parent.Map, false));
        }

        private void SpawnAttractionBeam()
        {
            Mote mote = (Mote)ThingMaker.MakeThing(ThingDef.Named("USH_ScarletBeam"), null);
            mote.exactPosition = parent.Position.ToVector3Shifted();
            mote.Scale = 65f;
            mote.rotationRate = 1.4f;
            GenSpawn.Spawn(mote, parent.Position, _currentMap, WipeMode.Vanish);
            FleckMaker.Static(parent.Position, _currentMap, FleckDefOf.PsycastAreaEffect, 10f);
        }
    }
}
