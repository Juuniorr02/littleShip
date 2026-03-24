using Godot;

public partial class Enemigo : CharacterBody2D
{
    [Export] public float Velocidad = 100f;
    [Export] public int Vida = 10;
    
    private Vector2 _retrocesoActual = Vector2.Zero;
    [Export] public float FriccionRetroceso = 5f; 

    private float _yOriginal; 

    // --- VARIABLES DE HUNDIMIENTO MEJORADAS ---
    private bool _estaHundiendose = false;
    private float _velocidadHundimiento = 60f; // Un poco más rápido para que se note
    private float _tiempoHundimiento = 0f;
    private float _duracionHundimiento = 3.5f; // Más tiempo para ver la caída
    private float _rotacionHundimiento; // Para que cada barco se incline distinto

    [Export] public int DañoPorSegundo = 5;
    [Export] public float DuracionFuego = 3f;
    private float _tiempoFuego = 0f;
    private Timer _timerFuego;
    public static Enemigo Instance;

    public override void _Ready()
    {

        AddToGroup("Enemigos");
        _yOriginal = GlobalPosition.Y;

        _timerFuego = new Timer();
        _timerFuego.WaitTime = 1f;
        _timerFuego.OneShot = false;
        _timerFuego.Connect("timeout", new Callable(this, "OnTimerFuegoTimeout"));
        AddChild(_timerFuego);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_estaHundiendose)
        {
            // 1. MOVIMIENTO DE HUNDIMIENTO REALISTA
            // Ya no usamos Velocity ni MoveAndSlide para que no choque con nada
            // Simplemente bajamos su posición global
            GlobalPosition += Vector2.Down * _velocidadHundimiento * (float)delta;
            
            // 2. ROTACIÓN (Se inclina hacia un lado como si entrara agua)
            Rotation += _rotacionHundimiento * (float)delta;

            // 3. DESVANECIMIENTO (Alpha)
            Color c = Modulate;
            c.A -= 0.3f * (float)delta; // Se vuelve transparente poco a poco
            Modulate = c;

            // 4. ELIMINACIÓN FINAL
            _tiempoHundimiento += (float)delta;
            if (_tiempoHundimiento >= _duracionHundimiento) 
            {
                QueueFree();
            }
            return; // Salimos para que no ejecute la lógica de movimiento normal
        }

        // --- LÓGICA NORMAL (Mientras está vivo) ---
        Vector2 movimientoBase = Vector2.Left * Velocidad;
        float diferenciaY = _yOriginal - GlobalPosition.Y;
        Vector2 fuerzaRetornoY = new Vector2(0, diferenciaY * 4.0f);

        Velocity = movimientoBase + _retrocesoActual + fuerzaRetornoY;
        _retrocesoActual = _retrocesoActual.Lerp(Vector2.Zero, (float)delta * FriccionRetroceso);

        MoveAndSlide();
        DetectarColisionJugador();
    }

    private void Hundirse()
    {
        if (_estaHundiendose) return;
        _estaHundiendose = true;

        // Decidimos aleatoriamente si se inclina a la izquierda o derecha al hundirse
        _rotacionHundimiento = (float)GD.RandRange(-0.5, 0.5);

        // Desactivamos colisiones para que atraviese el suelo/agua y no estorbe
        if (HasNode("CollisionShape2D")) GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true);
        if (HasNode("CollisionShape2D2")) GetNode<CollisionShape2D>("CollisionShape2D2").SetDeferred("disabled", true);
        
        GD.Print($"El barco {Name} se va al fondo...");
    }

    public void AplicarRetroceso(Vector2 fuerza)
    {
        if (_estaHundiendose) return;
        _retrocesoActual = fuerza; 
    }

    public void RecibirDmg(int cantidad)
    {
        if (_estaHundiendose) return;
        Vida -= cantidad;
        if (Vida <= 0) Hundirse();
    }

    public void Quemarse(int dañoPorSegundo, float duracion)
    {
        if (_estaHundiendose) return;
        DañoPorSegundo = dañoPorSegundo;
        DuracionFuego = duracion;
        _tiempoFuego = 0f;
        Modulate = new Color(1, 0.5f, 0.5f);
        _timerFuego.Start();
    }

    private void OnTimerFuegoTimeout()
    {
        if (_estaHundiendose) { _timerFuego.Stop(); return; }
        RecibirDmg(DañoPorSegundo);
        _tiempoFuego += 1f;
        if (_tiempoFuego >= DuracionFuego)
        {
            _timerFuego.Stop();
            Modulate = new Color(1, 1, 1);
        }
    }

    private void DetectarColisionJugador()
    {
        if (_estaHundiendose) return;
        for (int i = 0; i < GetSlideCollisionCount(); i++)
        {
            var collision = GetSlideCollision(i);
            if (collision.GetCollider() is Jugador jugador)
            {
                jugador.RecibirDmg(Vida);
                Hundirse();
                break;
            }
        }
    }

    public void Limpieza() => QueueFree();
}
