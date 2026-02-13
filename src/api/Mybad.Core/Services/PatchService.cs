using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Mybad.Core.Services
{
    public class PatchService
    {
        private Dictionary<string, int> _patchNameId = new();
        private readonly IHttpClientFactory _factory;

        public PatchService(IHttpClientFactory factory)
        {
            _factory = factory;
        }
        public int? ConvertPatchNameToId(string patchName)
        {
            return _patchNameId.TryGetValue(patchName, out int val) ? val : null;
        }

        public async Task SyncronizePatchInfo()
        {
            var http = _factory.CreateClient("ODota");

            var responce = await http.GetFromJsonAsync<List<PatchInfo>>("constants/patch");
            if (responce != null)
            {
                var accessiblePatches = responce.Where(r => r.Id >= 59); //59 - patch 7.40 - first patch in db
                foreach (var item in accessiblePatches)
                {
                    _patchNameId.TryAdd(item.Name, item.Id);
                }
            }
        }

        public List<string> GetAllPatchNames()
        {
            return _patchNameId.Keys.Where(k => _patchNameId[k] >= 59).ToList();
        }
    }

    internal class PatchInfo
    {
        [JsonPropertyName("name")]
        public required string Name { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }
    }

}
