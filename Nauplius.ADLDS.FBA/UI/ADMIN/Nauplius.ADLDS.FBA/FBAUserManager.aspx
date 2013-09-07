<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FBAUserManager.aspx.cs" Inherits="UI.ADMIN.Nauplius.ADLDS.FBA.FBAUserManager" DynamicMasterPageFile="~masterurl/default.master" %>

<%@ Register TagPrefix="wssuc" TagName="InputFormSection" src="~/_controltemplates/InputFormSection.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormControl" src="~/_controltemplates/InputFormControl.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="ButtonSection" src="~/_controltemplates/ButtonSection.ascx" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
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
	<!-- SPUrl Zone -->
	<tr>
		<td>
			<wssuc:InputFormSection ID="InputFormSection30" runat="server"
				Title="Zone"
				Description="The zone to be configured.">
				<template_inputformcontrols>
					<wssuc:InputFormControl runat="server" LabelText="Zone:">
						<template_control>
							<div class="ms-authoringcontrols">
								<SharePoint:UrlZonePicker runat="server" ID="ddlZonePicker" OnLoad="ZoneSelector_OnLoad" AutoPostBack="True" OnTextChanged="WebAppSelector_OnChanged"/>
							</div>
						</template_control>
					</wssuc:InputFormControl>
				</template_inputformcontrols>
			</wssuc:InputFormSection>
		</td>
	</tr>
	<!-- Current User Table -->
	<tr>
		<td>
			<asp:Table runat="server" ID="table1" style="margin:0 auto" CellSpacing="15" Width="100%">
				<asp:TableHeaderRow id="th1" runat="server">
					<asp:TableHeaderCell Text="Display Name" />
					<asp:TableHeaderCell Text="First Name" />
					<asp:TableHeaderCell Text="Last Name" />
					<asp:TableHeaderCell Text="Work Phone" />
				    <asp:TableHeaderCell Text="Job Title" />
				    <asp:TableHeaderCell Text="Office Location" />
				    <asp:TableHeaderCell Text="Password" />
				</asp:TableHeaderRow>    
			</asp:Table>
			<hr id="hr1" runat="server"/>
		</td>
	</tr>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Application Page
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
My Application Page
</asp:Content>
