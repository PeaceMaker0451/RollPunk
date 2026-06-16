using Newtonsoft.Json;
using RollPunk.Fields;

namespace NetcodeCommon
{
    public class SessionPatch
    {
        [JsonProperty] public List<FieldState> PendingFields = new();
        [JsonProperty] public List<Guid> RemoveFields = new();
    }
}
