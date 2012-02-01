<?xml version="1.0" encoding="utf-8"?>
<configurationSectionModel xmlns:dm0="http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core" dslVersion="1.0.0.0" Id="d0ed9acb-0435-4532-afdd-b5115bc4d562" namespace="Nauplius.SharePoint.ADLDS.UserProfiles" xmlSchemaNamespace="Nauplius.SharePoint.ADLDS.UserProfiles" assemblyName="Nauplius.SharePoint.ADLDS.Configuration" xmlns="http://schemas.microsoft.com/dsltools/ConfigurationSectionDesigner">
  <typeDefinitions>
    <externalType name="String" namespace="System" />
    <externalType name="Boolean" namespace="System" />
    <externalType name="Int32" namespace="System" />
    <externalType name="Int64" namespace="System" />
    <externalType name="Single" namespace="System" />
    <externalType name="Double" namespace="System" />
    <externalType name="DateTime" namespace="System" />
    <externalType name="TimeSpan" namespace="System" />
  </typeDefinitions>
  <configurationElements>
    <configurationElement name="Partition">
      <attributeProperties>
        <attributeProperty name="server" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="server" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="port" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="port" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/Int32" />
          </type>
        </attributeProperty>
        <attributeProperty name="dn" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="dn" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="useSSL" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="useSSL" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/Boolean" />
          </type>
        </attributeProperty>
        <attributeProperty name="webApplication" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="webApplication" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="logonAttribute" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="logonAttribute" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationSection name="PartitionsSection" codeGenOptions="Singleton, XmlnsProperty" xmlSectionName="partitionsSection">
      <elementProperties>
        <elementProperty name="Partitions" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="partitions" isReadOnly="false">
          <type>
            <configurationElementCollectionMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/Partitions" />
          </type>
        </elementProperty>
      </elementProperties>
    </configurationSection>
    <configurationElementCollection name="Partitions" xmlItemName="partition" codeGenOptions="Indexer, AddMethod, RemoveMethod, GetItemMethods">
      <itemType>
        <configurationElementMoniker name="/d0ed9acb-0435-4532-afdd-b5115bc4d562/Partition" />
      </itemType>
    </configurationElementCollection>
  </configurationElements>
  <propertyValidators>
    <validators />
  </propertyValidators>
</configurationSectionModel>