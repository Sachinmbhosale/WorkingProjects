﻿using System.Data;

namespace DataAccessLayer
{
    public interface IDBManager
    {
        DataProvider ProviderType
        {
            get;
            set;
        }

        string ConnectionString
        {
            get;
            set;
        }

        IDbConnection Connection
        {
            get;
        }
        IDbTransaction Transaction
        {
            get;
        }

        IDataReader DataReader
        {
            get;
        }
        IDbCommand Command
        {
            get;
        }

        IDbDataParameter[] Parameters
        {
            get;
        }

        void Open();
        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();
        void CreateParameters(int paramsCount);
        void AddParameters(int index, string paramName, object objValue, ParameterDirection paramDirection = ParameterDirection.Input);
        void AddParameters(int index, string paramName, object objValue, DbType dbType, int Size, ParameterDirection paramDirection = ParameterDirection.Input);
        IDataReader ExecuteReader(CommandType commandType, string
        commandText);
        DataSet ExecuteDataSet(CommandType commandType, string
        commandText);
        object ExecuteScalar(CommandType commandType, string commandText);
        int ExecuteNonQuery(CommandType commandType, string commandText);
        object GetOutputParameterValue(string paramName);
        void CloseReader();
        void Close();
        void Dispose();
        string ParseQuery(string query);
    }
}