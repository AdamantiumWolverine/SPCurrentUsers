# SPCurrentUsers
Farm Solution Package for SharePoint to display current users.

This is the improved version of the SPCurrentUsers based on the blog that was at http://blog.furuknap.net/find-number-of-users-currently-logged-on-to-a-sharepoint-site and was shared on https://SPCurrentUsers.codeplex.com.   

The blog version had a tutorial on how you would build a solution package for displaying the current users.  Unfortunately, the version in the blog was inefficient and the implementation from the blog was not meant for production environments.

Some of the issues, as I recall, were that the count of current users was placed into the Site Settings menu.  This meant that every access to a page on the site would also count all the current users.   Next, the version from the blog allowed you to set the session timeouts for each page, and therefore required a query of all users that logged in, and then a query of all page session timeouts, and then a count of logged in users based off the page being accessed.  This version uses a single session timeout significantly reducing the number of queries and amount of time required to establish the current user count.

There were other improvements made over the years, but I digress.  Here is the latest version of the code as of 2022-02-10.

The code was developed for the SP2013 platform, but I have verified the solution will install and work on SP2016 and SP2019.  I haven't yet used the code in production on SP2016 and SP2019 environments, yet, but plan to do so in the near future as we migrate to SP2019 in our production environment.


Download the WSP file here:
https://github.com/AdamantiumWolverine/SPCurrentUsers/raw/main/SPCurrentUsersSP2013/bin/Debug/SPCurrentUsers.wsp


PowerShell Add and Install Solution Commands:

   Add-SPSolution -LiteralPath .\SPCurrentUsers.wsp

   Install-SPSolution -Identity SPCurrentUsers.wsp -GACDeployment -force -CompatibilityLevel {All} -AllWebApplications
   
   
Once installed, you will be able to go to Site Settings -> Site Collection Features, and enable the SPCurrentUsers Setup Feature to activate the features that create the SPCurrentUsersUserTracker list and adds the control to track current logins.

The tool will add a link to the SPCurrentUsers Admin page from the Site Settings menu for users who are admins on the site.


The tool comes with a clean up _layouts web page to clear out old entries in the SPCurrentUsersUserTracker list.  
However, if you want to completely clear out the SPCurrentUsersUserTracker list as it has gotten too many entries over the years, you can deactivate all the SPCurrentUsers Features, then go to your site collection's /lists/SPCurrentUsersUserTracker list -> List Settings  and delete the list.  Then, reactivate the SPCurrentUsers Setup feature to recreate the list.



