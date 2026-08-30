using RollPunk.Client;

namespace RollPunk.Scripts.Client.Settings
{
    internal class FastSettingsAsker
    {
        public async void AskUserName()
        {
            string title = "Быстрая настроечка";
            string message = "Привет!\n" +
                "Введи свой игровой ник. Его будут видеть другие игроки.\n" +
                "(можно будет изменить в настройках)";
            
            var result = await Root.Forms.Dialogs.ShowStringInput(title, message, allowCancel: false, minSize: new(400, 250), placeholder: "Меня зовут....", okButtonText: "Привет, Роллпанк!");

            var settings = Root.Settings.LoadSettings();
            settings.Name = result.Value;
            Root.Settings.SaveSettings(settings);
        }
    }
}
