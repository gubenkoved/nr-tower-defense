using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace NRTowerDefense
{
    /// <summary>
    /// Игровой результат. Может быть XML сериализован
    /// </summary>
    [Serializable]
    public class GameRecord
    {
        public string LevelName { get; set; }
        public string PlayerName { get; set; }
        public double Score { get; set; }
        public DateTime Date { get; set; }
        public string Hash { get; set; }

        public GameRecord()
        {
        }

        public void SetData(string playerName, string levelName, double score, DateTime date)
        {
            LevelName = levelName;
            PlayerName = playerName;
            Score = score;
            Date = date;

            Hash = CalculateHash(this);
        }

        static public string CalculateHash(GameRecord record)
        {
            byte[] data = System.Text.Encoding.ASCII.GetBytes(record.LevelName + record.PlayerName + record.Score + record.Date + "secret");

            System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            data = x.ComputeHash(data);

            string hash = "";
            
            for (int i = 0; i < data.Length; i++)
                hash += data[i].ToString("x2").ToLower();

            return hash;
        }

        static public bool CheckHash(GameRecord record, string hash)
        {
            if (CalculateHash(record) == hash)
                return true;
            else
                return false;
        }

    }
}
