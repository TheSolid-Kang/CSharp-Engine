﻿using Engine._05.CStackTracer;
using MySqlX.XDevAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine._01.DBMgr
{
    public enum EXECUTE_TYPE { GET_DATE_TABLE, EXEUCUTE }
    public class MySQL_Mgr : DbMgr, IDisposable
    {
        public MySQL_Mgr()
          : this(DB_HOST, DB_PORT, DB_USER, DB_PASS, DB_NAME)
        { }
        public MySQL_Mgr(string _name)
      : this(DB_HOST, DB_PORT, DB_USER, DB_PASS, _name)
        { }
        public MySQL_Mgr(string _user, string _pass)
          : this(DB_HOST, DB_PORT, _user, _pass, DB_NAME)
        { }
        public MySQL_Mgr(string _host, string _port, string _user, string _pass, string _name)
        {
            host = _host;
            port = _port;
            user = _user;
            pass = _pass;
            name = _name;
            url = $"DATA SOURCE={host}; PORT={port}; DATABASE={name}; UID={user}; PASSWORD={pass}; CharSet={CHAR_SET}; Allow User Variables=TRUE;";
        }

        ~MySQL_Mgr()
        {
            Dispose();
        }
        private const string DB_HOST = "localhost";
        private const string DB_PORT = "3306";
        private const string DB_USER = "root";
        private const string DB_PASS = "root";
        private const string DB_NAME = "TwoMites";
        private const string CHAR_SET = "UTF8";

        private string host;
        private string port;
        private string user;
        private string pass;
        private string name;

        private string url;

        public object result;
        private DataTable _dataTable = null;
        public DataTable m_dataTable
        {
            get
            {
                var dataTable = _dataTable;
                return dataTable;
            }
            set
            {
                if (_dataTable != null)
                    _dataTable.Dispose();
                _dataTable = value;
            }
        }

        #region 멤버함수 선언부
        protected MySql.Data.MySqlClient.MySqlConnection ConnectDB() => _ConnectDb();
        protected MySql.Data.MySqlClient.MySqlCommand SetCommand(string _query, MySql.Data.MySqlClient.MySqlConnection _connect) => _SetCommand(_query, _connect);
        public void Execute(string _query) => _Execute(_query);
        public DataTable GetDataTable(string _query) => _GetDataTable(_query);
        public bool ExecuteMulQuery(List<string> _listQuery) => _ExecuteMulQuery(_listQuery);
        #endregion

        #region 멤버함수 정의부
        private MySql.Data.MySqlClient.MySqlConnection _ConnectDb() => new MySql.Data.MySqlClient.MySqlConnection(url);
        private MySql.Data.MySqlClient.MySqlCommand _SetCommand(string _query, MySql.Data.MySqlClient.MySqlConnection _connect) => new MySql.Data.MySqlClient.MySqlCommand(_query, _connect);


        #endregion

        #region 쿼리 별 실행 유형 
        private DataTable _GetDataTable(string _query)
        {
            try
            {
                using (var connection = _ConnectDb())
                using (var command = _SetCommand(_query, connection))
                {
                    connection.Open();
                    using (MySql.Data.MySqlClient.MySqlDataAdapter data_adapter = new MySql.Data.MySqlClient.MySqlDataAdapter(command))
                    using (m_dataTable = new DataTable())
                        if (data_adapter.Fill(m_dataTable) > 0)
                            return m_dataTable;
                }
            }
            catch (Exception _e)
            {
                CStackTracer.GetInstance().WriteTraceInfo("DB GetDataTable 에러: " + _e.Message);
                System.Diagnostics.Debug.WriteLine($"예외 == {_e.Message}");
            }
            return null;
        }
        private void _Execute(string _query)
        {
            try
            {
                using (var connection = _ConnectDb())
                using (var command = _SetCommand(_query, connection))
                {
                    connection.Open();
                    if(1 != command.ExecuteNonQuery())
                    {
                        throw new Exception("Execute() 함수 버그");
                    }
                }
            }
            catch (Exception _e)
            {
                CStackTracer.GetInstance().WriteTraceInfo("DB GetDataTable 에러: " + _e.Message);
                System.Diagnostics.Debug.WriteLine($"예외 == {_e.Message}");
            }
        }
        bool _ExecuteMulQuery(List<string> _listQuery)
        {
            try
            {
                _listQuery.ForEach(_query => _GetDataTable(_query));
            }
            catch (Exception _e)
            {

            }

            return true;
        }
        #endregion

        #region DB 백업 및 Restore
        public void BackUpDB(string path)
        {
            try
            {
                using (var connection = _ConnectDb())
                using (var command = new MySql.Data.MySqlClient.MySqlCommand())
                using (var mysql_backup = new MySql.Data.MySqlClient.MySqlBackup(command))
                {
                    connection.Open();
                    command.Connection = connection;
                    mysql_backup.ExportToFile(path);
                    connection.Close();
                }
            }
            catch (Exception _e)
            {
                CStackTracer.GetInstance().WriteTraceInfo("DB Backup 에러: " + _e.Message);
                System.Diagnostics.Debug.WriteLine(_e.Message);
            }
        }

        public void RestoreDB(string path)
        {
            try
            {
                using (var connection = _ConnectDb())
                using (var command = new MySql.Data.MySqlClient.MySqlCommand())
                using (var mysql_backup = new MySql.Data.MySqlClient.MySqlBackup(command))
                {
                    connection.Open();
                    command.Connection = connection;
                    mysql_backup.ImportFromFile(path);
                    connection.Close();
                }
            }
            catch (Exception _e)
            {
                CStackTracer.GetInstance().WriteTraceInfo("DB Restore 에러: " + _e.Message);
                System.Diagnostics.Debug.WriteLine(_e.Message);
            }

        }
        #endregion
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            using (result as DataTable)
            {
                ((DataTable)result)?.Dispose();
            }
            if (m_dataTable?.Rows.Count != 0)
            {
                m_dataTable?.Clear();
                m_dataTable?.Dispose();
            }
        }
    }
}
