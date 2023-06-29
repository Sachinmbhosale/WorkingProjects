﻿//Author      : Yogeesha naik
//Created on  : 23 May 2012

using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Web.UI.WebControls;
using System.IO;


public static class DisposeObjects
    {

        public static void DisposeObjList(params object[] anArray)
        {
            foreach (object obj in anArray)
            {
                if (obj is DataSet)
                {
                    DataSet ds = (DataSet)obj;
                    if (ds != null)
                    {
                        ds.Dispose();
                        ds = null;
                    }
                }
                else if (obj is DataTable)
                {
                    DataTable dt = (DataTable)obj;
                    if (dt != null)
                    {
                        dt.Dispose();
                        dt = null;
                    }
                }

                else if (obj is SqlCommand)
                {
                    SqlCommand sqlcmd = (SqlCommand)obj;
                    if (sqlcmd != null)
                    {
                        sqlcmd.Dispose();
                        sqlcmd = null;
                    }
                }
                else if (obj is SqlDataAdapter)
                {
                    SqlDataAdapter sqlda = (SqlDataAdapter)obj;
                    if (sqlda != null)
                    {
                        sqlda.Dispose();
                        sqlda = null;
                    }
                }
                else if (obj is SqlDataReader)
                {
                    SqlDataReader sqldr = (SqlDataReader)obj;
                    if (sqldr != null)
                    {
                        sqldr.Dispose();
                        sqldr = null;
                    }
                }

                else if (obj is OleDbCommand)
                {
                    OleDbCommand oledbcmd = (OleDbCommand)obj;
                    if (oledbcmd != null)
                    {
                        oledbcmd.Dispose();
                        oledbcmd = null;
                    }
                }
                else if (obj is OleDbDataAdapter)
                {
                    OleDbDataAdapter oledbda = (OleDbDataAdapter)obj;
                    if (oledbda != null)
                    {
                        oledbda.Dispose();
                        oledbda = null;
                    }
                }
                else if (obj is OleDbDataReader)
                {
                    OleDbDataReader oledbdr = (OleDbDataReader)obj;
                    if (oledbdr != null)
                    {
                        oledbdr.Dispose();
                        oledbdr = null;
                    }
                }
                else if (obj is ListItem)
                {
                    ListItem lstitem = (ListItem)obj;
                    if (lstitem != null)
                    {
                        lstitem = null;
                    }
                }
                else if (obj is StreamWriter)
                {
                    StreamWriter sw = (StreamWriter)obj;
                    if (sw != null)
                    {
                        sw.Flush();
                        sw.Close();
                        sw.Dispose();
                        GC.Collect();
                        sw = null;
                    }
                }
                else
                {
                }
            }
        }



        public static void ObjDispose(SqlCommand cmd)
        {
            if (cmd != null)
            {
                cmd.Dispose();
                cmd = null;
            }
        }

        public static void ObjDispose(DataSet ds)
        {
            if (ds != null)
            {
                ds.Dispose();
                ds = null;
            }
        }

        public static void ObjDispose(SqlDataAdapter da)
        {
            if (da != null)
            {
                da.Dispose();
            }
        }

        public static void ObjDispose(SqlDataReader dr)
        {
            if (dr != null)
            {
                dr.Dispose();
            }
        }

        public static void ObjDispose(DataTable dt)
        {
            if (dt != null)
            {
                dt.Dispose();
            }
        }



        //oledbObjects

        public static void ObjDispose(OleDbCommand cmd)
        {
            if (cmd != null)
            {
                cmd.Dispose();
            }
        }

        public static void ObjDispose(OleDbDataAdapter da)
        {
            if (da != null)
            {
                da.Dispose();
            }
        }

        public static void ObjDispose(OleDbDataReader dr)
        {
            if (dr != null)
            {
                dr.Dispose();
            }
        }

        public static void ObjDispose(ListItem lstitem)
        {
            if (lstitem != null)
            {
                lstitem = null;
            }
        }
    }

