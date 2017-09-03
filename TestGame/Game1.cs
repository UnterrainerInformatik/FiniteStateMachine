// *************************************************************************** 
// This is free and unencumbered software released into the public domain.
// 
// Anyone is free to copy, modify, publish, use, compile, sell, or
// distribute this software, either in source code form or as a compiled
// binary, for any purpose, commercial or non-commercial, and by any
// means.
// 
// In jurisdictions that recognize copyright laws, the author or authors
// of this software dedicate any and all copyright interest in the
// software to the public domain. We make this dedication for the benefit
// of the public at large and to the detriment of our heirs and
// successors. We intend this dedication to be an overt act of
// relinquishment in perpetuity of all present and future rights to this
// software under copyright law.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
// OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// 
// For more information, please refer to <http://unlicense.org>
// ***************************************************************************

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Animations.SpriteSheets;
using MonoGame.Extended.TextureAtlases;
using StateMachine;
using SpriteExtensions = MonoGame.Extended.Sprites.SpriteExtensions;

namespace TestGame
{
    /// <summary>
    ///     This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private TextureAtlas atlas;
        private AnimatedSprite hero;
        private bool lastWasRight;

        Fsm<string, Keys, GameTime> heroStateMachine;

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
        ///     LoadContent will be called once per game and is the place to load
        ///     all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            var texture = Content.Load<Texture2D>("spaceman");
            atlas = TextureAtlas.Create("hero-atlas", texture, 50, 50, 24, 1, 1);

            var factory = new SpriteSheetAnimationFactory(atlas);
            factory.Add("idle_left", new SpriteSheetAnimationData(new[] {1}));
            factory.Add("idle_right", new SpriteSheetAnimationData(new[] {0}));
            factory.Add("walk_left", new SpriteSheetAnimationData(new[] {16, 17, 18, 19, 20, 21, 22, 23}));
            factory.Add("walk_right", new SpriteSheetAnimationData(new[] {8, 9, 10, 11, 12, 13, 14, 15}));
            factory.Add("jump_left", new SpriteSheetAnimationData(new[] {5, 7}, isLooping: false));
            factory.Add("jump_right", new SpriteSheetAnimationData(new[] {4, 6}, isLooping: false));
            factory.Add("duck_left", new SpriteSheetAnimationData(new[] {3}));
            factory.Add("duck_right", new SpriteSheetAnimationData(new[] {2}));

            hero = new AnimatedSprite(factory);
            
            heroStateMachine = Fsm<string, Keys, GameTime>.Builder("idle_left")
                .State("idle_left")
                    .Update(args => hero.Play(args.State.Identifier))
                    .TransitionTo("walk_left").On(Keys.Left)
                    .TransitionTo("duck_left").On(Keys.Down)
                    .TransitionTo("jump_left").On(Keys.Up)
                    .TransitionTo("idle_right").On(Keys.Right)
                .State("idle_right")
                    .Update(args => hero.Play(args.State.Identifier))
                    .TransitionTo("walk_right").On(Keys.Right)
                    .TransitionTo("duck_right").On(Keys.Down)
                    .TransitionTo("jump_right").On(Keys.Up)
                    .TransitionTo("idle_left").On(Keys.Left)
                .State("walk_left")
                    .Update(args => {
                        if (Keyboard.GetState().IsKeyDown(Keys.Left))
                        {
                            hero.Play(args.State.Identifier);
                        }
                        else
                        {
                            args.Machine.JumpTo("idle_left");
                        }
                    })
                    .TransitionTo("duck_left").On(Keys.Down)
                    .TransitionTo("jump_left").On(Keys.Up)
                    .TransitionTo("idle_right").On(Keys.Right)
                .State("walk_right")
                    .Update(args => {
                        if (Keyboard.GetState().IsKeyDown(Keys.Right))
                        {
                            hero.Play(args.State.Identifier);
                        }
                        else
                        {
                            args.Machine.JumpTo("idle_right");
                        }
                    })
                    .TransitionTo("duck_right").On(Keys.Down)
                    .TransitionTo("jump_right").On(Keys.Up)
                    .TransitionTo("idle_left").On(Keys.Left)
                .State("jump_left")
                    .OnEnter(args => hero.Play(args.To.Identifier).OnCompleted = () => args.Fsm.JumpTo("idle_left"))
                .State("jump_right")
                    .OnEnter(args => hero.Play(args.To.Identifier).OnCompleted = () => args.Fsm.JumpTo("idle_right"))
                .State("duck_left")
                    .Update(args => {
                        if (Keyboard.GetState().IsKeyDown(Keys.Down))
                        {
                            hero.Play(args.State.Identifier);
                        }
                        else
                        {
                            args.Machine.JumpTo("idle_left");
                        }
                    })
                    .TransitionTo("walk_right").On(Keys.Right)
                    .TransitionTo("walk_left").On(Keys.Left)
                    .TransitionTo("jump_left").On(Keys.Up)
                .State("duck_right")
                    .Update(args => {
                        if (Keyboard.GetState().IsKeyDown(Keys.Down))
                        { 
                            hero.Play(args.State.Identifier);
                        }
                        else
                        {
                            args.Machine.JumpTo("idle_right");
                        }
                    })
                    .TransitionTo("walk_right").On(Keys.Right)
                    .TransitionTo("walk_left").On(Keys.Left)
                    .TransitionTo("jump_right").On(Keys.Up)
                .Build();
        }



        /// <summary>
        ///     Allows the game to run logic such as updating the world,
        ///     checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            hero.Position = new Vector2(GraphicsDevice.Viewport.Width / 2f, GraphicsDevice.Viewport.Height / 2f);

            if (Keyboard.GetState().CapsLock)
            {
                UpdateOld(gameTime);
            }
            else
            {
                UpdateNew(gameTime);
            }

            hero.Update(gameTime);
            base.Update(gameTime);
        }

        protected void UpdateNew(GameTime gameTime)
        {
            foreach (Keys key in Keyboard.GetState().GetPressedKeys())
            {
                heroStateMachine.Trigger(key);
            }
            heroStateMachine.Update(gameTime);
        }

        protected void UpdateOld(GameTime gameTime) {
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
        }

        /// <summary>
        ///     This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            SpriteExtensions.Draw(hero, spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}