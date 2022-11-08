using DMCLICK.Entityes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DMCLICK.Controllers
{
    public static class BrusnikaApiController
    {
        public static async Task<List<Result>> GetResultsAsync(bool test = false)
        {
            List<Result> results = new List<Result>();
            using (var client = new HttpClient())
            {

                var otvet = await client.GetAsync("https://ekaterinburg.brusnika.ru/api/flats?complex=68&offset=0").Result.Content.ReadAsStringAsync();

                Flat flat = JsonConvert.DeserializeObject<Flat>(otvet);

                foreach (var result in flat.results)
                {
                    results.Add(result);
                }
                string nextURL = flat.next;

                if (test)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        otvet = await client.GetAsync(nextURL).Result.Content.ReadAsStringAsync();
                        flat = JsonConvert.DeserializeObject<Flat>(otvet);
                        foreach (var result in flat.results)
                        {
                            results.Add(result);
                        }
                        nextURL = flat.next;
                    }
                }
                else
                {
                    int i = 8;
                    while (flat.next != null)
                    {
                        otvet = await client.GetAsync(nextURL).Result.Content.ReadAsStringAsync();
                        flat = JsonConvert.DeserializeObject<Flat>(otvet);
                        foreach (var result in flat.results)
                        {
                            results.Add(result);
                        }
                        nextURL = flat.next;
                    }
                }
            }
            return results.Where(el => el.owner == "Застройщик" && el.status == "Свободна").ToList();
        }
    }
}
