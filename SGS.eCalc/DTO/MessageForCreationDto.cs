using System;

public class MessageForCreationDto
    {
        public int SenderId { get; set; }
        public int RecipientId { get; set; }

        public String Content { get; set; }
        public DateTime MessageSent { get; set; }

        public MessageForCreationDto(){
            MessageSent = DateTime.Now;
        }
    }