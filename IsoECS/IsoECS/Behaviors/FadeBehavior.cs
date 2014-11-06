using IsoECS.Components;
using IsoECS.DataStructures;
using IsoECS.GamePlay;
using TecsDotNet;

namespace IsoECS.Behaviors
{
    public class FadeBehavior : Behavior
    {
        public double FadeTime { get; set; }
        public double FadeCounter { get; set; }
        public bool FadeIn { get; set; }
        public bool Skip { get; private set; }

        public FadeBehavior() : base()
        {
            Skip = false;
            FadeIn = true;
            FadeTime = 0.75;
            FadeCounter = FadeTime;
        }

        public override void Init(GameWorld world, Entity self)
        {
            base.Init(world, self);

            DrawableComponent drawable = self.Get<DrawableComponent>();

            if (FadeIn)
            {
                foreach (GameDrawable d in drawable.Get("Foreground"))
                {
                    if (d is DrawableSprite)
                        if (d.Alpha >= 1.0f)
                            Skip = true;

                    d.Visible = true;
                }
            }

            foreach (GameDrawable d in drawable.Get("Foreground"))
            {
                if(FadeIn)
                    FadeCounter = FadeTime - (int)(d.Alpha * FadeTime);
                else
                    FadeCounter = (int)(d.Alpha * FadeTime);
                break;
            }
        }

        public override BehaviorStatus Update(Entity self, double dt)
        {
            BehaviorStatus status = base.Update(self, dt);

            if (Skip)
                return BehaviorStatus.SUCCESS;

            FadeCounter -= dt;

            DrawableComponent drawable = self.Get<DrawableComponent>();

            foreach (GameDrawable d in drawable.Get("Foreground"))
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
                    foreach (GameDrawable d in drawable.Get("Foreground"))
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
