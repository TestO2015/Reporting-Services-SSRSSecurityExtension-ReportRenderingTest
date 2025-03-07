﻿using Microsoft.ReportingServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SSRSSecurityExtension
{
    public class SSRSAuthorization : Microsoft.ReportingServices.Interfaces.IAuthorizationExtension
    {
        bool HasUnRestrictedAccess(string userName)
        {
            return true;
        }

        bool UserMatchesPrincipal(string acePrincipal, IntPtr userToken)
        {
            return true;
        }

        #region IAuthorizationExtension Implementation 

        public string LocalizedName => null;

        public void SetConfiguration(string configuration)
        {
        }

        #region CheckAccess Methods

        public bool CheckAccess(
          string userName,
          IntPtr userToken,
          byte[] secDesc,
          ModelItemOperation modelItemOperation)
        {
            return true;
        }

        public bool CheckAccess(
         string userName,
         IntPtr userToken,
         byte[] secDesc,
         ModelOperation modelOperation)
        {
            return true;
        }

        /// <summary>
        /// Indicates whether a given user is authorized to access the item 
        /// for a given catalog operation.
        /// </summary>
        /// <param name="userName">The name of the user as returned by the 
        /// GetUserInfo method.</param>
        /// <param name="userToken">Pointer to the user ID returned by 
        /// GetUserInfo.</param>
        /// <param name="secDesc">The security descriptor returned by 
        /// CreateSecurityDescriptor.</param>
        /// <param name="requiredOperation">The operation being requested by 
        /// the report server for a given user.</param>
        /// <returns>True if the user is authorized.</returns>
        public bool CheckAccess(
           string userName,
           IntPtr userToken,
           byte[] secDesc,
           CatalogOperation requiredOperation)
        {
            return true;
        }

        // Overload for array of catalog operations
        public bool CheckAccess(
           string userName,
           IntPtr userToken,
           byte[] secDesc,
           CatalogOperation[] requiredOperations)
        {
            return true;
        }

        // Overload for Report operations
        public bool CheckAccess(
           string userName,
           IntPtr userToken,
           byte[] secDesc,
           ReportOperation requiredOperation)
        {
            return true;
        }

        // Overload for Folder operations
        public bool CheckAccess(
           string userName,
           IntPtr userToken,
           byte[] secDesc,
           FolderOperation requiredOperation)
        {
            return true;
        }

        // Overload for an array of Folder operations
        public bool CheckAccess(
           string userName,
           IntPtr userToken,
           byte[] secDesc,
           FolderOperation[] requiredOperations)
        {
            return true;
        }

        // Overload for Resource operations
        public bool CheckAccess(
           string userName,
           IntPtr userToken,
           byte[] secDesc,
           ResourceOperation requiredOperation)
        {
            return true;
        }

        // Overload for an array of Resource operations
        public bool CheckAccess(
           string userName,
           IntPtr userToken,
           byte[] secDesc,
           ResourceOperation[] requiredOperations)
        {
            return true;
        }

        // Overload for Datasource operations
        public bool CheckAccess(
           string userName,
           IntPtr userToken,
        byte[] secDesc,
           DatasourceOperation requiredOperation)
        {
            return true;
        }

        #endregion CheckAccess Methods

        public byte[] CreateSecurityDescriptor(AceCollection acl, SecurityItemType itemType, out string stringSecDesc)
        {
            // Creates a memory stream and serializes the ACL for storage.
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream result = new MemoryStream())
            {
                bf.Serialize(result, acl);
                stringSecDesc = null;
                return result.GetBuffer();
            }
        }

        public StringCollection GetPermissions(string userName, IntPtr userToken, SecurityItemType itemType, byte[] secDesc)
        {
            StringCollection permissions = new StringCollection();
            if (HasUnRestrictedAccess(userName))
            {
                permissions.AddRange(_fullPermissions.ToArray());
            }
            else
            {
                AceCollection acl = DeserializeAcl(secDesc);
                foreach (AceStruct ace in acl)
                {
                    if (0 == String.Compare(userName, ace.PrincipalName, true,
                          CultureInfo.CurrentCulture))
                    {
                        foreach (ModelItemOperation aclOperation in ace.ModelItemOperations)
                        {
                            if (!permissions.Contains(_modelItemOperNames[aclOperation]))
                                permissions.Add(_modelItemOperNames[aclOperation]);
                        }
                        foreach (ModelOperation aclOperation in ace.ModelOperations)
                        {
                            if (!permissions.Contains(_modelOperNames[aclOperation]))
                                permissions.Add(_modelOperNames[aclOperation]);
                        }
                        foreach (CatalogOperation aclOperation in ace.CatalogOperations)
                        {
                            if (!permissions.Contains(_catalogOperationNames[aclOperation]))
                                permissions.Add(_catalogOperationNames[aclOperation]);
                        }
                        foreach (ReportOperation aclOperation in ace.ReportOperations)
                        {
                            if (!permissions.Contains(_reportOperationNames[aclOperation]))
                                permissions.Add(_reportOperationNames[aclOperation]);
                        }
                        foreach (FolderOperation aclOperation in ace.FolderOperations)
                        {
                            if (!permissions.Contains(_folderOperationNames[aclOperation]))
                                permissions.Add(_folderOperationNames[aclOperation]);
                        }
                        foreach (ResourceOperation aclOperation in ace.ResourceOperations)
                        {
                            if (!permissions.Contains(_resourceOperationNames[aclOperation]))
                                permissions.Add(_resourceOperationNames[aclOperation]);
                        }
                        foreach (DatasourceOperation aclOperation in ace.DatasourceOperations)
                        {
                            if (!permissions.Contains(_dataSourceOperationNames[aclOperation]))
                                permissions.Add(_dataSourceOperationNames[aclOperation]);
                        }
                    }
                }
            }

            return permissions;
        }


        #endregion IAuthorizationExtension Implementation


        #region Supporting Static Methods (Adapated from MS Sample)

        static SSRSAuthorization()
        {
            InitializeMaps();
        }


        private static readonly Dictionary<ModelItemOperation, string> _modelItemOperNames = new Dictionary<ModelItemOperation, string>();
        private static readonly Dictionary<ModelOperation, string> _modelOperNames = new Dictionary<ModelOperation, string>();
        private static readonly Dictionary<CatalogOperation, string> _catalogOperationNames = new Dictionary<CatalogOperation, string>();
        private static readonly Dictionary<FolderOperation, string> _folderOperationNames = new Dictionary<FolderOperation, string>();
        private static readonly Dictionary<ReportOperation, string> _reportOperationNames = new Dictionary<ReportOperation, string>();
        private static readonly Dictionary<ResourceOperation, string> _resourceOperationNames = new Dictionary<ResourceOperation, string>();
        private static readonly Dictionary<DatasourceOperation, string> _dataSourceOperationNames = new Dictionary<DatasourceOperation, string>();
        private static readonly List<string> _fullPermissions = new List<string>();

        // Utility method used to create mappings to the various
        // operations in Reporting Services. These mappings support
        // the implementation of the GetPermissions method.
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes")]
        private static void InitializeMaps()
        {
            // create model operation names data
            _modelItemOperNames.Add(ModelItemOperation.ReadProperties, OperationNames.OperReadProperties);

            if (_modelItemOperNames.Count != Enum.GetValues(typeof(ModelItemOperation)).Length)
            {
                //Model item name mismatch
                throw new Exception("Model item name mismatch");
            }

            // create model operation names data
            _modelOperNames.Add(ModelOperation.Delete, OperationNames.OperDelete);
            _modelOperNames.Add(ModelOperation.ReadAuthorizationPolicy, OperationNames.OperReadAuthorizationPolicy);
            _modelOperNames.Add(ModelOperation.ReadContent, OperationNames.OperReadContent);
            _modelOperNames.Add(ModelOperation.ReadDatasource, OperationNames.OperReadDatasources);
            _modelOperNames.Add(ModelOperation.ReadModelItemAuthorizationPolicies, OperationNames.OperReadModelItemSecurityPolicies);
            _modelOperNames.Add(ModelOperation.ReadProperties, OperationNames.OperReadProperties);
            _modelOperNames.Add(ModelOperation.UpdateContent, OperationNames.OperUpdateContent);
            _modelOperNames.Add(ModelOperation.UpdateDatasource, OperationNames.OperUpdateDatasources);
            _modelOperNames.Add(ModelOperation.UpdateDeleteAuthorizationPolicy, OperationNames.OperUpdateDeleteAuthorizationPolicy);
            _modelOperNames.Add(ModelOperation.UpdateModelItemAuthorizationPolicies, OperationNames.OperUpdateModelItemSecurityPolicies);
            _modelOperNames.Add(ModelOperation.UpdateProperties, OperationNames.OperUpdatePolicy);

            if (_modelOperNames.Count != Enum.GetValues(typeof(ModelOperation)).Length)
            {
                //Model name mismatch
                throw new Exception("Model name mismatch");
            }

            // create operation names data
            _catalogOperationNames.Add(CatalogOperation.CreateRoles, OperationNames.OperCreateRoles);
            _catalogOperationNames.Add(CatalogOperation.DeleteRoles, OperationNames.OperDeleteRoles);
            _catalogOperationNames.Add(CatalogOperation.ReadRoleProperties, OperationNames.OperReadRoleProperties);
            _catalogOperationNames.Add(CatalogOperation.UpdateRoleProperties, OperationNames.OperUpdateRoleProperties);
            _catalogOperationNames.Add(CatalogOperation.ReadSystemProperties, OperationNames.OperReadSystemProperties);
            _catalogOperationNames.Add(CatalogOperation.UpdateSystemProperties, OperationNames.OperUpdateSystemProperties);
            _catalogOperationNames.Add(CatalogOperation.GenerateEvents, OperationNames.OperGenerateEvents);
            _catalogOperationNames.Add(CatalogOperation.ReadSystemSecurityPolicy, OperationNames.OperReadSystemSecurityPolicy);
            _catalogOperationNames.Add(CatalogOperation.UpdateSystemSecurityPolicy, OperationNames.OperUpdateSystemSecurityPolicy);
            _catalogOperationNames.Add(CatalogOperation.CreateSchedules, OperationNames.OperCreateSchedules);
            _catalogOperationNames.Add(CatalogOperation.DeleteSchedules, OperationNames.OperDeleteSchedules);
            _catalogOperationNames.Add(CatalogOperation.ReadSchedules, OperationNames.OperReadSchedules);
            _catalogOperationNames.Add(CatalogOperation.UpdateSchedules, OperationNames.OperUpdateSchedules);
            _catalogOperationNames.Add(CatalogOperation.ListJobs, OperationNames.OperListJobs);
            _catalogOperationNames.Add(CatalogOperation.CancelJobs, OperationNames.OperCancelJobs);
            _catalogOperationNames.Add(CatalogOperation.ExecuteReportDefinition, OperationNames.ExecuteReportDefinition);

            if (_catalogOperationNames.Count != Enum.GetValues(typeof(CatalogOperation)).Length)
            {
                //Catalog name mismatch
                throw new Exception("Catalog name mismatch");
            }

            _folderOperationNames.Add(FolderOperation.CreateFolder, OperationNames.OperCreateFolder);
            _folderOperationNames.Add(FolderOperation.Delete, OperationNames.OperDelete);
            _folderOperationNames.Add(FolderOperation.ReadProperties, OperationNames.OperReadProperties);
            _folderOperationNames.Add(FolderOperation.UpdateProperties, OperationNames.OperUpdateProperties);
            _folderOperationNames.Add(FolderOperation.CreateReport, OperationNames.OperCreateReport);
            _folderOperationNames.Add(FolderOperation.CreateResource, OperationNames.OperCreateResource);
            _folderOperationNames.Add(FolderOperation.ReadAuthorizationPolicy, OperationNames.OperReadAuthorizationPolicy);
            _folderOperationNames.Add(FolderOperation.UpdateDeleteAuthorizationPolicy, OperationNames.OperUpdateDeleteAuthorizationPolicy);
            _folderOperationNames.Add(FolderOperation.CreateDatasource, OperationNames.OperCreateDatasource);
            _folderOperationNames.Add(FolderOperation.CreateModel, OperationNames.OperCreateModel);
            if (_folderOperationNames.Count != Enum.GetValues(typeof(FolderOperation)).Length)
            {
                //Folder name mismatch
                throw new Exception("Folder name mismatch");
            }

            _reportOperationNames.Add(ReportOperation.Delete, OperationNames.OperDelete);
            _reportOperationNames.Add(ReportOperation.ReadProperties, OperationNames.OperReadProperties);
            _reportOperationNames.Add(ReportOperation.UpdateProperties, OperationNames.OperUpdateProperties);
            _reportOperationNames.Add(ReportOperation.UpdateParameters, OperationNames.OperUpdateParameters);
            _reportOperationNames.Add(ReportOperation.ReadDatasource, OperationNames.OperReadDatasources);
            _reportOperationNames.Add(ReportOperation.UpdateDatasource, OperationNames.OperUpdateDatasources);
            _reportOperationNames.Add(ReportOperation.ReadReportDefinition, OperationNames.OperReadReportDefinition);
            _reportOperationNames.Add(ReportOperation.UpdateReportDefinition, OperationNames.OperUpdateReportDefinition);
            _reportOperationNames.Add(ReportOperation.CreateSubscription, OperationNames.OperCreateSubscription);
            _reportOperationNames.Add(ReportOperation.DeleteSubscription, OperationNames.OperDeleteSubscription);
            _reportOperationNames.Add(ReportOperation.ReadSubscription, OperationNames.OperReadSubscription);
            _reportOperationNames.Add(ReportOperation.UpdateSubscription, OperationNames.OperUpdateSubscription);
            _reportOperationNames.Add(ReportOperation.CreateAnySubscription, OperationNames.OperCreateAnySubscription);
            _reportOperationNames.Add(ReportOperation.DeleteAnySubscription, OperationNames.OperDeleteAnySubscription);
            _reportOperationNames.Add(ReportOperation.ReadAnySubscription, OperationNames.OperReadAnySubscription);
            _reportOperationNames.Add(ReportOperation.UpdateAnySubscription, OperationNames.OperUpdateAnySubscription);
            _reportOperationNames.Add(ReportOperation.UpdatePolicy, OperationNames.OperUpdatePolicy);
            _reportOperationNames.Add(ReportOperation.ReadPolicy, OperationNames.OperReadPolicy);
            _reportOperationNames.Add(ReportOperation.DeleteHistory, OperationNames.OperDeleteHistory);
            _reportOperationNames.Add(ReportOperation.ListHistory, OperationNames.OperListHistory);
            _reportOperationNames.Add(ReportOperation.ExecuteAndView, OperationNames.OperExecuteAndView);
            _reportOperationNames.Add(ReportOperation.CreateResource, OperationNames.OperCreateResource);
            _reportOperationNames.Add(ReportOperation.CreateSnapshot, OperationNames.OperCreateSnapshot);
            _reportOperationNames.Add(ReportOperation.ReadAuthorizationPolicy, OperationNames.OperReadAuthorizationPolicy);
            _reportOperationNames.Add(ReportOperation.UpdateDeleteAuthorizationPolicy, OperationNames.OperUpdateDeleteAuthorizationPolicy);
            _reportOperationNames.Add(ReportOperation.Execute, OperationNames.OperExecute);
            _reportOperationNames.Add(ReportOperation.CreateLink, OperationNames.OperCreateLink);
            _reportOperationNames.Add(ReportOperation.Comment, OperationNames.OperComment);
            _reportOperationNames.Add(ReportOperation.ManageComments, OperationNames.OperManageComments);

            if (_reportOperationNames.Count != Enum.GetValues(typeof(ReportOperation)).Length)
            {
                //Report name mismatch
                throw new Exception("Report name mismatch");
            }

            _resourceOperationNames.Add(ResourceOperation.Delete, OperationNames.OperDelete);
            _resourceOperationNames.Add(ResourceOperation.ReadProperties, OperationNames.OperReadProperties);
            _resourceOperationNames.Add(ResourceOperation.UpdateProperties, OperationNames.OperUpdateProperties);
            _resourceOperationNames.Add(ResourceOperation.ReadContent, OperationNames.OperReadContent);
            _resourceOperationNames.Add(ResourceOperation.UpdateContent, OperationNames.OperUpdateContent);
            _resourceOperationNames.Add(ResourceOperation.ReadAuthorizationPolicy, OperationNames.OperReadAuthorizationPolicy);
            _resourceOperationNames.Add(ResourceOperation.UpdateDeleteAuthorizationPolicy, OperationNames.OperUpdateDeleteAuthorizationPolicy);
            _resourceOperationNames.Add(ResourceOperation.Comment, OperationNames.OperComment);
            _resourceOperationNames.Add(ResourceOperation.ManageComments, OperationNames.OperManageComments);

            if (_resourceOperationNames.Count != Enum.GetValues(typeof(ResourceOperation)).Length)
            {
                //Resource name mismatch
                throw new Exception("Resource name mismatch");
            }

            _dataSourceOperationNames.Add(DatasourceOperation.Delete, OperationNames.OperDelete);
            _dataSourceOperationNames.Add(DatasourceOperation.ReadProperties, OperationNames.OperReadProperties);
            _dataSourceOperationNames.Add(DatasourceOperation.UpdateProperties, OperationNames.OperUpdateProperties);
            _dataSourceOperationNames.Add(DatasourceOperation.ReadContent, OperationNames.OperReadContent);
            _dataSourceOperationNames.Add(DatasourceOperation.UpdateContent, OperationNames.OperUpdateContent);
            _dataSourceOperationNames.Add(DatasourceOperation.ReadAuthorizationPolicy, OperationNames.OperReadAuthorizationPolicy);
            _dataSourceOperationNames.Add(DatasourceOperation.UpdateDeleteAuthorizationPolicy, OperationNames.OperUpdateDeleteAuthorizationPolicy);

            if (_dataSourceOperationNames.Count != Enum.GetValues(typeof(DatasourceOperation)).Length)
            {
                //Datasource name mismatch
                throw new Exception("Datasource name mismatch");
            }

            // Initialize permission collection.
            foreach (CatalogOperation oper in _catalogOperationNames.Keys)
            {
                if (!_fullPermissions.Contains(_catalogOperationNames[oper]))
                    _fullPermissions.Add(_catalogOperationNames[oper]);
            }

            foreach (ModelItemOperation oper in _modelItemOperNames.Keys)
            {
                if (!_fullPermissions.Contains(_modelItemOperNames[oper]))
                    _fullPermissions.Add(_modelItemOperNames[oper]);
            }

            foreach (ModelOperation oper in _modelOperNames.Keys)
            {
                if (!_fullPermissions.Contains(_modelOperNames[oper]))
                    _fullPermissions.Add(_modelOperNames[oper]);
            }

            foreach (CatalogOperation oper in _catalogOperationNames.Keys)
            {
                if (!_fullPermissions.Contains(_catalogOperationNames[oper]))
                    _fullPermissions.Add(_catalogOperationNames[oper]);
            }

            foreach (ReportOperation oper in _reportOperationNames.Keys)
            {
                if (!_fullPermissions.Contains((string)_reportOperationNames[oper]))
                    _fullPermissions.Add((string)_reportOperationNames[oper]);
            }

            foreach (FolderOperation oper in _folderOperationNames.Keys)
            {
                if (!_fullPermissions.Contains((string)_folderOperationNames[oper]))
                    _fullPermissions.Add((string)_folderOperationNames[oper]);
            }

            foreach (ResourceOperation oper in _resourceOperationNames.Keys)
            {
                if (!_fullPermissions.Contains((string)_resourceOperationNames[oper]))
                    _fullPermissions.Add((string)_resourceOperationNames[oper]);
            }

            foreach (DatasourceOperation oper in _dataSourceOperationNames.Keys)
            {
                if (!_fullPermissions.Contains((string)_dataSourceOperationNames[oper]))
                    _fullPermissions.Add((string)_dataSourceOperationNames[oper]);
            }
        }


        #endregion Supporting Static Methods (Adapated from MS Sample)

        #region Private Supporting Methods

        private AceCollection DeserializeAcl(byte[] secDesc)
        {
            AceCollection acl = new AceCollection();
            if (secDesc != null)
            {
                BinaryFormatter bf = new BinaryFormatter();
                using (MemoryStream sdStream = new MemoryStream(secDesc))
                {
                    acl = (AceCollection)bf.Deserialize(sdStream);
                }
            }
            return acl;
        }

        #endregion Private Supporting Methods
    }
}
