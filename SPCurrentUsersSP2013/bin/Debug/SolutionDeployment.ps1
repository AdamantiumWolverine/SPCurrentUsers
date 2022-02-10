Add-PSSnapin 'Microsoft.SharePoint.PowerShell'; 
[void][System.Reflection.Assembly]::LoadWithPartialName("Microsoft.SharePoint")

###################################################################################
# Function to display the solution status of a solution package repeatedly until it reaches the end state
# EndStates: NotDeployed, GlobalAndWebApplicationDeployed
####################################################################################
Function DisplaySolutionStatusUntilEndState($SolutionPackage, $endstate)
{
  $continueLoop=$TRUE;

while ($continueLoop -eq $TRUE)
{
cls
$currentDate=Get-Date;
Write-Host '*********************' $currentDate '*****************************';
Write-Host('Displaying solution status and waiting for solution status to change to');
Write-Host('    [' + $endstate + ']');
Write-Host;

$farm=[Microsoft.SharePoint.Administration.SPFarm]::Local

$bFoundSolution=$FALSE;
foreach ($solution in $farm.Solutions) 
        {

	       $compareresult=[string]::Compare($solution.DisplayName,$SolutionPackage, $TRUE);
	      # Write-Host $compareresult + $solution.DisplayName + " vs " + $SolutionPackage;
	       if ($compareresult -eq 0)
		{     
		  $bFoundSolution=$TRUE;                      
                  Write-Host("**"+ $solution.DisplayName + ': ' + $solution.Status + ', ' + $solution.DeploymentState );
		  if ($solution.DeploymentState -eq $endstate)
			{
			   $continueLoop=$FALSE;
			}
                  else 
                        {
		  	   Start-Sleep -s 1;
			}
		}
		else
		{
			#Write-Host($solution.DisplayName + ': ' + $solution.Status + ', ' + $solution.DeploymentState );
		}

		
        }
	
	if ($bFoundSolution -eq $FALSE -and $endstate -eq 'NotDeployed')
		{
			$continueLoop=$FALSE;
		}
	Start-Sleep -s 1;
	
Write-Host;
}

}


###########################################################################################

Function WaitForNoDeployments()
{
  $continueLoop=$true;
  while ($continueLoop -eq $true)
   {
     $list=(Get-SPTimerJob | ?{$_.Name.IndexOf("solution-deployment") -ne -1})
       if ($list.Count -eq 0)
        {
          $continueLoop=$false;
        }
       else
        {
           Write-Output "$($list.Count) deployments are processing....Waiting for deployments to complete.";
           Start-Sleep -s 1;
        }
     }
}


###########################################################################################


echo off 
cls
$SolutionPackage = 'SPCurrentUsers.wsp';
$BINFOLDER = 'C:\Program Files\Common Files\Microsoft Shared\web server extensions\15\BIN';
$SITEURL = 'http://localhost';

$DEPLOYURL = $SITEURL;

#**********CODE TO PROMPT FOR DEPLOY URL**************
$DEPLOYURL=Read-Host 'Enter in the site you want to deploy the solution to e.g. ' + $SITEURL
If ($DEPLOYURL -eq '') {$DEPLOYURL=$SITEURL}

Write-Host $DEPLOYURL , $SITEURL

$REINSTALLSOLUTION=Read-Host 'Are you sure you want to re-install the '$SolutionPackage'? [Y] or [N]'

if ($REINSTALLSOLUTION -eq 'Y' -or $REINSTALLSOLUTION -eq 'y')
{
Write-Host Uninstalling solution.
Uninstall-SPSolution -Identity $SolutionPackage -CompatibilityLevel All -Confirm:$false -AllWebApplications

WaitForNoDeployments;

$bContinue='n'
DisplaySolutionStatusUntilEndState $SolutionPackage "NotDeployed";


Write-Host Solution Uninstalled.  
WaitForNoDeployments;
Write-Host "Remove-SPSolution...";
Remove-SPSolution -Identity $SolutionPackage -Confirm:$false
WaitForNoDeployments;

Write-Host Add-SPSolution...
Add-SPSolution -LiteralPath $pwd'\'$SolutionPackage
WaitForNoDeployments;


Write-Host Install-SPSolution
Install-SPSolution -Identity $SolutionPackage -GACDeployment -force -CompatibilityLevel {All} -AllWebApplications
WaitForNoDeployments;
}

$bContinue='n'
DisplaySolutionStatusUntilEndState $SolutionPackage "GlobalAndWebApplicationDeployed";


