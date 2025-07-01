using System.ComponentModel;

namespace ProyectoLenguajes.Models
{
    public enum EstadoPedido
    {
        [Description("A Tiempo")]
        ATiempo,
        [Description("Sobre Tiempo")]
        SobreTiempo,
        Demorado,
        Anulado,
        Entregado
    }
}
