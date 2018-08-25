using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ng_signalr_core
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", 
                new ChatMessage { UserName = user, Message = message });
        }

        public async Task SendMessageForComment(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessageWithComment",
                new ChatMessageWithComment { UserName = user, Message = message, Comment = "Great job!" });
        }

        public ChannelReader<int> Counter(int count, int delay)
        {
            var channel = Channel.CreateUnbounded<int>();

            // We don't want to await WriteItems, otherwise we'd end up waiting 
            // for all the items to be written before returning the channel back to
            // the client.
            _ = WriteItems(channel.Writer, count, delay);

            return channel.Reader;
        }

        private async Task WriteItems(ChannelWriter<int> writer, int count, int delay)
        {
            for (var i = 0; i < count; i++)
            {
                await writer.WriteAsync(i);
                await Task.Delay(delay);
            }

            writer.TryComplete();
        }
    }
    
    public class ChatMessage
    {
        public string UserName { get; set; }
        public string Message { get; set; }
    }

    public class ChatMessageWithComment : ChatMessage
    {
        public string Comment { get; set; }
    }
}
