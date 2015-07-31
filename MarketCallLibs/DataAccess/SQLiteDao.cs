using MarketCallLibs.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;

namespace MarketCallLibs.DataAccess
{
    public class SQLiteDao
    {
        private readonly string DB_FILE = "db.sqlite";
        //private readonly SQLiteConnection dbConnection;
    
        private SQLiteConnection GetConnection()
        {
            return new SQLiteConnection("Data Source=" + DB_FILE + ";Version=3;");
        }

        public SQLiteDao()
        {
            if (!File.Exists(DB_FILE))
                CreateSkeletonDb();

        }

        public void Insert(IEnumerable<Opinion> opinions)
        {
            var connection = GetConnection();
            connection.Open();

            foreach (var o in opinions)
            {
                StringBuilder opinionSql = new StringBuilder();
                opinionSql.Append("insert into opinion (id, date, signal, company, expert, opinion, price, symbol) values (");
                opinionSql.Append(o.GetHashCode() + ",");
                opinionSql.Append(o.Date.Ticks + ",");
                opinionSql.Append("'" + o.Signal.Replace("'","") + "',");
                opinionSql.Append("'" + o.Company.Replace("'", "") + "',");
                opinionSql.Append("'" + o.Expert.Replace("'", "") + "',");
                opinionSql.Append("'" + o.OpinionString.Replace("'", "") + "',");
                opinionSql.Append(o.Price + ",");
                opinionSql.Append("'" + o.Symbol.Replace("'", "") + "'");
                opinionSql.Append(")");
                
                SQLiteCommand command = new SQLiteCommand(opinionSql.ToString(), connection);
                try
                {
                    command.ExecuteNonQuery();
                }
                catch { }
            }

            connection.Close();
        }

        public DateTime GetLatestOpinionDate()
        {
            var connection = GetConnection();
            connection.Open();

            DateTime dt;
            try {
                string sql = String.Format("select max(date) as date from opinion");

                SQLiteCommand command = new SQLiteCommand(sql, connection);
                SQLiteDataReader reader = command.ExecuteReader();

                reader.Read();
                dt = new DateTime().AddTicks((long)reader["date"]);
            } catch
            {
                dt = new DateTime();
            }
            connection.Close();
            
            return dt;
        }

        public IEnumerable<Opinion> FindOpinionsByExpert(string expert)
        {
            var connection = GetConnection();
            connection.Open();

            string sql = String.Format("select * from opinion where expert like '%{0}%' order by date desc", expert);
            SQLiteCommand command = new SQLiteCommand(sql, connection);
            SQLiteDataReader reader = command.ExecuteReader();
            //date, signal, company, expert, opinion, price, symbol
            var opinions = new List<Opinion>();

            while (reader.Read())
            {
                var dateTime = new DateTime().AddTicks((long)reader["date"]);
                var o = new Opinion(dateTime,
                    reader["signal"].ToString(),
                    reader["company"].ToString(),
                    reader["expert"].ToString(),
                    reader["opinion"].ToString(),
                    decimal.Parse(reader["price"].ToString()),
                    reader["symbol"].ToString()
                    );
                opinions.Add(o);
            }

            connection.Close();

            return NameHelpers.RemoveWeakNameMatches(opinions, expert);
        }

        private void CreateSkeletonDb()
        {
            SQLiteConnection.CreateFile(DB_FILE);

            var connection = GetConnection();
            connection.Open();

            StringBuilder opinionSql = new StringBuilder();
            opinionSql.Append("create table opinion (");
            opinionSql.Append("id bigint primary key unique");
            opinionSql.Append(", date bigint");
            opinionSql.Append(", signal varchar(50)");
            opinionSql.Append(", company varchar(100)");
            opinionSql.Append(", expert varchar(75)");
            opinionSql.Append(", opinion varchar(256)");
            opinionSql.Append(", price float");
            opinionSql.Append(", symbol varchar(10)");
            opinionSql.Append(")");

            SQLiteCommand command = new SQLiteCommand(opinionSql.ToString(), connection);
            command.ExecuteNonQuery();

            connection.Close();
        }
    }
}
