using MarsAthletic.WebAPI.Interfaces;
using MarsAthletic.WebAPI.Models;
using MFiles.Mfws;
using MFiles.Mfws.Structs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarsAthletic.WebAPI.Helpers
{
    class OperationsHelper : IOperations
    {

        private readonly IConfigHelper _config;

        public OperationsHelper(IConfigHelper config)
        {
            _config = config;
        }

        public IEnumerable<Models.Department> GetDepartments()
        {

            var objectTypeIdForDepartment = 102;

            var client = new MfwsClient(_config.GetMFilesUrl() + "REST");

            //authenticate
            var result = client.Post<PrimitiveType<string>>(
                "/server/authenticationtokens",
                new Authentication { Username = _config.GetAccountName(), Password = _config.GetPassword(), VaultGuid = _config.GetVaultGuid() });

            //bind the token value
            client.Authentication = result.Value;

            //make request
            var response = client.Get<Results<ValueListItem>>(string.Format("/valuelists/{0}/items", objectTypeIdForDepartment.ToString()));

            var objectTypeList = new List<Department>();

            for (int i = 0; i < response.Items.Length; i++)
            {
                var departmentObject = response.Items[i];

                var department = new Department() { Name = departmentObject.Name, ExternalID = departmentObject.ID, InternalID = departmentObject.DisplayID, WorkLocationID = departmentObject.OwnerID };

                objectTypeList.Add(department);
            }


            return objectTypeList;

        }

        public IEnumerable<Models.WorkLocation> GetWorkLocations()
        {
            var objectTypeIdForWL = 106;

            var client = new MfwsClient(_config.GetMFilesUrl() + "REST");

            //authenticate
            var result = client.Post<PrimitiveType<string>>(
                "/server/authenticationtokens",
                new Authentication { Username = _config.GetAccountName(), Password = _config.GetPassword(), VaultGuid = _config.GetVaultGuid() });

            //bind the token value
            client.Authentication = result.Value;

            //make request
            var response = client.Get<Results<ValueListItem>>(string.Format("/valuelists/{0}/items", objectTypeIdForWL.ToString()));

            var objectTypeList = new List<WorkLocation>();

            for (int i = 0; i < response.Items.Length; i++)
            {
                var workLocationObject = response.Items[i];

                var workLocation = new WorkLocation() { Name = workLocationObject.Name, ExternalID = workLocationObject.ID, InternalID = workLocationObject.DisplayID };

                objectTypeList.Add(workLocation);
            }


            return objectTypeList;
        }

        public IEnumerable<Models.Employee> GetEmployees()
        {
            var objectTypeIdForWL = 101;

            var client = new MfwsClient(_config.GetMFilesUrl() + "REST");

            //authenticate
            var result = client.Post<PrimitiveType<string>>(
                "/server/authenticationtokens",
                new Authentication { Username = _config.GetAccountName(), Password = _config.GetPassword(), VaultGuid = _config.GetVaultGuid() });

            //bind the token value
            client.Authentication = result.Value;

            //make request
            var response = client.Get<Results<ValueListItem>>(string.Format("/valuelists/{0}/items", objectTypeIdForWL.ToString()));

            var objectTypeList = new List<Employee>();

            for (int i = 0; i < response.Items.Length; i++)
            {
                var employeeObject = response.Items[i];

                var employee = new Employee() { Name = employeeObject.Name, ExternalID = employeeObject.ID, InternalID = employeeObject.DisplayID, DepartmentID = employeeObject.OwnerID };

                objectTypeList.Add(employee);
            }

            return objectTypeList;
        }

        public bool DocumentsProcessEnded(int id)
        {

            var objectTypeForDocument = 0;
            var completedStateId = 120;

            var client = new MfwsClient(_config.GetMFilesUrl() + "REST");

            //authenticate
            var result = client.Post<PrimitiveType<string>>(
                "/server/authenticationtokens",
                new Authentication { Username = _config.GetAccountName(), Password = _config.GetPassword(), VaultGuid = _config.GetVaultGuid() });

            //bind the token value
            client.Authentication = result.Value;

            //make request
            var response = client.Get<ExtendedObjectVersion>(string.Format("/objects/{0}/{1}/-1?include=properties", objectTypeForDocument.ToString(), id.ToString()));

            if (response.Properties.AsQueryable().AsEnumerable().Where(x => x.PropertyDef == 39).FirstOrDefault() != null)
            {
                var property = response.Properties.AsQueryable().AsEnumerable().Where(x => x.PropertyDef == 39).FirstOrDefault();

                if (property.TypedValue.Lookup.Item == completedStateId)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            else
            {
                return false;
            }

        }

        public int CreateDocument(DocumentData data)
        {

            var client = new MfwsClient(_config.GetMFilesUrl() + "REST");

            //authenticate
            var result = client.Post<PrimitiveType<string>>(
                "/server/authenticationtokens",
                new Authentication { Username = _config.GetAccountName(), Password = _config.GetPassword(), VaultGuid = _config.GetVaultGuid() });

            //bind the token value
            client.Authentication = result.Value;

            //Create PropertyValues
            List<PropertyValue> propValues = new List<PropertyValue>();

            //Add SingleFile Property
            propValues.Add(new PropertyValue
            {
                PropertyDef = 22,
                TypedValue = new TypedValue { DataType = MFDataType.Boolean, Value = true }
            });

            //Class Id
            propValues.Add(new PropertyValue
            {
                PropertyDef = 100,
                TypedValue = new TypedValue
                {
                    DataType = MFDataType.Lookup,
                    Lookup = new Lookup
                    {
                        Item = 4,
                        Version = -1
                    },

                },

            });

            //Talep Eden Personel
            propValues.Add(new PropertyValue
            {
                PropertyDef = 1026,
                TypedValue = new TypedValue
                {
                    DataType = MFDataType.Lookup,
                    Lookup = new Lookup
                    {
                        Item = GetDisplayIDWithExternalID(client, 101, data.EmployeeId),
                        Version = -1
                    },

                },

            });

            //İşyeri
            propValues.Add(new PropertyValue
            {
                PropertyDef = 1060,
                TypedValue = new TypedValue
                {
                    DataType = MFDataType.MultiSelectLookup,
                    Lookups = new Lookup[]
                                    {
                                        new Lookup { Item = GetDisplayIDWithExternalID(client,106, data.LocationId), Version= -1}
                                    }

                }
            });

            //Cost
            propValues.Add(new PropertyValue
            {
                PropertyDef = 1064,
                TypedValue = new TypedValue { DataType = MFDataType.Integer, Value = data.Cost },

            });


            //Konu
            propValues.Add(new PropertyValue
            {
                PropertyDef = 1036,
                TypedValue = new TypedValue { DataType = MFDataType.Text, Value = data.DocumentName },

            });

            //Date
            propValues.Add(new PropertyValue
            {
                PropertyDef = 1024,
                TypedValue = new TypedValue { DataType = MFDataType.Date, Value = data.DateCreated },

            });

            var fileStream = new MemoryStream(data.ByteData);

            var uploadInfo = client.Post<UploadInfo>("/files", fileStream);

            uploadInfo.Title = data.DocumentName;
            uploadInfo.Extension = data.DocumentExtension.Replace(".", "");

            // Create the creation info.
            var creationInfo = new ObjectCreationInfo
            {
                PropertyValues = propValues.ToArray(),
                Files = new[] { uploadInfo }
            };

            try
            {
                var oObjectVersion = client.Post<ObjectVersion>("/objects/0", creationInfo);
                fileStream.Dispose();
                return oObjectVersion.ObjVer.ID;
            }
            catch (Exception)
            {
                fileStream.Dispose();
                throw;
            }

        }

        private int GetDisplayIDWithExternalID(MfwsClient client, int objectTypeId, int objectId)
        {
            //make request
            var response = client.Get<Results<ValueListItem>>(string.Format("/valuelists/{0}/items", objectTypeId.ToString()));

            var displayId = objectId;

            for (int i = 0; i < response.Items.Length; i++)
            {
                var valueListItem = response.Items[0];

                if (valueListItem.ID == objectId)
                {
                    displayId = Convert.ToInt32(valueListItem.DisplayID);
                    break;
                }
            }

            return displayId;
        }
    }
}
