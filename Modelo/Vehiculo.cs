using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruzaCruza.Modelo
{
    class Vehiculo
    {

        public int Id { get; set; } // identifica el carril
        public Texture2D Textura { get; set;} 
        public Carril Carril { get; set; } //  
        public int Distancia { get; set; } // dentro de ese vehiculo en se carril

        public Vehiculo(int id, Texture2D textura, Carril carril, int distancia) //creamos las propiedades en el constructor
        {
            this.Id = id;
            this.Textura = textura;
            this.Carril = carril;
            this.Distancia = distancia;
            
        }

        public Vector2 getposicion()  
        {
            return new Vector2(Distancia, Carril.PosicionY); //en coordenadas x e y

        }



    }
}
