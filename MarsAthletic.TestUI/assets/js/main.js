
$(document).ready(function() {

            function FillWorkLocations() {
                $.ajax({
                    type: "GET",
                    url: "http://localhost:1874/Rest/Operations/GetWorkLocations",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function(data)
                            {
                                 $.each(data, function (){
                                     $("#workLocations").append($("<option     />").val(this.ExternalID).text(this.Name));
                                });
                            },
                    error: function () {
                        alert("Web servis'e eriþilemiyor.");
                    }
                });
            }

            function FillEmployees() {
                $.ajax({
                    type: "GET",
                    url: "http://localhost:1874/Rest/Operations/GetEmployees",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function(data)
                            {
                                 $.each(data, function (){
                                     $("#employees").append($("<option     />").val(this.ExternalID).text(this.Name));
                                });
                            },
                    error: function () {
                        alert("Web servis'e eriþilemiyor.");
                    }
                });
            }


            function FillDepartments() {
                $.ajax({
                    type: "GET",
                    url: "http://localhost:1874/Rest/Operations/GetDepartments",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function(data)
                            {
                                 $.each(data, function (){
                                     $("#departments").append($("<option     />").val(this.ExternalID).text(this.Name));
                                });
                            },
                    error: function () {
                        alert("Web servis'e eriþilemiyor.");
                    }
                });
            }

            FillWorkLocations();
            FillEmployees();
            FillDepartments();

            $( "#datepicker" ).datepicker();

            var handleFileSelect = function(evt) {
                var files = evt.target.files;
                var file = files[0];



                if (files && file) {
                    var reader = new FileReader();

                    reader.onload = function(readerEvt) {
                        var binaryString = readerEvt.target.result;

                        document.getElementById("documentValue").value = btoa(binaryString);

                        var fileValue = $("#document").val().replace("C:\\fakepath\\", "")


                        $("#documentName").val(fileValue);

                    };

                    reader.readAsBinaryString(file);
                }
            };

            if (window.File && window.FileReader && window.FileList && window.Blob) {
                document.getElementById('document').addEventListener('change', handleFileSelect, false);
            } else {
                alert('The File APIs are not fully supported in this browser.');
            }

            function removeExtension(filename){
                var lastDotPosition = filename.lastIndexOf(".");
                if (lastDotPosition === -1) return filename;
                else return filename.substr(0, lastDotPosition);
            }

            function getExtension(filename) {
                return filename.split('.').pop().toLowerCase();
            }


            $("#createDocument").click(function(e) {
                
                var documentByteArray = $("#documentValue").val();

                if (!$.trim(documentByteArray) == "") 
                {
                    var validator = $( "#form1" ).validate();
                    var validated = validator.form();

                        if (validated) {
                            var documentName = removeExtension($("#documentName").val());

                            var extension = getExtension($("#documentName").val());

                            var dataObject = {};

                                    dataObject.DocumentName= documentName;
                                    dataObject.DocumentExtension = extension;
                                    dataObject.ByteData = documentByteArray;
                                    dataObject.EmployeeId= $("#employees").val();
                                    dataObject.LocationId= $("#workLocations").val();
                                    dataObject.DateCreated= $("#datepicker").val();
                                    dataObject.DepartmentId = $("#departments").val();
                                    dataObject.Cost = $("#cost").val();

                                   alert(JSON.stringify(dataObject));

                            $.ajax({
                                type: "POST",
                                url: "http://localhost:1874/Rest/Operations/Create",
                                contentType: "application/json; charset=utf-8",
                                data:JSON.stringify(dataObject),
                                dataType: "json",
                                success: function(data)
                                        {
                                            alert("Doküman baþarýyla oluþturuldu. ID : " + data.CreatedDocumentID);
                                        },
                                error: function () {
                                    alert("Web serviste hata oluþtu.");
                                }
                            });
                        };
                }
                else{
                    alert("Lütfen doküman seçiniz.");
                }

            });


        });