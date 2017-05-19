using System;

using Microsoft.Xna.Framework;

using Microsoft.Xna.Framework.Graphics;

namespace Ship_Fighters
{
    class BlueBullet
    {
        public Texture2D BlueBulletTexture;
        public Vector2 Position;
        public bool Active = true;
        public int Damage;

        public int Width

        {

            get { return BlueBulletTexture.Width; }

        }

        // Get the height of the enemy ship

        public int Height

        {

            get { return BlueBulletTexture.Height; }

        }
        public void Initialize(Texture2D texture, Vector2 position)

        {
            BlueBulletTexture = texture;
            Position = position;
            Active = true;
            Damage = 1;
        }

        public void Update(GameTime gameTime)

        {
            
            
        }

        
            public void Draw(SpriteBatch spriteBatch)

        {

            spriteBatch.Draw(BlueBulletTexture, Position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

        }

    
    }
}
