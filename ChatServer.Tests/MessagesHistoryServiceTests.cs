using System;
using Xunit;
using ChatServer.Services;
using System.IO;
using System.Collections.Generic;

namespace ChatServer.Tests
{
    public class MessagesHistoryServiceTests
    {
        public string FullFilePath { get; set; }
        public MessagesHistoryServiceTests()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            FullFilePath = $"{currentDirectory}/History.txt";
        }

        [Fact]
        public void ServiceCreation_ShouldCreateFile_IfItNotExist()
        {
            var service = new FileMessageHistory(FullFilePath);

            Assert.True(File.Exists(FullFilePath));
        }

        [Fact]
        public void GettingMessagesHistory_ShouldGetMessagesFromFile()
        {
            var messages = new string[] { "Hi", "How are you?", "Something else..." };
            WriteMessagesToFile(messages);

            var service = new FileMessageHistory(FullFilePath);

            Assert.Equal(service.GetMessageHistory(), messages);

        }

        [Fact]
        public void AddingMessageToHistory_ShouldAddItToFile_CountAreEqual()
        {
            var messages = new List<string> { "Hi", "How are you?", "Something else..." };
            WriteMessagesToFile(messages.ToArray());
            var service = new FileMessageHistory(FullFilePath);

            service.AddMessageToHistory("New Message");
            var receivedMessages = ReadMessagesFromFile();
            messages.Add("New Message");

            Assert.True(messages.Count == receivedMessages.Count);

        }

        [Fact]
        public void AfterAddingMessage_HistoryShouldBeSorted()
        {
            var messages = new List<string> { "Hi", "How are you?", "Something else..." };
            WriteMessagesToFile(messages.ToArray());
            var service = new FileMessageHistory(FullFilePath);

            service.AddMessageToHistory("New Message");
            var receivedMessages = ReadMessagesFromFile();
            //Add message to ethalon
            messages.Add("New Message");
            //Sort array
            messages.Sort();

            Assert.Equal(messages, receivedMessages);
        }

        private void WriteMessagesToFile(string[] messages)
        {
            using (var fileStream = new FileStream(FullFilePath, FileMode.Truncate, FileAccess.Write, FileShare.ReadWrite))
            {
                using (var fileWriter = new StreamWriter(fileStream))
                {
                    foreach (var message in messages)
                    {
                        fileWriter.WriteLine(message);
                    }
                }
            }
        }

        private List<string> ReadMessagesFromFile()
        {
            var receivedMessage = new List<string>();
            using (var fileStream = new FileStream(FullFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var fileReader = new StreamReader(fileStream))
                {
                    string message;
                    while ((message = fileReader.ReadLine()) != null)
                        receivedMessage.Add(message);
                }
            }
            return receivedMessage;
        }
    }
}
