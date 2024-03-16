// Decompiled with JetBrains decompiler
// Type: VCHE.CompRefinery
// Assembly: VCHE, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 872512D5-06CA-4BFC-9B2D-EE9D4D7F939F
// Assembly location: C:\Users\Joachim\Desktop\RimWorld.v1.3.3200\RimWorld.v1.3.3200\Mods\2792917473_vanilla_chemfuel_expanded\1.3\Assemblies\VCHE.dll

using PipeSystem;
using RimWorld;
using UnityEngine;
using Verse;

namespace LuciferiumExpansion
{
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
