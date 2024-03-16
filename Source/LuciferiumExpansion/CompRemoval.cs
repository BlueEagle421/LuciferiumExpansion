
using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace LuciferiumExpansion
{
    [StaticConstructorOnStartup]
    public class CompRemoval : ThingComp
    {
        private int nextProduceTick = -1, ticksGame;
        public CompProperties_LuciferiumPump Props => (CompProperties_LuciferiumPump)this.props;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            Messages.Message((string)"Heavy scarlet tank will soon be deconstructed. See mod's steam page comment section for more information", this.parent, MessageTypeDefOf.NegativeEvent, false);
        }

        public override void CompTick()
        {
            base.CompTick();
            ticksGame = Find.TickManager.TicksGame;
            if (this.nextProduceTick == -1)
                this.nextProduceTick = ticksGame + 15000;
            else if (ticksGame >= this.nextProduceTick)
            {
                this.DeconstructItself();
                this.nextProduceTick = ticksGame + 15000;
            }

        }

        private void DeconstructItself()
        {
            this.parent.Destroy(DestroyMode.Deconstruct);
        }

        public override string CompInspectStringExtra()
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (this != null)
            {
                    stringBuilder.Append((string)"Self deconstruct in: " + ((nextProduceTick - ticksGame) / 100).ToString());
                    stringBuilder.AppendLine();
            }
            return stringBuilder.ToString().TrimEnd();
        }
    }
}
