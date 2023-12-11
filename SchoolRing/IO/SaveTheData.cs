using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using SchoolRing.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace SchoolRing.IO
{
    public class SaveTheData
    {
        //private static string secretKey = "2B7E151628AED2A6";
        internal static string filePath = "models.txt";
        internal static string filePathVacations = "vacations.txt";
        internal static string filePathTimes = "time.txt";
        internal static string filePathProperties = "properties.txt";
        internal static string filePathNotes = "notes.txt";


        public static void SaveSchoolClasses()
        {
            EncryptedDataHandler dataHandler = new EncryptedDataHandler();

            List<ISchoolClass> schoolData = Program.GetModels().ToList();
            dataHandler.SaveEncryptedDataToFile(filePath, schoolData);
        }
        public static void SaveVacation()
        {
            EncryptedDataHandler dataHandler = new EncryptedDataHandler();

            List<IVacationalDays> vdData = Program.vdRepo.GetModels().ToList();
            dataHandler.SaveEncryptedDataToFile(filePathVacations, vdData);
        }
        public static void SaveTimes()
        {
            EncryptedDataHandler dataHandler = new EncryptedDataHandler();
            List<int> timeData = new List<int> { Program.ClassLength, Program.ShortBreakLength, Program.LongBreakLength, Program.LongBreakAfter };
            dataHandler.SaveEncryptedDataToFile(filePathTimes, timeData);
        }
        public static void SaveProperties()
        {
            EncryptedDataHandler dataHandler = new EncryptedDataHandler();

            Dictionary<string, string> properties = new Dictionary<string, string>();
            if (Program.HaveBeenIntoMainMenu)
                properties.Add("HaveBeenIntoMainMenu", "true");
            else
                properties.Add("HaveBeenIntoMainMenu", "false");

            if (Program.WithClassSchedule)
                properties.Add("WithClassSchedule", "true");
            else
                properties.Add("WithClassSchedule", "false");

            if (Program.isMessageShown)
                properties.Add("isMessageShown", "true");
            else
                properties.Add("isMessageShown", "false");

            if (Program.allowRinging)
                properties.Add("allowRinging", "true");
            else
                properties.Add("allowRinging", "false");
            properties.Add("fixedMelodyLength", $"{Program.fixedMelodyLength}");
            properties.Add("melodyForStartOfClassPath", $"{Program.melodyForStartOfClassPath}");
            properties.Add("melodyForEndOfClassPath", $"{Program.melodyForEndOfClassPath}");
            properties.Add("customIconPath", $"{Program.customIconPath}");
            dataHandler.SaveEncryptedDataToFile(filePathProperties, properties);
        }
        public static void SaveNotes()//TODO IMPLEMENT THIS
        {
            EncryptedDataHandler dataHandler = new EncryptedDataHandler();

            List<INote> noteData = Program.noteRepo.GetModels().ToList();
            dataHandler.SaveEncryptedDataToFile(filePathNotes, noteData);
        }
        public static void ReadProperties()
        {
            EncryptedDataHandler dataHandler = new EncryptedDataHandler();
            Dictionary<string, string> properties = dataHandler.ReadAndDecryptDataProperties(filePathProperties);
            if (properties["HaveBeenIntoMainMenu"] == "true")
                Program.HaveBeenIntoMainMenu = true;
            else
                Program.HaveBeenIntoMainMenu = false;
            if (properties["WithClassSchedule"] == "true")
                Program.WithClassSchedule = true;
            else
                Program.WithClassSchedule = false;
            if (properties["isMessageShown"] == "true")
                Program.isMessageShown = true;
            else
                Program.isMessageShown = false;
            if (properties["allowRinging"] == "true")
                Program.allowRinging = true;
            else
                Program.allowRinging = false;
            Program.fixedMelodyLength = int.Parse(properties["fixedMelodyLength"]);
            Program.melodyForStartOfClassPath = properties["melodyForStartOfClassPath"];
            Program.melodyForEndOfClassPath = properties["melodyForEndOfClassPath"];
            Program.customIconPath = properties["customIconPath"];
        }
        public static void ReadSchoolClasses()
        {
            EncryptedDataHandler dataHandler = new EncryptedDataHandler();
            List<SchoolClass> decryptedData = dataHandler.ReadAndDecryptData(filePath);

            foreach (var item in decryptedData.ToList().OrderBy(x => x.Day).ThenBy(x => x.IsPurvaSmqna).ThenBy(x => x.Num))
            {
                if (item.IsMerging)
                {
                    ISchoolClass schoolClass;
                    if (item.Num < 7)
                        schoolClass = decryptedData.First(x => x.Day == item.Day && x.Num == item.Num + 1 && x.IsPurvaSmqna == item.IsPurvaSmqna);
                    else
                        schoolClass = decryptedData.First(x => x.Day == item.Day && x.Num == 1 && x.IsPurvaSmqna == false);
                    item.MergeClassWith(schoolClass);
                }
                Program.AddRecord(item);
            }
        }
        public static void ReadVacations()
        {
            EncryptedDataHandler dataHandler = new EncryptedDataHandler();

            List<VacationalDays> decryptedDataVacations = dataHandler.ReadAndDecryptDataVacation(filePathVacations);
            foreach (var item in decryptedDataVacations)
            {
                Program.vdRepo.AddModel(item);
            }
        }

        public static void ReadTimes()
        {
            EncryptedDataHandler dataHandler = new EncryptedDataHandler();
            List<int> decryptedDataTimes = dataHandler.ReadAndDecryptDataTimes(filePathTimes);
            Program.ClassLength = decryptedDataTimes[0];
            Program.ShortBreakLength = decryptedDataTimes[1];
            Program.LongBreakLength = decryptedDataTimes[2];
            Program.LongBreakAfter = decryptedDataTimes[3];
        }

        public static void ReadNotes()//TODO IMPLEMENT
        {
            EncryptedDataHandler dataHandler = new EncryptedDataHandler();

            List<INote> decryptedDataNotes = dataHandler.ReadAndDecryptDataNotes(filePathNotes);
            foreach (var item in decryptedDataNotes)
            {
                Program.noteRepo.AddModel(item);
            }
        }
    }
    public class EncryptedDataHandler
    {
        private static readonly string encryptionKey = "2B7E151628AED2A6";
        //classes
        public void SaveEncryptedDataToFile(string filePath, List<ISchoolClass> data)
        {
            try
            {
                string serializedData = JsonConvert.SerializeObject(data);
                string encryptedData = EncryptData(serializedData, encryptionKey);
                File.Delete(filePath);
                File.WriteAllText(filePath, encryptedData);
                //System.Windows.Forms.MessageBox.Show($"Encrypted data saved to: {filePath}");
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Грешка при криптиране на данните: {ex.Message}");
            }
        }
        //vacations
        public void SaveEncryptedDataToFile(string filePath, List<IVacationalDays> data)
        {
            try
            {
                string serializedData = JsonConvert.SerializeObject(data);
                string encryptedData = EncryptData(serializedData, encryptionKey);
                File.Delete(filePath);
                File.WriteAllText(filePath, encryptedData);
                //System.Windows.Forms.MessageBox.Show($"Encrypted data saved to: {filePath}");
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Грешка при криптиране на данните: {ex.Message}");
            }
        }
        //vreme za chasove
        public void SaveEncryptedDataToFile(string filePath, List<int> data)
        {
            try
            {
                string serializedData = JsonConvert.SerializeObject(data);
                string encryptedData = EncryptData(serializedData, encryptionKey);
                File.Delete(filePath);
                File.WriteAllText(filePath, encryptedData);
                //System.Windows.Forms.MessageBox.Show($"Encrypted data saved to: {filePath}");
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Грешка при криптиране на данните: {ex.Message}");
            }
        }
        //properties
        public void SaveEncryptedDataToFile(string filePath, Dictionary<string, string> data)
        {
            try
            {
                string serializedData = JsonConvert.SerializeObject(data);
                string encryptedData = EncryptData(serializedData, encryptionKey);
                File.Delete(filePath);
                File.WriteAllText(filePath, encryptedData);
                //System.Windows.Forms.MessageBox.Show($"Encrypted data saved to: {filePath}");
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Грешка при криптиране на данните: {ex.Message}");
            }
        }
        //notes
        public void SaveEncryptedDataToFile(string filePath, List<INote> data)
        {
            try
            {
                string serializedData = JsonConvert.SerializeObject(data);
                string encryptedData = EncryptData(serializedData, encryptionKey);
                File.Delete(filePath);
                File.WriteAllText(filePath, encryptedData);
                //System.Windows.Forms.MessageBox.Show($"Encrypted data saved to: {filePath}");
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Грешка при криптиране на данните: {ex.Message}");
            }
        }
        //classes
        public List<SchoolClass> ReadAndDecryptData(string filePath)
        {
            try
            {
                string encryptedData = File.ReadAllText(filePath);
                string decryptedData = DecryptData(encryptedData, encryptionKey);
                List<SchoolClass> schoolClasses = JsonConvert.DeserializeObject<List<SchoolClass>>(decryptedData);
                return schoolClasses ?? new List<SchoolClass>();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Грешка при четене и декриптиране на данните: {ex.Message}");
                return new List<SchoolClass>();
            }
        }
        //vacation
        internal List<VacationalDays> ReadAndDecryptDataVacation(string filePath)
        {
            try
            {
                string encryptedData = File.ReadAllText(filePath);
                string decryptedData = DecryptData(encryptedData, encryptionKey);
                List<VacationalDays> vacations = JsonConvert.DeserializeObject<List<VacationalDays>>(decryptedData);
                return vacations ?? new List<VacationalDays>();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Грешка при четене и декриптиране на данните: {ex.Message}");
                return new List<VacationalDays>();
            }
        }
        //vreme za chasove
        internal List<int> ReadAndDecryptDataTimes(string filePath)
        {
            try
            {
                string encryptedData = File.ReadAllText(filePath);
                string decryptedData = DecryptData(encryptedData, encryptionKey);
                List<int> vacations = JsonConvert.DeserializeObject<List<int>>(decryptedData);
                return vacations ?? new List<int>();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Грешка при четене и декриптиране на данните: {ex.Message}");
                return new List<int>();
            }
        }
        //Properties
        internal Dictionary<string, string> ReadAndDecryptDataProperties(string filePath)
        {
            try
            {
                string encryptedData = File.ReadAllText(filePath);
                string decryptedData = DecryptData(encryptedData, encryptionKey);
                Dictionary<string, string> vacations = JsonConvert.DeserializeObject<Dictionary<string, string>>(decryptedData);
                return vacations ?? new Dictionary<string, string>();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Грешка при четене и декриптиране на данните: {ex.Message}");
                return new Dictionary<string, string>();
            }
        }
        //classes
        public List<INote> ReadAndDecryptDataNotes(string filePath)
        {
            try
            {
                string encryptedData = File.ReadAllText(filePath);
                string decryptedData = DecryptData(encryptedData, encryptionKey);
                List<INote> schoolClasses = JsonConvert.DeserializeObject<List<INote>>(decryptedData);
                return schoolClasses ?? new List<INote>();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Грешка при четене и декриптиране на данните: {ex.Message}");
                return new List<INote>();
            }
        }
        private string EncryptData(string data, string key)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.IV = new byte[16]; // Use a random IV for simplicity

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(data);
                    }

                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        private string DecryptData(string encryptedData, string key)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.IV = new byte[16]; // Use a random IV for simplicity

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(encryptedData)))
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                {
                    return srDecrypt.ReadToEnd();
                }
            }
        }
    }

}

