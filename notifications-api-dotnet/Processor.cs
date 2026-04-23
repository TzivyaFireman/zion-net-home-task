using NotificationApi.Providers;

namespace NotificationApi;

public class NotificationProcessor
{
    public void SendOne(Notification n)
    {
        n.Status = NotificationStatuses.Processing;
        n.Attempts += 1;
        n.LastAttemptAt = DateTime.Now.ToString("O");

        if (n.TargetChannels.Count == 0)
        {
            n.Status = NotificationStatuses.Failed;
            n.LastError = "No target channels";
            return;
        }
        var target = n.TargetChannels[0];

        var req = new Dictionary<string, string>
        {
            { "recipient", target.Value },
            { "message", n.Message }
        };

        ProviderResponse response;
        if (target.Type == "email")
        {
            response = EmailProvider.Send(req);
        }
        else if (target.Type == "sms")
        {
            response = SmsProvider.Send(req);
        }
        else if (target.Type == "push")
        {
            response = PushProvider.Send(req);
        }
        else
        {
            n.Status = NotificationStatuses.Failed;
            n.LastError = "Unknown channel";
            return;
        }

        n.Status = NotificationStatuses.Sent;
        n.LastError = response.Message;
    }

    public void SendAll()
    {
        var pending = Storage.Notifications.Where(n => n.Status == NotificationStatuses.Pending).ToList();
        foreach (var n in pending)
        {
            SendOne(n);
        }
    }
}
