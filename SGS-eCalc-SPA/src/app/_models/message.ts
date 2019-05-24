export interface Message {

    id: number;
    senderId: number;
    recipientId: number;
    senderKnownAs: string;
    senderPhotoUrl: string;
    recipientKnownAs: string;
    recipientPhotoUrl: string;
    content: string;
    isRead: boolean;
    dateRead: Date;
    messageSent: Date;
}
