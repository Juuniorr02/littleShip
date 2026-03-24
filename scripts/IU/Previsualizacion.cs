using Godot;
using System.Collections.Generic;

public partial class Previsualizacion : Node2D
{
    [Export] public int NumeroPuntos = 15; // Cuántos puntos dibujar
    [Export] public float EspaciadoTiempo = 0.1f; // Tiempo entre puntos
    [Export] public PackedScene PuntoEscena; // Una pequeña imagen circular (Sprite2D)

    private List<Sprite2D> _puntos = new List<Sprite2D>();

    public override void _Ready()
    {
        // Instanciamos los puntos y los ocultamos al inicio
        for (int i = 0; i < NumeroPuntos; i++)
        {
            var p = PuntoEscena.Instantiate<Sprite2D>();
            p.Visible = false;
            AddChild(p);
            _puntos.Add(p);
        }
    }

    public void ActualizarTrayectoria(Vector2 posicionInicial, Vector2 velocidadInicial, float gravedad)
    {
        for (int i = 0; i < NumeroPuntos; i++)
        {
            float t = i * EspaciadoTiempo;
            
            // Fórmula física de posición
            float x = velocidadInicial.X * t;
            float y = velocidadInicial.Y * t + 0.5f * gravedad * (t * t);
            
            _puntos[i].GlobalPosition = posicionInicial + new Vector2(x, y);
            _puntos[i].Visible = true;
        }
    }

    public void Ocultar()
    {
        foreach (var p in _puntos) p.Visible = false;
    }
}
