using Godot;

public partial class Spawner : Node2D
{
    [Export] public PackedScene EscenaEnemigo;
    [Export] public PackedScene EscenaEnemigo2;

    [Export] public float TiempoSpawn = 2.0f;

    private Timer _timer;
    private RandomNumberGenerator _rng = new RandomNumberGenerator();

    public override void _Ready()
    {
        _timer = new Timer();
        AddChild(_timer);

        _timer.WaitTime = TiempoSpawn;
        _timer.Timeout += OnSpawnTimerTimeout;
        _timer.Start();

        _rng.Randomize(); // Inicializa el generador aleatorio
    }

    private void OnSpawnTimerTimeout()
    {
        // Elegir enemigo aleatoriamente
        PackedScene escenaElegida = _rng.RandiRange(0, 1) == 0 
            ? EscenaEnemigo 
            : EscenaEnemigo2;

        var enemigo = escenaElegida.Instantiate<Node2D>();

        float alturaAgua = 497f;

        Vector2 pos = new Vector2(GetViewportRect().Size.X + 100, alturaAgua);

        enemigo.GlobalPosition = pos;

        GetTree().Root.AddChild(enemigo);
    }
}