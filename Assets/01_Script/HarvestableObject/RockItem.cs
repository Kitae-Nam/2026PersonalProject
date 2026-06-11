using UnityEngine;

namespace _01_Script.HarvestableObject
{
    public class RockItem : HarvestableObject
    {
        [SerializeField] private ParticleSystem rockParticles;
        public override void HarvestEffect()
        {
            base.HarvestEffect();
            rockParticles.Play();
        }

        public override void HarvestDoneEffect()
        {
            base.HarvestDoneEffect();
            rockParticles.Play();
        }
    }
}