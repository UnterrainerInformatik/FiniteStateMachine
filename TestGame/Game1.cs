using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Animations.SpriteSheets;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;

namespace TestGame
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private TextureAtlas atlas;
        private AnimatedSprite hero;
        private bool lastWasRight;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            Window.Title = $"StateMachine - {GetType().Name}";
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            var texture = Content.Load<Texture2D>("spaceman");
            atlas = TextureAtlas.Create("hero-atlas", texture, 50, 50, 24, 1, 1);

            var factory = new SpriteSheetAnimationFactory(atlas);
            factory.Add("idle_left", new SpriteSheetAnimationData(new[] { 1 }));
            factory.Add("idle_right", new SpriteSheetAnimationData(new[] { 0 }));
            factory.Add("walk_left", new SpriteSheetAnimationData(new[] {16, 17, 18, 19, 20, 21, 22, 23}));
            factory.Add("walk_right", new SpriteSheetAnimationData(new[] { 8, 9, 10, 11, 12, 13, 14, 15 }));
            factory.Add("jump_left", new SpriteSheetAnimationData(new[] { 5, 7 }, isLooping: false));
            factory.Add("jump_right", new SpriteSheetAnimationData(new[] { 4, 6 }, isLooping: false));
            factory.Add("duck_left", new SpriteSheetAnimationData(new[] { 3 }));
            factory.Add("duck_right", new SpriteSheetAnimationData(new[] { 2 }));

            hero = new AnimatedSprite(factory);
        }
        
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            hero.Position = new Vector2(GraphicsDevice.Viewport.Width / 2f, GraphicsDevice.Viewport.Height / 2f);

            var s = Keyboard.GetState();
            if (s.IsKeyDown(Keys.Left))
            {
                hero.Play("walk_left");
                lastWasRight = false;
            }
            else if (s.IsKeyDown(Keys.Right))
            {
                hero.Play("walk_right");
                lastWasRight = true;
            }
            else if (s.IsKeyDown(Keys.Up))
            {
                if (lastWasRight)
                {
                    hero.Play("jump_right");
                }
                else
                {
                    hero.Play("jump_left");
                }
            }
            else if (s.IsKeyDown(Keys.Down))
            {
                if (lastWasRight)
                {
                    hero.Play("duck_right");
                }
                else
                {
                    hero.Play("duck_left");
                }
            }
            else
            {
                if (lastWasRight)
                {
                    hero.Play("idle_right");
                }
                else
                {
                    hero.Play("idle_left");
                }
            }
            hero.Update(gameTime);
            
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            spriteBatch.Begin();
            spriteBatch.Draw(hero);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
