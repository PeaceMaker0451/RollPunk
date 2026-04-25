using RollPunk.Modding;

namespace RollPunk.Client.Runtime
{
    internal static class HookerBatchingExtension
    {
        public static object[] BatchHook(this ModHooker hooker, MutationCatcher catcher, string hookName, params object[] args)
        {
            using (new MutationsBatch(catcher))
            {
                return hooker.CallHook(hookName, args);
            }
        }
    }
}
