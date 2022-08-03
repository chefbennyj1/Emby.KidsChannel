define(['globalize', 'loading', 'paper-icon-button-light', 'formDialogStyle', 'emby-linkbutton', 'detailtablecss', 'emby-collapse', 'emby-input', 'emby-select'],
    function (globalize, loading) {

        var pluginId = "672630FC-6FB8-4207-9C7B-AB414CD85B53";
        ApiClient.getItems = function (type) {
            const url = this.getUrl("Items?Recursive=true&IncludeItemTypes=" + type);
            return this.getJSON(url);
        };

        ApiClient.createWatchParty = function (id) {
            const url = this.getUrl("CreateWatchParty");
            const options = {
                Id: id
            };
            return this.ajax({
                type: "POST",
                url: url,
                data: JSON.stringify(options),
                contentType: 'application/json'
            });

        };
        
       


        async function updateResults(view) {
            const correctionResults = view.querySelector('.correctionResults');

            const correctionResult = await ApiClient.getFilePathCorrections();

            correctionResults.innerHTML = renderTableItemsHtml(correctionResult.Items);

            view.querySelector('.processSelectedItems').addEventListener('click',
                async () => {
                    var correctionIds = [];
                    
                    correctionResults.querySelectorAll('.chkProcessItem').forEach(checkbox => {
                        if (checkbox.checked) {
                            correctionIds.push(checkbox.id);
                        }
                    });

                    require(['confirm'],
                        function(confirm) {
                            const message = "";
                            confirm(message, "").then(async function() {
                                loading.show();
                                

                                loading.hide();
                            });
                        });

                });

            view.querySelectorAll('.chkSelectAll').forEach(allCheckbox => {
                allCheckbox.addEventListener('click',
                    (e) => {
                        
                        var target     = e.target;
                        var table      = target.closest('.tblCorrectionResults');
                        var resultBody = table.querySelector('.resultBody');
                        var results    = resultBody.querySelectorAll('.chkProcessItem');
                        
                        if (target.checked) {
                            results.forEach(r => {
                                r.checked = true;
                            });
                        } else {
                            results.forEach(r => {
                                r.checked = false;
                            });
                        }
                    });
            });
        }

        return function (view) {

          view.addEventListener('viewshow',
            async function () {

              loading.show();
              var mediaSelect = view.querySelector('#mediaSelect');
              const createBtn = view.querySelector('#createBtn');
              const result = await ApiClient.getItems("Movie");
              
              result.Items.forEach(item => {
                mediaSelect.innerHTML += `<option value="${item.Id}">${item.Name}</option>`;
              });

              createBtn.addEventListener('click',
                async () => {
                    await ApiClient.createWatchParty(mediaSelect[mediaSelect.selectedIndex].value);
                });
              

              loading.hide();
            });

        };
    });