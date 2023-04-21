using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Blazor_web.Hubs
{
    public class InferenceHub : Hub
    {
        public async Task SendTop10Predictions(string predictionsJson)
        {
            await Clients.All.SendAsync("ReceivePredictions", predictionsJson);
        }


    }
}
