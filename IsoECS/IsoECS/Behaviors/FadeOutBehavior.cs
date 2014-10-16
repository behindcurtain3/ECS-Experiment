using System.Collections.Generic;
using IsoECS.Components;
using IsoECS.DataStructures;
using IsoECS.Entities;

namespace IsoECS.Behaviors
{
    public class FadeOutBehavior : Behavior
    {
        public int FadeTime { get; set; }
        public int FadeCounter { get; set; }

        public FadeOutBehavior() : base()
        {
            FadeTime = 750;
            FadeCounter = FadeTime;
        }

        public override void Init(EntityManager em, Entity self)
        {
            base.Init(em, self);
        }

        public override void Update(EntityManager em, Entity self, Stack<Behavior> state, int dt)
        {
            FadeCounter -= dt;

            DrawableComponent drawable = self.Get<DrawableComponent>();

            foreach (IGameDrawable d in drawable.Drawables)
            {
                d.Alpha = ((float)FadeCounter / (float)FadeTime);
            }

            if (FadeCounter <= 0)
            {
                foreach (IGameDrawable d in drawable.Drawables)
                {
                    d.Visible = false;
                }

                Status = BehaviorStatus.SUCCESS;
            }
        }
    }
}
