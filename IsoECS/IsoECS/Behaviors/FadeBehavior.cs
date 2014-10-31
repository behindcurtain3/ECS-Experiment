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
        public bool Skip { get; private set; }

        public FadeBehavior() : base()
        {
            Skip = false;
            FadeIn = true;
            FadeTime = 750;
            FadeCounter = FadeTime;
        }

        public override void Init(Entity self)
        {
            base.Init(self);

            DrawableComponent drawable = self.Get<DrawableComponent>();

            if (FadeIn)
            {
                foreach (IGameDrawable d in drawable.Get("Foreground"))
                {
                    if (d is DrawableSprite)
                        if (d.Alpha >= 1.0f)
                            Skip = true;

                    d.Visible = true;
                }
            }

            foreach (IGameDrawable d in drawable.Get("Foreground"))
            {
                if(FadeIn)
                    FadeCounter = FadeTime - (int)(d.Alpha * FadeTime);
                else
                    FadeCounter = (int)(d.Alpha * FadeTime);
                break;
            }
        }

        public override BehaviorStatus Update(Entity self, int dt)
        {
            BehaviorStatus status = base.Update(self, dt);

            if (Skip)
                return BehaviorStatus.SUCCESS;

            FadeCounter -= dt;

            DrawableComponent drawable = self.Get<DrawableComponent>();

            foreach (IGameDrawable d in drawable.Get("Foreground"))
            {
                if (d is DrawableSprite)
                {
                    if (FadeIn)
                        d.Alpha = 1f - ((float)FadeCounter / (float)FadeTime);
                    else
                        d.Alpha = ((float)FadeCounter / (float)FadeTime);
                }
            }

            if (FadeCounter <= 0)
            {
                if (!FadeIn)
                {
                    foreach (IGameDrawable d in drawable.Get("Foreground"))
                    {
                        if(d is DrawableSprite)
                            d.Visible = false;
                    }
                }

                return BehaviorStatus.SUCCESS;
            }

            return BehaviorStatus.WAIT;
        }
    }
}
