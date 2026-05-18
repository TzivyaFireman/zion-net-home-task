export interface TargetChannel {
  type: string;
  value: string;
}

export const NotificationStatus = {
  PENDING: "pending",
  PROCESSING: "processing",
  SENT: "sent",
  RETRY_PENDING: "retry_pending",
  FAILED: "failed",
} as const;

export class Notification {
  id: number;
  targetChannels: TargetChannel[];
  message: string;
  status: typeof NotificationStatus[keyof typeof NotificationStatus];
  createdAt: Date;
  attempts: number;
  lastAttemptAt: Date | null;
  lastError: string | null;
  smsSegments: number;

  constructor(id: number, targetChannels: TargetChannel[], message: string) {
    this.id = id;
    this.targetChannels = targetChannels;
    this.message = message;
    this.status = this.status = NotificationStatus.PENDING;;
    this.createdAt = new Date();
    this.attempts = 0;
    this.lastAttemptAt = null;
    this.lastError = null;
    this.smsSegments = 0;
  }
}
