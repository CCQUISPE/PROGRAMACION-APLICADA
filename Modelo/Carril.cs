using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruzaCruza.Modelo
{
    class Carril
    {
        public int Id { get; set; } // identifica el carril
        public int PosicionY { get; set; } // ubicacion vertical
        public int Velocidad { get; set; } // velocidad (positiva derecha, negativa izquierda)

        public Carril (int id, int posicionY, int velocidad) //creamos las propiedades
        {
            this.Id = id;
            this.PosicionY = posicionY;
            this.Velocidad = velocidad;
        }

    }
}
