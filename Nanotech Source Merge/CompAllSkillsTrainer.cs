using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace Nanotech
{
    public class CompAllSkillsTrainer : CompUseEffect
    {
        public CompProperties_AllSkillsTrainer Props => (CompProperties_AllSkillsTrainer)props;

        private bool Eligible(Pawn p, out string reason)
        {
            if (p == null || p.DestroyedOrNull())
            {
                reason = "Invalid target.";
                return false;
            }
            if (Props.humanlikesOnly && (p.RaceProps?.Humanlike != true))
            {
                reason = "Only humanlikes can use this.";
                return false;
            }
            if (p.skills == null || p.skills.skills == null)
            {
                reason = "No skills to train.";
                return false;
            }
            reason = null;
            return true;
        }

        public override void DoEffect(Pawn user)
        {
            base.DoEffect(user);

            if (!Eligible(user, out var whyNot))
            {
                Messages.Message(whyNot ?? "Cannot use.", new LookTargets(user), MessageTypeDefOf.RejectInput);
                return;
            }

            int affected = 0;

            foreach (var rec in user.skills.skills)
            {
                if (Props.skipDisabledSkills && rec.TotallyDisabled) continue;

                if (Props.setToTargetLevel)
                {
                    int cap = Mathf.Min(Props.maxLevelCap, SkillRecord.MaxLevel);
                    int target = Mathf.Clamp(Props.targetLevel, 0, cap);

                    if (rec.Level < target)
                    {
                        rec.Learn(999999f, direct: true);
                        if (rec.Level > target)
                        {
                            rec.Level = target;
                            rec.xpSinceLastLevel = 0f;
                        }
                        affected++;
                    }
                }
                else if (Props.xpPerSkill > 0f)
                {
                    rec.Learn(Props.xpPerSkill, direct: true);
                    if (rec.Level > Props.maxLevelCap)
                    {
                        rec.Level = Props.maxLevelCap;
                        rec.xpSinceLastLevel = 0f;
                    }
                    affected++;
                }
            }

            if (affected <= 0)
            {
                Messages.Message("No skills were affected.", new LookTargets(user), MessageTypeDefOf.NeutralEvent);
                return;
            }

            if (Props.showFloatText)
                MoteMaker.ThrowText(user.DrawPos, user.Map, Props.floatTextKey);

            Messages.Message($"Omni-trainer applied to {user.LabelShortCap} ({affected} skills affected).",
                new LookTargets(user), MessageTypeDefOf.PositiveEvent);

            parent.SplitOff(1)?.Destroy(DestroyMode.Vanish);
        }
    }
}
