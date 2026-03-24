using Godot;

public partial class MenuDerrota : CanvasLayer
{
    private Button btnVolver;
    private Button btnReiniciar;
	private int healthActual;
    private bool isPaused = false;
    public static Enemigo Instance;

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;

        btnVolver = GetNodeOrNull<Button>("PanelContainer/VBoxContainer/botones/volver");
        btnReiniciar = GetNodeOrNull<Button>("PanelContainer/VBoxContainer/botones/reiniciar");

        ConfigurarBoton(btnVolver);
        ConfigurarBoton(btnReiniciar);


        if (btnReiniciar != null)
            btnReiniciar.Pressed += OnReiniciar;

        if (btnVolver != null)
            btnVolver.Pressed += OnVolver;

        Visible = false;
    }

	public override void _Process(double delta)
    {
        UpdateMenuDerrota();
    }

    private void ConfigurarBoton(Button b)
    {
        if (b == null) return;

        b.ProcessMode = ProcessModeEnum.Always;
        b.MouseFilter = Control.MouseFilterEnum.Stop;
    }

    public void UpdateMenuDerrota()
    {
        if (Jugador.Instance != null && Jugador.Instance.GetHealth() <= 0)
        {
            isPaused = true;
            GetTree().Paused = true;
            Visible = true; // Mostrar menú
            Input.MouseMode = Input.MouseModeEnum.Visible;
        }
    }

    private void Pausar()
    {
        isPaused = true;
        GetTree().Paused = true;
        Visible = true;
        Input.MouseMode = Input.MouseModeEnum.Visible;
    }

    private void QuitarPausa()
    {
        isPaused = false;
        GetTree().Paused = false;
        Visible = false;
    }

    private void OnReiniciar()
	{
		QuitarPausa();
        Input.MouseMode = Input.MouseModeEnum.Visible;
        Jugador.Instance.Revivir();
        Enemigo.Instance.Limpieza();
    	GD.Print("Reiniciar partida");
    	GetTree().ReloadCurrentScene();
	}
    private void OnVolver()
	{
		QuitarPausa();
        Input.MouseMode = Input.MouseModeEnum.Visible;
    	GetTree().ChangeSceneToFile("res://scenes/ui/Menu.tscn");
	}
}