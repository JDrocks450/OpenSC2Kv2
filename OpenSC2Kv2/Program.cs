using OpenSC2Kv2.API;
using OpenSC2Kv2.Game;
using System;
using System.IO;

string errorMsg = string.Empty;
if (GameSettings.Default.InstallDir == null)
{
    errorMsg = "Your installation directory is not set yet. Please see the config file in the root directory of this application.";
    goto error;
}
try
{
    SC2Path.Setup(GameSettings.Default.InstallDir);
}
catch (Exception ex)
{
    errorMsg = ex.ToString();
    goto error;
}
goto go;

error:
File.WriteAllText("crashdump.txt", errorMsg);

go:
using var game = new OpenSC2Kv2.Game.OpenSC2KGame();
game.Run();

