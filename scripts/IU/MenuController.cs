using Godot;
using System;

public partial class MenuController : Node
{
    private PackedScene optionsScene;

    private AcceptDialog messageDialog;
    private Label dialogLabel;

    public override void _Ready()
    {
        Node menuRoot = GetParent();

        // ⭐ GameSettings
        var settings = GetTree().Root.GetNode<GameSettings>("GameSettings");
        settings.LoadConfig();
        settings.ApplySettings();

        Button jugarBtn = FindButton(menuRoot, "Jugar", "NewGame");
        Button optionsBtn = FindButton(menuRoot, "Opciones", "Options");
        Button quitBtn = FindButton(menuRoot, "Salir", "Quit");

        if (jugarBtn != null)
            jugarBtn.Pressed += OnJugarGame;

        if (optionsBtn != null)
            optionsBtn.Pressed += OnOptions;

        if (quitBtn != null)
            quitBtn.Pressed += OnQuit;

        // Dialog
        messageDialog = menuRoot.GetNodeOrNull<AcceptDialog>("MessageDialog");

        if (messageDialog == null)
        {
            foreach (var c in menuRoot.GetChildren())
            {
                if (c is AcceptDialog ad)
                {
                    messageDialog = ad;
                    break;
                }
            }
        }

        if (messageDialog != null)
            dialogLabel = messageDialog.GetNodeOrNull<Label>("DialogLabel");

        // ⭐ asegurar cursor visible en menú
        Input.MouseMode = Input.MouseModeEnum.Visible;
    }

    private Button FindButton(Node root, string textMatch, string nameContains)
    {
        if (root == null) return null;

        foreach (var c in root.GetChildren())
        {
            if (c is Button b)
            {
                if (b.Text == textMatch || b.Name.ToString().Contains(nameContains))
                    return b;
            }

            if (c is Node n)
            {
                var found = FindButton(n, textMatch, nameContains);
                if (found != null) return found;
            }
        }

        return null;
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