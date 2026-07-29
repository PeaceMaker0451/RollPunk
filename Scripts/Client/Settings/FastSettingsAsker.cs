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
                "(Интерфейса для изменения имени сейчас нет, так что пиши правильно с первого раза!)";
            
            var result = await ClientRoot.FormsManager.Dialogs.ShowStringInput(title, message, allowCancel: false);

            var settings = ClientRoot.SettingsManager.LoadSettings();
            settings.Name = result.Value;
            ClientRoot.SettingsManager.SaveSettings(settings);
        }
    }
}
