using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ParaProtector
{
    public class FallDude : IEntity
    {
        Rect fallRect;
        public float fallSpeed;
        CollisionDelegateReference myCDR;
        public bool Destroy = false;
        private static Random random = new Random();
        public FallDude()
        {
            SetSpriteFromFile("fallGuy");
            fallRect = new Rect(new Vector2(20, 35), new Vector2(random.Next(1, Game1.Instance.GraphicsDevice.Viewport.Width), -50));
            position = fallRect.position;
            myCDR = CollisionWorld.Instance.AddDelegate(CollisionWorld.CollisionGroupIdentifier.Faller, new CollisionDelegate(null, fallRect));
            fallSpeed = random.Next(50, 100);
        }

        public override void Update(float timeDelta)
        {
            try
            {
                if (CollisionWorld.Instance.CollisionCheck(fallRect, CollisionWorld.CollisionGroupIdentifier.MouseClick, Shape.CollideType.All))
                {
                    Game1.Instance.myPoints += 10;
                    Destroy = true;
                }
                else
                {
                    fallRect.position.Y += fallSpeed * timeDelta;
                    position = fallRect.position;
                    myCDR.shape = fallRect;
                    if (Game1.Instance.GraphicsDevice.Viewport.Height <= position.Y)
                    {
                        Game1.Instance.Lives -= 1;
                        Destroy = true;
                    }
                }
            }
            catch 
            {
                fallRect.position.Y += fallSpeed * timeDelta;
                position = fallRect.position;
                myCDR.shape = fallRect;
                if (Game1.Instance.GraphicsDevice.Viewport.Height <= position.Y)
                {
                    Game1.Instance.Lives -= 1;
                    Destroy = true;
                }
            }
        }

    }
}
