using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PlayerArea
{

    class Player
    {
        // Animation representing the player

        public Texture2D PlayerTexture;



        // Position of the Player relative to the upper left side of the screen

        public Vector2 Position;



        // State of the player

        public bool Active;



        // Amount of hit points that player has

        public int Health;


        public float rotation = 0.0f;
        // Get the width of the player ship

        public int Width

        {

            get { return PlayerTexture.Width; }

        }



        // Get the height of the player ship

        public int Height

        {

            get { return PlayerTexture.Height; }

        }
        public void Initialize(Texture2D texture, Vector2 position)

        {
            PlayerTexture = texture;
            Position = position;
            Active = true;
            Health = 10;
        }



        public void Update()

        {
            
        }



        
        public void Draw(SpriteBatch spriteBatch)

        {

            spriteBatch.Draw(PlayerTexture, new Vector2(Position.X + Width / 2, Position.Y + Height / 2), null, Color.White, rotation, new Vector2((Width / 2), (Height / 2)), 1f,

               SpriteEffects.None, 0f);

        }
    }
}
