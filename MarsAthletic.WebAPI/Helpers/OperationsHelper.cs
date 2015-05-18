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

                var department = new Department() { Name = departmentObject.Name, ExternalID = departmentObject.ID, InternalID = departmentObject.DisplayID };

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
            var objectTypeIdForEmp = 101;

            var client = new MfwsClient(_config.GetMFilesUrl() + "REST");

            //authenticate
            var result = client.Post<PrimitiveType<string>>(
                "/server/authenticationtokens",
                new Authentication { Username = _config.GetAccountName(), Password = _config.GetPassword(), VaultGuid = _config.GetVaultGuid() });

            //bind the token value
            client.Authentication = result.Value;

            var objectTypeList = new List<Employee>();

            //make request
            var response = client.Get<Results<ValueListItem>>(string.Format("/valuelists/{0}/items", objectTypeIdForEmp.ToString()));


            for (int i = 0; i < response.Items.Length; i++)
            {
                var employeeObject = response.Items[i];

                var employee = new Employee() { Name = employeeObject.Name, ExternalID = employeeObject.ID, InternalID = employeeObject.DisplayID };

                objectTypeList.Add(employee);
            }

            return objectTypeList;
        }

        public int GetStatusOfDocument(int id)
        {
            var objectTypeForDocument = 0;

            var completedStateIds = new int[] { 120, 200 };

            var waitingStatuses = new int[] { 101, 121, 105, 102, 108, 124, 125, 126, 133, 134, 136, 129, 111, 133, 122, 114, 137, 123, 117, 174, 175, 176, 179, 180, 183, 184, 188, 191, 192, 196, 198, 200 };

            var rejectStatuses = new int[] { 106, 103, 109, 128, 135, 112, 115, 118, 177, 181, 185, 189, 193 };

            var returnStatuses = new int[] { 178, 182, 186, 190, 201, 202, 203, 194, 197, 199, 107, 104, 130, 110, 127, 113, 116, 131, 132, 119 };

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

                if (completedStateIds.Contains(property.TypedValue.Lookup.Item))
                {
                    //Complated
                    return 1;
                }
                else if (rejectStatuses.Contains(property.TypedValue.Lookup.Item))
                {
                    //Rejected
                    return 2;
                }
                else if (waitingStatuses.Contains(property.TypedValue.Lookup.Item))
                {
                    //Waiting
                    return 0;
                }
                else if (returnStatuses.Contains(property.TypedValue.Lookup.Item))
                {
                    //Returned Back
                    return 3;
                }
                else
                {
                    //Unknown state return waiting..??
                    return 0;
                }
            }
            else
            {
                //No Workflow
                return 4;
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

            if (data.Description.Trim() != "" && data.Description != null)
            {
                propValues.Add(new PropertyValue
                {
                    PropertyDef = 1088,
                    TypedValue = new TypedValue { DataType = MFDataType.MultiLineText, Value = data.Description }
                });
            }

            //Talep Eden Personel
            propValues.Add(new PropertyValue
            {
                PropertyDef = 1026,
                TypedValue = new TypedValue
                {
                    DataType = MFDataType.Lookup,
                    Lookup = new Lookup
                    {
                        Item = GetExternalIDWithDisplayID(client, 101, data.EmployeeId),
                        Version = -1
                    },

                },

            });

            if (data.WithProductInfo)
            {
                //İş Akışı
                propValues.Add(new PropertyValue
                {
                    PropertyDef = 38,
                    TypedValue = new TypedValue
                    {
                        DataType = MFDataType.Lookup,
                        Lookup = new Lookup
                        {
                            Item = 101,
                            Version = -1
                        },

                    },

                });

                //Durum
                propValues.Add(new PropertyValue
                {
                    PropertyDef = 39,
                    TypedValue = new TypedValue
                    {
                        DataType = MFDataType.Lookup,
                        Lookup = new Lookup
                        {
                            Item = 101,
                            Version = -1
                        },

                    },

                });
            }
            else
            {
                //İş Akışı
                propValues.Add(new PropertyValue
                {
                    PropertyDef = 38,
                    TypedValue = new TypedValue
                    {
                        DataType = MFDataType.Lookup,
                        Lookup = new Lookup
                        {
                            Item = 105,
                            Version = -1
                        },

                    },

                });

                //Durum
                propValues.Add(new PropertyValue
                {
                    PropertyDef = 39,
                    TypedValue = new TypedValue
                    {
                        DataType = MFDataType.Lookup,
                        Lookup = new Lookup
                        {
                            Item = 174,
                            Version = -1
                        },

                    },

                });
            }

            //Departman
            propValues.Add(new PropertyValue
            {
                PropertyDef = 1021,
                TypedValue = new TypedValue
                {
                    DataType = MFDataType.MultiSelectLookup,
                    Lookups = new Lookup[]
                                    {
                                        new Lookup { Item = GetExternalIDWithDisplayID(client,102, data.DepartmentId) , Version= -1}
                                    }

                }
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
                                        new Lookup { Item = GetExternalIDWithDisplayID(client,106,data.LocationId) , Version= -1}
                                    }

                }
            });

            //Cost
            propValues.Add(new PropertyValue
            {
                PropertyDef = 1072,
                TypedValue = new TypedValue { DataType = MFDataType.Text, Value = data.Cost },

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

        private int GetExternalIDWithDisplayID(MfwsClient client, int objectTypeId, int objectId)
        {
            //make request
            var response = client.Get<Results<ValueListItem>>(string.Format("/valuelists/{0}/items", objectTypeId.ToString()));

            var displayId = objectId;

            for (int i = 0; i < response.Items.Length; i++)
            {
                var valueListItem = response.Items[i];

                if (valueListItem.DisplayID == objectId.ToString())
                {
                    displayId = valueListItem.ID;
                    break;
                }
            }

            return displayId;
        }
    }
}
