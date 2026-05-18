import { Notification, NotificationStatus } from "./models.js";
import * as storage from "./storage.js";
import { send as sendEmail } from "./providers/emailProvider.js";
import { send as sendSms } from "./providers/smsProvider.js";
import { send as sendPush } from "./providers/pushProvider.js";

export class NotificationProcessor {
  sendOne(n: Notification): void {
    n.status = NotificationStatus.PROCESSING;
    n.attempts++;
    n.lastAttemptAt = new Date();
    if (n.targetChannels.length === 0) {
      n.status = NotificationStatus.FAILED;
      n.lastError = "No target channels";
      return;
    }

    for (const target of n.targetChannels) {
      let response;

      if (target.type === "email") {
        response = sendEmail({
          recipient: target.value,
          message: n.message,
        });
      } else if (target.type === "sms") {
        response = sendSms({
          recipient: target.value,
          message: n.message,
        });
      } else if (target.type === "push") {
        response = sendPush({
          recipient: target.value,
          message: n.message,
        });
      } else {
        n.status = NotificationStatus.FAILED;
        n.lastError = "Unknown channel";
        return;
      }

      n.lastError = response.Message;
    }

    n.status = NotificationStatus.SENT;
  }

  sendAll(): void {
    const pending = storage.getAll().filter((n) => n.status === NotificationStatus.PENDING || n.status === NotificationStatus.RETRY_PENDING);
    for (const n of pending) {
      this.sendOne(n);
    }
  }
}
