////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	FeatureCode\SPCurrentUsersHelper.cs
//
// summary:	Implements the sp current users helper class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;

namespace SPCurrentUsers
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>  SPCurrentUsersHelper includes the helper functions used by SPCurrentUsers. </summary>
    ///
    /// <remarks>   William.chung, 9/15/2016. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class SPCurrentUsersHelper
    {

      
   /*     public static Int32 GetNumberOfCurrentUsers_SupportPageTimeouts()
        {
            try
            {
                // Grad the current web...
                SPWeb web = SPContext.Current.Web.Site.RootWeb;
                // ...and the pages list
                SPList pagesList = web.Lists["SPCurrentUsers URLs"];
                int returnValue = 0;
                // Get all pages for iteration...
                SPListItemCollection pages = pagesList.Items;
                foreach (SPListItem page in pages)
                {
                    // ...add all Pages' current users
                    returnValue +=
                        GetNumberOfCurrentUsersOnPage(web,page["Title"].ToString());
                }
                // ...and send the total back.
                return returnValue;
            }
            catch
            {

            }
            return -1;
        }

        
        public static Int32 GetNumberOfCurrentAnonymousUsers_SupportPageTimeOuts()
        {
            try
            {
                // Grad the current web...
                SPWeb web = SPContext.Current.Web.Site.RootWeb;
                // ...and the pages list
                SPList pagesList = web.Lists["SPCurrentUsers URLs"];
                int returnValue = 0;
                // Get all pages for iteration...
                SPListItemCollection pages = pagesList.Items;
                foreach (SPListItem page in pages)
                {
                    // ...add all Pages' current users
                    returnValue +=
                        GetNumberOfAnonymousUsersOnPage(web,page["Title"].ToString());
                }
                // ...and send the total back.
                return returnValue;
            }
            catch
            {

            }
            return -1;
        }
      */

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets number of current users on page. </summary>
        ///
        /// <remarks>   William.chung, 9/15/2016. </remarks>
        ///
        /// <param name="web">  The web. </param>
        /// <param name="URL">  URL of the document. </param>
        ///
        /// <returns>   The number of current users on page. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static Int32 GetNumberOfCurrentUsersOnPage(SPWeb web, string URL)
        {
            return _getNumberOfUsers(web, URL);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets number of anonymous users on page. </summary>
        ///
        /// <remarks>   William.chung, 9/15/2016. </remarks>
        ///
        /// <param name="web">  The web. </param>
        /// <param name="URL">  URL of the document. </param>
        ///
        /// <returns>   The number of anonymous users on page. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static Int32 GetNumberOfAnonymousUsersOnPage(SPWeb web, string URL)
        {
            return _getNumberOfAnonymousUsers(web,URL);
        }


        //This version of the code just uses the default session timeout rather than bother querying so often for all the different time outs.
        // There are major performance issues with doing this the other way especially when you have a lot of pages in a site collection.

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets number of current users. </summary>
        ///
        /// <remarks>   William.chung, 9/15/2016. </remarks>
        ///
        /// <param name="web">  The web. </param>
        ///
        /// <returns>   The number of current users. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static Int32 GetNumberOfCurrentUsers(SPWeb web)
        {
            try
            {
                //SPWeb web = SPContext.Current.Web.Site.RootWeb;
                SPQuery userQuery = new SPQuery();
              

                int returnValue = 0;

                SPList userList = web.Lists["SPCurrentUsers User Tracker"];//web.SiteUserInfoList;



                double pageSessionLength = GetPageSessionTimeout(web);

                // Convert the date 
                string sessionTime =
                    SPUtility.CreateISO8601DateTimeFromSystemDateTime(
                     DateTime.Now.AddMinutes(-pageSessionLength)
                    );

                // Build our query for finding current users. 
                // Note the use of IncludeTimeValue="True" to check against time
                userQuery.Query = @"
<Where>   
        <And>   
            <And>
            <IsNotNull>
                <FieldRef Name=""UserName"" />
            </IsNotNull>
            <Neq>
                <FieldRef Name=""UserName"" />
                <Value Type=""Text""></Value>
            </Neq>
            </And>
            <Gt>
                <FieldRef Name=""LastPageHitTime"" />
                <Value IncludeTimeValue=""TRUE"" 
                 Type=""DateTime"">" + sessionTime + @"</Value>
            </Gt>
       
            </And>
         
</Where>";

                userQuery.ViewFields = "<FieldRef Name=\"UserName\" />";
                //userQuery.ViewFieldsOnly = true;

                //To further optimize, get only the ID.

                // Get the users that have logged in since now minus session length
                SPListItemCollection users = userList.GetItems(userQuery);
                int userCount = users.Count;
                returnValue = userCount;

                // Return the result
                return returnValue;
            }
            catch (Exception ex) { }
            return -1;
        }



        //This version of the code just uses the default session timeout rather than bother querying so often for all the different time outs.
        // There are major performance issues with doing this the other way especially when you have a lot of pages in a site collection.

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets current users. </summary>
        ///
        /// <remarks>   William.chung, 9/15/2016. </remarks>
        ///
        /// <param name="web">  The web. </param>
        ///
        /// <returns>   The current users. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static SPListItemCollection GetCurrentUsers(SPWeb web)
        {
            try
            {
                //SPWeb web = SPContext.Current.Web.Site.RootWeb;
                SPQuery userQuery = new SPQuery();

                int returnValue = 0;

                SPList userList = web.Lists["SPCurrentUsers User Tracker"];//web.SiteUserInfoList;



                double pageSessionLength = GetPageSessionTimeout(web);

                // Convert the date 
                string sessionTime =
                    SPUtility.CreateISO8601DateTimeFromSystemDateTime(
                     DateTime.Now.AddMinutes(-pageSessionLength)
                    );

                // Build our query for finding current users. 
                // Note the use of IncludeTimeValue="True" to check against time
                userQuery.Query = @"
<OrderBy><FieldRef Name=""LastPageHitUrl"" /></OrderBy>
<Where>   
        <And>
        <Gt>
            <FieldRef Name=""LastPageHitTime"" />
            <Value IncludeTimeValue=""TRUE"" 
             Type=""DateTime"">" + sessionTime + @"</Value>
        </Gt>
        <And>
        <IsNotNull>
            <FieldRef Name=""UserName"" />
        </IsNotNull>
        <Neq>
            <FieldRef Name=""UserName"" />
            <Value Type=""Text""></Value>
        </Neq>
        </And>
        </And>
     
</Where>";

//To further optimize, get only the necessary fields.
                userQuery.ViewFields = "<FieldRef Name=\"ID\" /><FieldRef Name=\"Title\" /><FieldRef Name=\"UserName\" /><FieldRef Name=\"LastPageHitTime\" /><FieldRef Name=\"LastPageHitUrl\" />";
               // userQuery.ViewFieldsOnly = true;
                

                // Get the users that have logged in since now minus session length
                SPListItemCollection users = userList.GetItems(userQuery);
                
                return users;
            }
            catch { }
            return null;
        }


        //This version of the code just uses the default session timeout rather than bother querying so often for all the different time outs.
        // There are major performance issues with doing this the other way especially when you have a lot of pages in a site collection.

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets all anonymous and logged in current users. </summary>
        ///
        /// <remarks>   William.chung, 9/15/2016. </remarks>
        ///
        /// <param name="web">  The web. </param>
        ///
        /// <returns>   all anonymous and logged in current users. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static SPListItemCollection GetAllAnonymousAndLoggedInCurrentUsers(SPWeb web)
        {
            try
            {
                //SPWeb web = SPContext.Current.Web.Site.RootWeb;
                SPQuery userQuery = new SPQuery();

                int returnValue = 0;

                SPList userList = web.Lists["SPCurrentUsers User Tracker"];//web.SiteUserInfoList;



                double pageSessionLength = GetPageSessionTimeout(web);

                // Convert the date 
                string sessionTime =
                    SPUtility.CreateISO8601DateTimeFromSystemDateTime(
                     DateTime.Now.AddMinutes(-pageSessionLength)
                    );

                // Build our query for finding current users. 
                // Note the use of IncludeTimeValue="True" to check against time
                userQuery.Query = @"
<OrderBy><FieldRef Name=""LastPageHitUrl"" /></OrderBy>
<Where>   
       
        <Gt>
            <FieldRef Name=""LastPageHitTime"" />
            <Value IncludeTimeValue=""TRUE"" 
             Type=""DateTime"">" + sessionTime + @"</Value>
        </Gt>        
     
</Where>";

                //To further optimize, get only the necessary fields.
                userQuery.ViewFields = "<FieldRef Name=\"ID\" /><FieldRef Name=\"Title\" /><FieldRef Name=\"UserName\" /><FieldRef Name=\"LastPageHitTime\" /><FieldRef Name=\"LastPageHitUrl\" />";
                // userQuery.ViewFieldsOnly = true;


                // Get the users that have logged in since now minus session length
                SPListItemCollection users = userList.GetItems(userQuery);

                return users;
            }
            catch { }
            return null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets number of users. </summary>
        ///
        /// <remarks>   William.chung, 9/15/2016. </remarks>
        ///
        /// <param name="web">  The web. </param>
        /// <param name="URL">  URL of the document. </param>
        ///
        /// <returns>   The number of users. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private static int _getNumberOfUsers(SPWeb web, string URL)
        {
            try
            {
                //SPWeb web = SPContext.Current.Web.Site.RootWeb;
                SPQuery userQuery = new SPQuery();
                

                
                int returnValue = 0;

                SPList userList = web.Lists["SPCurrentUsers User Tracker"]; //web.SiteUserInfoList;



                double pageSessionLength = GetPageSessionTimeout(web,URL);


                

                // Convert the date 
                string sessionTime =
                    SPUtility.CreateISO8601DateTimeFromSystemDateTime(
                     DateTime.Now.AddMinutes(-pageSessionLength)
                    );

                // Build our query for finding current users. 
                // Note the use of IncludeTimeValue="True" to check against time
                userQuery.Query = @"
<Where>   
        <And>
        <Gt>
            <FieldRef Name=""LastPageHitTime"" />
            <Value IncludeTimeValue=""TRUE"" 
             Type=""DateTime"">" + sessionTime + @"</Value>
        </Gt>
        <And>
        <IsNotNull>
            <FieldRef Name=""UserName"" />
        </IsNotNull>
        <Neq>
            <FieldRef Name=""UserName"" />
            <Value Type=""Text""></Value>
        </Neq>
        </And>
        </And>
     
</Where>";
                userQuery.ViewFields = "<FieldRef Name=\"UserName\" />";
                //userQuery.ViewFieldsOnly = true;
                // Get the users that have logged in since now minus session length
                SPListItemCollection users = userList.GetItems(userQuery);
                int userCount = users.Count;
                returnValue += userCount;

                // Return the result
                return returnValue;
            }
            catch { }
            return -1;
        }


        //This one does not support individual page session time outs.
        //The performance penalty of running a query for each page in a site collection and
        //individually calculating the current sessions is unacceptable.

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets number of current anonymous users. </summary>
        ///
        /// <remarks>   William.chung, 9/15/2016. </remarks>
        ///
        /// <param name="web">  The web. </param>
        ///
        /// <returns>   The number of current anonymous users. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static Int32 GetNumberOfCurrentAnonymousUsers(SPWeb web)
        {
             try
            {
                //SPWeb web = SPContext.Current.Web.Site.RootWeb;
                SPQuery userQuery = new SPQuery();
                
                int returnValue = 0;
                
                SPList userList = web.Lists["SPCurrentUsers User Tracker"];

                if (userList == null )
                {
                    return -1;
                }

             

                double pageSessionLength = GetPageSessionTimeout(web);


                // Convert the date 
                string sessionTime =
                    SPUtility.CreateISO8601DateTimeFromSystemDateTime(
                     DateTime.Now.AddMinutes(-pageSessionLength)
                    );

                // Build our query for finding current users. 
                // Note the use of IncludeTimeValue="True" to check against time
                userQuery.Query = @"
<Where>   
        <And>
        <Gt>
            <FieldRef Name=""LastPageHitTime"" />
            <Value IncludeTimeValue=""TRUE"" 
             Type=""DateTime"">" + sessionTime + @"</Value>
        </Gt>
        <Or>
        <IsNull>
            <FieldRef Name=""UserName"" />
        </IsNull>
        <Eq>
            <FieldRef Name=""UserName"" />
            <Value Type=""Text""></Value>
        </Eq>
        </Or>
        </And>
     
</Where>";

                userQuery.ViewFields = "<FieldRef Name=\"Title\" />";
                //userQuery.ViewFieldsOnly = true;

                // Get the users that have logged in since now minus session length
                SPListItemCollection users = userList.GetItems(userQuery);
                int userCount = users.Count;
                returnValue = userCount;

                // Return the result
                return returnValue;
            }
            catch (Exception Exception)
             { }
            return -1;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets current anonymous users. </summary>
        ///
        /// <remarks>   William.chung, 9/15/2016. </remarks>
        ///
        /// <param name="web">  The web. </param>
        ///
        /// <returns>   The current anonymous users. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static SPListItemCollection GetCurrentAnonymousUsers(SPWeb web)
        {
            try
            {
                //SPWeb web = SPContext.Current.Web.Site.RootWeb;
                SPQuery userQuery = new SPQuery();

                int returnValue = 0;

                SPList userList = web.Lists["SPCurrentUsers User Tracker"];

                if (userList == null)
                {
                    return null;
                }



                double pageSessionLength = GetPageSessionTimeout(web);


                // Convert the date 
                string sessionTime =
                    SPUtility.CreateISO8601DateTimeFromSystemDateTime(
                     DateTime.Now.AddMinutes(-pageSessionLength)
                    );

                // Build our query for finding current users. 
                // Note the use of IncludeTimeValue="True" to check against time
                userQuery.Query = @"
<OrderBy><FieldRef Name=""LastPageHitUrl"" /></OrderBy>
<Where>   
        <And>       
            <Or>
            <IsNull>
                <FieldRef Name=""UserName"" />
            </IsNull>
            <Eq>
                <FieldRef Name=""UserName"" />
                <Value Type=""Text""></Value>
            </Eq>
            </Or>
        <Gt>
            <FieldRef Name=""LastPageHitTime"" />
            <Value IncludeTimeValue=""TRUE"" 
             Type=""DateTime"">" + sessionTime + @"</Value>
        </Gt>
        </And>
     
</Where>

";

                userQuery.ViewFields = "<FieldRef Name=\"Title\" /><FieldRef Name=\"LastPageHitTime\" /><FieldRef Name=\"LastPageHitUrl\" />";
                //userQuery.ViewFieldsOnly = true;
                // Get the users that have logged in since now minus session length
                SPListItemCollection users = userList.GetItems(userQuery);
                
                // Return the result
                return users;
            }
            catch { }
            return null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets page session timeout. </summary>
        ///
        /// <remarks>   William.chung, 9/15/2016. </remarks>
        ///
        /// <param name="web">  The web. </param>
        ///
        /// <returns>   The page session timeout. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static double GetPageSessionTimeout(SPWeb web)
        {
            return GetPageSessionTimeout(web,"");
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets page session timeout. </summary>
        ///
        /// <remarks>   William.chung, 9/15/2016. </remarks>
        ///
        /// <param name="web">  The web. </param>
        /// <param name="URL">  URL of the document. </param>
        ///
        /// <returns>   The page session timeout. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static double GetPageSessionTimeout(SPWeb web, string URL)
        {
            double defaultSessionLength = 0;
            try
            {
                defaultSessionLength = double.Parse(
                    web.Properties["SPCurrentUsersDefaultSessionDuration"]);
            }
            catch (Exception)
            {
                // If we can't read property, use default value
                defaultSessionLength = 15.0;
            }
            return defaultSessionLength;

            /*
            double pageSessionLength = 0.0;

            try
            {
                SPQuery pageQuery = new SPQuery();
                SPList pagesList = web.Lists["SPCurrentUsers URLs"];
            }
            catch (Exception ex)
            {
                pageSessionLength = 0;
            }

            // Build the CAML query for finding page timeout
            pageQuery.Query = @"
    <Where>
        <Eq>
            <FieldRef Name=""Title"" />
            <Value Type=""Text"">" + URL + @"</Value>
        </Eq>
    </Where>";



            // And retrieve the results
            SPListItemCollection pageItems = pagesList.GetItems(pageQuery);
            if (pageItems.Count == 1)
            {
                // The page is in our Pages list
                SPListItem page = pageItems[0];
                double.TryParse(page["Page Session Timeout"].ToString(), out pageSessionLength);
            }
            else
            {
                // Page not found
                // sesionLength will remain 0
                // and no results will be returned
            }


            return pageSessionLength;
             */

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets number of anonymous users. </summary>
        ///
        /// <remarks>   William.chung, 9/15/2016. </remarks>
        ///
        /// <param name="web">  The web. </param>
        /// <param name="URL">  URL of the document. </param>
        ///
        /// <returns>   The number of anonymous users. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private static int _getNumberOfAnonymousUsers(SPWeb web, string URL)
        {

            try
            {
                //SPWeb web = SPContext.Current.Web.Site.RootWeb;
                SPQuery userQuery = new SPQuery();
               
                int returnValue = 0;
               
                SPList userList = web.Lists["SPCurrentUsers User Tracker"];

                if (userList == null )
                {
                    return -1;
                }

                double pageSessionLength = GetPageSessionTimeout(web,URL);

               
                // Convert the date 
                string sessionTime =
                    SPUtility.CreateISO8601DateTimeFromSystemDateTime(
                     DateTime.Now.AddMinutes(-pageSessionLength)
                    );

                // Build our query for finding current users. 
                // Note the use of IncludeTimeValue="True" to check against time
                userQuery.Query = @"
<Where>   
        <And>
        <Gt>
            <FieldRef Name=""LastPageHitTime"" />
            <Value IncludeTimeValue=""TRUE"" 
             Type=""DateTime"">" + sessionTime + @"</Value>
        </Gt>
        <Or>
        <IsNull>
            <FieldRef Name=""UserName"" />
        </IsNull>
        <Eq>
            <FieldRef Name=""UserName"" />
            <Value Type=""Text""></Value>
        </Eq>
        </Or>
        </And>
     
</Where>";
                userQuery.ViewFields = "<FieldRef Name=\"Title\" />";
               // userQuery.ViewFieldsOnly = true;

                // Get the users that have logged in since now minus session length
                SPListItemCollection users = userList.GetItems(userQuery);
                int userCount = users.Count;
                returnValue += userCount;

                // Return the result
                return returnValue;
            }
            catch { }
            return -1;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Checks if a field is in a view already. </summary>
        ///
        /// <remarks>   William.chung, 9/15/2016. </remarks>
        ///
        /// <param name="view">         . </param>
        /// <param name="strFieldName"> . </param>
        ///
        /// <returns>   true if field in view, false if not. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static Boolean IsFieldInView(SPView view, string strFieldName)
        {

            try
            {

                SPField myField = null;
                foreach (SPField field in view.ParentList.Fields)
                {
                    if (field.StaticName == strFieldName || field.Title == strFieldName)
                    {
                        myField = field;
                        break;
                    }

                }

                if (myField != null)
                {

                    foreach (string strField in view.ViewFields)
                    {

                        if (myField.Title == strField || myField.StaticName == strField)
                        {
                            return true;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
            }

                return false;
            

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets display current users in site actions. </summary>
        ///
        /// <remarks>   William.chung, 9/15/2016. </remarks>
        ///
        /// <param name="web">  The web. </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public  static Boolean GetDisplayCurrentUsersInSiteActions(SPWeb web)
        {
            Boolean bDisplayCurrentUsers = false;
            try
            {
                bDisplayCurrentUsers = Boolean.Parse(web.Properties["SPCurrentUsersDisplayCurrentUsersInSiteActions"]);
            }
            catch (Exception)
            {
                // If we can't read property, use default value
                bDisplayCurrentUsers = false;
            }
            return bDisplayCurrentUsers;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets debug mode setting. </summary>
        ///
        /// <remarks>   William.chung, 9/15/2016. </remarks>
        ///
        /// <param name="web">  The web. </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static Boolean GetDebugModeSetting(SPWeb web)
        {
            Boolean bDebugMode = false;
            try
            {
                bDebugMode = Boolean.Parse(web.Properties["SPCurrentUsersDebugMode"]);
            }
            catch (Exception)
            {
                // If we can't read property, use default value
                bDebugMode = false;
            }
            return bDebugMode;
        }


       


    }
}
