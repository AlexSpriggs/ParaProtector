using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ParaProtector
{
    abstract public class IEntity
    {
        protected Texture2D m_Sprite = null;
        public Texture2D sprite
        {
            get
            {
                return m_Sprite;
            }
        }

        public Vector2 position = Vector2.Zero;
        public Color blendColor = Color.White;
        public float angle = 0.0F;

        protected bool autoCenterSprite = false;

        protected void SetSpriteFromFile(String file)
        {
            m_Sprite = Game1.Instance.Content.Load<Texture2D>(file);
        }

        public abstract void Update(float timeDelta);

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (m_Sprite == null)
                return;

            Vector2 offset;
            if (autoCenterSprite)
                offset = new Vector2(m_Sprite.Width / 2, m_Sprite.Height / 2);
            else
                offset = Vector2.Zero;

            spriteBatch.Draw(m_Sprite, position, null, blendColor, angle, offset, 1.0F, SpriteEffects.None, 0.5F);
        }
    }
}
