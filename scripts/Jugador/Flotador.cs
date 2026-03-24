using Godot;

public partial class Flotador : Node
{
	[Export] public float AmplitudVertical = 4.0f;
	[Export] public float VelocidadVertical = 1.5f;
	[Export] public float AmplitudRotacion = 0.04f;

	private Node2D _padre;
	private float _tiempo;
	private Vector2 _posicionRelativaOriginal; // Guardamos la posición local del sprite/arte

	public override void _Ready()
	{
		_padre = GetParent<Node2D>();
		// Guardamos donde está el objeto visual al inicio
		_posicionRelativaOriginal = _padre.Position;
		_tiempo = (float)GD.RandRange(0, 10); 
	}

	public override void _Process(double delta)
	{
		if (_padre == null) return;

		_tiempo += (float)delta;

		// Calculamos solo el "meneo"
		float offsetV = Mathf.Sin(_tiempo * VelocidadVertical) * AmplitudVertical;
		float offsetR = Mathf.Cos(_tiempo * VelocidadVertical * 0.7f) * AmplitudRotacion;

		// IMPORTANTE: Si el Flotador es hijo del SPRITE (el dibujo) y no del Enemigo (el cuerpo), 
		// el código de abajo moverá solo el dibujo y dejará que el Enemigo.cs mueva el cuerpo físico.
		
		// Si el Flotador es hijo directo del Enemigo (CharacterBody2D), usa esto:
		_padre.Rotation = offsetR;
		// Solo modificamos la posición visual si el Flotador está en un nodo visual separado.
	}
}
