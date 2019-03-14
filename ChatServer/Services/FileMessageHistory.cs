using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Services
{
    public class FileMessageHistory : IMessageHistoryService
    {
        public string FullFilePath { get; set; }
        public string FileName { get; set; }
        public FileMessageHistory(string fullFilePath)
        {
            try
            {
                FullFilePath = fullFilePath;
                FileName = Path.GetFileName(fullFilePath);

                if (!File.Exists(fullFilePath))
                {
                    File.Create(fullFilePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public Task AddMessageToHistoryAsync(string message)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var messages = new List<string>(await ReadMessagesFromFileAsync());
                    using (var fileStream = new FileStream(FullFilePath, FileMode.Truncate, FileAccess.Write, FileShare.ReadWrite))
                    {
                        using (var fileWriter = new StreamWriter(fileStream))
                        {
                            messages.Add(message);
                            messages.Sort();
                            messages.ForEach(item => fileWriter.WriteLine(item));
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new IOException("Error adding message to file");
                }
            });
        }

        public void AddMessageToHistory(string message)
        {
            try
            {
                var messages = new List<string>(ReadMessagesFromFile());
                using (var fileStream = new FileStream(FullFilePath, FileMode.Truncate, FileAccess.Write, FileShare.ReadWrite))
                {
                    using (var fileWriter = new StreamWriter(fileStream))
                    {
                        messages.Add(message);
                        messages.Sort();
                        messages.ForEach(item => fileWriter.WriteLine(item));
                    }
                }
            }
            catch (Exception ex)
            {
                throw new IOException("Error adding message to file");
            }
        }

        public Task<IEnumerable<string>> GetMessageHistoryAsync()
        {
            return ReadMessagesFromFileAsync();
        }

        public IEnumerable<string> GetMessageHistory()
        {
            return ReadMessagesFromFile();
        }

        private Task<IEnumerable<string>> ReadMessagesFromFileAsync()
        {
            return Task.Run(() =>
            {
                return ReadMessagesFromFile();
            });
        }

        public IEnumerable<string> ReadMessagesFromFile()
        {
            var messages = new List<string>();
            try
            {
                using (var fileStream = new FileStream(FullFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var fileReader = new StreamReader(fileStream))
                    {
                        string message;
                        while ((message = fileReader.ReadLine()) != null)
                        {
                            messages.Add(message);
                        }
                        return messages.AsEnumerable();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new IOException("Error reading message history");
            }
        }
    }
}