using Godot;
using System.Collections.Generic;

public partial class Jugador : CharacterBody2D
{
    [Export] public PackedScene[] EscenasProyectiles; 
    [Export] public float[] CooldownsBase = { 0.5f, 2.0f, 1.0f, 1.5f };
    
    private int _proyectilSeleccionado = 0;
    private Dictionary<int, double> _proximosDisparos = new();

    [Export] public float MinForce = 400f;
    [Export] public float MaxForce = 1500f;
    [Export] public float ChargeTimeMax = 1.2f; 
    public static Jugador Instance;

    private float _currentCharge = 0f;
    private bool _isCharging = false;
    [Export] private int health;
    private Node2D _pivoteCañon;
    private Marker2D _puntoDisparo;

    public override void _Ready()
    {
        _pivoteCañon = GetNode<Node2D>("PivoteCañon");
        _puntoDisparo = _pivoteCañon.GetNode<Marker2D>("PuntoDisparo");
        for (int i = 0; i < 4; i++) _proximosDisparos[i] = 0;
        Instance = this;
    }

public void CambiarMunicion(int indice)
{
    // Solo cambiamos si el índice es válido y si no es el proyectil que ya tenemos
    if (indice >= 0 && indice < EscenasProyectiles.Length && _proyectilSeleccionado != indice)
    {
        _proyectilSeleccionado = indice;
        
        // Texto para el Print según el índice
        string nombreProyectil = indice switch {
            0 => "NORMAL",
            1 => "FUEGO",
            2 => "RAFAGA",
            3 => "SONICO",
            _ => "DESCONOCIDO"
        };

        // Imprime en la consola de Godot con colorS
        GD.PrintRich($"[color=yellow][MUNICIÓN][/color] Cambiado a: [b]{nombreProyectil}[/b]");
    }
}

public override void _Process(double delta)
{
    _pivoteCañon.LookAt(GetGlobalMousePosition());

    // --- ATAJOS DE TECLADO (1, 2, 3, 4) ---
    if (Input.IsKeyPressed(Key.Key1)) CambiarMunicion(0);
    if (Input.IsKeyPressed(Key.Key2)) CambiarMunicion(1);
    if (Input.IsKeyPressed(Key.Key3)) CambiarMunicion(2);
    if (Input.IsKeyPressed(Key.Key4)) CambiarMunicion(3);

    ManejarDisparo(delta);
}

    private void ManejarDisparo(double delta)
    {
        double tiempoActual = Time.GetTicksMsec() / 1000.0;

        if (Input.IsActionPressed("disparar"))
        {
            if (tiempoActual >= _proximosDisparos[_proyectilSeleccionado])
            {
                _isCharging = true;
                _currentCharge = Mathf.Min(_currentCharge + (float)delta, ChargeTimeMax);
            }
        }
        else if (_isCharging)
        {
            Fire(_proyectilSeleccionado);
            _proximosDisparos[_proyectilSeleccionado] = tiempoActual + CooldownsBase[_proyectilSeleccionado];
            _isCharging = false;
            _currentCharge = 0f;
        }
    }

private void Fire(int tipo)
{
    // Verificación 1: ¿El array existe y tiene ese índice?
    if (EscenasProyectiles == null || tipo >= EscenasProyectiles.Length)
    {
        GD.PrintErr($"Error: El índice de proyectil {tipo} no existe en el array.");
        return;
    }

    // Verificación 2: ¿Hay una escena asignada en ese índice del Inspector?
    if (EscenasProyectiles[tipo] == null)
    {
        GD.PrintErr($"Error: No hay una escena asignada para el proyectil tipo {tipo} en el Inspector.");
        return;
    }

    var instancia = EscenasProyectiles[tipo].Instantiate();
    
    // Verificación 3: ¿La escena instanciada tiene el script 'Proyectil'?
    if (instancia is Proyectil bala)
    {
        GetTree().Root.AddChild(bala);
        bala.GlobalTransform = _puntoDisparo.GlobalTransform;
        float ratio = _currentCharge / ChargeTimeMax;
        float finalForce = Mathf.Lerp(MinForce, MaxForce, ratio);
        bala.ApplyImpulse(_pivoteCañon.GlobalTransform.X * finalForce);
    }
    else
    {
        GD.PrintErr("Error: La escena del proyectil no tiene el script 'Proyectil.cs' o no hereda de él.");
        instancia.QueueFree(); // Limpiamos la instancia fallida
    }
}

    public void RecibirDmg(int cantidad)
    {
        health -= cantidad;
        health = Mathf.Max(health, 0);

        GD.Print($"Jugador recibió {cantidad} de daño. Vida restante: {health}");
    }

    public void Revivir()
    {
        health = 100; // Restablece la vida a 100 o al valor que desees
        GD.Print("Jugador ha sido revivido. Vida restaurada a: " + health);
    }

    public int GetHealth() => health;
}
