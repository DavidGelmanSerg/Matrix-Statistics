using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Relational;
using CONFIGURATOR;

namespace DB_MANAGER
{
    public class DB_Manager
    {
        //methods

        //открыть подкючение
        public void OpenConnection(MySql.Data.MySqlClient.MySqlConnection connection, Configurator config)
        {
            if(connection.State.ToString() == "Open") connection.Close();

            string ConnectionString = $"server={config.IP_settings.Replace(" ", "")};uid={config.User_id_Settings}; pwd={config.Password_Settings};database={config.DB_Settings};Connection Timeout=3";
            connection.ConnectionString = ConnectionString;
            try
            {
                connection.Open();
                MessageBox.Show("Подключение успешно!");
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show("Ошибка подключения!\n" + ex.Message);
            }
        }
        //получить информацию о пассажирах
        public DataTable GetPassengersInfo(MySql.Data.MySqlClient.MySqlConnection connection)
        {
            DataTable passengers_table = new DataTable();

            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandType = CommandType.TableDirect;
            cmd.CommandText = "flight_company.Passengers";
            cmd.Connection = connection;

            MySqlDataReader row_set = cmd.ExecuteReader();

            for(int i = 0; i<row_set.FieldCount;i++)
            {
                passengers_table.Columns.Add(row_set.GetName(i));
            }
            while(row_set.Read())
            {
                string[] data_string = new string[row_set.FieldCount];  
                for(int i = 0;i < row_set.FieldCount;i++)
                {
                    data_string[i] = row_set[i].ToString();
                }
                passengers_table.Rows.Add(data_string);
            }
            row_set.Close();
            return passengers_table;
        }

        public DataTable GetFlightsInfo (MySql.Data.MySqlClient.MySqlConnection connection)
        {
            DataTable fligths_table = new DataTable();

            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandType = CommandType.TableDirect;
            cmd.CommandText = "flight_company.flight_schedule";
            cmd.Connection = connection;

            MySqlDataReader row_set = cmd.ExecuteReader();

            for (int i = 0; i < row_set.FieldCount; i++)
            {
                fligths_table.Columns.Add(row_set.GetName(i));
            }
            while (row_set.Read())
            {
                string[] data_string = new string[row_set.FieldCount];
                for (int i = 0; i < row_set.FieldCount; i++)
                {
                    data_string[i] = row_set[i].ToString();
                }
                fligths_table.Rows.Add(data_string);
            }
            row_set.Close();

            return fligths_table;
        }

        public DataTable GetArchiveInfo(MySql.Data.MySqlClient.MySqlConnection connection)
        {
            DataTable archive_table = new DataTable();

            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandType = CommandType.TableDirect;
            cmd.CommandText = "flight_company.Archive";
            cmd.Connection = connection;

            MySqlDataReader row_set = cmd.ExecuteReader();

            for (int i = 0; i < row_set.FieldCount; i++)
            {
                archive_table.Columns.Add(row_set.GetName(i));
            }
            while (row_set.Read())
            {
                string[] data_string = new string[row_set.FieldCount];
                for (int i = 0; i < row_set.FieldCount; i++)
                {
                    data_string[i] = row_set[i].ToString();
                }
                archive_table.Rows.Add(data_string);
            }
            row_set.Close();


            return archive_table;
        }

        public void AddPassenger(string name, string sname, string patr, string passport_number, string start_point, string end_point, string seat, string date_time, MySql.Data.MySqlClient.MySqlConnection connection)
        {
            /*Алгоритм:
             * Проверить, существует ли запись с таким пассажиром:
             * [
             *      ЕСЛИ ДА, то 
             *      {
             *              INSERT данные в промежуточную таблицу
             *              UPDATE количество мест для этого рейса
             *      }
             *      ЕСЛИ НЕТ, то
             *      {
             *              INSERT запись в таблицу пассажиров
             *              INSERT данные в промежуточную таблицу
             *              UPDATE количество мест для этого рейса
             *      }
             * ]
             */
            String addPassenger = $"use flight_company; " +
    $"INSERT INTO Passengers (passport_number, first_name, second_name, patronymic, seat) " +
    $"VALUES ({passport_number}, {name},{sname},{patr},{seat});\r\n " +
    $"INSERT INTO flight_schedule_has_passengers VALUES(" +
            $"(SELECT DISTINCT FLIGHT_NUMBER from flight_schedule where date_time = '{date_time}' and start_point = '{start_point}' and end_point = '{end_point}'), " +
            $"{date_time}, {start_point}, {end_point}, (SELECT COUNT(*) from Passengers));";


            try
            {
                MySqlCommand addPassengerCmd = new MySqlCommand(addPassenger, connection);
                addPassengerCmd.ExecuteNonQuery();
                MessageBox.Show("Пассажир успешно добавлен");
            }
            catch
            {
                MessageBox.Show("Что-то пошло не так");
                MessageBox.Show(addPassenger);
            }

          
        }

        public void UpdatePassenger(string name, string sname, string patr, string passport_number, MySql.Data.MySqlClient.MySqlConnection connection)
        {

        }

        public void DeletePassenger(string name, string sname, string patr, string passport_number, string start_point, string end_point, string date_time, MySql.Data.MySqlClient.MySqlConnection connection)
        {

        }
    }
}
