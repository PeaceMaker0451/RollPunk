using RollPunk.Debug;
using RollPunk.Modding;

namespace RollPunk.Client.Runtime
{
    internal static class HookerBatchingExtension
    {
        public static object[] BatchHook(this ModHooker hooker, MutationCatcher catcher, string hookName, params object[] args)
        {
            using (new MutationsBatch(catcher))
            {
                RPDebug.DebugLog($"[color=olive]Начинаем батчить хук {hookName}...[/color]");
                var result = hooker.CallHook(hookName, args);
                RPDebug.DebugLog($"[color=olive]Конец батчинга хука {hookName}[/color]");
                return result;
            }
        }
    }
}
