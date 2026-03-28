using RimWorld;
using Verse;

namespace Nanotech
{
    public class CompProperties_AllSkillsTrainer : CompProperties_UseEffect
    {
        public bool setToTargetLevel = true;
        public int targetLevel = 12;
        public float xpPerSkill = 0f;
        public bool skipDisabledSkills = true;
        public int maxLevelCap = 20;
        public bool humanlikesOnly = true;
        public bool showFloatText = true;
        public string floatTextKey = "All skills trained";

        public CompProperties_AllSkillsTrainer()
        {
            compClass = typeof(CompAllSkillsTrainer);
        }
    }
}
