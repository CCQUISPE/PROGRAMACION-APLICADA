using System;
using CruzaCruza.Modelo;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace CruzaCruza
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D jugador;
        Vector2 posicion;  //para amnteenr la pociion dentro de la pantalla

        
        Vehiculo[] vehiculos;  //array de vehiculos
        Carril[] carriles;     //array de carriles 

        
        int nivel = 0;

        Texture2D carblue;
        Texture2D carred;
        Texture2D decorado;

        Vector2 virtualScreen;

        SoundEffect SoundTecla;
        SoundEffect SoundCash;
        SoundEffect SoundFrenazo;
        SoundEffect SoundAmbiente;

        SoundEffectInstance SoundAmbienteInstance;

        Song musica;

        float scroll = 0;   //ira desde el alto de pantalla hasta 0

        bool KeyUpLibre;
        bool KeyDownLibre;
        bool KeyLeftLibre;
        bool KeyRightLibre;

        int VelocidadPierde;

        FaseJuego faseJuego;

        int puntuacion;
        int tiempo = 1000;
        int vidas =  3;

        SpriteFont gameFont;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            // Creamos una pantalla virtual con las dimensiones que consideremos		
            virtualScreen = new Vector2(800, 480);
        }

        public void StopSound()
        {
            if (MediaPlayer.State == MediaState.Playing && MediaPlayer.GameHasControl)
            {
                MediaPlayer.Stop();
            }

            if (SoundAmbienteInstance.State == SoundState.Playing)
            {
                SoundAmbienteInstance.Stop(true);
            }
        }
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            posicion = new Vector2(
                graphics.GraphicsDevice.Viewport.Width / 2,
                graphics.GraphicsDevice.Viewport.Height - 64);

            
            //el origen esde coodenadas esta en la parte superior derecha 0.0

            base.Initialize();

            carriles = new Carril[5];

            posicion.X = virtualScreen.X / 2; //centramos horizontalmente

            IniciarNivel();

            faseJuego = FaseJuego.Inicio;   //inioci seria inicio

            puntuacion = 0;

             
        }

        private void CrearVehiculos()
        {
            if(vehiculos != null) //si es la primera vez crea vehiculos
            {

                Array.Clear(vehiculos, 0, vehiculos.Length); //partimos de un array de vehiculos desde 0,

            }
            
            int totalVehiculos = nivel * 2; //incrementa de ados los vehiculos

            vehiculos = new Vehiculo[totalVehiculos];
            Random r = new Random();

            for (int i=0; i <totalVehiculos; i++)
            {
                int numerocarril = i % 5; //tendremos valores del 0 al 4

                int distancia = r.Next(64, graphics.GraphicsDevice.Viewport.Width - 64); //margen derecha 

                vehiculos[i] = new Vehiculo(i, carblue, carriles[numerocarril], distancia);//auto con carril

            }


        }

        private void CrearCarriles()
        {
            Array.Clear(carriles, 0, carriles.Length); //array de carriles desde cero hasta el tamañano maximo , si no existe lo limpiamos
           
            Random r = new Random();  ////asignamos un nro ramdom
            for (int i=0; i<5; i++) // for por la cantidad de carriles 
            {
                int velocidad = 0;

                int maxVelocidad = Math.Min(nivel, 5);  //definimos un maximo de velocidad
                do
                {
                    velocidad = r.Next(-nivel, nivel); //
                } while (velocidad == 0); // 

                carriles[i] = new Carril(i, 96 + i * 64, velocidad); 
                //tienen 64 de ancho y con esto dejamos que tenga espacio 

            }

        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            jugador = Content.Load<Texture2D>("man");
            carblue = Content.Load<Texture2D>("carblue");
            carred = Content.Load<Texture2D>("carred");
            decorado = Content.Load<Texture2D>("decorado");
            
            gameFont = Content.Load<SpriteFont>("Fonts/LetraNormal");

            SoundTecla = Content.Load<SoundEffect>("Sounds/tecla");
            SoundCash = Content.Load<SoundEffect>("Sounds/cash");
            SoundFrenazo = Content.Load<SoundEffect>("Sounds/frenazo");
            SoundAmbiente = Content.Load<SoundEffect>("Sounds/ambiente");

            SoundAmbienteInstance = SoundAmbiente.CreateInstance();
            SoundAmbienteInstance.Volume = 0.2f;
            SoundAmbienteInstance.IsLooped = true;  //propiedad si el sonida se repite esta en true

            musica = Content.Load<Song>("Sounds/musica");

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                StopSound();
                Exit();
            }

            // TODO: Add your update logic here

            if (scroll > 0)
            {

                scroll = scroll - 3;  //colocamos 3 para que se desplase moredaramente, rapido 10
                if (scroll < 0) scroll = 0;  // corrige el efecto rebote

            }

            switch (faseJuego)
            {

                case FaseJuego.Inicio:
                    UpdateInicio();
                    break;
                case FaseJuego.Juego:
                    UpdateJuego();
                    break;
                case FaseJuego.Gana:
                    UpdateGana();
                    break;
                case FaseJuego.Pierde:
                    UpdatePierde();
                    break;
            } 

            base.Update(gameTime);
        }

        private void UpdatePierde()
        {
                      

            if(posicion.X>0 && posicion.X<virtualScreen.X)
            {

                posicion.X += VelocidadPierde;  //sacamos el personaje

            }
                
            else
            {

                vidas--;

                if(vidas > 0)
                {

                    posicion.Y = virtualScreen.Y - 64;
                    faseJuego = FaseJuego.Juego;

                    if (MediaPlayer.State != MediaState.Playing && MediaPlayer.GameHasControl)
                    {
                        MediaPlayer.Play(musica);
                    }
                   
                }
                    
                else
                {

                    faseJuego = FaseJuego.Inicio;
                    nivel = 0;
                    vidas = 3;
                    IniciarNivel();
                    
                }

                posicion.X = virtualScreen.X / 2;
                VelocidadPierde = 0;
                
                
            }
            
        }

        private void UpdateGana()
        {
            puntuacion = puntuacion + tiempo;   //sumamos el tiempo a la puntuacion
            IniciarNivel();
            faseJuego = FaseJuego.Juego;
          

        }

        private void IniciarNivel()
        {
            nivel++;
            tiempo = 1000;  //el tiempo se reseto en 1000 cada vez que avanza de nivel

            scroll = virtualScreen.Y; //inicia scroll

            CrearCarriles();
            CrearVehiculos();

            posicion.Y = virtualScreen.Y -64; //comienza en el nuevo nivel en la mismo posicion 

        }

        private void UpdateJuego()
        {
            if (tiempo > 0 && scroll == 0) tiempo--;  //drementamos el tiempo, si el tiempo es positivo y que no haya scroll(no cambie de pantalla)

            if (tiempo ==0)   //si nos quedamos sin tiempo , perdemos
            {

                faseJuego = FaseJuego.Pierde;
                VelocidadPierde = 3;   //velocidad de retirada del movimiento
                tiempo = 1000; // ponemos el tiempo en 1000 nuevamente previamwente a perder
            }

            ControlDeTeclado();
            MoverVehiculos();
            ComprobarColisiones();
            ComprobarLimites();

        }

        private void ComprobarLimites()   ///para dejar al jugador dentro de la pantalla y validar que llegue al otro lado
        {
            if (posicion.X > virtualScreen.X)
                posicion.X = virtualScreen.X;

            if (posicion.X < 0)
                posicion.X = 0;

            if (posicion.Y > virtualScreen.Y)
                posicion.Y = virtualScreen.Y;

            if (posicion.Y < 0)          

                Gana();
                
        }

        private void Gana()
        {
          faseJuego = FaseJuego.Gana;
          SoundCash.Play(1, 0, 0);
        }

        private void UpdateInicio()
        {

            MoverVehiculos();

            KeyboardState state = Keyboard.GetState();
                       

            if (state.IsKeyDown(Keys.Enter))
            {
                faseJuego = FaseJuego.Juego;
                SoundAmbienteInstance.Play();

                if(MediaPlayer.State != MediaState.Playing && MediaPlayer.GameHasControl)  //validamos que no este reproducciendose y
                    //validamos si el juego tiene el control se puede reproducir
                {

                    MediaPlayer.Play(musica);
                }
                

            }
            

        }

        private void ComprobarColisiones()
        {

            Rectangle rectanguloJugador = new Rectangle(    ///objeto rectangulo que sive para defonir el objeto rectangulo
                posicion.ToPoint(),                         //posicion superior lo da el Topoint lo convierte en un punto,
                jugador.Bounds.Size                         //bounds define el rectangulo del jugador
                );
            foreach (Vehiculo vehiculo in vehiculos)        //recorremos los vehiculos y comprobamos el rectangulo dec/u 
            {
                Rectangle rectanguloVehiculo =
                    new Rectangle(vehiculo.getposicion().ToPoint()
                    , vehiculo.Textura.Bounds.Size);

                if (rectanguloVehiculo.Intersects(rectanguloJugador)) //si hay interseccion entre el jugador y el vehiculo
                {

                    vehiculo.Textura = carred;

                    Pierde(vehiculo);

                }

            }


        }

        private void Pierde(Vehiculo vehiculo)
        {
            StopSound();
            SoundFrenazo.Play(1, 0, 0);
            VelocidadPierde = vehiculo.Carril.Velocidad;
            faseJuego = FaseJuego.Pierde;
        }
        private void MoverVehiculos()
        {
            foreach (var vehiculo in vehiculos)
            {
                vehiculo.Distancia += vehiculo.Carril.Velocidad;  //incrementa la distancia con la velocidad del carril

                if (vehiculo.Distancia > graphics.GraphicsDevice.Viewport.Width) //si al distancia de vehiculo supera el hancho de la pantalla que aparezaca del otro lado
                {
                    vehiculo.Distancia = -vehiculo.Textura.Width;     //Colocamos  - para que no aprezca tan artificial

                }
                if (vehiculo.Distancia < -vehiculo.Textura.Width) //si al distancia de vehiculo supera el hancho de la pantalla que aparezaca del otro lado
                {
                    vehiculo.Distancia = graphics.GraphicsDevice.Viewport.Width;     //Colocamos  - para que no aprezca tan artificial

                }


            }


        }

        private void ControlDeTeclado()
        {
            var estadoTeclado = Keyboard.GetState();  ///generamos la variable estadoTeclado, para validar el estado


            if (KeyUpLibre && estadoTeclado.IsKeyDown(Keys.Up))
            {
                MueveArriba();
                KeyUpLibre = false;
            }
            if (KeyDownLibre && estadoTeclado.IsKeyDown(Keys.Down))
            {
                MueveAbajo();
                KeyDownLibre = false;
            }
            if (KeyRightLibre && estadoTeclado.IsKeyDown(Keys.Right))
            {
                MueveDerecha();
                KeyRightLibre = false;
            }
            if (KeyLeftLibre && estadoTeclado.IsKeyDown(Keys.Left))
            {
                MueveIzquierda();
                KeyLeftLibre = false;
            }

            if (estadoTeclado.IsKeyUp(Keys.Up))
            {
                KeyUpLibre = true;
            }
            if (estadoTeclado.IsKeyUp(Keys.Down))
            {
                KeyDownLibre = true;
            }
            if (estadoTeclado.IsKeyUp(Keys.Right))
            {
                KeyRightLibre = true;
            }
            if (estadoTeclado.IsKeyUp(Keys.Left))
            {
                KeyLeftLibre = true;
            }
        }

        private void MueveArriba()
        {
            posicion.Y -= 25;
            SoundTecla.Play(0.5f, 0, 0);    //volumen de 0 , tono se emite con misma frecuencia en cero, en cero se escucha centrado
        }

        private void MueveAbajo()
        {
            posicion.Y += 25;
            SoundTecla.Play(0.5f, 0, 0);
        }

        private void MueveIzquierda()
        {
            posicion.X -= 25;
            SoundTecla.Play(0.5f, 0, 0);

        }

        private void MueveDerecha()
        {
            posicion.X += 25;
            SoundTecla.Play(0.5f, 0, 0);

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

           

            //decorado
            spriteBatch.Draw(decorado, new Vector2(0,-scroll),Color.White);

            //parte inferior del decorado
            spriteBatch.Draw(decorado, new Vector2(0, virtualScreen.Y - scroll), Color.White);


            DibujarPuntuacion();

            if (faseJuego == FaseJuego.Inicio)
            {

                var color = Color.DarkRed;

                var tick = gameTime.TotalGameTime.TotalMilliseconds / 100;
                color.A = (byte)(155 + Math.Sin(tick) * 100);

                DibujarMensajeInicio(color);

            }

            float rotation = 0f;

            if (faseJuego == FaseJuego.Pierde)  //si pierde el jugador saldra de pantalla rotando
            {
                rotation = (float)gameTime.TotalGameTime.TotalMilliseconds / 500;
                spriteBatch.Draw(jugador, posicion,
                scale: Vector2.One,
                rotation: rotation,
                origin: new Vector2(jugador.Width / 2, jugador.Height / 2),
                color: Color.White,
                effects: SpriteEffects.None,
                layerDepth: 0,
                sourceRectangle: null);
            }
            else
            {
                spriteBatch.Draw(jugador, posicion + new Vector2(0, -scroll),
                   scale: Vector2.One,
                   rotation: 0,
                   origin: new Vector2(0, 0),
                   color: Color.White,
                   effects: SpriteEffects.None,
                   layerDepth: 0,
                   sourceRectangle: null);
            }

            
            
            DibujarVehiculos();

            spriteBatch.End();

            
            base.Draw(gameTime);
        }

        private void DibujarMensajeInicio(Color color)
        {

            string mensaje = "Pulsa Enter para empezar";
            var longitud = gameFont.MeasureString(mensaje).X;

            spriteBatch.DrawString(gameFont, mensaje,
                new Vector2(
                    (graphics.GraphicsDevice.Viewport.Width - longitud) / 2,
                     graphics.GraphicsDevice.Viewport.Height / 3)
                     ,color);

            mensaje = String.Format("Muevete con las flechas");
            spriteBatch.DrawString(gameFont, mensaje,
                new Vector2(
                     (graphics.GraphicsDevice.Viewport.Width - longitud) / 2,
                     graphics.GraphicsDevice.Viewport.Height / 2)
                     , color); ;

        }

        private void DibujarPuntuacion()   ///dibujamos puntuacion, tiempo, nivel y vidas
        {
            string mensaje = String.Format("Puntos : {0:D4}", puntuacion);
            var longitud = gameFont.MeasureString(mensaje).X;    // cuanto ocupa
            spriteBatch.DrawString(gameFont, mensaje,
                new Vector2(
                    virtualScreen.X - longitud, 40)
                    , Color.White);

            mensaje = String.Format("Tiempo : {0:D4}", tiempo);
            spriteBatch.DrawString(gameFont, mensaje,
                new Vector2(
                    virtualScreen.X - longitud, 0)
                    , Color.White);

            mensaje = String.Format("Nivel : {0:D2}", nivel);
            spriteBatch.DrawString(gameFont, mensaje,
                new Vector2(0,0), Color.White);


            mensaje = String.Format("Vidas : {0:D2}", vidas);
            spriteBatch.DrawString(gameFont, mensaje,
                new Vector2(0, 40), Color.White);



        }

        private void DibujarVehiculos()
        {
            foreach (Vehiculo v in vehiculos)
            {
                spriteBatch.Draw(v.Textura, v.getposicion() + new Vector2(0, -scroll), Color.White);
                 

            }

        }
    }
}
