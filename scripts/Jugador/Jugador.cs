using Godot;

public partial class Jugador : Node2D
{
    [Export] public PackedScene EscenaProyectil;
    
    // Configuración de fuerza
    [Export] public float MinForce = 400f;
    [Export] public float MaxForce = 1500f;
    [Export] public float ChargeTimeMax = 1.2f; 

    private float _currentCharge = 0f;
    private bool _isCharging = false;
    
    private Node2D _pivoteCañon;
    private Marker2D _puntoDisparo;

    public override void _Ready()
    {
        _pivoteCañon = GetNode<Node2D>("PivoteCañon");
        _puntoDisparo = _pivoteCañon.GetNode<Marker2D>("PuntoDisparo");
    }

    public override void _Process(double delta)
    {
        // Apuntar al ratón (con el pivote que creamos antes)
        _pivoteCañon.LookAt(GetGlobalMousePosition());

        // Gestión de la carga
        if (Input.IsActionPressed("disparar"))
        {
            _isCharging = true;
            _currentCharge += (float)delta;
            _currentCharge = Mathf.Min(_currentCharge, ChargeTimeMax);
            
            // Log para debug (puedes borrarlo luego)
            // GD.Print($"Cargando: {(_currentCharge / ChargeTimeMax) * 100}%");
        }
        else if (_isCharging)
        {
            Fire();
            _isCharging = false;
            _currentCharge = 0f;
        }
    }

    private void Fire()
    {
        var bala = EscenaProyectil.Instantiate<Proyectil>();
        GetTree().Root.AddChild(bala);
        
        // Posicionamos la bala en la punta del cañón
        bala.GlobalTransform = _puntoDisparo.GlobalTransform;

        // Calculamos la fuerza final según el tiempo de carga
        float ratio = _currentCharge / ChargeTimeMax;
        float finalForce = Mathf.Lerp(MinForce, MaxForce, ratio);

        // Aplicamos el impulso inicial (Linear Velocity)
        // Usamos GlobalTransform.X para que salga hacia donde apunta el cañón
        bala.ApplyImpulse(_pivoteCañon.GlobalTransform.X * finalForce);
    }
}
