using System.Collections.Generic;
using IsoECS.Components;
using IsoECS.DataStructures;
using IsoECS.Entities;

namespace IsoECS.Behaviors
{
    public class FadeBehavior : Behavior
    {
        public int FadeTime { get; set; }
        public int FadeCounter { get; set; }
        public bool FadeIn { get; set; }

        public FadeBehavior() : base()
        {
            FadeIn = true;
            FadeTime = 750;
            FadeCounter = FadeTime;
        }

        public override void Init(EntityManager em, Entity self)
        {
            base.Init(em, self);

            if (FadeIn)
            {
                DrawableComponent drawable = self.Get<DrawableComponent>();
                foreach (IGameDrawable d in drawable.Drawables)
                {
                    d.Visible = true;
                }
            }
        }

        public override void Update(EntityManager em, Entity self, Stack<Behavior> state, int dt)
        {
            FadeCounter -= dt;

            DrawableComponent drawable = self.Get<DrawableComponent>();

            foreach (IGameDrawable d in drawable.Drawables)
            {
                if (FadeIn)
                    d.Alpha = 1f - ((float)FadeCounter / (float)FadeTime);
                else
                    d.Alpha = ((float)FadeCounter / (float)FadeTime);
            }

            if (FadeCounter <= 0)
            {
                if (!FadeIn)
                {
                    foreach (IGameDrawable d in drawable.Drawables)
                    {
                        d.Visible = false;
                    }
                }

                Status = BehaviorStatus.SUCCESS;
            }
        }
    }
}
