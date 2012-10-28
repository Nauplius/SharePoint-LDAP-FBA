<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FBAWebApplicationSettings.aspx.cs" Inherits="Nauplius.ADLDS.FBA.Layouts.Nauplius.ADLDS.FBA.FBAWebApplicationSettings" DynamicMasterPageFile="~masterurl/default.master" %>

<%@ Register TagPrefix="wssuc" TagName="InputFormSection" src="~/_controltemplates/InputFormSection.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormControl" src="~/_controltemplates/InputFormControl.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="ButtonSection" src="~/_controltemplates/ButtonSection.ascx" %>


<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

</asp:Content>

<!--
      <FieldRef ID="{159BE31B-AD34-41C4-AE89-B2B742459F18}" Name="WebApplicationUrl" Required="TRUE"/>
      <FieldRef ID="{89B264CD-9A4F-4105-812C-FF674DFFA40B}" Name="WebApplicationZone" Required="TRUE" DefaultValue="Default"/>
      <FieldRef ID="{7457B4B9-5B56-4A03-BCF1-B6E82FE16B4C}" Name="WebApplicationMembershipProvider" Required="TRUE"/>
      <FieldRef ID="{A57B89A8-B2F1-4AA0-A826-C9B2033D88BD}" Name="WebApplicationRoleProvider" Required="TRUE"/>
      <FieldRef ID="{91305BFB-E562-4E56-99D5-A5E0CF15746B}" Name="ADLDSServer" Required="TRUE"/>
      <FieldRef ID="{7DE35286-5B5F-4100-83A8-A888FD109E0D}" Name="ADLDSPort" Required="TRUE"/>
      <FieldRef ID="{3AE45F13-C6CE-4A37-BEC1-58CB2C5B4C30}" Name="ADLDSServer2"/>
      <FieldRef ID="{5FE4E247-52B1-4B9A-90E0-C7DC0698F9F7}" Name="ADLDSPort2"/>
      <FieldRef ID="{C49D12F2-C1E5-472F-9CBD-DE0C43B9E5E7}" Name="ADLDSUseSSL" Required="TRUE"/>
      <FieldRef ID="{2D5A409C-8E31-49F7-B335-23431B19F63D}" Name="ADLDSLoginAttrib" Required="TRUE" DefaultValue="mail"/>
      <FieldRef ID="{FC5F4F93-FEA4-445F-8A3B-CFC354678616}" Name="ADLDSUserDNAttrib" Required="TRUE" DefaultValue="distinguishedName"/>
      <FieldRef ID="{F8059812-1799-43D2-8C88-1AD3BEA93C50}" Name="ADLDSUserContainer" Required="TRUE"/>
      <FieldRef ID="{398F9D04-DE74-4A2E-BA38-4DC0F4DDD8D2}" Name="ADLDSUserObjectClass" Required="TRUE" DefaultValue="user"/>
      <FieldRef ID="{CE575435-0E3C-45DA-81AD-92A6B8AB036B}" Name="ADLDSUserFilter" DefaultValue="(ObjectClass=user)"/>
      <FieldRef ID="{C8C7D4F6-4E13-4C48-B907-51C88ACECAFD}" Name="ADLDSUserScope" Required="TRUE" DefaultValue="Subtree"/>
      <FieldRef ID="{0532A823-E048-4BB9-8043-10FD91995530}" Name="ADLDSUserOtherReqAttrib" Required="TRUE" DefaultValue="sn,givenName,cn"/>
      <FieldRef ID="{FAAD7C91-A135-4929-A1FA-5817D4263CCA}" Name="ADLDSGroupContainer" Required="TRUE"/>
      <FieldRef ID="{E6B66342-27CD-4B95-883E-E99338440B44}" Name="ADLDSGroupNameAttrib" Required="TRUE" DefaultValue="cn"/>
      <FieldRef ID="{4D5B0827-8145-4ED0-B680-2F60AC96368E}" Name="ADLDSGroupNameAltSearchAttrib" Required="TRUE" DefaultValue="cn"/>
      <FieldRef ID="{112FDADA-7220-4DE2-8AA1-DB83C9B93466}" Name="ADLDSGroupMemAttrib" Required="TRUE" DefaultValue="member"/>
      <FieldRef ID="{EE4E176E-705E-4681-BC62-44AC91C62E0F}" Name="ADLDSGroupDNAttrib" Required="TRUE" DefaultValue="distinguishedName"/>
      <FieldRef ID="{84548193-CF98-4211-B477-E67966F0AEE9}" Name="ADLDSGroupUserFilter" Required="TRUE" DefaultValue="(&amp;(ObjectCategory=user)(ObjectClass=person))"/>
      <FieldRef ID="{790B97AF-F962-491E-BE87-2C81B42FA37A}" Name="ADLDSGroupFilter" Required="TRUE" DefaultValue="(&amp;(objectCategory=Group)(objectClass=group)"/>
      <FieldRef ID="{B0A71037-CB4C-4CDA-BC70-682A28A6F5D6}" Name="ADLDSGroupScope" Required="TRUE" DefaultValue="Subtree"/>
