using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using WorkflowBLL.Classes;
using WorkflowBAL;

namespace Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin
{
    public static class WorkFlowCommonFns
    {
        public static void SetGridHeaderForMultiLanguage(ref GridView gridViewControl, string vGridType, string vLoginToken, int iLoginOrgId, int iUserLanguageID, int iUserID)
        {
            WorkflowLanguages objLanguages = new WorkflowLanguages();
            DBResult objResult = new DBResult();
            objResult= objLanguages.GetGridHeaders(vLoginToken, iLoginOrgId, vGridType, iUserLanguageID, iUserID);

            if (objResult.dsResult != null && objResult.dsResult.Tables.Count != 0 && objResult.dsResult.Tables[0].Rows.Count != 0)
            {
                try
                {
                    for (int gridColIteration = 0; gridColIteration < gridViewControl.HeaderRow.Cells.Count; gridColIteration++)
                    {
                        for (int i = 0; i < objResult.dsResult.Tables[0].Rows.Count; i++)
                        {
                            if (gridViewControl.HeaderRow.Cells[gridColIteration].Text == objResult.dsResult.Tables[0].Rows[i]["EnglishText"].ToString())
                            {
                                gridViewControl.HeaderRow.Cells[gridColIteration].Text = objResult.dsResult.Tables[0].Rows[i]["LocalText"].ToString();
                                break;
                            }
                        }
                    }
                }
                catch 
                {
                }
            }
        }
    }
}