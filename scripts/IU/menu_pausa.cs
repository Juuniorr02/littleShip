using Godot;

public partial class menu_pausa : CanvasLayer
{
    private Button btnGuardarSalir;
    private Button btnVolver;

    private bool isPaused = false;

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;

        btnGuardarSalir = GetNodeOrNull<Button>("CenterContainer/VBoxContainer/guardarsalir");
        btnVolver = GetNodeOrNull<Button>("CenterContainer/VBoxContainer/volver");

        ConfigurarBoton(btnGuardarSalir);
        ConfigurarBoton(btnVolver);

        if (btnGuardarSalir != null)
            btnGuardarSalir.Pressed += OnGuardarSalir;

        if (btnVolver != null)
            btnVolver.Pressed += Salir;

        Visible = false;
    }

    private void ConfigurarBoton(Button b)
    {
        if (b == null) return;

        b.ProcessMode = ProcessModeEnum.Always;
        b.MouseFilter = Control.MouseFilterEnum.Stop;
    }

    public override void _Input(InputEvent e)
    {
        if (e.IsActionPressed("pausa"))
        {
            if (isPaused) QuitarPausa();
            else Pausar();
        }
    }

    private void Pausar()
    {
        isPaused = true;
        GetTree().Paused = true;
        Visible = true;
        Input.MouseMode = Input.MouseModeEnum.Visible;
    }

	private void Salir()
    {
        QuitarPausa();
        Input.MouseMode = Input.MouseModeEnum.Visible;
        GetTree().ChangeSceneToFile("res://scenes/ui/Menu.tscn");
    }

    private void QuitarPausa()
    {
        isPaused = false;
        GetTree().Paused = false;
        Visible = false;
    }

	private void OnGuardarSalir()
	{
		QuitarPausa();
	}
}