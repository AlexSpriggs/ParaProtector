using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ParaProtector
{
    public abstract class Shape
    {
        public enum CollideType
        {
            Contains,
            Intersects,
            All
        }

        public virtual bool Collides(Shape shape, Shape.CollideType detectMethod = CollideType.All)
        {
            if (shape is Rect)
                return Collides(shape as Rect, detectMethod);
            if (shape is Point)
                return Collides(shape as Point, detectMethod);
            if (shape is Circle)
                return Collides(shape as Circle, detectMethod);

            throw new Exception("Unsupported IShape specialization.");
        }

        public abstract bool Collides(Rect rect, CollideType detectMethod = CollideType.All);
        public abstract bool Collides(Point point, CollideType detectMethod = CollideType.All);
        public abstract bool Collides(Circle circle, CollideType detectMethod = CollideType.All);

        public Vector2 position;
    }

    public class Rect : Shape
    {
        /* Note to implementer: How the position and size are referenced internally are irrelevant, getters/setters would be appropriate for implementing
         * one of the property sets below.
         */
        public float bottom
        {
            get
            {
                return this.position.Y + this.size.Y;
            }
        }
        public float left
        {
            get { return this.position.X; }
        }
        public float top
        {
            get { return this.position.Y; }
        }
        public float right
        {
            get
            {
                return this.position.X + this.size.X; ;
            }
        }


        public Vector2 size;


        public Rect(Vector2 size)
        {
            this.size = size;

        }

        public Rect(Vector2 size, Vector2 position)
        {
            this.size = size;
            this.position = position;
        }

        public Rect(Rectangle sourceRect)
        {
            this.size = new Vector2(sourceRect.Width, sourceRect.Height);
            this.position = new Vector2(sourceRect.X, sourceRect.Y);
        }

        public Rect(Texture2D sourceTexture)
        {
            this.size = new Vector2(sourceTexture.Width, sourceTexture.Height);
            this.position = new Vector2(sourceTexture.Bounds.X, sourceTexture.Bounds.Y);
        }

        public Rect(float x, float y, float width, float height)
        {
            this.size = new Vector2(width, height);
            this.position = new Vector2(x, y);
        }


        /// <summary>
        /// Returns an XNA Rectangle object matching the current Rect.
        /// </summary>
        /// <returns>A matching Microsoft.Xna.Framework.Rectangle object.</returns>
        public Rectangle ToXNACompatible()
        {
            return new Rectangle((int)this.position.X, (int)this.position.Y, (int)this.size.X, (int)this.size.Y);
        }


        /// <summary>
        /// Stretches the rect to contain the given point.
        /// </summary>
        /// <param name="position"></param>
        public void Contain(Vector2 position)
        {
            if (this.left <= position.X && this.right >= position.X)
            {
                //do nothing
            }
            else if (this.left > position.X)
            {
                this.position.X = this.position.X - (this.position.X - position.X);
            }
            else if (this.right < position.X)
            {
                this.size.X = this.size.X + (position.X - this.right);
            }

            if (this.bottom <= position.Y && this.top >= position.Y)
            {
                //do nothing
            }
            else if (this.bottom > position.Y)
            {
                this.size.Y = this.size.Y + (this.bottom - position.Y);
            }
            else if (this.top < position.Y)
            {
                this.position.Y = this.position.Y + (position.Y - this.position.Y);
            }
        }



        #region Shape Collides Overrides
        public override bool Collides(Rect rect, CollideType detectMethod = CollideType.All)
        {
            return rect.left <= this.right && rect.right >= this.left && rect.top <= this.bottom && rect.bottom >= this.top;
        }

        public override bool Collides(Point point, CollideType detectMethod = CollideType.All)
        {
            //TODO: Fix
            if (this.top <= point.y && this.left <= point.x && this.right >= point.x && this.bottom >= point.y)
                return true;
            else
                return false;
        }

        public override bool Collides(Circle circle, CollideType detectMethod = CollideType.All)
        {
            Vector2 circleDistance = new Vector2((float)Math.Abs(circle.position.X - this.position.X), (float)Math.Abs(circle.position.Y - this.position.Y));

            if (circleDistance.X > (this.size.X / 2 + circle.radius)) return false;
            if (circleDistance.Y > (this.size.Y / 2 + circle.radius)) return false;

            if (circleDistance.X <= (this.size.X / 2)) return true;
            if (circleDistance.Y <= (this.size.Y / 2)) return true;

            float cornerDistance_sq = (float)(Math.Pow((double)(circleDistance.X - this.size.X / 2), 2) + Math.Pow((double)(circleDistance.Y - this.size.Y / 2), 2));
            return (cornerDistance_sq <= (float)Math.Pow(circle.radius, 2));
        }
        #endregion
    }



    public class Point : Shape
    {
        public float x;
        public float y;

        public Point(Vector2 position)
        {
            x = position.X;
            y = position.Y;
        }

        public Point(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Returns a Vector2 object representing the position of this point.
        /// </summary>
        /// <returns></returns>
        public Vector2 ToVector2()
        {
            return new Vector2(x, y);
        }


        #region Shape Collides Overrides
        public override bool Collides(Rect rect, CollideType detectMethod = CollideType.All)
        {
            if (rect.top >= this.y && rect.left <= this.x && rect.right >= this.x && rect.bottom <= this.y)
                return true;
            else
                return false;
        }

        public override bool Collides(Point point, CollideType detectMethod = CollideType.All)
        {
            if (point.x == this.x && point.y == this.y)
                return true;
            else
                return false;
        }

        public override bool Collides(Circle circle, CollideType detectMethod = CollideType.All)
        {
            return false;
        }
        #endregion
    }



    public class Circle : Shape
    {
        public float radius;
        public Vector2 position;

        public Circle(Vector2 position, float radius)
        {
            this.position = position;

            this.radius = radius;
        }

        public Circle(float x, float y, float radius)
        {
            this.position = new Vector2(x, y);

            this.radius = radius;
        }

        #region IShape Collides Overrides
        public override bool Collides(Rect rect, CollideType detectMethod = CollideType.All)
        {
            Vector2 circleDistance = new Vector2((float)Math.Abs(this.position.X - rect.position.X), (float)Math.Abs(this.position.Y - rect.position.Y));

            if (circleDistance.X > (rect.size.X / 2 + this.radius)) return false;
            if (circleDistance.Y > (rect.size.Y / 2 + this.radius)) return false;

            if (circleDistance.X <= (rect.size.X / 2)) return true;
            if (circleDistance.Y <= (rect.size.Y / 2)) return true;

            float cornerDistance_sq = (float)(Math.Pow((double)(circleDistance.X - rect.size.X / 2), 2) + Math.Pow((double)(circleDistance.Y - rect.size.Y / 2), 2));
            return (cornerDistance_sq <= (float)Math.Pow(this.radius, 2));
        }

        public override bool Collides(Point point, CollideType detectMethod = CollideType.All)
        {
            return false;
        }

        public override bool Collides(Circle circle, CollideType detectMethod = CollideType.All)
        {
            double Distance;

            Distance = Math.Sqrt((Math.Pow((double)(circle.position.X - this.position.X), 2)) + (Math.Pow((double)(circle.position.Y - this.position.Y), 2)));

            if (Distance <= (this.radius + circle.radius))
                return true;
            else
                return false;
        }
        #endregion
    }
}
