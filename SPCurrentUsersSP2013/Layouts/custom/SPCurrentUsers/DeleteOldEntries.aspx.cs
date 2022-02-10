using System;
using System.Collections;
using System.Configuration;
using System.Data;
//using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
//using System.Xml.Linq;

using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using System.Text;
using Microsoft.SharePoint.Utilities;
using System.Collections.Generic;

public partial class DeleteOldEntries : Microsoft.SharePoint.WebControls.LayoutsPageBase
{

    protected void Page_PreInit(object sender, EventArgs e)
    {
        Page.MasterPageFile = SPContext.Current.Web.MasterUrl;

    }

    protected void Page_Load(object sender, EventArgs e)
    {
        hlBack.NavigateUrl = SPContext.Current.Site.RootWeb.Url + "/_layouts/SPCurrentUsersAdministration.aspx";
    }


    protected void Delete_Old_Entries_Clicked(object sender, EventArgs e)
    {

        lblOutput.Text = "Running Delete Old Entries Script...<br />";

        //Must at least have full control of a web to be able to use this
        if (SPContext.Current.Web.DoesUserHavePermissions(SPBasePermissions.ManageWeb))
        {
            SPSecurity.RunWithElevatedPrivileges(
                delegate()
                {
                    StringBuilder sbOutput = new StringBuilder();

                    SPWebApplication webApp = SPContext.Current.Site.WebApplication;

                    lblWebAppTitle.Text = webApp.Name;

                    int iTotalCount = 0;
                    int rowlimit = int.Parse(ddlRowLimit.SelectedValue);

                    foreach (SPSite site in webApp.Sites)
                    {
                        try
                        {
                            sbOutput.Append("<hr /><strong style=\"font-size: medium;\">Checking site: " + site.Url + "</strong><br />");
                            using (SPWeb web = site.OpenWeb(site.RootWeb.ID))
                            {

                                SPList checkForList = null;
                                try
                                {
                                    checkForList = web.Lists["SPCurrentUsers User Tracker"];

                                }
                                catch (Exception ex)
                                {
                                    sbOutput.Append("<div style=\"font-size: xx-small;\">Current user tracking list was not found in the site. <br /> You can <a href=\"" + site.RootWeb.Url + "/_layouts/ManageFeatures.aspx?Scope=Site\" target=\"_blank\">activate the SPCurrentUsers Setup Feature</a> to start tracking usage data. <br /> </div>");//Error Message: " + ex.Message + "</div><br />");
                                    continue;
                                }
                                try
                                {
                                    if (checkForList != null)
                                    {

                                        sbOutput.Append("There were: " + checkForList.Items.Count + " total records in the SPCurrentUserTracker list.<br />");

                                        if (checkForList.Items.Count > rowlimit)
                                        {
                                            sbOutput.Append("NOTE: To optimize this web page's response time only up to "+rowlimit+" entries will be deleted per run.<br />");
                                        }


                                        string strnumYears = ddlNumYears.SelectedValue;

                                        int iNumYears = int.Parse(strnumYears);

                                        if (iNumYears >= 1)
                                        {
                                            DateTime dtOlderThan = DateTime.Now.AddYears(-1 * iNumYears);
                                            SPQuery query = new SPQuery();
                                            query.RowLimit = uint.Parse(rowlimit.ToString());
                                            
                                            query.Query = "<Where><Lt><FieldRef Name='Modified' Type='DateTime' IncludeTimeValue='FALSE' />"+
                                                "<Value IncludeTimeValue='FALSE' Type='DateTime'><Today OffsetDays='"+-1*iNumYears*365+"' /></Value></Lt></Where><OrderBy><FieldRef Name='ID' /></OrderBy>";//" + SPUtility.CreateISO8601DateTimeFromSystemDateTime(dtOlderThan) + "

                                            

                                          //  sbOutput.Append(HttpUtility.HtmlEncode(query.Query) + "<br />");

                                            SPListItemCollection itemsToDelete = checkForList.GetItems(query);

                                            if (itemsToDelete.Count > 0)
                                            {
                                                sbOutput.Append("Items to delete: " + itemsToDelete.Count + "<br />");
                                                List<int> idlist = new List<int>();
                                                foreach (SPListItem item in itemsToDelete)
                                                {
                                                    idlist.Add(item.ID);
                                                    //sbOutput.Append("Modified: " + item["Modified"].ToString() + "<br />");
                                                }

                                                Boolean allowunsafe = web.AllowUnsafeUpdates;
                                                web.AllowUnsafeUpdates = true;
                                                foreach (int i in idlist)
                                                {

                                                    checkForList.GetItemById(i).Delete();

                                                }
                                                web.AllowUnsafeUpdates = allowunsafe;

                                                sbOutput.Append("<div style='color: green'>"+idlist.Count + " old records were deleted successfully.</div>");
                                            }
                                            else
                                            {
                                                sbOutput.Append("<div style='color: green;'>No items to delete.  DB is clean.</div>");
                                            }
                                        }




                                    }
                                }
                                catch (Exception ex)
                                {
                                    sbOutput.Append("<div style=\"font-size: xx-small;\">Error occurred deleting old records: " + ex.Message + "</div><br />");
                                }

                            }
                        }
                        catch (Exception ex)
                        {
                            sbOutput.Append("Error occurred: " + ex.ToString() + "<br />");
                        }
                    }


                    

                    lblOutput.Text = sbOutput.ToString();
                });
        } // End if user permissions
        else
        {
            lblOutput.Text = "You do not have enough permissions to run this script.";
        }
    }
}
