using System.Linq;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Nanotech
{
    public class CompUseEffect_InstallHediffSimple : CompUseEffect
    {
        public CompProperties_InstallHediffSimple Props => (CompProperties_InstallHediffSimple)props;

        public override void DoEffect(Pawn user)
        {
            base.DoEffect(user);
            if (user == null || user.DestroyedOrNull() || user.Map == null) return;

            if (Props.humanlikesOnly && (user.RaceProps?.Humanlike != true))
            {
                Messages.Message("Only humanlikes can use this.", user, MessageTypeDefOf.RejectInput);
                return;
            }
            if (Props.hediff == null)
            {
                Messages.Message("Implant not configured (hediff).", user, MessageTypeDefOf.RejectInput);
                return;
            }
            if (Props.failIfAlreadyInstalled && user.health.hediffSet.HasHediff(Props.hediff))
            {
                Messages.Message($"{user.LabelShortCap} already has {Props.hediff.label}.", user, MessageTypeDefOf.RejectInput);
                return;
            }

            var part = FindTargetPart(user);

            if (part == null && RequiresBodyPart(Props.hediff))
            {
                Messages.Message("No valid body part to install on.", user, MessageTypeDefOf.RejectInput);
                return;
            }

            if (part != null && Props.allowInstallOnMissing && user.health.hediffSet.PartIsMissing(part))
            {
                TryRestoreMissingChain(user, part);
                if (user.health.hediffSet.PartIsMissing(part))
                {
                    Messages.Message($"Cannot restore missing {part.Label}.", user, MessageTypeDefOf.RejectInput);
                    return;
                }
            }

            var h = HediffMaker.MakeHediff(Props.hediff, user, part);
            user.health.AddHediff(h);

            Messages.Message($"{Props.hediff.label.CapitalizeFirst()} installed on {user.LabelShortCap}.",
                user, MessageTypeDefOf.PositiveEvent);

            parent.SplitOff(1)?.Destroy(DestroyMode.Vanish);
        }

        private void TryRestoreMissingChain(Pawn pawn, BodyPartRecord part)
        {
            var hediffs = pawn.health.hediffSet.hediffs;
            BodyPartRecord cur = part;
            while (cur != null)
            {
                for (int i = hediffs.Count - 1; i >= 0; i--)
                {
                    if (hediffs[i] is Hediff_MissingPart mp && mp.Part == cur)
                        pawn.health.RemoveHediff(mp);
                }
                cur = cur.parent;
            }
        }

        private BodyPartRecord FindTargetPart(Pawn p)
        {
            var body = p.RaceProps?.body;
            if (body == null) return null;

            if (Props.targetParts != null && Props.targetParts.Count > 0)
            {
                BodyPartRecord fallbackMissing = null;
                foreach (var def in Props.targetParts)
                {
                    foreach (var c in body.AllParts)
                    {
                        if (c.def != def) continue;
                        bool hasHere = p.health.hediffSet.hediffs.Any(h => h.def == Props.hediff && h.Part == c);
                        if (hasHere) continue;
                        bool missing = p.health.hediffSet.PartIsMissing(c);
                        if (!missing) return c;
                        if (Props.allowInstallOnMissing && fallbackMissing == null)
                            fallbackMissing = c;
                    }
                }
                return fallbackMissing;
            }

            if (Props.targetPart != null)
            {
                var c = body.AllParts.FirstOrDefault(bpr => bpr.def == Props.targetPart);
                if (c == null) return null;
                bool hasHere = p.health.hediffSet.hediffs.Any(h => h.def == Props.hediff && h.Part == c);
                if (hasHere) return null;
                return c;
            }

            return null;
        }

        private bool RequiresBodyPart(HediffDef def)
        {
            return def.addedPartProps != null;
        }
    }
}
