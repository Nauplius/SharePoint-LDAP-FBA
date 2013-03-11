<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ADLDSGlobalSettings.aspx.cs" Inherits="Nauplius.ADLDS.UserProfiles.Layouts.Nauplius.ADLDS.UserProfiles.ADLDSGlobalSettings" DynamicMasterPageFile="~masterurl/default.master" %>

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
				Title="Claims Identifier"
				Description="Enter the Claims identifier prefix for AD LDS/ADAM users.">
				<template_inputformcontrols>
					<wssuc:InputFormControl runat="server" LabelText="Claims identifier:" ExampleText="i:0#.f" LabelAssociatedControlId="tBCI">
						<Template_Control>                   
							<div class="ms-authoringcontrols">
								<SharePoint:InputFormTextBox runat="server" ID="tBCI" Width="60%" />
								<SharePoint:InputFormRequiredFieldValidator runat="server" ID="tBCIReqField" ErrorMessage="Claims identifier required." 
									SetFocusOnError="true" ControlToValidate="tBCI" />                     
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
				Title="LDAP Filter"
				Description="Enter the LDAP filter.">
				<template_inputformcontrols>
					<wssuc:InputFormControl runat="server" LabelText="LDAP filter:" ExampleText="(&(objectClass=user))" LabelAssociatedControlId="tBLF">
						<Template_Control>                   
							<div class="ms-authoringcontrols">
								<SharePoint:InputFormTextBox runat="server" ID="tBLF" Width="60%" />
								<SharePoint:InputFormRequiredFieldValidator runat="server" ID="tBLFReqField" ErrorMessage="An LDAP filter required." 
									SetFocusOnError="true" ControlToValidate="tBLF" />                     
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
				Title="Delete Profiles"
				Description="Deletes User Profiles when the user is not found in AD LDS/ADAM.">
				<template_inputformcontrols>
					<wssuc:InputFormControl runat="server" LabelText="Delete profiles:" LabelAssociatedControlId="cBDelProf">
						<Template_Control>                   
							<div class="ms-authoringcontrols">
								<SharePoint:InputFormCheckBox runat="server" ID="cBDelProf" />                    
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
				Title="Department Attribute Field"
				Description="The field associated with Department.">
				<template_inputformcontrols>
					<wssuc:InputFormControl runat="server" LabelText="Department:" ExampleText="department" LabelAssociatedControlId="tBDept">
						<Template_Control>                   
							<div class="ms-authoringcontrols">
								<SharePoint:InputFormTextBox runat="server" ID="tBDept" Width="60%" />              
							</div>
						</Template_Control>
					</wssuc:InputFormControl>
				</template_inputformcontrols>
			</wssuc:InputFormSection>
		</td>
	</tr>
	  <td>
			<wssuc:InputFormSection ID="InputFormSection5" runat="server"
				Title="FirstName Attribute Field"
				Description="The field associated with FirstName.">
				<template_inputformcontrols>
					<wssuc:InputFormControl runat="server" LabelText="FirstName:" ExampleText="givenName" LabelAssociatedControlId="tBFN">
						<Template_Control>                   
							<div class="ms-authoringcontrols">
								<SharePoint:InputFormTextBox runat="server" ID="tBFN" Width="60%" />              
							</div>
						</Template_Control>
					</wssuc:InputFormControl>
				</template_inputformcontrols>
			</wssuc:InputFormSection>
		</td>
	</tr>
	  <td>
			<wssuc:InputFormSection ID="InputFormSection6" runat="server"
				Title="LastName Attribute Field"
				Description="The field associated with LastName.">
				<template_inputformcontrols>
					<wssuc:InputFormControl runat="server" LabelText="LastName:" ExampleText="sn" LabelAssociatedControlId="tBLN">
						<Template_Control>                   
							<div class="ms-authoringcontrols">
								<SharePoint:InputFormTextBox runat="server" ID="tBLN" Width="60%" />              
							</div>
						</Template_Control>
					</wssuc:InputFormControl>
				</template_inputformcontrols>
			</wssuc:InputFormSection>
		</td>
	</tr>
	  <td>
			<wssuc:InputFormSection ID="InputFormSection7" runat="server"
				Title="Office Attribute Field"
				Description="The field associated with Office.">
				<template_inputformcontrols>
					<wssuc:InputFormControl runat="server" LabelText="Office:" ExampleText="physicalDeliveryOfficeName" LabelAssociatedControlId="tBOff">
						<Template_Control>                   
							<div class="ms-authoringcontrols">
								<SharePoint:InputFormTextBox runat="server" ID="tBOff" Width="60%" />              
							</div>
						</Template_Control>
					</wssuc:InputFormControl>
				</template_inputformcontrols>
			</wssuc:InputFormSection>
		</td>
	</tr>
	  <td>
			<wssuc:InputFormSection ID="InputFormSection8" runat="server"
				Title="PreferredName Attribute Field"
				Description="The field associated with PreferredName.">
				<template_inputformcontrols>
					<wssuc:InputFormControl runat="server" LabelText="PreferredName:" ExampleText="displayName" LabelAssociatedControlId="tBPN">
						<Template_Control>                   
							<div class="ms-authoringcontrols">
								<SharePoint:InputFormTextBox runat="server" ID="tBPN" Width="60%" />              
							</div>
						</Template_Control>
					</wssuc:InputFormControl>
				</template_inputformcontrols>
			</wssuc:InputFormSection>
		</td>
	</tr>
	  <td>
			<wssuc:InputFormSection ID="InputFormSection9" runat="server"
				Title="Title Attribute Field"
				Description="The field associated with Title.">
				<template_inputformcontrols>
					<wssuc:InputFormControl runat="server" LabelText="Title:" ExampleText="title" LabelAssociatedControlId="tBTitle">
						<Template_Control>                   
							<div class="ms-authoringcontrols">
								<SharePoint:InputFormTextBox runat="server" ID="tBTitle" Width="60%" />              
							</div>
						</Template_Control>
					</wssuc:InputFormControl>
				</template_inputformcontrols>
			</wssuc:InputFormSection>
		</td>
	</tr>
	  <td>
			<wssuc:InputFormSection ID="InputFormSection10" runat="server"
				Title="WebSite Attribute Field"
				Description="The field associated with WebSite.">
				<template_inputformcontrols>
					<wssuc:InputFormControl runat="server" LabelText="WebSite:" ExampleText="wWWHomePage" LabelAssociatedControlId="tBWS">
						<Template_Control>                   
							<div class="ms-authoringcontrols">
								<SharePoint:InputFormTextBox runat="server" ID="tBWS" Width="60%" />              
							</div>
						</Template_Control>
					</wssuc:InputFormControl>
				</template_inputformcontrols>
			</wssuc:InputFormSection>
		</td>
	</tr>
	<tr>
	  <td>
			<wssuc:InputFormSection ID="InputFormSection11" runat="server"
				Title="WorkEmail Attribute Field"
				Description="The field associated with WorkEmail.">
				<template_inputformcontrols>
					<wssuc:InputFormControl runat="server" LabelText="WorkEmail:" ExampleText="mail" LabelAssociatedControlId="tBWE">
						<Template_Control>                   
							<div class="ms-authoringcontrols">
								<SharePoint:InputFormTextBox runat="server" ID="tBWE" Width="60%" />              
							</div>
						</Template_Control>
					</wssuc:InputFormControl>
				</template_inputformcontrols>
			</wssuc:InputFormSection>
		</td>
	</tr>
	<tr>
	  <td>
			<wssuc:InputFormSection ID="InputFormSection12" runat="server"
				Title="WorkPhone Attribute Field"
				Description="The field associated with WorkPhone.">
				<template_inputformcontrols>
					<wssuc:InputFormControl runat="server" LabelText="WorkPhone:" ExampleText="telephoneNumber" LabelAssociatedControlId="tBWP">
						<Template_Control>                   
							<div class="ms-authoringcontrols">
								<SharePoint:InputFormTextBox runat="server" ID="tBWP" Width="60%" />              
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
Nauplius - AD LDS/ADAM User Profile Global Configuration
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
AD LDS/ADAM User Profile Global Configuration
</asp:Content>