-->

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
    <!-- Membership Provider -->
    <tr>
        <td>
            <wssuc:InputFormSection ID="InputFormSection3" runat="server"
            Title="Membership Provider Name"
            Description="Membership provider.">
                <template_inputformcontrols>
                    <wssuc:InputFormControl runat="server" LabelText="">
                        <template_control>
                            <div class="ms-authoringcontrols">
                                <SharePoint:InputFormTextBox runat="server" ID="txtMemProv" TextMode="SingleLine"/>
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
                    <wssuc:InputFormControl runat="server" LabelText="">
                        <template_control>
                            <div class="ms-authoringcontrols">
                                <SharePoint:InputFormTextBox runat="server" ID="txtRoleProv" TextMode="SingleLine"/>
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
            Title="AD LDS Server Name"
            Description="Server name.">
                <template_inputformcontrols>
                    <wssuc:InputFormControl runat="server" LabelText="">
                        <template_control>
                            <div class="ms-authoringcontrols">
                                <SharePoint:InputFormTextBox runat="server" ID="txtServName" TextMode="SingleLine"/>
                            </div>
                        </template_control>
                    </wssuc:InputFormControl>
                </template_inputformcontrols>
            </wssuc:InputFormSection>
        </td>
    </tr>
    <!-- AD LDS Port -->
    <tr>
        <td>
            <wssuc:InputFormSection ID="InputFormSection6" runat="server"
            Title="AD LDS Server Port"
            Description="Server port.">
                <template_inputformcontrols>
                    <wssuc:InputFormControl runat="server" LabelText="">
                        <template_control>
                            <div class="ms-authoringcontrols">
                                <SharePoint:InputFormTextBox runat="server" ID="txtPortNumb" TextMode="SingleLine"/>
                            </div>
                        </template_control>
                    </wssuc:InputFormControl>
                </template_inputformcontrols>
            </wssuc:InputFormSection>
        </td>
    </tr>
    <!-- AD LDS Server 2 -->
    <tr>
        <td>
            <wssuc:InputFormSection ID="InputFormSection7" runat="server"
            Title="AD LDS Server Name 2"
            Description="Server name 2.">
                <template_inputformcontrols>
                    <wssuc:InputFormControl runat="server" LabelText="">
                        <template_control>
                            <div class="ms-authoringcontrols">
                                <SharePoint:InputFormTextBox runat="server" ID="txtServName2" TextMode="SingleLine"/>
                            </div>
                        </template_control>
                    </wssuc:InputFormControl>
                </template_inputformcontrols>
            </wssuc:InputFormSection>
        </td>
    </tr>
    <!-- AD LDS Port 2-->
    <tr>
        <td>
            <wssuc:InputFormSection ID="InputFormSection8" runat="server"
            Title="AD LDS Server Port 2"
            Description="Server port 2.">
                <template_inputformcontrols>
                    <wssuc:InputFormControl runat="server" LabelText="">
                        <template_control>
                            <div class="ms-authoringcontrols">
                                <SharePoint:InputFormTextBox runat="server" ID="txtPortNumb2" TextMode="SingleLine"/>
                            </div>
                        </template_control>
                    </wssuc:InputFormControl>
                </template_inputformcontrols>
            </wssuc:InputFormSection>
        </td>
    </tr>
    <!-- Use SSL -->
    <tr>
        <td>
            <wssuc:InputFormControl ID="InputFormSection9" runat="server"
            Title="Use SSL"
            Description="Enable SSL?">
                <template_inputformcontrols>
                    <wssuc:InputFormControl runat="server" LabelText="">
                        <template_control>
                            <div class="ms-authoringcontrols">
							    <SharePoint:InputFormRadioButton ID="btnUseSSLTrue" runat="server" GroupName="0" LabelText="Enabled" />
							    <SharePoint:InputFormRadioButton ID="btnUseSSLFalse" runat="server" GroupName="0" LabelText="Disabled"/>
                            </div>
                        </template_control>
                    </wssuc:InputFormControl>
                </template_inputformcontrols>
            </wssuc:InputFormControl>
        </td>
    </tr>
    <!-- AD LDS Login Attribute -->
    <tr>
        <td>
            <wssuc:InputFormSection ID="InputFormSection10" runat="server"
            Title="AD LDS Login Attribute"
            Description="Login attribute.">
                <template_inputformcontrols>
                    <wssuc:InputFormControl runat="server" LabelText="">
                        <template_control>
                            <div class="ms-authoringcontrols">
                                <SharePoint:InputFormTextBox runat="server" ID="txtLoginAttrib" TextMode="SingleLine"/>
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
            <wssuc:InputFormSection ID="InputFormSection11" runat="server"
            Title="AD LDS User Distinguished Name Attribute"
            Description="User DN attrib.">
                <template_inputformcontrols>
                    <wssuc:InputFormControl runat="server" LabelText="">
                        <template_control>
                            <div class="ms-authoringcontrols">
                                <SharePoint:InputFormTextBox runat="server" ID="txtUsrDnAttrib" TextMode="SingleLine"/>
                            </div>
                        </template_control>
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
                    <wssuc:InputFormControl runat="server" LabelText="">
                        <template_control>
                            <div class="ms-authoringcontrols">
                                <SharePoint:InputFormTextBox runat="server" ID="txtUsrContainer" TextMode="SingleLine"/>
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
                    <wssuc:InputFormControl runat="server" LabelText="">
                        <template_control>
                            <div class="ms-authoringcontrols">
                                <SharePoint:InputFormTextBox runat="server" ID="txtUsrObjClass" TextMode="SingleLine"/>
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
                    <wssuc:InputFormControl runat="server" LabelText="">
                        <template_control>
                            <div class="ms-authoringcontrols">
                                <SharePoint:InputFormTextBox runat="server" ID="txtUsrFilter" TextMode="SingleLine"/>
                            </div>
                        </template_control>
                    </wssuc:InputFormControl>
                </template_inputformcontrols>
            </wssuc:InputFormSection>
        </td>
    </tr>
    <!-- AD LDS User Scope -->
    <tr>
        <td>
            <wssuc:InputFormSection ID="InputFormSection15" runat="server"
            Title="AD LDS User Scope"
            Description="User Scope.">
                <template_inputformcontrols>
                    <wssuc:InputFormControl runat="server" LabelText="">
                        <template_control>
                            <div class="ms-authoringcontrols">
                                <SharePoint:InputFormTextBox runat="server" ID="txtUsrScope" TextMode="SingleLine"/>
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
                    <wssuc:InputFormControl runat="server" LabelText="">
                        <template_control>
                            <div class="ms-authoringcontrols">
                                <SharePoint:InputFormTextBox runat="server" ID="txtUsrOtherAttribs" TextMode="SingleLine"/>
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
                    <wssuc:InputFormControl runat="server" LabelText="">
                        <template_control>
                            <div class="ms-authoringcontrols">
                                <SharePoint:InputFormTextBox runat="server" ID="txtGrpContainer" TextMode="SingleLine"/>
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
                    <wssuc:InputFormControl runat="server" LabelText="">
                        <template_control>
                            <div class="ms-authoringcontrols">
                                <SharePoint:InputFormTextBox runat="server" ID="txtGrpAttrib" TextMode="SingleLine"/>
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
                    <wssuc:InputFormControl runat="server" LabelText="">
                        <template_control>
                            <div class="ms-authoringcontrols">
                                <SharePoint:InputFormTextBox runat="server" ID="txtGrpAltSearchAttrib" TextMode="SingleLine"/>
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
                    <wssuc:InputFormControl runat="server" LabelText="">
                        <template_control>
                            <div class="ms-authoringcontrols">
                                <SharePoint:InputFormTextBox runat="server" ID="txtGrpMemAttrib" TextMode="SingleLine"/>
                            </div>
                        </template_control>
                    </wssuc:InputFormControl>
                </template_inputformcontrols>
            </wssuc:InputFormSection>
        </td>
    </tr>
    <!-- AD LDS Group Distingiushed Name Attribute -->
    <tr>
        <td>
            <wssuc:InputFormSection ID="InputFormSection21" runat="server"
            Title="AD LDS Group Distinguished Name Attribute"
            Description="Group DN attrib.">
                <template_inputformcontrols>
                    <wssuc:InputFormControl runat="server" LabelText="">
                        <template_control>
                            <div class="ms-authoringcontrols">
                                <SharePoint:InputFormTextBox runat="server" ID="txtGrpDNAttrib" TextMode="SingleLine"/>
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
                    <wssuc:InputFormControl runat="server" LabelText="">
                        <template_control>
                            <div class="ms-authoringcontrols">
                                <SharePoint:InputFormTextBox runat="server" ID="txtGrpUsrFilter" TextMode="SingleLine"/>
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
                    <wssuc:InputFormControl runat="server" LabelText="">
                        <template_control>
                            <div class="ms-authoringcontrols">
                                <SharePoint:InputFormTextBox runat="server" ID="txtGrpFilter" TextMode="SingleLine"/>
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
            <wssuc:InputFormSection ID="InputFormSection24" runat="server"
            Title="AD LDS Group Scope"
            Description="Group Scope.">
                <template_inputformcontrols>
                    <wssuc:InputFormControl runat="server" LabelText="">
                        <template_control>
                            <div class="ms-authoringcontrols">
                                <SharePoint:InputFormTextBox runat="server" ID="txtGrpScope" TextMode="SingleLine"/>
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
Application Page
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
My Application Page
</asp:Content>
