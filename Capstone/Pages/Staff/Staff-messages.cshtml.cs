using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Capstone.Pages.Staff
{
    public class Staff_messagesModel : PageModel
    {
        public List<Conversation> Conversations { get; set; }
        public Conversation SelectedConversation { get; set; }
        public int? SelectedConversationId { get; set; }

        [BindProperty]
        public string Content { get; set; }

        [BindProperty]
        public string Recipient { get; set; }

        public void OnGet(int? conversationId)
        {
            // Sample conversations for testing, replace with actual data retrieval
            Conversations = new List<Conversation>
            {
                new Conversation
                {
                    Id = 1,
                    Sender = "John Doe",
                    LastMessageSnippet = "Hey, how are you?",
                    LastMessageDate = DateTime.Now.AddHours(-1),
                    Messages = new List<Message>
                    {
                        new Message { Sender = "John Doe", Content = "Hey, how are you?", DateSent = DateTime.Now.AddHours(-1) },
                        new Message { Sender = "You", Content = "I'm doing good, thanks!", DateSent = DateTime.Now.AddMinutes(-45) }
                    }
                },
                new Conversation
                {
                    Id = 2,
                    Sender = "Jane Smith",
                    LastMessageSnippet = "Let's meet tomorrow.",
                    LastMessageDate = DateTime.Now.AddDays(-1),
                    Messages = new List<Message>
                    {
                        new Message { Sender = "Jane Smith", Content = "Let's meet tomorrow.", DateSent = DateTime.Now.AddDays(-1) }
                    }
                }
            };

            if (conversationId.HasValue)
            {
                SelectedConversationId = conversationId;
                SelectedConversation = Conversations.FirstOrDefault(c => c.Id == conversationId);
            }
        }

        public IActionResult OnPost(int? conversationId)
        {
            if (!string.IsNullOrEmpty(Content) && conversationId.HasValue)
            {
                // Add the new message to the selected conversation
                var conversation = Conversations.FirstOrDefault(c => c.Id == conversationId.Value);
                if (conversation != null)
                {
                    conversation.Messages.Add(new Message
                    {
                        Sender = "You",
                        Content = Content,
                        DateSent = DateTime.Now
                    });
                }
            }

            // Redirect back to the same conversation after posting a message
            return RedirectToPage(new { conversationId = conversationId });
        }

        // Data model for a conversation
        public class Conversation
        {
            public int Id { get; set; }
            public string Sender { get; set; }
            public string LastMessageSnippet { get; set; }
            public DateTime LastMessageDate { get; set; }
            public List<Message> Messages { get; set; } = new List<Message>();
        }

        // Data model for a message
        public class Message
        {
            public string Sender { get; set; }
            public string Content { get; set; }
            public DateTime DateSent { get; set; }
        }
    }
}
