<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ADLDSWebApplicationSettings.aspx.cs" Inherits="Nauplius.ADLDS.UserProfiles.Layouts.Nauplius.ADLDS.UserProfiles.ADLDSWebApplicationSettings" DynamicMasterPageFile="~masterurl/default.master" %>

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
	  <col style="width: 40%"></col>
	  <col style="width: 60%"></col>
   </colgroup>
   <tr>
	  <td>
			<wssuc:InputFormSection ID="InputFormSection1" runat="server"
				Title="Web Application"
				Description="Select a web application.">
				<template_inputformcontrols>
					<wssuc:InputFormControl runat="server" LabelText="">
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
	<tr>
		<td>
			<wssuc:InputFormSection ID="InputFormSection2" runat="server"
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
	<tr>
		<td>
			<wssuc:InputFormSection ID="InputFormSection3" runat="server"
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
	<tr>
		<td>
			<wssuc:InputFormSection ID="InputFormSection4" runat="server"
				Title="AD LDS/ADAM Distinguished Name"
				Description="Enter the Distinguished Name of the server.">
				<template_inputformcontrols>
					<wssuc:InputFormControl runat="server" LabelText="Distinguished name" ExampleText="DC=example,DC=local" LabelAssociatedControlId="tBDNPath">
						<Template_Control>                   
							<div class="ms-authoringcontrols">
								<SharePoint:InputFormTextBox runat="server" ID="tBDNPath" Width="60%" />     
								<SharePoint:InputFormRequiredFieldValidator runat="server" ID="tBDNPathReqField" ErrorMessage="Distinguished name is required." 
									SetFocusOnError="true" ControlToValidate="tBDNPath" />    
							</div>
						</Template_Control>
					</wssuc:InputFormControl>
				</template_inputformcontrols>
			</wssuc:InputFormSection>
		</td>
	</tr>
	<tr>
		<td>
			<wssuc:InputFormSection ID="InputFormSection5" runat="server"
				Title="SSL Connection"
				Description="If using SSL, make sure the SSL certificate is trusted and is using a name as defined in the Server Name parameter.">
				<template_inputformcontrols>
					<wssuc:InputFormControl runat="server" LabelText="Use SSL" LabelAssociatedControlId="cBUseSSL">
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
	<tr>
		<td>
			<wssuc:InputFormSection ID="InputFormSection6" runat="server"
				Title="User Login Attribute"
				Description="The Login Attribute is the attribute that FBA users log into the SharePoint site with.">
				<template_inputformcontrols>
					<wssuc:InputFormControl runat="server" LabelText="Logon attribute" ExampleText="sAMAccountName" LabelAssociatedControlId="tBLoginAttrib">
						<Template_Control>                   
							<div class="ms-authoringcontrols">
								<SharePoint:InputFormTextBox runat="server" ID="tBLoginAttrib" Width="60%" />
								<SharePoint:InputFormRequiredFieldValidator runat="server" ID="tBLoginAttribReqField" ErrorMessage="Logon attribute is required." 
									SetFocusOnError="true" ControlToValidate="tBLoginAttrib" />
							</div>
						</Template_Control>
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
Nauplius - AD LDS/ADAM User Profile Web Application Configuration
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
AD LDS/ADAM User Profile Web Application Configuration
</asp:Content>
