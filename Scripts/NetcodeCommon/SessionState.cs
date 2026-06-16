using Newtonsoft.Json;
using RollPunk.Fields;

namespace NetcodeCommon
{
    public class SessionState
    {
        [JsonProperty] public List<FieldState> Fields;
    }
}
