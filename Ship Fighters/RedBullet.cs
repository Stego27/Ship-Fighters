using System;

using Microsoft.Xna.Framework;

using Microsoft.Xna.Framework.Graphics;

namespace Ship_Fighters
{
    class RedBullet
    {
        public Texture2D RedBulletTexture;
        public Vector2 Position;
        public bool Active = true;
        public int Damage;

        public int Width

        {

            get { return RedBulletTexture.Width; }

        }

        // Get the height of the enemy ship

        public int Height

        {

            get { return RedBulletTexture.Height; }

        }
        public void Initialize(Texture2D texture, Vector2 position)

        {
            RedBulletTexture = texture;
            Position = position;
            Active = true;
            Damage = 1;
        }

        public void Update(GameTime gameTime)

        {
            
            
        }

        
            public void Draw(SpriteBatch spriteBatch)

        {

            spriteBatch.Draw(RedBulletTexture, Position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

        }

    
    }
}
