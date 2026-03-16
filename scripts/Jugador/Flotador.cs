using Godot;
using System;

public partial class Flotador : Node
{
    // Variables para ajustar el "feeling" desde el inspector de cada barco
    [Export] public float AmplitudVertical = 4.0f;
    [Export] public float VelocidadVertical = 1.5f;
    [Export] public float AmplitudRotacion = 0.04f;

    private Node2D _padre; // El barco que queremos mover
    private float _tiempo;
    private float _yInicial;

    public override void _Ready()
    {
        // Obtenemos la referencia al barco (su padre)
        _padre = GetParent<Node2D>();
        _yInicial = _padre.Position.Y;
        
        // Desfase aleatorio para que no todos los barcos bailen al unísono
        _tiempo = (float)GD.RandRange(0, 10); 
    }

    public override void _Process(double delta)
    {
        if (_padre == null) return;

        _tiempo += (float)delta;

        // Calculamos el movimiento
        float offsetV = Mathf.Sin(_tiempo * VelocidadVertical) * AmplitudVertical;
        float offsetR = Mathf.Cos(_tiempo * VelocidadVertical * 0.7f) * AmplitudRotacion;

        // Aplicamos al padre
        _padre.Position = new Vector2(_padre.Position.X, _yInicial + offsetV);
        _padre.Rotation = offsetR;
    }
}
