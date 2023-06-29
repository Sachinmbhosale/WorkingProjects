using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ionic.Zip;
using System.IO;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Data;


public static class AppConfiguration
{

    /// <summary>
    /// Test that the server is connected
    /// </summary>
    /// <param name="connectionString">The connection string</param>
    /// <returns>true if the connection is opened</returns>
    public static bool CheckDatabaseMSSQL(string connectionString)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (SqlException ex)
            {
                throw ex;
                //return false;
            }
        }
    }

    public static bool CheckDatabaseMYSQL(string connectionString)
    {
        bool isConn = false;
        MySqlConnection conn = null;
        try
        {
            conn = new MySqlConnection(connectionString);
            conn.Open();
            isConn = true;
        }
        catch (ArgumentException a_ex)
        {
            throw a_ex;
            /*
            Console.WriteLine("Check the Connection String.");
            Console.WriteLine(a_ex.Message);
            Console.WriteLine(a_ex.ToString());
            */
        }
        catch (MySqlException ex)
        {
            throw ex;
            /*string sqlErrorMessage = "Message: " + ex.Message + "\n" +
            "Source: " + ex.Source + "\n" +
            "Number: " + ex.Number;
            Console.WriteLine(sqlErrorMessage);
            */
            /*
            isConn = false;
            switch (ex.Number)
            {
                //http://dev.mysql.com/doc/refman/5.0/en/error-messages-server.html
                case 1042: // Unable to connect to any of the specified MySQL hosts (Check Server,Port)
                    break;
                case 0: // Access denied (Check DB name,username,password)
                    break;
                default:
                    break;
            }*/
        }
        finally
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }
        return isConn;
    }
}
