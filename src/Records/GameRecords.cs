using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Windows;

namespace NRTowerDefense
{
    static public class GameRecordsExtenstion
    {
        static public void Save(this GameRecords gameRecords)
        {
            try
            {
                if (gameRecords.Records != null)
                {
                    using (var writer = new StreamWriter(gameRecords.FileName))
                    {
                        var serialiser = new XmlSerializer(typeof(GameRecords));

                        serialiser.Serialize(writer, gameRecords);

                        writer.Close();
                    }
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("Ошибка при сериализции: " + exc.Message);
            }
        }
    }

    /// <summary>
    /// Таблица игровых результатов
    /// </summary>
    [Serializable]
    public class GameRecords
    {
        public const string DEFAULT_FILE_NAME = "records.nrtd";

        [XmlIgnore]
        public string FileName;

        public List<GameRecord> Records;

        public GameRecords()
        {
            Records = new List<GameRecord>();
        }

        public GameRecords(string sourceFileName)
        {
            FileName = DEFAULT_FILE_NAME;

            Records = tryReadRecordsFromFile(sourceFileName);

            if (Records == null)
                Records = new List<GameRecord>();
        }

        public void AddRecord(GameRecord record, bool saveNow = true)
        {
            Records.Add(record);

            if (saveNow)
                this.Save();
        }

        static private List<GameRecord> tryReadRecordsFromFile(string fileName)
        {           
            try
            {
                GameRecords records;
                List<GameRecord> validRecords = new List<GameRecord>();

                using (var reader = new StreamReader(fileName))
                {
                    var serialiser = new XmlSerializer(typeof(GameRecords));
                    records = (GameRecords)serialiser.Deserialize(reader);
                }

                foreach (GameRecord record in records.Records)
                {
                    if (GameRecord.CheckHash(record, record.Hash))
                    {
                        validRecords.Add(record);
                    }
                    else
                    {
                        MessageBox.Show("Файл рекордов повреждён. Обнаружено нарушение хэш-суммы");
                    }
                }

                return validRecords;
            }
            catch (Exception)
            {
                MessageBox.Show("Файл рекордов повреждён. Ошибка при десериализации");
                return null;
            }
        }
    }
}
