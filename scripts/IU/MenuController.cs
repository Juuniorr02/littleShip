using Godot;
using System;

public partial class MenuController : Node
{
    private PackedScene optionsScene;

    private AcceptDialog messageDialog;
    private Label dialogLabel;

    public override void _Ready()
    {
        var settings = GetTree().Root.GetNode<GameSettings>("GameSettings");
        settings.LoadConfig();
        settings.ApplySettings();

        Button jugarBtn = GetNodeOrNull<Button>("%Jugar");
        Button optionsBtn = GetNodeOrNull<Button>("%Opciones");
        Button quitBtn = GetNodeOrNull<Button>("%Salir");

        if (jugarBtn != null)
            jugarBtn.Pressed += OnJugarGame;

        if (optionsBtn != null)
            optionsBtn.Pressed += OnOptions;

        if (quitBtn != null)
            quitBtn.Pressed += OnQuit;

        messageDialog = GetNodeOrNull<AcceptDialog>("MessageDialog");

        if (messageDialog != null)
            dialogLabel = messageDialog.GetNodeOrNull<Label>("DialogLabel");

        Input.MouseMode = Input.MouseModeEnum.Visible;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent && keyEvent.Pressed)
        {
            if (keyEvent.Keycode == Key.Escape)
                GetTree().Quit();
        }
    }

    public void OnJugarGame()
    {
        GD.Print("Iniciando nueva partida...");
        GetTree().ChangeSceneToFile("res://scenes/main.tscn");
    }

    public void OnOptions()
    {
        GetTree().ChangeSceneToFile("res://scenes/ui/options.tscn");
    }

    public void OnQuit()
    {
        GetTree().Quit();
    }

    private void ShowMessage(string text)
    {
        if (dialogLabel != null)
            dialogLabel.Text = text;

        if (messageDialog != null)
            messageDialog.PopupCentered();
        else
            GD.Print(text);
    }
}