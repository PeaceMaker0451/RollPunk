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
            
            var result = await ClientRoot.FormsManager.Dialogs.ShowStringInput(title, message, allowCancel: false, minSize: new(400, 250), placeholder: "Меня зовут....", okButtonText: "Привет, Роллпанк!");

            var settings = ClientRoot.SettingsManager.LoadSettings();
            settings.Name = result.Value;
            ClientRoot.SettingsManager.SaveSettings(settings);
        }
    }
}
