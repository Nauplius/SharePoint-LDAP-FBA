<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FBAWebApplicationSettings.aspx.cs" Inherits="UI.ADMIN.Nauplius.ADLDS.FBA.FBAWebApplicationSettings" DynamicMasterPageFile="~masterurl/default.master" %>

<%@ Register TagPrefix="wssuc" TagName="InputFormSection" src="~/_controltemplates/InputFormSection.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormControl" src="~/_controltemplates/InputFormControl.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="ButtonSection" src="~/_controltemplates/ButtonSection.ascx" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    
<table border="0" cellspacing="0" cellpadding="0" class="ms-propertysheet" width="100%">
	<wssuc:ButtonSection runat="server" TopButtons="true" BottomSpacing="5" ShowSectionLine="false" ShowStandardCancelButton="false">
		<Template_Buttons>
			<asp:Button UseSubmitBehavior="false" runat="server" class="ms-ButtonHeightWidth" OnClick="btnSave_OnSave" Text="<%$Resources:wss,multipages_okbutton_text%>" id="btnSaveTop" accesskey="<%$Resources:wss,okbutton_accesskey%>" />
			<asp:Button UseSubmitBehavior="false" runat="server" class="ms-ButtonHeightWidth" OnClick="btnCancel" Text="<%$Resources:wss,multipages_cancelbutton_text%>" id="btnCancelTop" accesskey="<%$Resources:wss,cancelbutton_accesskey%>" CausesValidation="false"/>
		</Template_Buttons>
	</wssuc:ButtonSection>
    <colgroup>
        <col style="width: 40%" />
        <col style="width: 60%" />
    </colgroup>
    <!-- Web Application Selector -->
    <tr>
        <td>
			<wssuc:InputFormSection ID="InputFormSection1" runat="server"
				Title="Web Application"
				Description="Select a web application.">
				<template_inputformcontrols>
					<wssuc:InputFormControl runat="server" LabelText="Web Application:">
						<Template_Control>                   
							<div class="ms-authoringcontrols">
								<SharePoint:WebApplicationSelector ID="ddlWebApp" runat="server" OnContextChange="WebAppSelector_OnChanged" />                        
							</div>
						</Template_Control>
					</wssuc:InputFormControl>
				</template_inputformcontrols>
			</wssuc:InputFormSection>
		</td>
	</tr>
    <!-- Membership Provider -->
    <tr>
        <td>
            <wssuc:InputFormSection ID="InputFormSection3" runat="server"
            Title="Membership Provider Name"
            Description="Membership provider.">
                <template_inputformcontrols>
                    <wssuc:InputFormControl runat="server" LabelText="Name of the Membership provider:" ExampleText="ExampleMembership">
                        <template_control>
                            <div class="ms-authoringcontrols">
                                <SharePoint:InputFormTextBox runat="server" ID="txtMemProv" TextMode="SingleLine" Width="40%"/>
                            </div>
                        </template_control>
                    </wssuc:InputFormControl>
                </template_inputformcontrols>
            </wssuc:InputFormSection>
        </td>
    </tr>
    <!-- Role Provider -->
    <tr>
        <td>
            <wssuc:InputFormSection ID="InputFormSection4" runat="server"
            Title="Role Provider Name"
            Description="Role provider.">
                <template_inputformcontrols>
                    <wssuc:InputFormControl runat="server" LabelText="Name of the Role provider:" ExampleText="ExampleRole">
                        <template_control>
                            <div class="ms-authoringcontrols">
                                <SharePoint:InputFormTextBox runat="server" ID="txtRoleProv" TextMode="SingleLine" Width="40%"/>
                            </div>
                        </template_control>
                    </wssuc:InputFormControl>
                </template_inputformcontrols>
            </wssuc:InputFormSection>
        </td>
    </tr>
    <!-- Custom URL for Login -->
    <tr>
        <td>
            <wssuc:InputFormSection ID="InputFormSection26" runat="server"
            Title="Custom Login Url"
            Description="Enter the custom URL used for login by this Web Application.">
                <template_inputformcontrols>
                    <wssuc:InputFormControl runat="server" LabelText="Relative Url to the Login page:" ExampleText="/_Layouts/Nauplius.ADLDS.FBA/login.aspx">
                        <template_control>
                            <div class="ms-authoringcontrols">
                                <SharePoint:InputFormTextBox runat="server" ID="txtCustomUrl" TextMode="SingleLine" Width="60%"/>
                            </div>
                        </template_control>
                    </wssuc:InputFormControl>
                </template_inputformcontrols>
            </wssuc:InputFormSection>
        </td>
    </tr>
    <!-- AD LDS Server -->
    <tr>
        <td>
            <wssuc:InputFormSection ID="InputFormSection5" runat="server"
				Title="AD LDS/ADAM Server Name"
				Description="Enter the hostname or fully qualified name of the Active Directory Lightweight Directory Services/Active Directory Application Mode server. If using SSL, make sure the SSL certificate matches the name used here.">
				<template_inputformcontrols>
					<wssuc:InputFormControl runat="server" LabelText="Server name:" ExampleText="hostname.example.com" LabelAssociatedControlId="tBSN">
						<Template_Control>                   
							<div class="ms-authoringcontrols">
								<SharePoint:InputFormTextBox runat="server" ID="tBSN" Width="60%" />
								<SharePoint:InputFormRequiredFieldValidator runat="server" ID="tBSNReqField" ErrorMessage="Server name is required." 
									SetFocusOnError="true" ControlToValidate="tBSN" />   
							</div>
						</Template_Control>
					</wssuc:InputFormControl>
				</template_inputformcontrols>
            </wssuc:InputFormSection>
        </td>
    </tr>
    <!-- AD LDS Port -->
    <tr>
        <td>
            <wssuc:InputFormSection ID="InputFormSection6" runat="server"
				Title="AD LDS/ADAM Port Number"
				Description="Enter the port number for the AD LDS/ADAM server.">
				<template_inputformcontrols>
					<wssuc:InputFormControl runat="server" LabelText="Port number:" ExampleText="389, 636, 1024-65535" LabelAssociatedControlId="tBPrtNo">
						<Template_Control>                   
							<div class="ms-authoringcontrols">
								<SharePoint:InputFormTextBox runat="server" ID="tBPrtNo" Width="60%" MaxLength="5" />
								<SharePoint:InputFormRequiredFieldValidator runat="server" ID="tBPrtNoReqField" ErrorMessage="Port number is required." 
									SetFocusOnError="true" ControlToValidate="tBPrtNo" />
								<SharePoint:InputFormCustomValidator runat="server" ID="tBPortNoCustValidation" OnServerValidate="portValidation" 
								ErrorMessage="Port is invalid for AD LDS/ADAM." SetFocusOnError="true" ControlToValidate="tBPrtNo" />  
							</div>
						</Template_Control>
					</wssuc:InputFormControl>
				</template_inputformcontrols>
			</wssuc:InputFormSection>
        </td>
    </tr>
    <!-- Use SSL -->
    <tr>
        <td>
            <wssuc:InputFormSection ID="InputFormSection9" runat="server"
				Title="SSL Connection"
				Description="If using SSL, make sure the SSL certificate is trusted and is using a name as defined in the Server Name parameter.">
				<template_inputformcontrols>
					<wssuc:InputFormControl runat="server" LabelText="Use SSL:" LabelAssociatedControlId="cBUseSSL">
						<Template_Control>                   
							<div class="ms-authoringcontrols">
								<SharePoint:InputFormCheckBox runat="server" ID="cBUseSSL" /> 
							</div>
						</Template_Control>
					</wssuc:InputFormControl>
				</template_inputformcontrols>
            </wssuc:InputFormSection>
        </td>
    </tr>
    <!-- AD LDS Login Attribute -->
    <tr>
        <td>
            <wssuc:InputFormSection ID="InputFormSection10" runat="server"
				Title="User Login Attribute"
				Description="The Login Attribute is the attribute that FBA users log into the SharePoint site with.">
				<template_inputformcontrols>
					<wssuc:InputFormControl runat="server" LabelText="Logon attribute:" ExampleText="sAMAccountName" LabelAssociatedControlId="tBLoginAttrib">
						<Template_Control>                   
							<div class="ms-authoringcontrols">
								<SharePoint:InputFormTextBox runat="server" ID="tBLoginAttrib" Width="40%" />
								<SharePoint:InputFormRequiredFieldValidator runat="server" ID="tBLoginAttribReqField" ErrorMessage="Logon attribute is required." 
									SetFocusOnError="true" ControlToValidate="tBLoginAttrib" />
							</div>
						</Template_Control>
					</wssuc:InputFormControl>
				</template_inputformcontrols>
            </wssuc:InputFormSection>
        </td>
    </tr>
    <!-- AD LDS User Container -->
    <tr>
        <td>
            <wssuc:InputFormSection ID="InputFormSection12" runat="server"
            Title="AD LDS User Container"
            Description="User Container.">
                <template_inputformcontrols>
                    <wssuc:InputFormControl runat="server" LabelText="User Container:" ExmapleText="CN=Users,DC=example,DC=local">
                        <template_control>
                            <div class="ms-authoringcontrols">
                                <SharePoint:InputFormTextBox runat="server" ID="txtUsrContainer" TextMode="SingleLine" Width="60%"/>
                            </div>
                        </template_control>
                    </wssuc:InputFormControl>
                </template_inputformcontrols>
            </wssuc:InputFormSection>
        </td>
    </tr>
    <!-- AD LDS User Object Class -->
    <tr>
        <td>
            <wssuc:InputFormSection ID="InputFormSection13" runat="server"
            Title="AD LDS User Object Class"
            Description="User object class.">
                <template_inputformcontrols>
                    <wssuc:InputFormControl runat="server" LabelText="Object class representing the user:" ExampleText="user">
                        <template_control>
                            <div class="ms-authoringcontrols">
                                <SharePoint:InputFormTextBox runat="server" ID="txtUsrObjClass" TextMode="SingleLine" Text="user" Width="40%"/>
                            </div>
                        </template_control>
                    </wssuc:InputFormControl>
                </template_inputformcontrols>
            </wssuc:InputFormSection>
        </td>
    </tr>
    <!-- AD LDS User Filter -->
    <tr>
        <td>
            <wssuc:InputFormSection ID="InputFormSection14" runat="server"
            Title="AD LDS User Filter"
            Description="User Filter.">
                <template_inputformcontrols>
                    <wssuc:InputFormControl runat="server" LabelText="LDAP Filter for Users:" ExampleText="(ObjectClass=*)">
                        <template_control>
                            <div class="ms-authoringcontrols">
                                <SharePoint:InputFormTextBox runat="server" ID="txtUsrFilter" TextMode="SingleLine" Text="(ObjectClass=*)" Width="40%"/>
                            </div>
                        </template_control>
                    </wssuc:InputFormControl>
                </template_inputformcontrols>
            </wssuc:InputFormSection>
        </td>
    </tr>
    <!-- AD LDS User Scope -->
    <!-- ToDo: CONVERT TO DROPDOWN: Base, Subtree, OneLevel (??? -->
    <tr>
        <td>
            <wssuc:InputFormSection ID="InputFormSection15" runat="server"
            Title="AD LDS User Scope"
            Description="User Scope.">
                <template_inputformcontrols>
                    <wssuc:InputFormControl runat="server" LabelText="User LDAP Search Scope:" ExampleText="Subtree">
                        <template_control>
                            <div class="ms-authoringcontrols">
                                <SharePoint:InputFormTextBox runat="server" ID="txtUsrScope" TextMode="SingleLine" Text="Subtree"/>
                            </div>
                        </template_control>
                    </wssuc:InputFormControl>
                </template_inputformcontrols>
            </wssuc:InputFormSection>
        </td>
    </tr>
    <!-- AD LDS User Distinguished Name Attribute -->
    <tr>
        <td>
            <wssuc:InputFormSection ID="InputFormSection31" runat="server"
                Title="AD LDS User Distinguished Name Attribute"
                Description="The LDAP attribute used for user object's Distinguished Name.">
                <template_inputformcontrols>
                    <wssuc:InputFormControl runat="server" LabelText="User Distinguished Name Attribute:" ExampleText="distinguishedName">
                        <template_control>
                            <div class="ms-authoringcontrols">
                                <SharePoint:InputFormTextBox runat="server" ID="txtUsrDNAttrib" TextMode="SingleLine" Text="distinguishedName" Width="40%"/>
                            </div>
                        </template_control>
                    </wssuc:InputFormControl>
                </template_inputformcontrols>
            </wssuc:InputFormSection>
        </td>
    </tr>
    <!-- AD LDS User Other Required Attributes -->
    <tr>
        <td>
            <wssuc:InputFormSection ID="InputFormSection16" runat="server"
            Title="AD LDS User Other Required Attributes"
            Description="User other required attributes.">
                <template_inputformcontrols>
                    <wssuc:InputFormControl runat="server" LabelText="Other attributes required for User objects:" ExampleText="sn,givenname,cn">
                        <template_control>
                            <div class="ms-authoringcontrols">
                                <SharePoint:InputFormTextBox runat="server" ID="txtUsrOtherAttribs" TextMode="SingleLine" Text="sn,givenname,cn" Width="40%"/>
                            </div>
                        </template_control>
                    </wssuc:InputFormControl>
                </template_inputformcontrols>
            </wssuc:InputFormSection>
        </td>
    </tr>
    <!-- AD LDS Group Container -->
    <tr>
        <td>
            <wssuc:InputFormSection ID="InputFormSection17" runat="server"
            Title="AD LDS Group Container"
            Description="Group Container.">
                <template_inputformcontrols>
                    <wssuc:InputFormControl runat="server" LabelText="Group LDAP container:" ExampleText="CN=Groups,DC=example,DC=local">
                        <template_control>
                            <div class="ms-authoringcontrols">
                                <SharePoint:InputFormTextBox runat="server" ID="txtGrpContainer" TextMode="SingleLine" Width="60%"/>
                            </div>
                        </template_control>
                    </wssuc:InputFormControl>
                </template_inputformcontrols>
            </wssuc:InputFormSection>
        </td>
    </tr>
    <!-- AD LDS Group Name Attribute -->
    <tr>
        <td>
            <wssuc:InputFormSection ID="InputFormSection18" runat="server"
            Title="AD LDS Group Name Attribute"
            Description="Group Name Attribute.">
                <template_inputformcontrols>
                    <wssuc:InputFormControl runat="server" LabelText="Group Name LDAP attribute:" ExampleText="cn">
                        <template_control>
                            <div class="ms-authoringcontrols">
                                <SharePoint:InputFormTextBox runat="server" ID="txtGrpNameAttrib" TextMode="SingleLine" Text="cn" Width="40%"/>
                            </div>
                        </template_control>
                    </wssuc:InputFormControl>
                </template_inputformcontrols>
            </wssuc:InputFormSection>
        </td>
    </tr>
    <!-- AD LDS Group Alternate Search Attribute -->
    <tr>
        <td>
            <wssuc:InputFormSection ID="InputFormSection19" runat="server"
            Title="AD LDS Group Alternate Search Attribute"
            Description="Group alternate search attribute.">
                <template_inputformcontrols>
                    <wssuc:InputFormControl runat="server" LabelText="Group Alternate Search LDAP Attribute:" ExampleText="cn">
                        <template_control>
                            <div class="ms-authoringcontrols">
                                <SharePoint:InputFormTextBox runat="server" ID="txtGrpAltSearchAttrib" TextMode="SingleLine" Text="cn" Width="40%"/>
                            </div>
                        </template_control>
                    </wssuc:InputFormControl>
                </template_inputformcontrols>
            </wssuc:InputFormSection>
        </td>
    </tr>
    <!-- AD LDS Group Member Attribute -->
    <tr>
        <td>
            <wssuc:InputFormSection ID="InputFormSection20" runat="server"
            Title="AD LDS Group Member Attribute"
            Description="Group Member Attribute.">
                <template_inputformcontrols>
                    <wssuc:InputFormControl runat="server" LabelText="Group LDAP Member Attribute:" ExampleText="member">
                        <template_control>
                            <div class="ms-authoringcontrols">
                                <SharePoint:InputFormTextBox runat="server" ID="txtGrpMemAttrib" TextMode="SingleLine" Text="member" Width="40%"/>
                            </div>
                        </template_control>
                    </wssuc:InputFormControl>
                </template_inputformcontrols>
            </wssuc:InputFormSection>
        </td>
    </tr>
    <!-- AD LDS Group Distingiushed Name Path -->
    <tr>
        <td>
            <wssuc:InputFormSection ID="InputFormSection21" runat="server"
            Title="AD LDS Group Distinguished Name Attribute"
            Description="Group DN attrib.">
                <template_inputformcontrols>
                    <wssuc:InputFormControl runat="server" LabelText="Group Distinguished Name LDAP Attribute:" ExampleText="distinguishedName">
                        <template_control>
                            <div class="ms-authoringcontrols">
                                <SharePoint:InputFormTextBox runat="server" ID="txtGrpDNAttrib" TextMode="SingleLine" Text="distinguishedName" Width="40%"/>
                            </div>
                        </template_control>
                    </wssuc:InputFormControl>
                </template_inputformcontrols>
            </wssuc:InputFormSection>
        </td>
    </tr>
    <!-- AD LDS Group User Filter -->
    <tr>
        <td>
            <wssuc:InputFormSection ID="InputFormSection22" runat="server"
            Title="AD LDS Group User Filter"
            Description="Group user filter.">
                <template_inputformcontrols>
                    <wssuc:InputFormControl runat="server" LabelText="Group User LDAP Filter:" ExampleText="&amp;(objectClass=user)(objectCategory=person)">
                        <template_control>
                            <div class="ms-authoringcontrols">
                                <SharePoint:InputFormTextBox runat="server" ID="txtGrpUsrFilter" TextMode="SingleLine" 
                                    Text="&amp;(objectClass=user)(objectCategory=person)" Width="60%"/>
                            </div>
                        </template_control>
                    </wssuc:InputFormControl>
                </template_inputformcontrols>
            </wssuc:InputFormSection>
        </td>
    </tr>
    <!-- AD LDS Group Filter -->
    <tr>
        <td>
            <wssuc:InputFormSection ID="InputFormSection23" runat="server"
            Title="AD LDS Group Filter"
            Description="Group filter.">
                <template_inputformcontrols>
                    <wssuc:InputFormControl runat="server" LabelText="Group LDAP Filter:" ExampleText="&amp;(objectCategory=Group)(objectClass=group)">
                        <template_control>
                            <div class="ms-authoringcontrols">
                                <SharePoint:InputFormTextBox runat="server" ID="txtGrpFilter" TextMode="SingleLine" 
                                    Text="&amp;(objectCategory=Group)(objectClass=group)" Width="60%"/>
                            </div>
                        </template_control>
                    </wssuc:InputFormControl>
                </template_inputformcontrols>
            </wssuc:InputFormSection>
        </td>
    </tr>
    <!-- AD LDS Group Scope -->
    <!-- ToDo: CONVERT TO DROPDOWN -->
    <tr>
        <td>
            <wssuc:InputFormSection ID="InputFormSection24" runat="server"
            Title="AD LDS Group Scope"
            Description="Group Scope.">
                <template_inputformcontrols>
                    <wssuc:InputFormControl runat="server" LabelText="Group Container:" ExampleText="Subtree">
                        <template_control>
                            <div class="ms-authoringcontrols">
                                <SharePoint:InputFormTextBox runat="server" ID="txtGrpScope" TextMode="SingleLine" Text="Subtree"/>
                            </div>
                        </template_control>
                    </wssuc:InputFormControl>
                </template_inputformcontrols>
            </wssuc:InputFormSection>
        </td>
    </tr>
    <!-- AD LDS Group's User DN Attribute" -->
    <tr>
        <td>
            <wssuc:InputFormSection ID="InputFormSection25" runat="server"
                Title="AD LDS Group User Distinguished Name Attribute"
                Description="The LDAP attribute used for group object's Distinguished Name.">
                <template_inputformcontrols>
                    <wssuc:InputFormControl runat="server" LabelText="Group User Distinguished Name Attribute:" ExampleText="distinguishedName">
                        <template_control>
                            <div class="ms-authoringcontrols">
                                <SharePoint:InputFormTextBox runat="server" ID="txtGrpUsrDnAttrib" TextMode="SingleLine" Text="distinguishedName" Width="40%"/>
                            </div>
                        </template_control>
                    </wssuc:InputFormControl>
                </template_inputformcontrols>
            </wssuc:InputFormSection>
        </td>
    </tr>

	<wssuc:ButtonSection runat="server" TopButtons="true" BottomSpacing="5" ShowSectionLine="false" ShowStandardCancelButton="false">
		<Template_Buttons>
			<asp:Button UseSubmitBehavior="false" runat="server" class="ms-ButtonHeightWidth" OnClick="btnSave_OnSave" Text="<%$Resources:wss,multipages_okbutton_text%>" id="btnSaveBottom" accesskey="<%$Resources:wss,okbutton_accesskey%>" />
			<asp:Button UseSubmitBehavior="false" runat="server" class="ms-ButtonHeightWidth" OnClick="btnCancel" Text="<%$Resources:wss,multipages_cancelbutton_text%>" id="btnCancelBottom" accesskey="<%$Resources:wss,cancelbutton_accesskey%>" CausesValidation="false"/>
		</Template_Buttons>
	</wssuc:ButtonSection>
</table>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Nauplius - AD LDS/ADAM FBA Web Application Configuration
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
AD LDS/ADAM FBA Web Application Configuration
</asp:Content>
