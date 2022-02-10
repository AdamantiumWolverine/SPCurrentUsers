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

public partial class WebAppStats : Microsoft.SharePoint.WebControls.LayoutsPageBase
{

    protected void Page_PreInit(object sender, EventArgs e)
    {
        Page.MasterPageFile = SPContext.Current.Web.MasterUrl;

    }

    protected void Page_Load(object sender, EventArgs e)
    {
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

                                sbOutput.Append("<a href=\"" + checkForList.DefaultViewUrl + "\">SPCurrentUsers User Tracker List</a> | <a href=\""+site.RootWeb.Url+"/_layouts/SPCurrentUsersAdministration.aspx\">Site SPCurrentUser Page</a> <br />");


                                SPListItemCollection totalUsers = SPCurrentUsers.SPCurrentUsersHelper.GetAllAnonymousAndLoggedInCurrentUsers(web);

                                sbOutput.Append("<div style=\"font-size: small;\">"+totalUsers.Count + " users using site " + site.Url + "</div>");

                                iTotalCount += totalUsers.Count;


                            }
                            catch (Exception ex)
                            {
                                sbOutput.Append("<div style=\"font-size: xx-small;\">Current user tracking list was not found in the site. <br /> You can <a href=\"" + site.RootWeb.Url + "/_layouts/ManageFeatures.aspx?Scope=Site\" target=\"_blank\">activate the SPCurrentUsers Setup Feature</a> to start tracking usage data. <br /> Error Message: " + ex.Message + "</div><br />");
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        sbOutput.Append("Error occurred: " + ex.ToString() + "<br />");
                    }
                }


                sbOutput.Append("<hr /><div style=\"margin: 10px; padding: 10px; font-size: x-large;\"><strong>Total Users In This Web Application: </strong>" + iTotalCount.ToString() + "</div>");

                lblOutput.Text = sbOutput.ToString();
            });
        } // End if user permissions
    }
}
