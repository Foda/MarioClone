using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SFML_Test
{
    class Sprite
    {
        //The current position of the Sprite
        public Vector2 Position = new Vector2(0, 0);

        //Phys movement state
        public ConstHelper.AirState airState = ConstHelper.AirState.AIR;

        //The texture object used when drawing the sprite
        public Texture2D mSpriteTexture;

        public Color color = Color.White;

        //The Size of the Sprite (with scale applied)
        public Rectangle Size;

        public bool FlipHorz = false;

        //MetaData for this sprite to define misc stuff
        public string MetaData;

        public int Height = 16;

        public bool isVisible = true;

        //The Rectangular area from the original image that 
        //defines the Sprite. 
        Rectangle mSource;
        public Rectangle Source
        {
            get { return mSource; }
            set
            {
                mSource = value;
                Size = new Rectangle(0, 0, (int)(mSource.Width * 1), (int)(mSource.Height * 1));
            }
        }

        //Load the texture for the sprite using the Content Pipeline
        public void LoadContent(ContentManager theContentManager, string theAssetName)
        {
            mSpriteTexture = theContentManager.Load<Texture2D>(System.IO.Path.GetFullPath(theAssetName));
        }

        //Draw the sprite to the screen
        public virtual void Draw(SpriteBatch theSpriteBatch)
        {
            if (isVisible == false) return;

            if(FlipHorz == true)
                theSpriteBatch.Draw(mSpriteTexture, new Vector2((int)Position.X, (int)Position.Y), Source,
                    color, 0.0f, Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 0);
            else
                theSpriteBatch.Draw(mSpriteTexture, new Vector2((int)Position.X, (int)Position.Y), Source,
                color, 0.0f, Vector2.Zero, 1, SpriteEffects.None, 0);
        }

        public virtual Rectangle GetRect()
        {
            return new Rectangle((int)Position.X, (int)Position.Y, 16, 16);
        }
    }
}
