using System;
using System.Data;
using System.Web.UI.WebControls;

namespace Lotex.Common
{
    public static class DropdownBinder
    {

        #region common functions

        public static bool BindDropDown(DropDownList control, DataSet dataSource, string dataValueField, string dataTextField)
        {
            if (dataSource.Tables.Count.Equals(0))
                throw new Exception("Data source is empty.");

            DataTable dataTable = dataSource.Tables[0];
            //DataTable dataTable = new DataTable();

            return BindDropDown(control, dataTable, dataValueField, dataTextField);
        }

        public static bool BindDropDown(DropDownList control, DataTable dataSource, string dataValueField, string dataTextField)
        {
            if (dataSource.Rows.Count.Equals(0))
                throw new Exception("Data source is empty.");

            bool status = false;

            control.DataSource = dataSource;
            control.DataValueField = dataValueField;
            control.DataTextField = dataTextField;
            control.DataBind();

            return status;
        }



        #endregion
    }
}

