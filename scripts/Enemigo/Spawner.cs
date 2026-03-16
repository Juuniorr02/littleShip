using Godot;

public partial class Spawner : Node2D
{
    [Export] public PackedScene EscenaEnemigo;
    [Export] public float TiempoSpawn = 2.0f;
    private Timer _timer;

    public override void _Ready()
    {
        _timer = new Timer();
        AddChild(_timer);
        _timer.WaitTime = TiempoSpawn;
        _timer.Timeout += OnSpawnTimerTimeout;
        _timer.Start();
    }

	private void OnSpawnTimerTimeout()
	{
		var enemigo = EscenaEnemigo.Instantiate<Enemigo>();
		
		// Obtenemos la posición Y del jugador (ajustala si quieres que sea un poco más arriba/abajo)
		// Suponiendo que tienes una referencia al jugador o usas una altura fija:
		float alturaAgua = 497f; // Cambia este número por la Y donde esté tu barco
		
		// Aparece fuera de pantalla a la derecha (X) y a la altura del agua (Y)
		Vector2 pos = new Vector2(GetViewportRect().Size.X + 100, alturaAgua);
		
		enemigo.GlobalPosition = pos;
		GetTree().Root.AddChild(enemigo);
	}
}
