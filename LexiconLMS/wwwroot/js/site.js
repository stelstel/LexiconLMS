
let remove = document.getElementsByClassName("remove");         // This classname is "activated" when one clicks addToList()


let result = []

function addToList() {

    let form = $('.activityForm');
    let formData = form.serializeArray();

    let temp = {};
    formData.forEach(d => temp[d.name] = d.value || '');
    result.push(temp);

    let activity = `${temp.ActivityName} ${temp.ActivityStartTime} ${temp.ActivityEndTime}`;
    

    document.querySelector('.activityForm').reset();

    if (form === '') {                                                  // Kolla om någon input skrivits in.
        alert("Tomt inputfält!");
    } else {
        let li = document.createElement("LI");                        // Skapa ny <li>-nod.
        let elementText = document.createTextNode(activity);            // Tillsätt inputvärde till en ny text-nod.
        li.appendChild(elementText);                                  // Lägg till text/barn-nod till nya <li>-noden. 
        let xButton = document.createElement("button");                 // Skapa x-knapp.
        let x = document.createTextNode("\u00D7");                      // Skapa variabel med x-symbol.
        xButton.className = "remove";                                   // Ge x-knappen ett klassnamn.
        xButton.appendChild(x);                                         // Lägg till x-symbolen till x-knappen.
        li.appendChild(xButton);                                        // Lägg till x-knappnoden till <li>
        document.getElementById("theList").appendChild(li);             // Lägg till listpunkt till listan i sig.
    }

            

    for (i = 0; i < remove.length; i++) {                           // Detta lägger till ta-bort-funktionen
        remove[i].onclick = function () {
            let parent = this.parentElement;
            parent.style.display = "none";
            for (var j = 0; j < result.length; j++) {
                if (result[j] === temp) {
                    result.splice(j, 1);                        // Delete current object from list when the x-button is clicked.
                }
            }
        }

    }

}



function sendJson() {

    let token = $('input[name="__RequestVerificationToken"]').val();                    // ??

    let module = {};
    $('#module').serializeArray().forEach(d => module[d.name] = d.value || '');        // Get formdata for module

    $.ajax({
        contentType: 'application/x-www-form-urlencoded; charset=utf-8',
        dataType: 'json',                                                              // We are sending an json-object (Modules) and an array of json-objects (Activities)
        type: 'POST',
        url: '/Modules/Create',                                                        // url for the /Controller/Action
        data: {                                                                        // Set values for both arguments the Action is expecting
            viewModel: {
                data: result,                                       
                module: module
            },
            __RequestVerificationToken: token
        },
        success: function (response) {
            window.location.href = response.redirectToUrl;
            $('#result').html('"sendJson()" successfully called.');
        },
        failure: function (response) {
            $('#result').html(response);
        }
    }); 
}

function sendJsonToEdit() {

    let token = $('input[name="__RequestVerificationToken"]').val();                    // ??

    let module = {};
    $('#module').serializeArray().forEach(d => module[d.name] = d.value || '');         // Get formdata for module

    $.ajax({
        contentType: 'application/x-www-form-urlencoded; charset=utf-8',
        dataType: 'json',                                                               // We are sending an json-object (Modules) and an array of json-objects (Activities)
        type: 'POST',
        url: '/Modules/Edit',                                                           // url for the /Controller/Action
        data: {                                                                         // Set values for both arguments the Action is expecting
            viewModel: {
                data: result,
                module: module
            },
            __RequestVerificationToken: token
        },
        success: function (response) {                                                          // Input till successfunktionen är alltid det vi får tillbaka från servern
            window.location.href = response.redirectToUrl;
            console.log("stuff");
            $('#result').html('"sendJsonToEdit()" successfully called.');

        },
        failure: function (response) {
            $('#result').html(response);
        }
    });
}





//function AjaxGoodRedirect(urlAction) {
//    $.ajax({
//        type: "POST", // see http://haacked.com/archive/2009/06/24/json-hijacking.aspx 
//        url: urlAction,
//        data: {}, //to send data see more at http://bit.ly/mvc_ajax_jquery            
//        datatype: "JSON",
//        contentType: "application/json; charset=utf-8",
//        success: function (returndata) {
//            if (returndata.ok) window.location = returndata.newurl;
//            else window.alert(returndata.message);
//        }
//    });
//}​​
        


