using System;
using System.Data;
using Lotex.EnterpriseSolutions.CoreDAL;

namespace Lotex.EnterpriseSolutions.CoreBL
{
    public class DynamicControlBL
    {
       public DataSet DynamicPopulateDropdown(int Templated, string action)
       {
           DynamicControlsDAL objDynamicControlsDAL = new DynamicControlsDAL();
           DataSet dsDetails = new DataSet();
           try
           {
               dsDetails = objDynamicControlsDAL.DynamicPopulateDropdown(Templated, action);
           }
           catch (Exception)
           {

               throw;
           }
           return dsDetails;
       }

       public DataSet DynamicLoadDropdownBasedOnValue(int Templated)
       {
           DynamicControlsDAL objDynamicControlsDAL = new DynamicControlsDAL();
           DataSet dsDetails = new DataSet();
           try
           {
               dsDetails = objDynamicControlsDAL.DynamicLoadDropdownBasedOnValue(Templated);
           }
           catch (Exception)
           {
               
               throw;
           }
           return dsDetails;
       }
    }
}
